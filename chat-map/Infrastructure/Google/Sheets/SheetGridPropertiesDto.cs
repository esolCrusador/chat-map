using Newtonsoft.Json;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class SheetGridPropertiesDto
    {
        [JsonProperty("rowCount")]
        public int RowCount { get; set; }
        [JsonProperty("columnCount")]
        public int ColumnCount { get; set; }
        [JsonProperty("frozenRowCount")]
        public int FrozenRowCount { get; set; }
    }
}
