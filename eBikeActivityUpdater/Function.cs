using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using eBikeActivityUpdater.Models;
using static eBikeActivityUpdater.Constants;
using static eBikeActivityUpdater.Constants.EnvironmentVariables;
using static eBikeActivityUpdater.Constants.FormatStrings;
using static eBikeActivityUpdater.Constants.Headers;
using static eBikeActivityUpdater.Constants.Routes;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace eBikeActivityUpdater
{
    public class Function
    {
        private readonly HttpClient _httpClient = new HttpClient{ BaseAddress = new Uri("https://www.strava.com/") };

        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            await EnsureAccessToken();

            var activities = await GetRecentActivities();

            Console.WriteLine($"{activities.Count} activities retrieved for update: {JsonConvert.SerializeObject(activities)}");

            foreach (var activity in activities)
            {
                var isUpdated = await IsActivityUpdated(activity);

                if (isUpdated)
                {
                    Console.WriteLine($"Activity with id {activity.Id} has already been updated.");
                }
                else
                {
                    var updateableActivity = activity.ToUpdateable(Environment.GetEnvironmentVariable(ActivityType), Environment.GetEnvironmentVariable(GearId));

                    await UpdateActivity(activity, updateableActivity);

                    await NotifyUpdate(activity);

                    Console.WriteLine($"Activity with id {activity.Id} updated to have activity type {updateableActivity.Type} and gear id {updateableActivity.GearId}.");
                }
            }

            return input?.ToUpper();
        }

        private async Task EnsureAccessToken()
        {
            var queryParams = string.Format(
                QueryParamFormat,
                Environment.GetEnvironmentVariable(ClientId),
                Environment.GetEnvironmentVariable(ClientSecret),
                Environment.GetEnvironmentVariable(RefreshToken));

            var request = new HttpRequestMessage(HttpMethod.Post, $"{TokenRoute}?{queryParams}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var refreshTokenString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Refresh token response string: {refreshTokenString}");

                var refreshTokenResponse = JsonConvert.DeserializeObject<RefreshTokenResponse>(refreshTokenString);

                var accessToken = refreshTokenResponse.AccessToken;

                if (_httpClient.DefaultRequestHeaders.Contains(Authorization))
                {
                    _httpClient.DefaultRequestHeaders.Remove(Authorization);
                }

                _httpClient.DefaultRequestHeaders.Add(Authorization, $"Bearer {accessToken}");
            }
            else
            {
                Console.WriteLine($"Refresh token error: {JsonConvert.SerializeObject(response)}");
            }
        }

        private async Task<List<Activity>> GetRecentActivities()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetActivitiesRoute);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var activitiesString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Activities response string: {activitiesString}");

                var activities = JsonConvert.DeserializeObject<List<Activity>>(activitiesString);

                var minutesToBackdate = int.Parse(Environment.GetEnvironmentVariable(BackdateMinutes));

                return activities.Where(a => a.StartDate.ToUniversalTime() > DateTime.UtcNow.AddMinutes(-minutesToBackdate)).ToList();
            }
            else
            {
                Console.WriteLine($"Activities error: {JsonConvert.SerializeObject(response)}");
            }

            return null;
        }

        private async Task UpdateActivity(Activity activity, UpdateableActivity updateableActivity)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{Routes.UpdateActivityRoute}/{activity.Id}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(updateableActivity), System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await LogUpdate(activity);
            }
            else 
            { 
                Console.WriteLine($"Update error: {JsonConvert.SerializeObject(response)}");
            }
        }

        private async Task<bool> IsActivityUpdated(Activity activity)
        {
            var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            var table = Table.LoadTable(client, "eBikeActivityUpdater");

            var document = await table.GetItemAsync(activity.Id);

            return document != null && document["Id"].AsLong() == activity.Id;
        }

        private async Task LogUpdate(Activity activity)
        {
            var client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
            var table = Table.LoadTable(client, "eBikeActivityUpdater");
            var jsonText = JsonConvert.SerializeObject(activity);
            var item = Document.FromJson(jsonText);

            var result = await table.PutItemAsync(item);
        }

        private async Task NotifyUpdate(Activity activity)
        {
            var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast1);

            var request = new PublishRequest
            {
                Subject = $"Strava activity with start time {activity.StartDate} updated.",
                Message = $"https://www.strava.com/activities/{activity.Id}{Environment.NewLine}{Environment.NewLine}Original activity: {JsonConvert.SerializeObject(activity)}",
                TopicArn = "arn:aws:sns:us-east-1:540629508292:eBikeActivityUpdater",
            };

            var response = await client.PublishAsync(request);
        }
    }
}
