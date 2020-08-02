using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.AgentQuestions
{
    public class AgentQuestions
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [StringLength(1000)]
        public string Question { get; set; }

        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual AgentUsers Agent { get; set; }

        public virtual AgentAnswers Answer { get; set; }
    }
}
