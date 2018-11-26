
using Newtonsoft.Json;

namespace ChatMap.Infrastructure.SocialNetwork.Instagram
{
    public class ProfileInfoDto
    {
        [JsonProperty("pk")]
        public long UserId { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("profile_pic_url")]
        public string ProfilePictureUrl { get; set; }
        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }
        [JsonProperty("has_anonymous_profile_picture")]
        public bool HasAnonymousProfilePicture { get; set; }
        [JsonProperty("media_count")]
        public int MediaCount { get; set; }
        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }
        [JsonProperty("following_count")]
        public int FollowingCount { get; set; }
        [JsonProperty("following_tag_count")]
        public int FollowingTagCount { get; set; }
        [JsonProperty("biography")]
        public string Biography { get; set; }
        [JsonProperty("external_url")]
        public string ExternalUrl { get; set; }
        [JsonProperty("reel_auto_archive")]
        public string ReelAutoArchive { get; set; }
        [JsonProperty("usertags_count")]
        public int UsertagsAount { get; set; }
        [JsonProperty("is_interest_account")]
        public bool IsInterestAccount { get; set; }
        [JsonProperty("hd_profile_pic_url_info")]
        public HdProfilePictureInfoDto HdProfilePictureInfo { get; set; }
        [JsonProperty("has_highlight_reels")]
        public bool HasHighlightReels { get; set; }
        [JsonProperty("is_potential_business")]
        public bool IsPotentialBusiness { get; set; }
        [JsonProperty("auto_expand_chaining")]
        public bool AutoExpandChaining { get; set; }
        [JsonProperty("highlight_reshare_disabled")]
        public bool HighlightReshareDisabled { get; set; }
    }
}