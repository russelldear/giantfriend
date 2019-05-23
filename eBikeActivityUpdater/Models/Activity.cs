using System;
using Newtonsoft.Json;

namespace eBikeActivityUpdater.Models
{
    public partial class Activity
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("ResourceState")]
        public long ResourceState { get; set; }

        [JsonProperty("ExternalId")]
        public string ExternalId { get; set; }

        [JsonProperty("UploadId")]
        public string UploadId { get; set; }

        [JsonProperty("Athlete")]
        public Athlete Athlete { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public object Description { get; set; }

        [JsonProperty("Distance")]
        public double Distance { get; set; }

        [JsonProperty("MovingTime")]
        public long MovingTime { get; set; }

        [JsonProperty("ElapsedTime")]
        public long ElapsedTime { get; set; }

        [JsonProperty("TotalElevationGain")]
        public double TotalElevationGain { get; set; }

        [JsonProperty("elev_high")]
        public double ElevHigh { get; set; }

        [JsonProperty("elev_low")]
        public double ElevLow { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("start_date_local")]
        public DateTime StartDateLocal { get; set; }

        [JsonProperty("Timezone")]
        public string Timezone { get; set; }

        [JsonProperty("AchievementCount")]
        public long AchievementCount { get; set; }

        [JsonProperty("PrCount")]
        public long PrCount { get; set; }

        [JsonProperty("KudosCount")]
        public long KudosCount { get; set; }

        [JsonProperty("CommentCount")]
        public long CommentCount { get; set; }

        [JsonProperty("AthleteCount")]
        public long AthleteCount { get; set; }

        [JsonProperty("photo_count")]
        public long PhotoCount { get; set; }

        [JsonProperty("TotalPhotoCount")]
        public long TotalPhotoCount { get; set; }

        [JsonProperty("Trainer")]
        public bool Trainer { get; set; }

        [JsonProperty("Commute")]
        public bool Commute { get; set; }

        [JsonProperty("Manual")]
        public bool Manual { get; set; }

        [JsonProperty("Private")]
        public bool Private { get; set; }

        [JsonProperty("DeviceName")]
        public object DeviceName { get; set; }

        [JsonProperty("EmbedToken")]
        public object EmbedToken { get; set; }

        [JsonProperty("Flagged")]
        public bool Flagged { get; set; }

        [JsonProperty("WorkoutType")]
        public object WorkoutType { get; set; }

        [JsonProperty("gear_id")]
        public string GearId { get; set; }

        [JsonProperty("AverageSpeed")]
        public double AverageSpeed { get; set; }

        [JsonProperty("MaxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonProperty("AverageCadence")]
        public long AverageCadence { get; set; }

        [JsonProperty("AverageTemp")]
        public long AverageTemp { get; set; }

        [JsonProperty("AverageWatts")]
        public double AverageWatts { get; set; }

        [JsonProperty("MaxWatts")]
        public long MaxWatts { get; set; }

        [JsonProperty("HasHeartrate")]
        public bool HasHeartrate { get; set; }

        [JsonProperty("Calories")]
        public long Calories { get; set; }

        [JsonProperty("SufferScore")]
        public long SufferScore { get; set; }
    }
}
