using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class TextFormatDto
    {
        [JsonProperty("foregroundColor")]
        public SheetColorDto ForegroundColor { get; set; }
        [JsonProperty("fontFamily")]
        public string FontFamily { get; set; }
        [JsonProperty("fontSize")]
        public int FontSize { get; set; }
        [JsonProperty("bold")]
        public bool Bold { get; set; }
        [JsonProperty("italic")]
        public bool Italic { get; set; }
        [JsonProperty("strikethrough")]
        public bool Strikethrough { get; set; }
        [JsonProperty("underline")]
        public bool Underline { get; set; }
    }
}
