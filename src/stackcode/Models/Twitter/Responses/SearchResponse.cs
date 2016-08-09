using System.Collections.Generic;

namespace stackcode.Models.Twitter.Responses
{
    public class TwitterSearchResponse
    {
        public Status[] Statuses { get; set; }
        public SearchMetadata SearchMetadata { get; set; }
    }
}