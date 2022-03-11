namespace _3ai.solutions._3CX.Models
{
    public class MeetingsListResponse : BaseResponse
    {
        public Result result { get; set; }
        public class Result
        {
            public Scheduledmeeting[] scheduledMeetings { get; set; }
        }
    }
}