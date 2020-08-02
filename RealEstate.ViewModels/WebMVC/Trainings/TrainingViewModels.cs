using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Trainings
{
    public class TrainingViewModels
    {
        
    }

    public class TrainingListViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime TrainingDate { get; set; }
        public string TrainingTheme { get; set; }
        public string AdditionalDescription { get; set; }
        public string TrainingMaterialsFolderLink { get; set; }
    }

    public class TrainingCreateViewModel
    {
        [Required(ErrorMessage = "Датата е задължителна!")]
        public DateTime? TrainingDate { get; set; }
        [Required(ErrorMessage = "Темата на обучението е задължителна")]
        public string TrainingTheme { get; set; }
        public string AdditionalDescription { get; set; }
        public string TrainingMaterialsFolderLink { get; set; }
    }

    public class TrainingEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Датата е задължителна!")]
        public DateTime? TrainingDate { get; set; }
        [Required(ErrorMessage = "Темата на обучението е задължителна")]
        public string TrainingTheme { get; set; }
        public string AdditionalDescription { get; set; }
        public string TrainingMaterialsFolderLink { get; set; }
    }
}