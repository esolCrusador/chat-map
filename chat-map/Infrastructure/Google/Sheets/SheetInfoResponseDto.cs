using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class SheetInfoResponseDto
    {
        [JsonProperty("spreadsheetId")]
        public string SpreadsheetId { get; set; }
        [JsonProperty("properties")]
        public DocumentPropertiesDto Properties { get; set; }
        [JsonProperty("sheets")]
        public List<SheetDto> Sheets { get; set; }
        [JsonProperty("spreadsheetUrl")]
        public string SpreadsheetUrl { get; set; }
    }
}
