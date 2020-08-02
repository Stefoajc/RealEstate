using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.SmsSubSystem
{
    public class SmsRequests
    {
        public SmsRequests()
        {
            RequestId = RandomString(100);
            RequestType = "BOTH";
        }

        public SmsRequests(long phoneNumber, string text, string requestType = "SMS")
        {
            RequestId = RandomString(100);
            To = phoneNumber;
            Text = text;
            RequestType = requestType;
        }

        [Key, StringLength(100)]
        public string RequestId { get; set; }
        public long To { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Text { get; set; }

        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }

        [Required]
        public string RequestType { get; set; }

        //For Sms messages over the max char limit
        public byte? SmsParts { get; set; }

        private string RandomString(int size)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            var stringChars = new char[size];
            var random = new Random();

            for (int i = 0; i < size; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }            

            var finalString = new string(stringChars);
            return finalString;
        }
    }
}
