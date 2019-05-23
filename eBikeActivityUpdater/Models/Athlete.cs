using Newtonsoft.Json;

namespace eBikeActivityUpdater.Models
{
    public partial class Athlete
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("ResourceState")]
        public long ResourceState { get; set; }

        [JsonProperty("FirstName")]
        public object FirstName { get; set; }

        [JsonProperty("LastName")]
        public object LastName { get; set; }

        [JsonProperty("ProfileMedium")]
        public object ProfileMedium { get; set; }

        [JsonProperty("Profile")]
        public object Profile { get; set; }

        [JsonProperty("City")]
        public object City { get; set; }

        [JsonProperty("State")]
        public object State { get; set; }

        [JsonProperty("Country")]
        public object Country { get; set; }

        [JsonProperty("Sex")]
        public object Sex { get; set; }

        [JsonProperty("Friend")]
        public object Friend { get; set; }

        [JsonProperty("Follower")]
        public object Follower { get; set; }

        [JsonProperty("Premium")]
        public bool Premium { get; set; }

        [JsonProperty("CreatedAt")]
        public object CreatedAt { get; set; }

        [JsonProperty("UpdatedAt")]
        public object UpdatedAt { get; set; }

        [JsonProperty("FollowerCount")]
        public long FollowerCount { get; set; }

        [JsonProperty("FriendCount")]
        public long FriendCount { get; set; }

        [JsonProperty("MutualFriendCount")]
        public long MutualFriendCount { get; set; }

        [JsonProperty("Ftp")]
        public object Ftp { get; set; }

        [JsonProperty("Email")]
        public object Email { get; set; }

        [JsonProperty("MeasurementPreference")]
        public object MeasurementPreference { get; set; }

        [JsonProperty("Weight")]
        public object Weight { get; set; }
    }
}
