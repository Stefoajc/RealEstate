using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Notifications
{
    public class NotificationTypes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Type { get; set; }
        public string ImageAssociation { get; set; }

        public virtual ISet<Notifications> Notifications { get; set; }
    }

    public enum NotificationType
    {
        Basic,
        Birthday,
        Holiday,
        Report,
        Learning,
        Property,
        Search,
        Post,
        Calendar,
        Question,
        Material,
        Contact,
        Inquery
    }
}