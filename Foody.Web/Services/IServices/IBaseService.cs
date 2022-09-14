using Foody.Web.Models;

namespace Foody.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
       ResponseDto response { get; set; }

        Task<T> SendRequestAsync<T>(APIRequest request);
    }
}
