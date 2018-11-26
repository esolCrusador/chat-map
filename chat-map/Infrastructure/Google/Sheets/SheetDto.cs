using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class SheetDto
    {
        [JsonProperty("properties")]
        public SheetPropertiesDto Properties { get; set; }
    }
}
