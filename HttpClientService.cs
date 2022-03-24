using _3ai.solutions._3CX.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace _3ai.solutions._3CX
{
    //https://documenter.getpostman.com/view/17439253/UUxzBnmk
    public class HttpClientService : System.IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _uri;
        private readonly string _apiKey;

        public HttpClientService(string uri, string apiKey)
        {
            _uri = uri;
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(uri)
            };
            _httpClient.DefaultRequestHeaders.Add("3CX-ApiKey", apiKey);
        }

        public async Task<AdhocResponse> AdhocAsync(string email, int extension, string subject)
        {
            return await InvokePostAsync<AdhocResponse, string>($"webmeeting/api/v1/adhoc?email={email}&extension={extension}&subject={subject}", "");
        }

        public AdhocResponse Adhoc(string email, int extension, string subject)
        {
            return InvokePost<AdhocResponse, string>($"webmeeting/api/v1/adhoc?email={email}&extension={extension}&subject={subject}", "");
        }

        public async Task<BaseResponse> PostParticipantsAsync(string meetingId, List<Participant> participants)
        {
            return await InvokePostAsync<BaseResponse, List<Participant>>($"webmeeting/api/v1/participants/{meetingId}", participants);
        }

        public BaseResponse PostParticipants(string meetingId, List<Participant> participants)
        {
            return InvokePost<BaseResponse, List<Participant>>($"webmeeting/api/v1/participants/{meetingId}", participants);
        }

        //public async Task<BaseResponse> DeleteParticipantsAsync(string meetingId, List<string> emails)
        //{
        //    return await InvokeDeleteAsync<BaseResponse>($"webmeeting/api/v1/participants/{meetingId}");
        //}

        public BaseResponse DeleteParticipants(string meetingId, List<string> emails)
        {
#if NETSTANDARD
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(emails);
#elif NET
            var data = System.Text.Json.JsonSerializer.Serialize(emails);
#endif
            return InvokeDelete<BaseResponse>($"webmeeting/api/v1/participants/{meetingId}", data);
        }

        public async Task<ScheduledResponse> PostScheduledAsync(ScheduledRequest request)
        {
            return await InvokePostAsync<ScheduledResponse, ScheduledRequest>($"webmeeting/api/v1/scheduled", request);
        }

        public ScheduledResponse PostScheduled(ScheduledRequest request)
        {
            return InvokePost<ScheduledResponse, ScheduledRequest>($"webmeeting/api/v1/scheduled", request);
        }

        public async Task<BaseResponse> DeleteScheduledAsync(string meetingId)
        {
            return await InvokeDeleteAsync<BaseResponse>($"webmeeting/api/v1/scheduled/{meetingId}");
        }

        public BaseResponse DeleteScheduled(string meetingId)
        {
            return InvokeDelete<BaseResponse>($"webmeeting/api/v1/scheduled/{meetingId}");
        }

        public async Task<ScheduledResponse> GetScheduledAsync(string meetingId)
        {
            return await InvokeGetAsync<ScheduledResponse>($"webmeeting/api/v1/scheduled/{meetingId}");
        }

        public ScheduledResponse GetScheduled(string meetingId)
        {
            return InvokeGet<ScheduledResponse>($"webmeeting/api/v1/scheduled/{meetingId}");
        }

        public MeetingsListResponse MeetingList(string subjectContains = "", int daysLimit = 0, int extension = 0)
        {
            string requestUri = "webmeeting/api/v1/meetings/list?";
            if (!string.IsNullOrEmpty(subjectContains))
                requestUri += $"subjectContains={subjectContains}&";
            if (daysLimit != 0)
                requestUri += $"daysLimit={daysLimit}&";
            if (extension != 0)
                requestUri += $"extension={extension}&";
            return InvokeGet<MeetingsListResponse>(requestUri);
        }

        public async Task<MeetingsListResponse> MeetingListAsync(string subjectContains = "", int daysLimit = 0, int extension = 0)
        {
            string requestUri = "webmeeting/api/v1/meetings/list?";
            if (!string.IsNullOrEmpty(subjectContains))
                requestUri += $"subjectContains={subjectContains}&";
            if (daysLimit != 0)
                requestUri += $"daysLimit={daysLimit}&";
            if (extension != 0)
                requestUri += $"extension={extension}&";
            return await InvokeGetAsync<MeetingsListResponse>(requestUri);
        }

        public MeetingsActiveResponse MeetingActive()
        {
            string requestUri = "webmeeting/api/v1/meetings/active";
            return InvokeGet<MeetingsActiveResponse>(requestUri);
        }

        public async Task<CountparticipantsResponse> CountParticipantsAsync(string meetingId)
        {
            string requestUri = $"webmeeting/api/v1/countparticipants/{meetingId}";
            return await InvokeGetAsync<CountparticipantsResponse>(requestUri);
        }

        public CountparticipantsResponse CountParticipants(string meetingId)
        {
            string requestUri = $"webmeeting/api/v1/countparticipants/{meetingId}";
            return InvokeGet<CountparticipantsResponse>(requestUri);
        }

        private Tout InvokePost<Tout, Tin>(string requestUri, Tin obj)
        {
#if NETSTANDARD
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
#elif NET
            var data = System.Text.Json.JsonSerializer.Serialize(obj);
#endif
            using (var webClient = new System.Net.WebClient
            {
                BaseAddress = _uri
            })
            {
                webClient.Headers.Add("3CX-ApiKey", _apiKey);
                webClient.Headers.Add("content-type", "application/json");
                var resContent = webClient.UploadString(requestUri, "POST", data);
#if NETSTANDARD
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Tout>(resContent);
#elif NET
                return System.Text.Json.JsonSerializer.Deserialize<Tout>(resContent);
#endif
            }
        }

        private async Task<Tout> InvokePostAsync<Tout, Tin>(string requestUri, Tin obj)
        {
#if NETSTANDARD
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
#elif NET
            var data = System.Text.Json.JsonSerializer.Serialize(obj);
#endif
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync(requestUri, content);
            var resContent = await res.Content.ReadAsStringAsync();
#if NETSTANDARD
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Tout>(resContent);
#elif NET
            return System.Text.Json.JsonSerializer.Deserialize<Tout>(resContent);
#endif
        }

        private async Task<Tout> InvokeGetAsync<Tout>(string requestUri)
        {
            var res = await _httpClient.GetAsync(requestUri);
            var resContent = await res.Content.ReadAsStringAsync();
#if NETSTANDARD
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Tout>(resContent);
#elif NET6_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Deserialize<Tout>(resContent);
#endif
        }

        private Tout InvokeGet<Tout>(string requestUri)
        {
            using (var webClient = new System.Net.WebClient
            {
                BaseAddress = _uri
            })
            {
                webClient.Headers.Add("3CX-ApiKey", _apiKey);
                webClient.Headers.Add("content-type", "application/json");
                var resContent = webClient.DownloadString(requestUri);
#if NETSTANDARD
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Tout>(resContent);
#elif NET6_0_OR_GREATER
                return System.Text.Json.JsonSerializer.Deserialize<Tout>(resContent);
#endif
            }
        }

        private async Task<Tout> InvokeDeleteAsync<Tout>(string requestUri)
        {
            var res = await _httpClient.DeleteAsync(requestUri);
            var resContent = await res.Content.ReadAsStringAsync();
#if NETSTANDARD
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Tout>(resContent);
#elif NET6_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Deserialize<Tout>(resContent);
#endif
        }

        private Tout InvokeDelete<Tout>(string requestUri, string data = "")
        {
            using (var webClient = new System.Net.WebClient
            {
                BaseAddress = _uri
            })
            {
                webClient.Headers.Add("3CX-ApiKey", _apiKey);
                webClient.Headers.Add("content-type", "application/json");
                var resContent = webClient.UploadString(requestUri, "DELETE", data);
#if NETSTANDARD
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Tout>(resContent);
#elif NET6_0_OR_GREATER
                return System.Text.Json.JsonSerializer.Deserialize<Tout>(resContent);
#endif
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            System.GC.SuppressFinalize(this);
        }
    }
}
