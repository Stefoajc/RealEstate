using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    /// <summary>
    /// Reservations can only be seen by the Client created them and the Admin
    /// </summary>
    public class Reservations
    {
        public Reservations()
        {
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int ReservationId { get; set; }

        [ForeignKey("ClientUser")]
        public string ClientUserId { get; set; }
        public virtual ClientUsers ClientUser { get; set; }

        [ForeignKey("Rental")]
        public int? RentalId { get; set; }
        public virtual RentalsInfo Rental { get; set; }

        [ForeignKey("Property")]
        public int? PropertyId { get; set; }
        public virtual Properties Property { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public ApproveStatus ApproveStatus { get; set; }

        public decimal CaparoPrice { get; set; }
        public decimal FullPrice { get; set; }

    }


    public enum PaymentStatus
    {
        Pending,
        CaparoPayed,
        FullPayed
    }

    public enum ApproveStatus
    {
        Pending,
        CanceledByOwner,
        CanceledByClient,
        Approved
    }
}
