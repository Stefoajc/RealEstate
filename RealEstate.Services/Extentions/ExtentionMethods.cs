using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RealEstate.Data;
using RealEstate.Model;

namespace RealEstate.Services.Extentions
{

    public static class ExceptionExtentions
    {
        public static object ToJson(this Exception ex)
        {
            return new { Message = ex.Message, Source = ex.Source, StackTrace = ex.StackTrace, Type = ex.GetType().ToString() };
        }
    }

    public static class StringExtentions
    {
        public static string FirstCharToUpper(this string s)
        {
            // Check for empty string.  
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.  
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static List<int> AllLastIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        /// <summary>
        /// Takes a Proper-Cased variable name and adds a space before each
        /// capital letter.
        /// </summary>
        public static string AddSpaces(this string text)
        {
            // Special case: if input is all UPPERCASE, add no spaces
            if (text == text.ToUpperInvariant())
                return text;

            var result = String.Empty;

            for (int i = 0; i < text.Length; i++)
            {
                // If we have multiple upper-case in a row, insert a space only before the last one
                if (i > 0 && char.IsUpper(text[i]) && (i == text.Length - 1 || char.IsLower(text[i - 1]) || char.IsLower(text[i + 1])))
                    result += " ";

                result += text.Substring(i, 1);
            }
            return result;
        }


        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static string TrimStartExact(this string target, string trimString)
        {
            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string TrimEndExact(this string target, string trimString)
        {
            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        public static string Utf16ToUtf8(this string utf16String)
        {
            /**************************************************************
             * Every .NET string will store text with the UTF16 encoding, *
             * known as Encoding.Unicode. Other encodings may exist as    *
             * Byte-Array or incorrectly stored with the UTF16 encoding.  *
             *                                                            *
             * UTF8 = 1 bytes per char                                    *
             *    ["100" for the ansi 'd']                                *
             *    ["206" and "186" for the russian 'κ']                   *
             *                                                            *
             * UTF16 = 2 bytes per char                                   *
             *    ["100, 0" for the ansi 'd']                             *
             *    ["186, 3" for the russian 'κ']                          *
             *                                                            *
             * UTF8 inside UTF16                                          *
             *    ["100, 0" for the ansi 'd']                             *
             *    ["206, 0" and "186, 0" for the russian 'κ']             *
             *                                                            *
             * We can use the convert encoding function to convert an     *
             * UTF16 Byte-Array to an UTF8 Byte-Array. When we use UTF8   *
             * encoding to string method now, we will get a UTF16 string. *
             *                                                            *
             * So we imitate UTF16 by filling the second byte of a char   *
             * with a 0 byte (binary 0) while creating the string.        *
             **************************************************************/

            // Storage for the UTF8 string
            string utf8String = String.Empty;

            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Fill UTF8 bytes inside UTF8 string
            for (int i = 0; i < utf8Bytes.Length; i++)
            {
                // Because char always saves 2 bytes, fill char with 0
                byte[] utf8Container = new byte[2] { utf8Bytes[i], 0 };
                utf8String += BitConverter.ToChar(utf8Container, 0);
            }

            // Return UTF8
            return utf8String;
        }
    }

    public static class StreamExtentions
    {
        public static byte[] ReadFully(this Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }

    public static class DateTimeExtentions
    {
        public static double UnixTicks(this DateTime dt)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }

        public static long GetMiliseconds(this DateTime dt)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt;
            return (long)(d2 - d1).TotalMilliseconds;
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }


        public static string TimeFromNowString(this DateTime dt)
        {
            var difference = DateTime.Now - dt;

            if (difference.TotalDays > 1)
            {
                var ceiledDays = (int) Math.Ceiling(difference.TotalDays);
                return ceiledDays == 1 ? $"{ceiledDays} ден" : $"{ceiledDays} дни";
            }
            if (difference.TotalHours > 1)
            {
                return $"{Math.Ceiling(difference.TotalHours)} ч.";
            }

            return $"{Math.Ceiling(difference.TotalMinutes)} мин.";
        }
    }



    public static class RegexExtentions
    {
        /// <summary>
        /// Equivalent to PHP preg_match but only for 3 requied parameters
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="input"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool preg_match(string pattern, string input, out List<string> matches)
        {
            Regex regex = new Regex(pattern);
            var match = regex.Match(input);
            var groups = (from object g in match.Groups select g.ToString()).ToList();

            matches = groups;
            return match.Success;
        }

        /// <summary>
        /// Equivalent to PHP preg_replace
        /// <see cref="http://stackoverflow.com/questions/166855/c-sharp-preg-replace"/>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string preg_replace(this string input, string[] pattern, string[] replacements)
        {
            if (replacements.Length != pattern.Length)
                throw new ArgumentException("Replacement and Pattern Arrays must be balanced");

            for (var i = 0; i < pattern.Length; i++)
            {
                input = Regex.Replace(input, pattern[i], replacements[i]);
            }

            return input;
        }
    }

    public static class IdentityExtensions
    {
        #region Generic Claims Methods

        //private static Claim AddOrUpdateClaim(this IIdentity identity, string key, string value)
        //{
        //    AddUpdateClaim(identity, key, value);
        //    return new Claim(key, value); ;
        //}

        //private static void AddUpdateClaim(this IIdentity identity, string key, string value)
        //{
        //    var claimsIdentity = identity as ClaimsIdentity;
        //    if (claimsIdentity == null)
        //        return;

        //    // check for existing claim and remove it
        //    var existingClaim = claimsIdentity.FindFirst(key);
        //    if (existingClaim != null)
        //        claimsIdentity.RemoveClaim(existingClaim);

        //    // add new claim
        //    claimsIdentity.AddClaim(new Claim(key, value));
        //    var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
        //    authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties() { IsPersistent = true });
        //}

        //public static void AddUpdateClaims(this IIdentity identity, string key, List<string> values)
        //{
        //    var claimsIdentity = identity as ClaimsIdentity;
        //    if (claimsIdentity == null)
        //        return;

        //    // check for existing claims and remove them
        //    var existingClaim = claimsIdentity.FindAll(key).ToList().Clone();
        //    if (existingClaim.Count > 0)
        //    {
        //        foreach (var claim in existingClaim)
        //        {
        //            claimsIdentity.RemoveClaim(claim);
        //        }
        //    }
        //    // add new claims
        //    foreach (var value in values)
        //    {
        //        claimsIdentity.AddClaim(new Claim(key, value));
        //    }

        //    var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
        //    authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties() { IsPersistent = true });
        //}

        public static string GetProfileImage(this IIdentity identity)
        {
            return GetClaimValue(identity, "ProfileImage");
        }

        public static string GetClaimValue(this IIdentity identity, string key)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return null;

            var claim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }

        public static List<string> GetClaimValues(this IIdentity identity, string key)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return null;

            var claims = claimsIdentity.Claims.Where(c => c.Type == key).Select(c => c.Value).ToList();
            return claims;
        }

