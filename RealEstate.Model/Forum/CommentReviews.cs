using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Forum
{
    public class CommentReviews : ForumReviews
    {
        public CommentReviews() : base() { }

        [ForeignKey("Comment")]
        public int? CommentId { get; set; }
        public Comments Comment { get; set; }
    }
}