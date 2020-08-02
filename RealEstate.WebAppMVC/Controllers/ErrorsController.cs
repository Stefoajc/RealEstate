using System.Web.Mvc;

namespace RealEstate.WebAppMVC.Controllers
{
    public class ErrorsController : Controller
    {
        /// <summary>
        /// 500 Server Error
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="errorStackTrace"></param>
        /// <returns></returns>
        // GET: Errors
        public ActionResult Index(string errorMessage = null, string errorStackTrace = null)
        {
            return View(new ErrorMessageViewModel(errorMessage, errorStackTrace));
        }
        
        public ActionResult NotFound(string errorMessage = null, string errorStackTrace = null)
        {
            return View("HttpNotFound", new ErrorMessageViewModel(errorMessage, errorStackTrace));
        }
        
        public ActionResult NotAuthorized(string errorMessage = null, string errorStackTrace = null)
        {
            return View(new ErrorMessageViewModel(errorMessage, errorStackTrace));
        }


        public class ErrorMessageViewModel
        {
            public ErrorMessageViewModel(string errorMessage, string errorStackTrace)
            {
                ErrorMessage = errorMessage;
                ErrorStackTrace = errorStackTrace;
            }

            public string ErrorMessage { get; set; }
            public string ErrorStackTrace { get; set; }
        }
    }
}