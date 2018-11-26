﻿using Newtonsoft.Json;

namespace ChatMap.Infrastructure.SocialNetwork.Instagram
{
    public class HdProfilePictureInfoDto
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
