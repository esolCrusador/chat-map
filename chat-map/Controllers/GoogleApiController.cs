using ChatMap.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace chat_map.Controllers
{
    [Route("api/google")]
    [ApiController]
    public class GoogleApiController : ControllerBase
    {
        private readonly ConfigurationService _configurationService;

        public GoogleApiController(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet("map.js")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 604800 /* 1 week */ )]
        public async Task<Stream> Map([FromQuery] string callbackMethod = "initMap")
        {
            using(var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(
                    $"https://maps.googleapis.com/maps/api/js?key={_configurationService.GoogleApiKey}&callback={callbackMethod}",
                    HttpCompletionOption.ResponseHeadersRead
                    );

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    throw new HttpRequestException($"Map request failed with Code={response.StatusCode} and content=\r\n{responseContent}");
                }

                Response.ContentType = "text/javascript";
                return await response.Content.ReadAsStreamAsync();
            }
        }

        [HttpGet("check-config")]
        public ObjectResult CheckConfig()
        {
            return Ok(
                new
                {
                    _configurationService.GoogleApiKey,
                    hh = HttpResponseHeader.ContentType.ToString()
                }
            );
        }
    }
}