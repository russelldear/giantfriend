using Newtonsoft.Json;

namespace eBikeActivityUpdater.Models
{
    public partial class UpdateableActivity
    {
        [JsonProperty("commute")]
        public bool Commute { get; set; }

        [JsonProperty("trainer")]
        public bool Trainer { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("gear_id")]
        public string GearId { get; set; }
    }
}
