using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace InternetBanking.Services.API
{
    public class AbacusApiService : IAbacusApiService
    {
        private static readonly HttpClient _httpClient;

        private static readonly string BaseUrl = "https://api.abacushub.io";

        static AbacusApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                Device.RuntimePlatform == Device.iOS ? "MembersAccess/iOS" : "MembersAccess/Android");

            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public AbacusApiService(string apiToken)
        {
            if(!_httpClient.DefaultRequestHeaders.Contains("X-Fern-Token"))
            { 
                _httpClient.DefaultRequestHeaders.Add("X-Fern-Token", apiToken);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string resource)
        {
            try
            {
                return await _httpClient
                    .GetAsync(resource)
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string resource, IDictionary<string, string> parameters)
        {
            try
            {
                return await _httpClient
                    .GetAsync(ParseParameters(resource, parameters))
                    .ConfigureAwait(false);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string resource, string body)
        {
            try
            {
                return await _httpClient
                    .PostAsync(resource, new StringContent(body, Encoding.UTF8, "application/json"))
                    .ConfigureAwait(false);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> PutAsync(string resource, string body)
        {
            try
            {
                return await _httpClient
                    .PutAsync(resource, new StringContent(body, Encoding.UTF8, "application/json"))
                    .ConfigureAwait(false);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string resource)
        {
            try
            {
                return await _httpClient
                    .DeleteAsync(resource)
                    .ConfigureAwait(false);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private static string ParseParameters(string resource, IDictionary<string, string> parameters)
        {
            var stringBuilder = new StringBuilder(resource);

            if (parameters.Keys.Count > 0)
            {
                stringBuilder.Append("?");
            }

            foreach (var kvp in parameters)
            {
                stringBuilder.Append($"{kvp.Key}={kvp.Value}&");
            }

            var result = stringBuilder.ToString();

            return result.EndsWith("&", StringComparison.Ordinal) ? result.Substring(0, result.Length - 1) : result;
        }
    }
}