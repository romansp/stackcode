using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace stackcode.Models.Twitter.Responses
{
    public class TwitterTimelineResponse
    {
        public Status[] Statuses { get; set; }
    }
}
