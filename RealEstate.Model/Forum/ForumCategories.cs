using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.Forum
{
    public class ForumCategories
    {
        [Key]
        public int ForumCategoryId { get; set; }

        public string Name { get; set; }
        public string Info { get; set; }

        /// <summary>
        /// Are themes allowed to be open in the category
        /// </summary>
        public bool IsOpenForThemes { get; set; }

        public bool IsClosed { get; set; } = false;

        [ForeignKey("ForumCategoryParent")]
        public int? ForumCategoryParentId { get; set; }
        public virtual ForumCategories ForumCategoryParent { get; set; }

        [ForeignKey("Creator")]
        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public virtual ISet<Themes> Themes { get; set; } = new HashSet<Themes>();
    }
}