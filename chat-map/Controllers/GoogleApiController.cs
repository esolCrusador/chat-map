using ChatMap.Infrastructure.Google.Sheets;
using ChatMap.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChatMap.Models;
using ChatMap.Services.Contracts;

namespace chat_map.Controllers
{
    [Route("api/google")]
    [ApiController]
    public class GoogleApiController : ControllerBase
    {
        private readonly GoogleApiClient _googleApiService;
        private readonly ConfigurationService _configurationService;
        private readonly IChatService _chatService;

        public GoogleApiController(ConfigurationService configurationService, GoogleApiClient googleApiService, IChatService chatService)
        {
            _configurationService = configurationService;
            _googleApiService = googleApiService;
            _chatService = chatService;
        }

        [HttpGet("map.js")]
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

                Response.ContentType = response.Content.Headers.ContentType.ToString();
                Response.Headers.Add("cache-control", response.Headers.CacheControl.ToString());
                Response.Headers.Add("vary", "Accept-Language");

                return await response.Content.ReadAsStreamAsync();
            }
        }

        [HttpGet("sheet-info/{sheetId}")]
        public async Task<IReadOnlyCollection<PersonModel>> GetSheetInfo([FromRoute]string sheetId, CancellationToken cancellation)
        {
            return await _chatService.GetPersons(sheetId, cancellation);
        }

        [HttpGet("sheet-info/{sheetId}/adjust")]
        public async Task<IReadOnlyCollection<PersonModel>> AdjustSheetInfo([FromRoute]string sheetId, CancellationToken cancellation)
        {
            return await _chatService.AdjustPersons(sheetId, cancellation);
        }

        //[HttpGet("check-config")]
        //public ObjectResult CheckConfig()
        //{
        //    return Ok(
        //        new
        //        {
        //            _configurationService.GoogleApiKey,
        //        }
        //    );
        //}
    }
}