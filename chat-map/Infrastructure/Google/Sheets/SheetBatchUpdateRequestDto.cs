using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class SheetBatchUpdateRequestDto
    {
        [JsonProperty("data")]
        public List<SheetValuesDto> Values { get; set; }
    }
}
