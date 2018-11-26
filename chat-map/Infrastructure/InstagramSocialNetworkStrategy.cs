using ChatMap.Models;
using ChatMap.Infrastructure.SocialNetwork.Instagram;
using ChatMap.Infrastructure.Contracts;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Infrastructure
{
    public class InstagramSocialNetworkStrategy : ISocialNetworkStrategy
    {
        private static readonly Regex ParseIdRegex = new Regex("\"profilePage_(?<profileId>\\d+)\"", RegexOptions.Compiled);
        private readonly IDownloadClientsPool _downloadlClientsPool;

        public Contracts.SocialNetwork SocialNetwork => Contracts.SocialNetwork.Instagram;

        public InstagramSocialNetworkStrategy(IDownloadClientsPool downloadClientsPool)
        {
            _downloadlClientsPool = downloadClientsPool;
        }

        private static string EscapeUsername(string userName)
        {
            return userName != null && userName[0] == '@' ? userName.Substring(1) : userName;
        }

        private async Task<long> GetIdByName(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            string instagramDoc;
            using (HttpClientContainer downloader = await _downloadlClientsPool.Consume(cancellationToken))
            {
                instagramDoc = await downloader.GetAsString($"https://www.instagram.com/{EscapeUsername(username)}/", cancellationToken);
            }
            cancellationToken.ThrowIfCancellationRequested();

            Match match = ParseIdRegex.Match(instagramDoc);
            if (!match.Success)
                throw new Exception($"The page does not have profile Id\r\n{instagramDoc}");

            return long.Parse(match.Groups["profileId"].Value);
        }

        public async Task<string> GetProfileUrl(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            long userId = await GetIdByName(userName, cancellationToken);

            string responseString;
            using (HttpClientContainer downloader = await _downloadlClientsPool.Consume(cancellationToken))
            {
                responseString = await downloader.GetAsString($"https://i.instagram.com/api/v1/users/{userId}/info/", cancellationToken);
            }

            var result = JsonConvert.DeserializeObject<InstagramResponseDto>(responseString);

            if (result.Status != HttpStatusCode.OK)
                throw new HttpRequestException("Instagram response was not succesful");

            return result.User.ProfilePictureUrl;
        }
    }
}
