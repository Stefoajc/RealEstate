using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Forum
{
    public class Themes
    {
        public Themes()
        {
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int ThemeId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; private set; }

        #region ForeignKeys
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Creator")]
        public string CreatorId { get; set; }
        #endregion

        #region NavProperties
        public virtual ForumCategories Category { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public virtual ISet<Posts> Posts { get; set; } = new HashSet<Posts>();
        #endregion
    }
}