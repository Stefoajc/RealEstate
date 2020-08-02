using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Forum
{
    public class ThemeCreateViewModel
    {
        [Required(ErrorMessage = "Въведете име на темата")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Изберете категория")]
        public int CategoryId { get; set; }
    }

    public class ThemeEditViewModel
    {
        [Required(ErrorMessage = "Изберете тема за редактиране")]
        public int ThemeId { get; set; }
        [Required(ErrorMessage = "Въведете име на темата")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Изберете категория")]
        public int CategoryId { get; set; }
    }

    public class ThemeDetailsViewModel
    {
        public int ThemeId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CreatorId { get; set; }
    }

    public class ThemeLinkViewModel
    {
        public int ThemeId { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; }
    }
}