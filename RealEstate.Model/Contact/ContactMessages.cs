using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.Contact
{
    public class ContactMessages
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
