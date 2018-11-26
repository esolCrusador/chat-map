using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class DefaultFormatDto
    {
        [JsonProperty("backgroundColor")]
        public SheetColorDto BackgroundColor { get; set; }
        [JsonProperty("padding")]
        public PaddingDto Padding { get; set; }
        [JsonProperty("verticalAlignment")]
        public string VerticalAlignment { get; set; }
        [JsonProperty("wrapStrategy")]
        public string WrapStrategy { get; set; }
        [JsonProperty("textFormat")]
        public TextFormatDto TextFormat { get; set; }
    }
}
