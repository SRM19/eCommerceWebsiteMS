using Foody.Web.Models;
using Foody.Web.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Foody.Web.Services
{
    public class BaseService : IBaseService
    {
        public ResponseDto response { get; set; }

        public IHttpClientFactory httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient;
            this.response = new ResponseDto();
        }
      
        public async Task<T> SendRequestAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("FoodyAPI");
                HttpRequestMessage httpRequest = new HttpRequestMessage();
                httpRequest.Headers.Add("Accept", "application/json");
                httpRequest.RequestUri = new Uri(apiRequest.Url);
                if(apiRequest.Data != null)
                {
                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        System.Text.Encoding.UTF8,"application/json");
                }

                if (!string.IsNullOrEmpty(apiRequest.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",apiRequest.AccessToken);
                }

                HttpResponseMessage httpResponse = null;
                switch (apiRequest.Method)
                {
                    case Utils.Constants.RequestType.POST:
                        httpRequest.Method = HttpMethod.Post;
                        break;
                    case Utils.Constants.RequestType.PUT:
                        httpRequest.Method = HttpMethod.Put;
                        break;
                    case Utils.Constants.RequestType.DELETE:
                        httpRequest.Method = HttpMethod.Delete;
                        break;
                    default: httpRequest.Method = HttpMethod.Get;
                        break;
                }
                

                httpResponse = await client.SendAsync(httpRequest);
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(responseContent);
                return result;

            }catch (Exception ex)
            {
                var err = new ResponseDto
                {
                    DisplayMessage = "Error",
                    ErrorMessage = new List<string> { ex.Message },
                    IsSuccess = false
                };
                var responseObj = JsonConvert.SerializeObject(err);
                var errResponse = JsonConvert.DeserializeObject<T>(responseObj);
                return errResponse;
            }
            
        }
        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
