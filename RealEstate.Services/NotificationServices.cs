using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Model.Notifications;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class NotificationServices : BaseService, INotificationCreator
    {
        public NotificationServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) 
            : base(unitOfWork, userMgr) {}

        /// <summary>
        /// Create notification
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userToNotifyId"></param>
        public async Task CreateIndividualNotification(NotificationCreateViewModel model, string userToNotifyId)
        {
            if (!await IsUserExisting(userToNotifyId))
            {
                throw new ArgumentException("Потребителят, за който се създава известието не е намерен!");
            }

            if (!await unitOfWork.NotificationsRepository.ExistType(model.NotificationTypeId))
            {
                throw new ArgumentException("Не е намерен типът на нотификацията!");
            }

            Notifications notification = new Notifications
            {
                IsGlobal = false,
                NotificationLink = model.NotificationLink,
                NotificationText = model.NotificationText,
                NotificationPicture = model.NotificationPicture,
                TypeId = model.NotificationTypeId,
                IsSeen = false,
                IsClicked = false,
                UserId = userToNotifyId
            };

            unitOfWork.NotificationsRepository.Add(notification);
            await unitOfWork.SaveAsync();
        }
        

        public async Task CreateGlobalNotification(NotificationCreateViewModel model, string userCreatorId)
        {
            if (!await IsUserExisting(userCreatorId))
            {
                throw new ArgumentException("Потребителят, за който се създава известието не е намерен!");
            }

            if (!await unitOfWork.NotificationsRepository.ExistType(model.NotificationTypeId))
            {
                throw new ArgumentException("Не е намерен типът на нотификацията!");
            }

            Notifications notification = new Notifications
            {
                IsGlobal = true,
                NotificationLink = model.NotificationLink,
                NotificationText = model.NotificationText,
                NotificationPicture = model.NotificationPicture,
                TypeId = model.NotificationTypeId,
                IsSeen = false,
                IsClicked = false,
                UserCreatorId = userCreatorId
            };

            unitOfWork.NotificationsRepository.Add(notification);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Get all User Notifications
        /// </summary>
        /// <returns></returns>
        public async Task<List<NotificationListViewModel>> GetAllUserNotifications(string userId, int? pageNumber = null, int? pageSize = null)
        {
            var notificationsQuery = unitOfWork.NotificationsRepository
                .Include(n => n.NotificationType)
                .Include(n => n.UserCreator)
                .Include(n => n.UserCreator.Images)
                .Include(n => n.UsersSawNotifications)
                .Include(n => n.UsersClickedNotifications)
                .Where(n => n.UserId == userId || (n.UserCreatorId != userId && n.UserCreatorId != null ));

            notificationsQuery = notificationsQuery.OrderByDescending(n => n.CreatedOn);

            if (pageNumber != null && pageSize != null)
            {
                //TODO:Make sure that there is enough notifications for this pageNumber (ex. 12 are 2 pages * 6 / pageNumber 3 is invalid)

                var notificationsToSkipCount = pageNumber * pageSize;
                notificationsQuery = notificationsQuery
                    .Skip((int) notificationsToSkipCount)
                    .Take((int) pageSize);
            }

            return await notificationsQuery.Select(n =>
                new NotificationListViewModel
                {
                    NotificationId = n.NotificationId,
                    NotificationLink = n.NotificationLink,
                    NotificationText = n.NotificationText,
                    NotificationPicture = n.NotificationPicture,

                    NotificationType = n.NotificationType.Type,
                    NotificationTypeImage = n.NotificationType.ImageAssociation,

                    IsSeen = n.IsSeen || n.UsersSawNotifications.Any(u => u.Id == userId),
                    IsClicked = n.IsClicked || n.UsersClickedNotifications.Any(u => u.Id == userId),

                    NotificationSourcePicture = n.UserCreator != null
                        ? n.UserCreator.Images.Select(i => i.ImagePath)
                            .FirstOrDefault() ?? "/Resources/no-image-person.png"
                        : "/Resources/Logo.jpg",

                    CreatedOn = n.CreatedOn
                })
                .ToListAsync();
        }

        public async Task<int> GetNotificationsCount(string userId)
        {
            return await unitOfWork.NotificationsRepository
                .Where(IsOwnerOrNotCreatorAndWithoutCreator(userId))
                .CountAsync();
        }


        /// <summary>
        /// Get the count of all not seen notifications
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetNotSeenNotificationsCount(string userId)
        {
            return await unitOfWork.NotificationsRepository.GetAll()
                .CountAsync(n => (!n.IsSeen && n.UserId == userId)
                            || (n.UserCreatorId != userId && n.UserCreatorId != null && n.UsersSawNotifications.All(u => u.Id != userId)));
        }

        /// <summary>
        /// Mark all notifications as seen
        /// </summary>
        public async Task MarkAllAsSeen(string userId)
        {
            var allIndividualNotifications = await unitOfWork.NotificationsRepository
                .Where(n => n.UserId == userId && !n.IsSeen && !n.IsGlobal)
                .ToListAsync();

            foreach (var notification in allIndividualNotifications)
            {
                notification.IsSeen = true;
                unitOfWork.NotificationsRepository.Edit(notification);
            }

            var allGlobalNotifications = await unitOfWork.NotificationsRepository
                .Include(n => n.UsersSawNotifications)
                .Where(n => n.UserCreatorId != userId && n.UserCreatorId != null && n.UsersSawNotifications.All(u => u.Id != userId))
                .ToListAsync();

            var currentUser = await unitOfWork.UsersRepository
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Потребителят не е намерен!");

            foreach (var notification in allGlobalNotifications)
            {
                notification.UsersSawNotifications.Add(currentUser);
                currentUser.NotificationsSeen.Add(notification);
                unitOfWork.NotificationsRepository.Edit(notification);
            }

            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Mark notification as seen
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="userId"></param>
        public async Task MarkAsClicked(int notificationId, string userId)
        {
            var notification = await unitOfWork.NotificationsRepository
                .Include(n => n.UsersSawNotifications)
                .Include(n => n.UsersClickedNotifications)
                .Where(n => n.UserId == userId || (n.UserCreatorId != userId && n.UserCreatorId != null && n.NotificationId == notificationId))
                .FirstOrDefaultAsync();

            if (notification != null)
            {
                if (notification.IsGlobal)
                {
                    var currentUser = await unitOfWork.UsersRepository
                        .Where(u => u.Id == userId)
                        .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Потребителят не е намерен!");

                    if (notification.UsersClickedNotifications.All(u => u.Id != currentUser.Id))
                    {
                        notification.UsersSawNotifications.Add(currentUser);
                        notification.UsersClickedNotifications.Add(currentUser);
                    }
                }
                else
                {
                    notification.IsSeen = true;
                    notification.IsClicked = true;
                }

                unitOfWork.NotificationsRepository.Edit(notification);
                await unitOfWork.SaveAsync();
            }
        }

        /// <summary>
        /// Mark notification as seen
        /// </summary>
        /// <param name="userId"></param>
        public async Task MarkAllAsClicked(string userId)
        {
            var notifications = await unitOfWork.NotificationsRepository
                .Include(n => n.UsersSawNotifications)
                .Include(n => n.UsersClickedNotifications)
                .Where(n => n.UserId == userId)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                if (notification.IsGlobal)
                {
                    var currentUser = await unitOfWork.UsersRepository
                                          .Where(u => u.Id == userId)
                                          .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Потребителят не е намерен!");

                    if (notification.UsersClickedNotifications.All(u => u.Id != currentUser.Id))
                    {
                        notification.UsersSawNotifications.Add(currentUser);
                        notification.UsersClickedNotifications.Add(currentUser);
                    }
                }
                else
                {
                    notification.IsSeen = true;
                    notification.IsClicked = true;
                }

                unitOfWork.NotificationsRepository.Edit(notification);
                await unitOfWork.SaveAsync();
            }
        }


        /// <summary>
        /// Delete all User's Notifications
        /// </summary>
        public async Task DeleteAll(string userId)
        {
            var allNotifications = await unitOfWork.NotificationsRepository
                .Where(n => n.UserId == userId)
                .ToListAsync();

            foreach (var notification in allNotifications)
            {
                unitOfWork.NotificationsRepository.Delete(notification);
            }

            await unitOfWork.SaveAsync();
        }

        #region Filtrations

        /// <summary>
        /// Filter notification where the notification is meant to be for the passed User
        /// Or the notification is Global and the user is not creator of it
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static System.Linq.Expressions.Expression<Func<Notifications, bool>> IsOwnerOrNotCreatorAndWithoutCreator(string userId)
            => n => n.UserId == userId || (n.UserCreatorId != userId && n.UserCreatorId != null);

        #endregion

        #region Validations

        private Task<bool> IsUserExisting(string userToNotifyId) => userManager.Users.AnyAsync(u => u.Id == userToNotifyId);

        #endregion


        //MOCKS

        List<NotificationListViewModel> notificationsMockedList = new List<NotificationListViewModel>
        {
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-1),
                IsSeen = true,
                IsClicked = false,
                NotificationType = "Въпрос",
                NotificationTypeImage = "/Resources/NotificationTypes/question-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-2),
                IsSeen = false,
                IsClicked = false,
                NotificationType = "Материал",
                NotificationTypeImage = "/Resources/NotificationTypes/material-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-3),
                IsSeen = true,
                IsClicked = true,
                NotificationType = "Пост",
                NotificationTypeImage = "/Resources/NotificationTypes/post-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-1),
                IsSeen = true,
                IsClicked = false,
                NotificationType = "Въпрос",
                NotificationTypeImage = "/Resources/NotificationTypes/question-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-2),
                IsSeen = false,
                IsClicked = false,
                NotificationType = "Материал",
                NotificationTypeImage = "/Resources/NotificationTypes/material-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-3),
                IsSeen = true,
                IsClicked = true,
                NotificationType = "Пост",
                NotificationTypeImage = "/Resources/NotificationTypes/post-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-1),
                IsSeen = true,
                IsClicked = false,
                NotificationType = "Въпрос",
                NotificationTypeImage = "/Resources/NotificationTypes/question-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-2),
                IsSeen = false,
                IsClicked = false,
                NotificationType = "Материал",
                NotificationTypeImage = "/Resources/NotificationTypes/material-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            },
            new NotificationListViewModel
            {
                CreatedOn = DateTime.Now.AddDays(-3),
                IsSeen = true,
                IsClicked = true,
                NotificationType = "Пост",
                NotificationTypeImage = "/Resources/NotificationTypes/post-notification.png",
                NotificationPicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationSourcePicture = @"https://localhost:44379/Resources/Users/Agent/Profile/ZC9MCxNKT4iGFpuchN9z_arguing.jpg",
                NotificationLink = @"https://localhost:44379/",
                NotificationText = "Lorem ipsum dolor sit amet"
            }

        };

        public List<NotificationListViewModel> GetAllUserNotificationsMock(string userId, int? pageNumber = null, int? pageSize = null)
        {
            var notifications = notificationsMockedList
                .OrderByDescending(n => n.CreatedOn);

            if (pageNumber != null && pageSize != null)
            {
                var notificationsToSkipCount = pageNumber * pageSize;
                return notifications
                    .Skip((int)notificationsToSkipCount)
                    .Take((int)pageSize)
                    .ToList();
            }

            return notifications.ToList();
        }

        public int GetNotificationsCountMock(string userId)
        {
            return notificationsMockedList.Count;
        }
    }
}