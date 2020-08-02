using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.ViewModels.WebMVC
{
    class NotificationViewModels
    {
    }

    public class NotificationListViewModel
    {
        public int NotificationId { get; set; }
        public string NotificationText { get; set; }
        public string NotificationLink { get; set; }
        public string NotificationPicture { get; set; }

        public string NotificationType { get; set; }
        public string NotificationTypeImage { get; set; }

        //This could be agent profile picture/company logo
        public string NotificationSourcePicture { get; set; } 

        public bool IsSeen { get; set; }
        public bool IsClicked { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    public class NotificationCreateViewModel
    {
        public string NotificationText { get; set; }
        public string NotificationLink { get; set; }
        public string NotificationPicture { get; set; }

        public int NotificationTypeId { get; set; }
    }
}
