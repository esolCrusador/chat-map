using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class SheetPropertiesDto
    {
        [JsonProperty("sheetId")]
        public int SheetId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("sheetType")]
        public string SheetType { get; set; }
        [JsonProperty("gridProperties")]
        public SheetGridPropertiesDto GridProperties { get; set; }
    }
}
