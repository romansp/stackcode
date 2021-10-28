using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace parse_stackcode
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stackCodes = await ParseStackCodesAsync();

            if (stackCodes.Any())
            {
                var directory = Directory.CreateDirectory("../../../../../_posts");
                WriteStackCodes(stackCodes, directory);
            }
        }

        private static async Task<List<StackCode>> ParseStackCodesAsync()
        {
            using var tweetsDump = File.OpenRead("stackcode-dump.html");
            using var tagsFile = File.OpenRead("tags.json");
            var tagLookup = await JsonSerializer.DeserializeAsync<StackCodeTags>(tagsFile, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var preTitle = "A daily screenshot from the Stack Overflow codebase";
            var titleRegex = new Regex(@"((A daily screenshot from the Stack Overflow codebase \((.+)\).+)#StackCode)");
            var stackCodes = new List<StackCode>();

            var document = new HtmlDocument();
            document.Load(tweetsDump);
            var page = document.DocumentNode;
            var tweets = page.QuerySelectorAll("li.stream-item");

            foreach (var tweet in tweets)
            {
                var timeNode = tweet.QuerySelector(".time");
                var tweetTextNode = tweet.QuerySelector(".tweet-text");
                var tweetId = tweet.Attributes["data-item-id"].Value;

                var tweetText = tweetTextNode.InnerText;
                if (tweetText.Contains(preTitle))
                {
                    var timestampNode = timeNode.QuerySelector("._timestamp");
                    var timestamp = timestampNode.Attributes["data-time"].Value;

                    var titleMatch = titleRegex.Match(tweetText);
                    var text = WebUtility.HtmlDecode(titleMatch.Groups[2].Value);
                    var title = WebUtility.HtmlDecode(titleMatch.Groups[3].Value);
                    title = FirstLetterToUpper(title);

                    var thumbnailNode = tweetTextNode.QuerySelector(".twitter-timeline-link");
                    var thumbnailhandle = thumbnailNode.InnerText.Split('/')[1];

                    stackCodes.Add(new StackCode
                    {
                        Layout = "stackcode",
                        Name = "Nick Craver",
                        TwitterHandle = "Nick_Craver",
                        Title = title,
                        Text = text,
                        ThumbnailHandle = thumbnailhandle,
                        Tweet = $"https://twitter.com/Nick_Craver/status/{tweetId}",
                        Date = UnixTimeStampToDateTime(double.Parse(timestamp)),
                        Tags = BuildTags(tweetId, tagLookup.TweetsToTags)
                    });
                }
            }

            return stackCodes;
        }

        private static IEnumerable<string> BuildTags(string tweetId, Dictionary<string, List<string>> tagLookup)
        {
            return tagLookup.TryGetValue(tweetId, out var tags) ? tags : new[] { "not-mapped" };
        }

        private static void WriteStackCodes(List<StackCode> stackCodes, DirectoryInfo outputDirectory)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithIndentedSequences()
                .Build();

            foreach (var stackCode in stackCodes)
            {
                var normalizedTitle = NormalizeTitle(stackCode);

                if (!string.IsNullOrEmpty(normalizedTitle)) {
                    normalizedTitle = $"-{normalizedTitle}";
                }

                var fileName = Path.Combine(outputDirectory.FullName, $"{stackCode.Date:yyyy-MM-dd}{normalizedTitle}.md");

                var yaml = serializer.Serialize(stackCode);
                var contents = 
$@"---
{yaml}
---
";
                File.WriteAllText(fileName, contents);
            }
        }

        private static string NormalizeTitle(StackCode stackCode)
        {
            var lowercase = stackCode.Title.ToLowerInvariant();
            var replaced = lowercase
                .Replace(" ", "-")
                .Replace(",", string.Empty)
                .Replace("'", string.Empty)
                .Replace("’", string.Empty)
                .Replace("“", string.Empty)
                .Replace("”", string.Empty)
                ;
            return GetSafeFilename(replaced);
        }

        public static string GetSafeFilename(string filename)
        {
            return string.Join(string.Empty, filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }

    internal class StackCode
    {
        public string Layout { get; set; }
        public string Name { get; set; }
        public string TwitterHandle { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ThumbnailHandle { get; set; }
        public string Tweet { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
