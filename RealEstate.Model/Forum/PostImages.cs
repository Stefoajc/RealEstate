using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Model.Forum
{
    public class PostImages
    {
        [Key]
        public int ImageId { get; set; }
        public string ImagePath { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public virtual Posts Post { get; set; }
    }
}
