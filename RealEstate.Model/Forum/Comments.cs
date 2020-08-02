using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Forum
{
    public class Comments
    {
        public Comments()
        {
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int CommentId { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; private set; }

        #region ForeignKeys
        [ForeignKey("Post")]
        public int PostId { get; set; }
        [ForeignKey("CommentWriter")]
        public string UserId { get; set; }
        #endregion

        #region NavProperties
        public virtual Posts Post { get; set; }
        public virtual ApplicationUser CommentWriter { get; set; }
        public virtual ISet<CommentReviews> Reviews { get; set; } = new HashSet<CommentReviews>();
        #endregion
    }
}