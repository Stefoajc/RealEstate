using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Forum
{
    public abstract class ForumReviews
    {
        public ForumReviews()
        {
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int ReviewId { get; set; }
        public string ReviewText { get; set; }
        public byte Score { get; set; }
        public DateTime CreatedOn { get; private set; }

        #region ForeignKeys
        [ForeignKey("ReviewCreator")]
        public string UserId { get; set; }
        #endregion

        #region NavProperties
        public virtual ApplicationUser ReviewCreator { get; set; }
        #endregion
    }
}