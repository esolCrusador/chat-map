using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class PaddingDto
    {
        [JsonProperty("top")]
        public int Top { get; set; }
        [JsonProperty("right")]
        public int Right { get; set; }
        [JsonProperty("bottom")]
        public int Bottom { get; set; }
        [JsonProperty("left")]
        public int Left { get; set; }
    }
}
