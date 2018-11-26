using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ChatMap.Infrastructure
{
    public class ConfigurationService
    {
        private readonly IConfigurationRoot _configurationRoot;

        public ConfigurationService(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public string GoogleApiKey
        {
            get
            {
                return _configurationRoot.GetValue<string>("GoogleApiKey");
            }
        }

        public int MaxDownloads
        {
            get
            {
                return _configurationRoot.GetSection("AppSettings").GetValue<int>("MaxDownloads");
            }
        }
    }
}
