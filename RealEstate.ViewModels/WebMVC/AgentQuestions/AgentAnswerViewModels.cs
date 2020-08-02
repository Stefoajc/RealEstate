using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.AgentQuestions
{
    public class AnswerCreateViewModel
    {
        [Required(ErrorMessage = "Не е въведен отговор")]
        [StringLength(Int32.MaxValue, MinimumLength = 20, ErrorMessage = "Отговорът трябва да е поне 20 символа")]
        public string Answer { get; set; }

        [Required(ErrorMessage = "Не е избран въпрос, за който е отговорът.")]
        public int QuestionId { get; set; }
    }

    public class AnswerEditViewModel
    {
        [Required(ErrorMessage = "Не е избран отговор за редакция!")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не е въведен отговор")]
        [StringLength(Int32.MaxValue, MinimumLength = 20, ErrorMessage = "Отговорът трябва да е поне 20 символа")]
        public string Answer { get; set; }
    }
}