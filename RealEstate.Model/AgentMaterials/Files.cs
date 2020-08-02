using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.AgentMaterials
{
    public class Files
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ForeignKey("Folder")]
        public int? FolderId { get; set; }
        public virtual Folders Folder { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public string RelativePath { get; set; }    
        public long SizeInBytes { get; set; }


        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual ApplicationUser Agent { get; set; }
    }
}
