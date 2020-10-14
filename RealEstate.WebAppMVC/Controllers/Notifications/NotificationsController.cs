using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Services;

namespace RealEstate.WebAppMVC.Controllers.Notifications
{
    [Authorize]
    public class NotificationsController : Controller
    {

        private readonly NotificationServices _notificationsManager;

        public NotificationsController(NotificationServices notificationsManager)
        {
            _notificationsManager = notificationsManager;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var notifications = await _notificationsManager.GetAllUserNotifications(userId, 0, 14);
            await _notificationsManager.MarkAllAsSeen(userId);

            return View(notifications);
        }

        [HttpGet]
        public async Task<ActionResult> List(int pageNumber, int pageSize)
        {
            var userId = User.Identity.GetUserId();
            var notifications = await _notificationsManager.GetAllUserNotifications(userId, pageNumber, pageSize);
            await _notificationsManager.MarkAllAsSeen(userId);

            return Json(notifications, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> NotificationsCount()
        {
            var userId = User.Identity.GetUserId();           
            var notificationsCount = await _notificationsManager.GetNotificationsCount(userId);

            return Json(notificationsCount, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var userId = User.Identity.GetUserId();
            await _notificationsManager.MarkAllAsClicked(userId);

            return Json("STATUS_OK");
        }

        [HttpPost]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var userId = User.Identity.GetUserId();
            await _notificationsManager.MarkAsClicked(id, userId);

            return Json("STATUS_OK");
        }
    }
}