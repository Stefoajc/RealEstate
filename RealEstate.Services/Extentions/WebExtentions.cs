using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace RealEstate.Services.Extentions
{
    public static class WebExtentions
    {
        public static void RedirectWithData(List<KeyValuePair<string, string>> data, string paymentUrl)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Charset = "utf-8";
            response.Clear();

            StringBuilder s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat($"<form name='form' action='{paymentUrl}' method='post'>");
            foreach (KeyValuePair<string, string> key in data)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key.Key, key.Value);
            }
            s.Append("</form></body></html>");
            response.Write(s.ToString());
            response.End();
        }

        public static string CreatePaymentForm(List<KeyValuePair<string, string>> data, string url)
        {
            StringBuilder s = new StringBuilder();
            s.Append("<html>");
            s.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            s.AppendFormat("<form name='form' action='{0}' method='post'>", url);
            foreach (KeyValuePair<string, string> key in data)
            {
                s.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key.Key, key.Value);
            }
            s.Append("</form></body></html>");

            return s.ToString();
        }

        /// <summary>
        /// Gets The IP address of the server pc on which the app is running
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            return ConfigurationManager.AppSettings["DomainIP"];
        }


        /// <summary>
        /// Gets the ip address of the client sending the requiest request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetIPAddress(HttpRequestBase request)
        {
            string szRemoteAddr = request.UserHostAddress;
            string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
            string szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > 0)
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!IsLocalHost(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }

        private static bool IsLocal(DirectoryInfo dir)
        {
            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (string.Compare(dir.Root.FullName, d.Name, StringComparison.OrdinalIgnoreCase) == 0) //[drweb86] Fix for different case.
                {
                    return (d.DriveType != DriveType.Network);
                }
            }
            throw new DriveNotFoundException();
        }

        private static bool IsLocalHost(string input)
        {
            IPAddress[] host;
            //get host addresses
            try
            {
                host = Dns.GetHostAddresses(input);
            }
            catch (Exception)
            {
                return false;
            }
            //get local adresses
            IPAddress[] local = Dns.GetHostAddresses(Dns.GetHostName());
            //check if local
            return host.Any(hostAddress => IPAddress.IsLoopback(hostAddress) || local.Contains(hostAddress));
        }

    }
}