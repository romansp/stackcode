using Newtonsoft.Json;

namespace stackcode.Models.Twitter
{
    public class Hashtag
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("indices")]
        public int[] Indices { get; set; }
    }
}