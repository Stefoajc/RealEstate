using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Model.MailList
{
    public class EmailList
    {
        public EmailList()
        {
            EmailId = Guid.NewGuid().ToString();
        }

        [Key]
        public string EmailId { get; set; }
        public string EmailAddress { get; set; }
    }
}
