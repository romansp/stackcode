using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using stackcode.Models;
using stackcode.Models.Twitter;
using stackcode.Models.Twitter.Responses;
using System.Linq;

namespace stackcode.Services
{
    public class TwitterStackCodesService : IStackCodesService
    {
        private readonly IMemoryCache _cache;
        private readonly AppSettings _appSettings;

        public TwitterStackCodesService(IOptions<AppSettings> appSettings, IMemoryCache cache)
        {
            _cache = cache;
            _appSettings = appSettings.Value;
        }

        protected Task<string> TwitterAccessToken => GetTwitterAccessTokenAsync();

        public async Task<Status[]> GetStackCodesFromSearchAsync()
        {
            var query = WebUtility.UrlEncode("#StackCode from:nick_craver");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await TwitterAccessToken);
            var responseString = await httpClient.GetStringAsync($"https://api.twitter.com/1.1/search/tweets.json?q={query}");

            var searchResponse = JsonConvert.DeserializeObject<TwitterSearchResponse>(responseString);
            return searchResponse.Statuses;
        }

        public async Task<Status[]> GetStackCodesFromTimelineAsync(long? maxId = null)
        {
            var result = new List<Status>();

            var nickCraverId = 95030839;
            var firstStackCodeTweetId = 717292620175826946;
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await TwitterAccessToken);
            var templateUrl = $"https://api.twitter.com/1.1/statuses/user_timeline.json?user_id={nickCraverId}&since_id={firstStackCodeTweetId}";

            var url = templateUrl;
            if (maxId != null)
            {
                url = url + $"&max_id={maxId}";
            }
            
            while (result.Count < 20)
            {
                var responseString = await httpClient.GetStringAsync(url);
                var statuses = JsonConvert.DeserializeObject<Status[]>(responseString);

                foreach (var status in statuses)
                {
                    if (status.Entities.Hashtags.Any(c => c.Text == "StackCode") && status.Entities?.Media?.Any() != null)
                    {
                        result.Add(status);
                    }
                }

                var lastStatus = statuses.OrderBy(d => d.Id).FirstOrDefault();
                if (lastStatus != null)
                {
                    url = templateUrl + $"&max_id={lastStatus.Id}";
                }
            }

            return result.ToArray();
        }

        private async Task<string> GetTwitterAccessTokenAsync()
        {
            string twitterAccessToken;

            if (_cache.TryGetValue("twitterAccessToken", out twitterAccessToken))
                return twitterAccessToken;

            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token");
            var authInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes($"{_appSettings.TwitterApiKey}:{_appSettings.TwitterApiSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await httpClient.SendAsync(request);
            var jsonToken = await response.Content.ReadAsStringAsync();
            dynamic token = JsonConvert.DeserializeObject(jsonToken);
            twitterAccessToken = token.access_token.ToString();

            _cache.Set("twitterAccessToken", twitterAccessToken, TimeSpan.FromMinutes(15));

            return twitterAccessToken;
        }
    }
}