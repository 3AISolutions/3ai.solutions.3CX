namespace _3ai.solutions._3CX.Models
{
    public class Scheduled
    {
        public string meetingid { get; set; }
        public string type { get; set; }
        public string openlink { get; set; }
        public string pin { get; set; }
        public string dialIn { get; set; }
        public bool hostJoinFirst { get; set; }
        public bool selfModeration { get; set; }
        public bool hideParticipants { get; set; }
        public bool audioOnStart { get; set; }
        public bool videoOnStart { get; set; }
        public bool chatOnStart { get; set; }
        public string datetime { get; set; }
        public int duration { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public Organizer organizer { get; set; }
        public Participant[] participants { get; set; }
    }
}