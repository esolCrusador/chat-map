using ChatMap.Models;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Infrastructure
{
    public interface IDownloadClientsPool
    {
        Task<HttpClientContainer> Consume(CancellationToken cancellationToken = default(CancellationToken));
        void Release(HttpClient httpClient);
    }
}
