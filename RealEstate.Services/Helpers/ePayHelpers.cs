using System;
using System.Configuration;
using System.Security.Cryptography;

namespace RealEstate.Services.Helpers
{
    public class ePayHelpers
    {
        /// <summary>
        /// UTF8 !
        /// </summary>
        /// <param name="tobeSigned"></param>
        /// <returns></returns>
        public static string HMAC_SHA1_Encoding(string tobeSigned)
        {
            using (HMACSHA1 hmac = new HMACSHA1(System.Text.Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["ePayClientSecret"])))
            {
                hmac.Initialize();
                byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(tobeSigned);
                return BitConverter.ToString(hmac.ComputeHash(encodedBytes)).Replace("-", "").ToLower();
            }
        }
    }
}