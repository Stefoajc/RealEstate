using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model.Forum
{
    public class Tags
    {
        [Key]
        public int TagId { get; set; }
        public string Name { get; set; }

        public virtual ISet<Posts> Posts { get; set; } = new HashSet<Posts>();
    }
}
