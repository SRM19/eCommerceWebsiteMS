using static Foody.Web.Utils.Constants;

namespace Foody.Web.Models
{
    public class APIRequest
    {
        public RequestType Method { get; set; } = RequestType.GET;

        public string Url { get; set; }

        public object Data { get; set; }

        public string AccessToken { get; set; }
    }
}
