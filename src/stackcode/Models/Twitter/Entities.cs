using Newtonsoft.Json;

namespace stackcode.Models.Twitter
{
    public class Entities
    {
        [JsonProperty("hashtags")]
        public Hashtag[] Hashtags { get; set; }

        [JsonProperty("media")]
        public Media[] Media { get; set; }
    }
}