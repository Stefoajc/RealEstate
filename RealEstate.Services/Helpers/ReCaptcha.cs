using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;

namespace RealEstate.Services.Helpers
{
    /// <summary>
    /// ReCaptcha validator class
    /// </summary>
    public class ReCaptcha
    {
        public static bool IsValid(string reCaptchaAnswer)
        {
            var client = new System.Net.WebClient();

            string PrivateKey = ConfigurationManager.AppSettings["recaptchaPrivateKey"];

            var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, reCaptchaAnswer));

            var captchaResponse = JsonConvert.DeserializeObject<ReCaptcha>(GoogleReply);

            return captchaResponse.Success.ToLower() == "true" ? true : false;
        }
        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }

    }
}