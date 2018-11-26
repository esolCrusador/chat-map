using ChatMap.Infrastructure;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Models
{
    public struct HttpClientContainer : IDisposable
    {
        private readonly DownloadClientsPool _downloadClientsPool;
        private readonly HttpClient _httpClient;

        public HttpClientContainer(DownloadClientsPool downloadClientsPool, HttpClient httpClient)
        {
            _downloadClientsPool = downloadClientsPool;
            _httpClient = httpClient;
        }


        public void Dispose()
        {
            _downloadClientsPool.Release(_httpClient);
        }

        public async Task<string> GetAsString(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
            using (HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken))
            {
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"The request {request.Method} {request.RequestUri} was not successful. The request failed with status {response.StatusCode} and response:\r\n{content}");

                return content;
            }
        }

        public async Task<string> RequestAsString(HttpMethod method, string url, string body, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (HttpRequestMessage request = new HttpRequestMessage(method, url) { Content = new StringContent(body) })
            using (HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken))
            {
                string content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"The request {request.Method} {request.RequestUri} was not successful. The request failed with status {response.StatusCode} and response:\r\n{content}");

                return content;
            }
        }
    }
}
