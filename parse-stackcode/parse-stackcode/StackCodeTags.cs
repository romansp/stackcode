using System.Collections.Generic;

namespace parse_stackcode
{
    internal class StackCodeTags
    {
        public List<string> AllTags { get; set; }
        public Dictionary<string, List<string>> TweetsToTags { get; set; }
    }
}