using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChatMap.Infrastructure.Google.Sheets
{
    public class SheetValuesDto
    {
        [JsonProperty("range")]
        public string Range { get; set; }
        [JsonProperty("majorDimension")]
        public string MajorDimension { get; set; }
        [JsonProperty("values")]
        public List<List<string>> Values { get; set; }

        public override string ToString()
        {
            return $"{Range}:{JsonConvert.SerializeObject(Values)}";
        }
    }
}
