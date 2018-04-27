using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class Notifications
    {
        public Notifications()
        {
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int NotificationId { get; set; }
        public string NotificationText { get; set; }
        public string NotificationLink { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsSeen { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
