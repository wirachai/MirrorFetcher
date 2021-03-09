using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace TerraFetcher.Services
{
    /// <summary>
    /// To send notify message via LINE.
    /// For access token, you need to generate it by going to https://notify-bot.line.me.
    /// </summary>
    public class LineNotify
    {
        private readonly string apiUrl = "https://notify-api.line.me/api/notify";
        private readonly List<string> allowedTokens = new List<string>();
        private const int MessageMaxLength = 1000 - 20;

        public LineNotify()
        {
        }

        public LineNotify(List<string> allowedTokens)
        {
            this.allowedTokens = allowedTokens;
        }

        public string Send(string token, string message)
        {
            return Send(token, message, null, null);
        }

        public string Send(string token, string message, string stickerPackageId, string stickerId)
        {
            if (allowedTokens.Count > 0 && allowedTokens.Contains(token) == false)
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Authorization", $"Bearer {token}");
                int numberOfMessage = (message.Length / MessageMaxLength) + 1;
                for (int i = 0; i < numberOfMessage; i++)
                {
                    var sendingMessage = i != numberOfMessage - 1
                        ? message.Substring(i * MessageMaxLength, MessageMaxLength)
                        : message.Substring(i * MessageMaxLength);

                    var requestParams = new NameValueCollection
                    {
                        { "message", sendingMessage }
                    };
                    if (i == 0 && string.IsNullOrEmpty(stickerId) == false)
                    {
                        requestParams.Add(new NameValueCollection
                            {
                                { "stickerPackageId", stickerPackageId },
                                { "stickerId", stickerId }
                            }
                        );
                    }
                    byte[] response = client.UploadValues(apiUrl, requestParams);
                    result.Append(Encoding.UTF8.GetString(response));
                }

                return result.ToString();
            }
        }
    }
}