using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealEstate.Data;
using RealEstate.Model.SmsSubSystem;
using RealEstate.Services.Interfaces;

namespace RealEstate.Services
{
    public class NotificationSmsService : ISmsService
    {
        private readonly RealEstateDbContext dbContext = new RealEstateDbContext();

        public void AddSmsDeliveryStatus(SmsDeliveryStatuses smsDeliveryStatus)
        {
            dbContext.SmsDeliveryStatuses.Add(smsDeliveryStatus);
            dbContext.SaveChanges();
        }

        public async Task<bool> SendSmsAsync(string number, string text, string encoding = "gsm-03-38")
        {
            var phoneNumber = long.Parse(PhoneNumberNormalize(number));

            using (var client = new HttpClient())
            {
                var smsUrl = "https://bsms.voicecom.bg/multichannel-api/sendmulti/";
                var uri = new Uri(smsUrl);

                //Create SmsRequestObject
                var smsRequest = new SmsRequests(phoneNumber, text, "SMS");
                dbContext.SmsRequests.Add(smsRequest);
                await dbContext.SaveChangesAsync();


                var smsObject = new SmsObject(smsRequest.RequestId, phoneNumber, text, encoding);
                var smsObjectAsJsonString = JsonConvert.SerializeObject(smsObject);
                var content = new StringContent(smsObjectAsJsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);


                //Save result from the sms send request
                string textResult = await response.Content.ReadAsStringAsync();
                var smsRequestResult = JsonConvert.DeserializeObject<dynamic>(textResult);                
                dbContext.SmsRequests.Attach(smsRequest);
                smsRequest.ReturnCode = smsRequestResult.return_code;
                smsRequest.ReturnMessage = smsRequestResult.return_message;
                smsRequest.SmsParts = smsRequestResult.channels.sms.message_parts;
                await dbContext.SaveChangesAsync();

                return smsRequestResult.return_code == 0;
            }
        }

        public async Task<bool> SendViberMessageAsync(string number, string text, string image_url, string btnUrl, string btnTitle)
        {
            var phoneNumber = int.Parse(PhoneNumberNormalize(number));

            using (var client = new HttpClient())
            {
                var smsUrl = "https://bsms.voicecom.bg/multichannel-api/sendmulti/";
                var uri = new Uri(smsUrl);

                var smsRequest = new SmsRequests(phoneNumber, text, "VIBER");
                var smsObject = new SmsObject(smsRequest.RequestId, phoneNumber, text, image_url, btnUrl, btnTitle);
                var smsObjectAsJsonString = JsonConvert.SerializeObject(smsObject);
                var content = new StringContent(smsObjectAsJsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);

                //Save result from the sms send request
                string textResult = await response.Content.ReadAsStringAsync();
                var smsRequestResult = JsonConvert.DeserializeObject<dynamic>(textResult);
                dbContext.SmsRequests.Attach(smsRequest);
                smsRequest.ReturnCode = smsRequestResult.return_code;
                smsRequest.ReturnMessage = smsRequestResult.return_message;
                await dbContext.SaveChangesAsync();

                return smsRequestResult.return_code == 0 && smsRequestResult.return_message == "Message accepted";
            }
        }

        private string PhoneNumberNormalize(string phoneNumber)
        {
            //fix numbers like 08XX XXX XXX
            if (phoneNumber.Length == 10 && (phoneNumber.StartsWith("08") || phoneNumber.StartsWith("09")))
            {
                phoneNumber = phoneNumber.TrimStart('0');
                phoneNumber = "359" + phoneNumber;
            }
            //fix numbers like 8XX XXX XXX
            if (phoneNumber.Length == 9 && (phoneNumber.StartsWith("8") || phoneNumber.StartsWith("9")))
            {
                phoneNumber = "359" + phoneNumber;
            }
            //fix numbers like +3598XX XXX XXX
            if (phoneNumber.Length == 13 && phoneNumber.StartsWith("+"))
            {
                phoneNumber = phoneNumber.TrimStart('+');
            }
            //The number is correct if its length is 12, starts with 359 then 8 or 9 then only digits to the end
            if (phoneNumber.Length == 12 && Regex.IsMatch(phoneNumber, @"^359[8-9]{1}[0-9]+$"))
            {
                return phoneNumber;
            }
            else
            {
                throw new ArgumentException("Телефонният номер е в неправилен формат");
            }
        }
    }


