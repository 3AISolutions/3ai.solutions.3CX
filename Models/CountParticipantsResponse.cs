namespace _3ai.solutions._3CX.Models
{
    public class CountparticipantsResponse : BaseResponse
    {
        public Result result { get; set; }
        public class Result
        {
            public string meetingid { get; set; }
            public int participants { get; set; }
        }
    }
}