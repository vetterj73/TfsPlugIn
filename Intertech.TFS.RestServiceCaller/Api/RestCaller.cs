using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.TFS.RestServiceCaller.Api
{
    public class RestCaller
    {
        private readonly string _baseUrl;
        public RestCaller(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<string> MakeHttpCall<T>(string apiUrl, T request, HttpMethod method) where T : new()
        {

            var handler = new HttpClientHandler { Credentials = new NetworkCredential(Constants.RestUserName, Constants.RestPassword, Constants.RestDomain) };


            using (var client = new HttpClient(handler))
            {
                HttpResponseMessage response;
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (method == HttpMethod.Delete)
                    response = await client.DeleteAsync(apiUrl);
                else if (method == HttpMethod.Get)
                    response = await client.GetAsync(apiUrl);
                else if (method == HttpMethod.Post)
                    response = await client.PostAsJsonAsync(apiUrl, request);
                else if (method == HttpMethod.Put)
                    response = await client.PutAsJsonAsync(apiUrl, request);
                else
                    return null;

                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}