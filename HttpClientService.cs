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
            return await InvokePostAsync<AdhocResponse, string>($"webmeeTIng/api/v1/adhoc?email={email}&extension={extension}&subject={subject}", "");
        }

        public AdhocResponse Adhoc(string email, int extension, string subject)
        {
            return InvokePost<AdhocResponse, string>($"webmeeTIng/api/v1/adhoc?email={email}&extension={extension}&subject={subject}", "");
        }

        public async Task<BaseResponse> PostParticipantsAsync(string meeTIngId, List<Participant> participants)
        {
            return await InvokePostAsync<BaseResponse, List<Participant>>($"webmeeTIng/api/v1/participants/{meeTIngId}", participants);
        }

        public BaseResponse PostParticipants(string meeTIngId, List<Participant> participants)
        {
            return InvokePost<BaseResponse, List<Participant>>($"webmeeTIng/api/v1/participants/{meeTIngId}", participants);
        }

        //public async Task<BaseResponse> DeleteParticipantsAsync(string meeTIngId, List<string> emails)
        //{
        //    return await InvokeDeleteAsync<BaseResponse>($"webmeeTIng/api/v1/participants/{meeTIngId}");
        //}

        public BaseResponse DeleteParticipants(string meeTIngId, List<string> emails)
        {
#if NETSTANDARD
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(emails);
#elif NET
            var data = System.Text.Json.JsonSerializer.Serialize(emails);
#endif
            return InvokeDelete<BaseResponse>($"webmeeTIng/api/v1/participants/{meeTIngId}", data);
        }

        public async Task<ScheduledResponse> PostScheduledAsync(ScheduledRequest request)
        {
            return await InvokePostAsync<ScheduledResponse, ScheduledRequest>($"webmeeTIng/api/v1/scheduled", request);
        }

        public ScheduledResponse PostScheduled(ScheduledRequest request)
        {
            return InvokePost<ScheduledResponse, ScheduledRequest>($"webmeeTIng/api/v1/scheduled", request);
        }

        public async Task<BaseResponse> DeleteScheduledAsync(string meeTIngId)
        {
            return await InvokeDeleteAsync<BaseResponse>($"webmeeTIng/api/v1/scheduled/{meeTIngId}");
        }

        public BaseResponse DeleteScheduled(string meeTIngId)
        {
            return InvokeDelete<BaseResponse>($"webmeeTIng/api/v1/scheduled/{meeTIngId}");
        }

        public async Task<ScheduledResponse> GetScheduledAsync(string meeTIngId)
        {
            return await InvokeGetAsync<ScheduledResponse>($"webmeeTIng/api/v1/scheduled/{meeTIngId}");
        }

        public ScheduledResponse GetScheduled(string meeTIngId)
        {
            return InvokeGet<ScheduledResponse>($"webmeeTIng/api/v1/scheduled/{meeTIngId}");
        }

        public MeeTIngsListResponse MeeTIngList(string subjectContains = "", int daysLimit = 0, int extension = 0)
        {
            string requestUri = "webmeeTIng/api/v1/meeTIngs/list?";
            if (!string.IsNullOrEmpty(subjectContains))
                requestUri += $"subjectContains={subjectContains}&";
            if (daysLimit != 0)
                requestUri += $"daysLimit={daysLimit}&";
            if (extension != 0)
                requestUri += $"extension={extension}&";
            return InvokeGet<MeeTIngsListResponse>(requestUri);
        }

        public async Task<MeeTIngsListResponse> MeeTIngListAsync(string subjectContains = "", int daysLimit = 0, int extension = 0)
        {
            string requestUri = "webmeeTIng/api/v1/meeTIngs/list?";
            if (!string.IsNullOrEmpty(subjectContains))
                requestUri += $"subjectContains={subjectContains}&";
            if (daysLimit != 0)
                requestUri += $"daysLimit={daysLimit}&";
            if (extension != 0)
                requestUri += $"extension={extension}&";
            return await InvokeGetAsync<MeeTIngsListResponse>(requestUri);
        }

        public MeeTIngsActiveResponse MeeTIngActive()
        {
            string requestUri = "webmeeTIng/api/v1/meeTIngs/active";
            return InvokeGet<MeeTIngsActiveResponse>(requestUri);
        }

        public async Task<CountparticipantsResponse> CountParticipantsAsync(string meeTIngId)
        {
            string requestUri = $"webmeeTIng/api/v1/countparticipants/{meeTIngId}";
            return await InvokeGetAsync<CountparticipantsResponse>(requestUri);
        }

        public CountparticipantsResponse CountParticipants(string meeTIngId)
        {
            string requestUri = $"webmeeTIng/api/v1/countparticipants/{meeTIngId}";
            return InvokeGet<CountparticipantsResponse>(requestUri);
        }

#if NETSTANDARD
        private TOut InvokePost<TOut, TIn>(string requestUri, TIn obj)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            using (var webClient = new System.Net.WebClient
            {
                BaseAddress = _uri
            })
            {
                webClient.Headers.Add("3CX-ApiKey", _apiKey);
                webClient.Headers.Add("content-type", "application/json");
                var resContent = webClient.UploadString(requestUri, "POST", data);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TOut>(resContent);
            }
        }
#endif

        private async Task<TOut> InvokePostAsync<TOut, TIn>(string requestUri, TIn obj)
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
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TOut>(resContent);
#elif NET
            return System.Text.Json.JsonSerializer.Deserialize<TOut>(resContent);
#endif
        }

        private async Task<TOut> InvokeGetAsync<TOut>(string requestUri)
        {
            var res = await _httpClient.GetAsync(requestUri);
            var resContent = await res.Content.ReadAsStringAsync();
#if NETSTANDARD
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TOut>(resContent);
#elif NET
            return System.Text.Json.JsonSerializer.Deserialize<TOut>(resContent);
#endif
        }

#if NETSTANDARD
        private TOut InvokeGet<TOut>(string requestUri)
        {
            using (var webClient = new System.Net.WebClient
            {
                BaseAddress = _uri
            })
            {
                webClient.Headers.Add("3CX-ApiKey", _apiKey);
                webClient.Headers.Add("content-type", "application/json");
                var resContent = webClient.DownloadString(requestUri);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<TOut>(resContent);
            }
        }
#endif

        private async Task<TOut> InvokeDeleteAsync<TOut>(string requestUri)
        {
            var res = await _httpClient.DeleteAsync(requestUri);
            var resContent = await res.Content.ReadAsStringAsync();
#if NETSTANDARD
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TOut>(resContent);
#elif NET
            return System.Text.Json.JsonSerializer.Deserialize<TOut>(resContent);
#endif
        }

#if NETSTANDARD
        private TOut InvokeDelete<TOut>(string requestUri, string data = "")
        {
            using (var webClient = new System.Net.WebClient
            {
                BaseAddress = _uri
            })
            {
                webClient.Headers.Add("3CX-ApiKey", _apiKey);
                webClient.Headers.Add("content-type", "application/json");
                var resContent = webClient.UploadString(requestUri, "DELETE", data);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TOut>(resContent);
            }
        }
#endif

        public void Dispose()
        {
            _httpClient?.Dispose();
            System.GC.SuppressFinalize(this);
        }
    }
}
