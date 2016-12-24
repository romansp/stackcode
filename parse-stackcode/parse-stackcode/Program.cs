using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace parse_stackcode
{
    class Program
    {
        private const string Pretitle = "A daily screenshot from the Stack Overflow codebase";
        private static readonly Regex TitleRegex = new Regex(@"((A daily screenshot from the Stack Overflow codebase \((.+)\).+)#StackCode)");

        static void Main(string[] args)
        {
            var stackCodes = new List<StackCode>();
            using (var file = File.OpenRead("stackcode-dump.html"))
            {
                var document = new HtmlDocument();
                document.Load(file);
                var page = document.DocumentNode;
                var headerNodes = page.QuerySelectorAll(".stream-item-header");
                foreach (var headerNode in headerNodes)
                {
                    var timeNode = headerNode.QuerySelector(".time");
                    var textNode = headerNode.NextSibling;
                    var tweetNode = textNode.NextSibling;

                    var tweet = tweetNode.InnerText;
                    if (tweet.Contains(Pretitle))
                    {
                        var timestampNode = timeNode.QuerySelector("._timestamp");
                        var tweetTimestampNode = timeNode.QuerySelector(".tweet-timestamp");
                        var timestamp = timestampNode.Attributes["data-time"].Value;

                        var titleMatch = TitleRegex.Match(tweet);
                        var text = titleMatch.Groups[2].Value;
                        var title = FirstLetterToUpper(titleMatch.Groups[3].Value);


                        var thumbnailNode = tweetNode.QuerySelector(".twitter-timeline-link");
                        var thumbnailhandle = thumbnailNode.InnerText.Split('/')[1];

                        stackCodes.Add(new StackCode
                        {
                            Layout = "stackcode",
                            Name = "Nick Craver",
                            Twitterhandle = "Nick_Craver",
                            Title = title,
                            Text = text,
                            Thumbnailhandle = thumbnailhandle,
                            Tweet = $"https://twitter.com{tweetTimestampNode.Attributes["href"].Value}",
                            Date = UnixTimeStampToDateTime(double.Parse(timestamp)),
                            Tags = new[] { "not-mapped" }
                        });
                    }
                }
            }
            foreach (var stackCode in stackCodes)
            {
                var directory = Directory.CreateDirectory("_posts");

                var safeForFilenameTitle = GetSafeFilename(NormalizeTitle(stackCode));

                var fileName = Path.Combine(directory.Name, $"{stackCode.Date.ToString("yyyy-MM-dd")}-{safeForFilenameTitle}.md");
                File.WriteAllText(fileName, $@"---
layout: {stackCode.Layout}
name: {stackCode.Name}
twitterhandle: {stackCode.Twitterhandle}
title: ""{stackCode.Title}.""
text: ""{stackCode.Text}""
thumbnailhandle: {stackCode.Thumbnailhandle}
tweet: {stackCode.Tweet}
date: {stackCode.Date:yyyy-MM-dd HH:mm:ss+0000}
tags: [""{stackCode.Tags[0]}""]
---
");
            }
        }

        private static string NormalizeTitle(StackCode stackCode)
        {
            var lowercase = stackCode.Title.ToLowerInvariant();
            return lowercase
                .Replace(" ", "-")
                .Replace(",", "");
        }

        public static string GetSafeFilename(string filename)
        {

            return string.Join("", filename.Split(Path.GetInvalidFileNameChars()));

        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
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
        public string Twitterhandle { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Thumbnailhandle { get; set; }
        public string Tweet { get; set; }
        public DateTime Date { get; set; }
        public string[] Tags { get; set; }
    }
}
