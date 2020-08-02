using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using Newtonsoft.Json;

namespace RealEstate.ViewModels.CustomDataAnnotations
{
    public class ValidateRecaptchaAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var reCaptchaAnswer = (string) value;
            return ReCaptcha.IsValid(reCaptchaAnswer);
        }
    }

    /// <summary>
    /// ReCaptcha validator class
    /// </summary>
    class ReCaptcha
    {
        public static bool IsValid(string reCaptchaAnswer)
        {
            var client = new System.Net.WebClient();

            string privateKey = ConfigurationManager.AppSettings["recaptchaSecretKey"];

            var googleReply = client.DownloadString($"https://www.google.com/recaptcha/api/siteverify?secret={privateKey}&response={reCaptchaAnswer}");

            var captchaResponse = JsonConvert.DeserializeObject<ReCaptcha>(googleReply);

            return captchaResponse.Success.ToLower() == "true";
        }

        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
