using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Forum
{
    public class ForumReviewCreateViewModel
    {
        public string ReviewText { get; set; }
        [Required(ErrorMessage = "Изберете резултат!")]
        public byte Score { get; set; }
        [Required(ErrorMessage = "Изберете оценен елемент!")]
        public int ReviewedItemId { get; set; }
    }

    public class ForumReviewEditViewModel
    {
        public int ReviewId { get; set; }
        public string ReviewText { get; set; }
        public byte Score { get; set; }
    }




    public class PostReviewCreateViewModel
    {
        public string ReviewText { get; set; }
        public byte Score { get; set; }

        public int PosttId { get; set; }
    }

    public class PostReviewEditViewModel
    {
        public int ReviewId { get; set; }
        public string ReviewText { get; set; }
        public byte Score { get; set; }
    }
}