        public static string GetPhoneNumber(this IIdentity identity)
        {
            return GetClaimValue(identity, "PhoneNumber");
        }

        public static string IsPhoneNumberConfirmed(this IIdentity identity)
        {
            return GetClaimValue(identity, "IsPhoneNumberConfirmed");
        }


        public static async Task<bool> HasPhoneNumberConfirmedAsync(this IIdentity identity)
        {
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new RealEstateDbContext()));

            return await userManager.HasPhoneNumberAsync(identity.GetUserId()) &&
                   await userManager.IsPhoneNumberConfirmedAsync(identity.GetUserId());
        }

        public static bool HasPhoneNumberConfirmed(this IIdentity identity)
        {
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new RealEstateDbContext()));

            return userManager.HasPhoneNumber(identity.GetUserId()) &&
                   userManager.IsPhoneNumberConfirmed(identity.GetUserId());
        }

        public static bool HasPhoneNumber(this IIdentity identity)
        {
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new RealEstateDbContext()));
            return userManager.HasPhoneNumber(identity.GetUserId());
        }

        public static async Task<bool> HasPhoneNumberAsync(this IIdentity identity)
        {
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new RealEstateDbContext()));
            return await userManager.HasPhoneNumberAsync(identity.GetUserId());
        }

        public static int GetNewNotificationsCount(this IIdentity identity)
        {
            if (!identity.IsAuthenticated)
            {
                return 0;
            }

            var userId = identity.GetUserId();
            var dbContext = new RealEstateDbContext();
            return dbContext.Notifications
                .Count(n => (!n.IsSeen && n.UserId == userId) 
                || (n.UserCreatorId != userId && n.UserCreatorId != null && n.UsersSawNotifications.All(u => u.Id != userId)));
        }
        #endregion
    }
}