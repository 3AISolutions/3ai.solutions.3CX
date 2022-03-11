namespace _3ai.solutions._3CX.Models
{
    public class MeetingsActiveResponse : BaseResponse
    {
        public Result result { get; set; }
        public class Result
        {
            public int count { get; set; }
            public Click2meet[] Click2Meet { get; set; }
            public Scheduled[] Scheduled { get; set; }
        }
    }
}