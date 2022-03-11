using System;
using System.Collections.Generic;
using System.Text;

namespace _3ai.solutions._3CX.Models
{
    public class ScheduledRequest
    {
        public bool hostJoinFirst { get; set; }
        public bool hideParticipants { get; set; }
        public bool selfModeration { get; set; }
        public bool audioOnStart { get; set; }
        public bool videoOnStart { get; set; }
        public bool chatOnStart { get; set; }
        public DateTime dateTime { get; set; }
        public int duration { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public string extension { get; set; }
        public string type { get; set; }
        public Participant[] participants { get; set; }
    }
}