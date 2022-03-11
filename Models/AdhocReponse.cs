namespace _3ai.solutions._3CX.Models
{
    public class AdhocReponse : BaseResponse
    {
        public Result result { get; set; }
        public class Result
        {
            public string meetingid { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public string owner { get; set; }
        }
    }
}