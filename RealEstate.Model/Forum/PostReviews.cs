using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Forum
{
    public class PostReviews : ForumReviews
    {
        public PostReviews() : base() { }

        [ForeignKey("Post")]
        public int? PostId { get; set; }
        public virtual Posts Post { get; set; }
    }
}