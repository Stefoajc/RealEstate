using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class NotificationServices : BaseService
    {
        public NotificationServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }

        /// <summary>
        /// Create notification
        /// </summary>
        /// <param name="model"></param>
        public void CreateNotification(NotificationCreateViewModel model)
        {
            Notifications notification = new Notifications
            {
                NotificationLink = model.NotificationLink,
                NotificationText = model.NotificationText,
                UserId = model.UserId
            };

            UnitOfWork.NotificationsRepository.Add(notification);
            UnitOfWork.Save();
        }

        /// <summary>
        /// Get all User Notifications
        /// </summary>
        /// <returns></returns>
        public List<NotificationListViewModel> GetAllUserNotifications()
        {
            var userId = User.Identity.GetUserId();

            var notifications = UnitOfWork.NotificationsRepository
                .FindBy(n => n.UserId == userId)
                .Select(n =>
                new NotificationListViewModel
                {
                    NotificationLink = n.NotificationLink,
                    NotificationText = n.NotificationText,
                    CreatedOn = n.CreatedOn
                })
                .OrderBy(n => n.CreatedOn)
                .ToList();

            return notifications;
        }

        /// <summary>
        /// Get the count of all not seen notifications
        /// </summary>
        /// <returns></returns>
        public int GetNotSeenNotificationsCount()
        {
            var userId = User.Identity.GetUserId();

            return UnitOfWork.NotificationsRepository.FindBy(n => n.UserId == userId && !n.IsSeen).Count();
        }

        /// <summary>
        /// Mark all notifications as seen
        /// </summary>
        public void MarkAllAsSeen()
        {
            var userId = User.Identity.GetUserId();

            var allNotifications = UnitOfWork.NotificationsRepository.FindBy(n => n.UserId == userId && !n.IsSeen).ToList();

            foreach (var notification in allNotifications)
            {
                notification.IsSeen = true;
                UnitOfWork.NotificationsRepository.Edit(notification);
            }

            UnitOfWork.Save();
        }

        /// <summary>
        /// Mark notification as seen
        /// </summary>
        /// <param name="notificationId"></param>
        public void MarkAsSeen(int notificationId)
        {
            var userId = User.Identity.GetUserId();

            var notification = UnitOfWork.NotificationsRepository
                .FindBy(n => n.UserId == userId && n.NotificationId == notificationId).FirstOrDefault();

            if (notification != null)
            {
                notification.IsSeen = true;
                UnitOfWork.NotificationsRepository.Edit(notification);
                UnitOfWork.Save();
            }
        }

        /// <summary>
        /// Delete all User's Notifications
        /// </summary>
        public void DeleteAll()
        {
            var userId = User.Identity.GetUserId();

            var allNotifications = UnitOfWork.NotificationsRepository.FindBy(n => n.UserId == userId && !n.IsSeen).ToList();

            foreach (var notification in allNotifications)
            {
                UnitOfWork.NotificationsRepository.Delete(notification);
            }

            UnitOfWork.Save();
        }
    }
}