using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Forum
{
    public class Posts
    {
        public Posts()
        {
            CreatedOn = DateTime.Now;
            Views = 0;
        }

        [Key]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; private set; }
        public string VideoUrl { get; set; }
        public long Views { get; set; }

        #region ForeignKeys
        [ForeignKey("Theme")]
        public int? ThemeId { get; set; }
        [ForeignKey("PostCreator")]
        public string CreatorId { get; set; }
        #endregion

        #region NavProperties
        public virtual Themes Theme { get; set; }
        public virtual ApplicationUser PostCreator { get; set; }
        public virtual ISet<PostImages> Images { get; set; } = new HashSet<PostImages>();
        public virtual ISet<Comments> Comments { get; set; } = new HashSet<Comments>();
        public virtual ISet<PostReviews> PostReviews { get; set; } = new HashSet<PostReviews>();
        public virtual ISet<Tags> Tags { get; set; } = new HashSet<Tags>();
        #endregion
    }
}