using Newtonsoft.Json;

namespace stackcode.Models.Twitter
{
    public class Size
    {
        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("resize")]
        public string Resize { get; set; }
    }
}