    public class SmsObject
    {
        public SmsObject() { }

        //Sms sending ctor
        public SmsObject(string requestId, long to, string text, string encoding = "gsm-03-38")
        {
            this.to = to;
            sms = new Sms(text, encoding);
            sid = int.Parse(ConfigurationManager.AppSettings["LinkMobilityServiceId"]);
            token = CreateMD5(requestId + ConfigurationManager.AppSettings["LinkMobilityKey"]);
            priority = 1;
            send_order = new[] { "sms" };
            callback_url = "https://www.sproperties.net/AddSmsDeliveryStatus";
            defer = null;
            request_id = requestId;
        }

        //Viber message ctor
        public SmsObject(string requestId, long to, string text, string imgUrl, string btnUrl, string btnTitle)
        {
            this.to = to;
            viber = new Viber(text, imgUrl, btnUrl, btnTitle);
            sid = int.Parse(ConfigurationManager.AppSettings["LinkMobilityServiceId"]);
            token = CreateMD5(requestId + ConfigurationManager.AppSettings["LinkMobilityKey"]);
            send_order = new[] { "sms" };
            callback_url = "https://www.sproperties.net/AddSmsDeliveryStatus";
            defer = null;
        }

        public int sid { get; set; }
        public string request_id { get; set; }
        public long to { get; set; }
        public string token { get; set; }
        public byte priority { get; set; }
        //Delay 
        public DateTime? defer { get; set; }
        public string[] send_order { get; set; }
        public string callback_url { get; set; }
        public Sms sms { get; set; }
        public Viber viber { get; set; }

        //{
        //    "sid": 9999,
        //    "request_id": "client-unique-id",
        //    "to": 359888123456,
        //    "token": "675a6a1626d5a502ef09fbf26d795cf1",
        //    "priority": 1,
        //    "defer": "2018-04-10 10:00:00",
        //    "send_order": [
        //    "viber",
        //    "sms"
        //        ],
        //    "callback_url": "https://client-domain.com/client-dlr-script",
        //    "sms": {
        //        "text": "Example SMS text. Примерен SMS Текст.",
        //        "encoding": "UTF-8",
        //        "concatenate": 2,
        //        "from": "SMS_Sender",
        //        "validity": {
        //            "ttl": 10,
        //            "units": "min"
        //        },
        //        "mccmnc": "0"
        //    },
        //    "viber": {
        //        "text": "Example Viber text. Примерен Вайбър Текст.",
        //        "image_url": "https://client-domain.com/viber-image.png",
        //        "from": "Viber Sender",
        //        "button": {
        //            "url": "https://client-domain.com/promo/",
        //            "title": "Button title"
        //        },
        //        "validity": {
        //            "ttl": 30,
        //            "units": "sec"
        //        }
        //    }
        //}

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }

    public class Sms
    {
        public Sms(string text, string encoding = "gsm-03-38")
        {
            this.text = text;
            this.encoding = encoding;
            concatenate = 10;
            from = "sProperties";
            validity = new Validity
            {
                ttl = 3,
                units = "day"
            };

            mccmnc = 284;
        }

        public string text { get; set; }
        public string encoding { get; set; }
        public int concatenate { get; set; }
        public string from { get; set; }

        public Validity validity { get; set; }

        public int mccmnc { get; set; }
    }

    public class Viber
    {
        public Viber()
        {
            from = "sProperties Ltd.";
            validity = new Validity(3, "day");
        }

        public Viber(string text, string imageUrl = null, string btnUrl = null, string btnTitle = null)
        {
            this.text = text;
            image_url = imageUrl;
            from = "sProperties Ltd.";
            validity = new Validity(3, "day");
            if (btnTitle != null && btnUrl != null)
            {
                button = new Button(btnUrl, btnTitle);
            }
        }

        public string text { get; set; }
        public string image_url { get; set; }
        public string from { get; set; }
        public Button button { get; set; }
        public Validity validity { get; set; }
    }

    public class Validity
    {
        public Validity() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttl">Time to live in numbers</param>
        /// <param name="units">sec/min/hour/day</param>
        public Validity(int ttl, string units)
        {
            this.ttl = ttl;
            this.units = units;
        }

        public int ttl { get; set; }
        public string units { get; set; }
    }

    public class Button
    {
        public Button() { }

        public Button(string url, string title)
        {
            this.url = url;
            this.title = title;
        }
        public string url { get; set; }
        public string title { get; set; }
    }
}

