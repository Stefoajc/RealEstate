using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.AgentMaterials
{
    public class Folders
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        public virtual Folders Parent { get; set; }

        public string Name { get; set; }
        public string RelativePath { get; set; }

        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public ApplicationUser Agent { get; set; }


        public virtual ISet<Files> ChildFiles { get; set; } = new HashSet<Files>();
        public virtual ISet<Folders> ChildFolders { get; set; } = new HashSet<Folders>();
    }
}