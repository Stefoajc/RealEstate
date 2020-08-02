using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RealEstate.Services.Extentions;

namespace RealEstate.WebAppMVC.Controllers.MailList
{
    public class MailTrackController : Controller
    {
        // <img src="https://www.sproperties.net/MailTrack/TrackOpenRate?email=test@test.test&campaignName=test" alt="" />
        [HttpGet]
        public ActionResult TrackOpenRate(string email, string campaignName)
        {
            var baseWebAppFolder = HttpRuntime.AppDomainAppPath.TrimEnd('\\');
            var campaignFolder = Path.Combine(baseWebAppFolder, "Campaigns");
            var campaignFile = Path.Combine(campaignFolder, campaignName + ".txt");

            //Create Folder for the current campaign
            Directory.CreateDirectory(campaignFolder);
            using (var file = System.IO.File.Open(campaignFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var stream = new StreamWriter(file, Encoding.UTF8))
                {
                    stream.WriteLine("Email: " + email + " | Date: " + DateTime.Now + " | IP: " + WebExtentions.GetIPAddress(Request));
                }
            }

            return new ContentResult();
        }
    }
}