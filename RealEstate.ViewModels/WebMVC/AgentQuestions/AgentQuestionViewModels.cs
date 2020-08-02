using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.AgentQuestions
{

    public class AgentQuestionListViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public DateTime CreatedOn { get; set; }

        public string AskingAgentId { get; set; }
        public string AskingAgentName { get; set; }
        public string AskingAgentImage { get; set; }


        public int? AnswerId { get; set; }
        public string Answer { get; set; }
        public DateTime? AnswerCreatedOn { get; set; }
        public DateTime? AnswerModifiedOn { get; set; }
        public bool? IsAccepted { get; set; }

        public string AnsweringAgentId { get; set; }
        public string AnsweringAgentName { get; set; }
        public string AnsweringAgentImage { get; set; }
        

        public bool IsAllowedToAccept { get; set; }

        public bool IsAllowedToEditQuestion { get; set; }
        public bool IsAllowedToDeleteQuestion { get; set; }

        public bool IsAllowedToEditAnswer { get; set; }
        public bool IsAllowedToDeleteAnswer { get; set; }
    }

    public class AgentQuestionCreateViewModel
    {
        [Required(ErrorMessage = "Полето за въпрос е празно")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Въпросът трябва да е от 10 до 1000 символа дълъг!")]
        public string Question { get; set; }
    }

    public class AgentQuestionEditViewModel
    {
        [Required(ErrorMessage = "Не е установено кой въпрос се коригира")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Полето за въпрос е празно")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Въпросът трябва да е от 10 до 1000 символа дълъг!")]
        public string Question { get; set; }
    }
}