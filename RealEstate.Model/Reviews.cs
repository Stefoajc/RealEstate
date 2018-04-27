using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model
{
    /// <summary>
    /// Only clients can make reviews
    /// </summary>
    public class Reviews
    {
        [Key]
        public int ReviewId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ClientUsers User { get; set; }

        public int? ReviewScore { get; set; }
        public string ReviewText { get; set; }
    }

    /// <summary>
    /// Reviews of the properties visited by the client (only visitors can give review)
    /// </summary>
    public class PropertyReviews:Reviews
    {
        //Make Composite with ReviewId
        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        public virtual Properties Property { get; set; }
    }

    /// <summary>
    /// Reviews of the Sight
    /// </summary>
    public class SightReviews : Reviews
    {
        //Make Composite with ReviewId
        [ForeignKey("Sight")]
        public int SightId { get; set; }

        //NavProperty
        public virtual Sights Sight { get; set; }
    }

    /// <summary>
    /// Reviews of the City
    /// </summary>
    public class CityReviews : Reviews
    {
        //Make Composite with ReviewId
        [ForeignKey("City")]
        public int CityId { get; set; }       
        public virtual Cities City { get; set; }
    }

    public class OwnerReviews : Reviews
    {
        [ForeignKey("OwnerUser")]
        public string OwnerId { get; set; }
        public virtual OwnerUsers OwnerUser { get; set; }
    }

    public class AgentReviews : Reviews
    {
        [ForeignKey("AgentUser")]
        public string AgentId { get; set; }
        public virtual AgentUsers AgentUser { get; set; }
    }
}
