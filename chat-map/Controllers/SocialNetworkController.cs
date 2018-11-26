using ChatMap.Infrastructure;
using ChatMap.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace chat_map.Controllers
{
    [Route("api/social-network")]
    [ApiController]
    public class SocialNetworkController : ControllerBase
    {
        private readonly SocialNetworkResolver _socialNetworkResolver;

        public SocialNetworkController(SocialNetworkResolver socialNetworkResolver)
        {
            _socialNetworkResolver = socialNetworkResolver;
        }

        [HttpGet("avatar/{socialNetwork}/{userName}")]
        public async Task<string> GetAvatar([FromRoute]SocialNetwork socialNetwork, [FromRoute]string userName, CancellationToken cancellationToken)
        {
            var strategy = _socialNetworkResolver.Resolve(socialNetwork);

            return await strategy.GetProfileUrl(userName, cancellationToken);
        }
    }
}