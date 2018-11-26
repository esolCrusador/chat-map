using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class SheetColorDto
    {
        [JsonProperty("red")]
        public int? Red { get; set; }
        [JsonProperty("green")]
        public int? Green { get; set; }
        [JsonProperty("blue")]
        public int? Blue { get; set; }
    }

}
