using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;

namespace ChatMap.Infrastructure.SocialNetwork.Instagram
{
    public class InstagramResponseDto
    {
        [JsonProperty("user")]
        public ProfileInfoDto User { get; set; }
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public HttpStatusCode Status { get; set; }
    }
}
