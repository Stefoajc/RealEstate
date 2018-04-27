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
        public string NotificationText { get; set; }
        public string NotificationLink { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class NotificationCreateViewModel
    {
        public string NotificationText { get; set; }
        public string NotificationLink { get; set; }
        public string UserId { get; set; }
    }
}
