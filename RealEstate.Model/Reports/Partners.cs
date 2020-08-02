using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Reports
{
    /// <summary>
    /// This POCO represents the brokers from other agencies which
    /// are working with us
    /// </summary>
    public class Partners
    {
        public int Id { get; set; }
        public string PartnerName { get; set; }
        public string PartnerCompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string SocialMediaAccount { get; set; }

        public string AdditionalInformation { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ForeignKey("PartnerType")]
        public int PartnerTypeId { get; set; }
        public virtual PartnerTypes PartnerType { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual Cities City { get; set; }

        [ForeignKey("Agent")]
        public string AgentIdCreator { get; set; }
        public virtual AgentUsers Agent { get; set; }

        //This is used in reports (when property is shared with agent from another agency)
        public virtual ISet<Reports> Reports { get; set; } = new HashSet<Reports>();
    }
}