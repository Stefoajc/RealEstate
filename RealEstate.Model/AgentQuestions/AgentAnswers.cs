using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstate.Model.AgentQuestions
{
    public class AgentAnswers
    {
        [ForeignKey("Question")]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public string Answer { get; set; }

        public bool IsAcceptedAsCorrect { get; set; }


        [ForeignKey("Agent")]
        public string AgentId { get; set; }
        public virtual AgentUsers Agent { get; set; }

        public virtual AgentQuestions Question { get; set; }
    }
}
