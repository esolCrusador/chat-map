using Microsoft.Extensions.Configuration;

namespace ChatMap.Services
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
    }
}
