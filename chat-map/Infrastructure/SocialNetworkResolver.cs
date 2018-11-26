using ChatMap.Infrastructure.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ChatMap.Infrastructure
{
    public class SocialNetworkResolver
    {
        private readonly Dictionary<Contracts.SocialNetwork, ISocialNetworkStrategy> _socialNetworks;
        public SocialNetworkResolver(IEnumerable<ISocialNetworkStrategy> socialNetworks)
        {
            _socialNetworks = socialNetworks.ToDictionary(sn => sn.SocialNetwork, sn => sn);
        }

        public ISocialNetworkStrategy Resolve(Contracts.SocialNetwork socialNetwork)
        {
            return _socialNetworks[socialNetwork];
        }
    }
}
