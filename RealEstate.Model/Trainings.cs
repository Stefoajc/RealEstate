using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Model
{
    public class Trainings
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required]
        public DateTime TrainingDate { get; set; }
        [Required]
        public string TrainingTheme { get; set; }
        public string AdditionalDescription { get; set; }
        public string TrainingMaterialsFolderLink { get; set; }
    }
}