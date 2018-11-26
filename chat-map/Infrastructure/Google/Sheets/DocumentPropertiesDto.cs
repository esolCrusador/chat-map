using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class DocumentPropertiesDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("autoRecalc")]
        public string AutoRecalc { get; set; }
        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }
        [JsonProperty("defaultFormat")]
        public DefaultFormatDto DefaultFormat { get; set; }
    }
}
