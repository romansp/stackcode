using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using stackcode.Serialization;
using stackcode.Services;

namespace stackcode.Models.Twitter
{
    public class Status
    {
        [JsonProperty("created_at")]
        [JsonConverter(typeof(TwitterDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("id_str")]
        public string IdStr { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("truncated")]
        public bool Truncated { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("entities")]
        public Entities Entities { get; set; }
    }
}