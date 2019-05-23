using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using eBikeActivityUpdater.Models;
using static eBikeActivityUpdater.Constants;
using static eBikeActivityUpdater.Constants.EnvironmentVariables;
using static eBikeActivityUpdater.Constants.FormatStrings;
using static eBikeActivityUpdater.Constants.Headers;
using static eBikeActivityUpdater.Constants.Routes;

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
                var updateableActivity = activity.ToUpdateable();

                await UpdateActivity(activity.Id, updateableActivity);

                Console.WriteLine($"Activity with id {activity.Id} updated.");
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

                return activities.Where(a => a.StartDateLocal.ToUniversalTime() > DateTime.UtcNow.AddMinutes(-minutesToBackdate)).ToList();
            }
            else
            {
                Console.WriteLine($"Activities error: {JsonConvert.SerializeObject(response)}");
            }

            return null;
        }

        private async Task UpdateActivity(long activityId, UpdateableActivity activity)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{Routes.UpdateActivityRoute}/{activityId}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(activity), System.Text.Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Update error: {JsonConvert.SerializeObject(response)}");
            }
        }
    }
}
