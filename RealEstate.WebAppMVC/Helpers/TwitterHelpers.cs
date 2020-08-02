using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RealEstate.WebAppMVC.Helpers
{
    public class TwitterHelpers
    {
        public static async Task<TwitterDto> TwitterLogin(string oauthToken, string oauthTokenSecret, string oauthConsumerKey, string oauthConsumerSecret)
        {
            // oauth implementation details
            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";

            // unique request details
            var oauthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            var resourceUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";
            var request_query = "include_email=true";
            // create oauth signature
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}";

            var baseString = string.Format(baseFormat,
                                        oauthConsumerKey,
                                        oauthNonce,
                                        oauth_signature_method,
                                        oauthTimestamp,
                                        oauthToken,
                                        oauth_version
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resourceUrl) + "&" + Uri.EscapeDataString(request_query), "%26", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauthConsumerSecret),
                                    "&", Uri.EscapeDataString(oauthTokenSecret));

            string oauthSignature;
            using (HMACSHA1 hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(
                    hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", oauth_timestamp=\"{4}\", oauth_token=\"{5}\", oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauthConsumerKey),
                                    Uri.EscapeDataString(oauthNonce),
                                    Uri.EscapeDataString(oauthSignature),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauthTimestamp),
                                    Uri.EscapeDataString(oauthToken),
                                    Uri.EscapeDataString(oauth_version)
                            );


            // make the request

            ServicePointManager.Expect100Continue = false;
            resourceUrl += "?include_email=true";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resourceUrl);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            return JsonConvert.DeserializeObject<TwitterDto>(await new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEndAsync());
        }
    }

    public class TwitterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}