using Newtonsoft.Json;

namespace stackcode.Models.Twitter
{
    public class Sizes
    {
        [JsonProperty("small")]
        public Size Small { get; set; }

        [JsonProperty("medium")]
        public Size Medium { get; set; }

        [JsonProperty("thumb")]
        public Size Thumb { get; set; }

        [JsonProperty("large")]
        public Size Large { get; set; }
    }
}