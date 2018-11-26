using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Infrastructure.Contracts
{
    public interface ISocialNetworkStrategy
    {
        SocialNetwork SocialNetwork { get; }
        Task<string> GetProfileUrl(string userName, CancellationToken cancellationToken = default(CancellationToken));
    }
}
