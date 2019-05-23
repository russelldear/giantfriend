using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using eBikeActivityUpdater.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace eBikeActivityUpdater
{
    public class Function
    {
        private string _accessToken;

        private HttpClient _httpClient = new HttpClient() { BaseAddress = new Uri("https://www.strava.com/") };

        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            await EnsureAccessToken();

            var activities = await GetRecentActivities();

            Console.WriteLine(JsonConvert.SerializeObject(activities));

            foreach (var activity in activities)
            {
                var updateableActivity = activity.ToUpdateable();

                await UpdateActivity(activity.Id, updateableActivity);
            }

            return input?.ToUpper();
        }

        private async Task EnsureAccessToken()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/v3/oauth/token?client_id={Environment.GetEnvironmentVariable("ClientId")}&client_secret={Environment.GetEnvironmentVariable("ClientSecret")}&refresh_token={Environment.GetEnvironmentVariable("RefreshToken")}&grant_type=refresh_token");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var refreshTokenString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Refresh token string: {refreshTokenString}");

                var refreshTokenResponse = JsonConvert.DeserializeObject<RefreshTokenResponse>(refreshTokenString);

                _accessToken = refreshTokenResponse.AccessToken;

                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
            }
            else
            {
                Console.WriteLine($"Refresh token error: {JsonConvert.SerializeObject(response)}");
            }
        }

        private async Task<List<Activity>> GetRecentActivities()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/v3/athlete/activities");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var activitiesString = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Activities string: {activitiesString}");

                var activities = JsonConvert.DeserializeObject<List<Activity>>(activitiesString);

                return activities.Where(a => a.StartDateLocal.ToUniversalTime() > DateTime.UtcNow.AddHours(-1)).ToList();
            }
            else
            {
                Console.WriteLine($"Activities error: {JsonConvert.SerializeObject(response)}");
            }

            return null;
        }

        private async Task UpdateActivity(long activityId, UpdateableActivity activity)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/v3/activities/{activityId}")
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
