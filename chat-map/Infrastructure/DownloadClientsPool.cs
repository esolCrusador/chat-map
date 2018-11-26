using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChatMap.Models;

namespace ChatMap.Infrastructure
{
    public class DownloadClientsPool: IDownloadClientsPool, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _semaphoreSlim;

        public DownloadClientsPool(ConfigurationService configurationService)
        {
            _semaphoreSlim = new SemaphoreSlim(configurationService.MaxDownloads);
            _httpClient = new HttpClient();
        }

        public async Task<HttpClientContainer> Consume(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);

            return new HttpClientContainer(this, _httpClient);
        }

        public void Release(HttpClient httpClient)
        {
            _semaphoreSlim.Release();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _semaphoreSlim.Dispose();
        }
    }
}
