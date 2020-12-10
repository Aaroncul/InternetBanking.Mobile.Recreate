using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InternetBanking.Services.API
{
    public interface IAbacusApiService
    {
        Task<HttpResponseMessage> GetAsync(string resource);

        Task<HttpResponseMessage> GetAsync(string resource, IDictionary<string, string> parameters);

        Task<HttpResponseMessage> PostAsync(string resource, string body);

        Task<HttpResponseMessage> PutAsync(string resource, string body);

        Task<HttpResponseMessage> DeleteAsync(string resource);
    }
}

