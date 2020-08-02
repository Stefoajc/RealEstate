using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Forum
{
    public class ForumCategoryViewModels
    {
    }

    public class ForumCategoryCreateViewModel
    {
        [Required(ErrorMessage = "Въведете име!")]
        public string Name { get; set; }
        public string Info { get; set; }
        [Required(ErrorMessage = "Изберете дали е отворена за създаване на теми!")]
        public bool IsOpenForThemes { get; set; }

        public int? ForumCategoryParentId { get; set; }
    }

    public class ForumCategoryEditViewModel
    {
        [Required(ErrorMessage = "Изберете категория за редактиране!")]
        public int ForumCategoryId { get; set; }
        [Required(ErrorMessage = "Въведете име!")]
        public string Name { get; set; }
        public string Info { get; set; }
        [Required(ErrorMessage = "Изберете дали е отворена за създаване на теми!")]
        public bool IsOpenForThemes { get; set; }
    }

    public class ForumCategoryDetailViewModel
    {
        public int ForumCategoryId { get; set; }
        public int? ForumCategoryParentId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public bool IsOpenForThemes { get; set; }
    }

    public class ForumCategoriesLinkViewModel
    {
        public int ForumCategoryId { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; }
    }
}