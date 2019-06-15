namespace eBikeActivityUpdater
{
    public class Constants
    {
        public static class Routes
        {
            public const string TokenRoute = "api/v3/oauth/token";
            public const string GetActivitiesRoute = "api/v3/athlete/activities?page=1&per_page=5";
            public const string UpdateActivityRoute = "api/v3/activities";
        }

        public static class Headers
        {
            public const string Authorization = "Authorization";
        }

        public static class EnvironmentVariables
        {
            public const string ClientId = "ClientId";
            public const string ClientSecret = "ClientSecret";
            public const string RefreshToken = "RefreshToken";
            public const string BackdateMinutes = "BackdateMinutes";
            public const string ActivityType = "ActivityType";
            public const string GearId = "GearId";
        }

        public static class FormatStrings
        {
            public const string QueryParamFormat = "client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token";
        }
    }
}
