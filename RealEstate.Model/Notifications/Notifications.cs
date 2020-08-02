using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Notifications
{
    public class Notifications
    {
        [Key]
        public int NotificationId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string NotificationLink { get; set; }
        public string NotificationText { get; set; }
        public string NotificationPicture { get; set; }



        [ForeignKey("NotificationType")]
        public int TypeId { get; set; }
        public virtual NotificationTypes NotificationType { get; set; }

        public bool IsGlobal { get; set; } = true;

        #region Individual notification
        //The only user who will be notified
        [ForeignKey("UserToNotify")]
        public string UserId { get; set; }
        public virtual ApplicationUser UserToNotify { get; set; }

        public bool IsSeen { get; set; } = false;
        public bool IsClicked { get; set; } = false;
        #endregion


        #region Global notifications

        //The user who generated notification for others
        //Used also to filter out the notification for the creator
        public string UserCreatorId { get; set; }
        public virtual ApplicationUser UserCreator { get; set; }

        public virtual ISet<ApplicationUser> UsersSawNotifications { get; set; } = new HashSet<ApplicationUser>();
        public virtual ISet<ApplicationUser> UsersClickedNotifications { get; set; } = new HashSet<ApplicationUser>();
        #endregion

    }
}