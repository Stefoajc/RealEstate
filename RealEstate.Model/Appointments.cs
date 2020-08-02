using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    public class Appointments
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime AppointmentDate { get; set; }
        public bool IsApprovedByAgent { get; set; } = false;
        public string AppointmentDescription { get; set; }


        //[Required(ErrorMessage = "Въведете име!")]
        //public string ClientName { get; set; }
        //[Required(ErrorMessage = "Въведете телефонен номер!")]
        //public string ClientPhoneNumber { get; set; }
        //[EmailAddress]
        //public string ClientEmail { get; set; }

        public virtual NonRegisteredAppointmentUsers NonRegisteredUser { get; set; }

        [ForeignKey("IssuerUser")]
        public string IssuerUserId { get; set; }
        public virtual ApplicationUser IssuerUser { get; set; }


        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual AgentUsers Agent { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual PropertiesBase Property { get; set; }

        public bool isDeleted { get; set; }
    }

    public class NonRegisteredAppointmentUsers
    {
        [Key]
        [ForeignKey("Appointment")]
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientEmail { get; set; }

        public virtual Appointments Appointment { get; set; }
    }
}
