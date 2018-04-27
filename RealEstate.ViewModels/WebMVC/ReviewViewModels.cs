using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class ReviewViewModels
    {
        
    }

    public class CreateReviewViewModel
    {
        public int? ReviewScore { get; set; }
        public string ReviewText { get; set; }

        [Required]
        public object ReviewForeignKey { get; set; }
    }


    public class EditReviewViewModel
    {
        public int ReviewId { get; set; }
        public string ReviewText { get; set; }
        public int? ReviewScore { get; set; }
    }

    public class ListReviewViewModel
    {
        public string ReviewUserFullname { get; set; }
        public string  UserProfileImagePath { get; set; }
        public int? ReviewScore { get; set; }
        public string ReviewText { get; set; }
    }

    public class ListUserReviewsViewModel
    {
        public int PropertyId { get; set; } // For Link to property Details
        public string PropertyName { get; set; }
        public string PropertyImage { get; set; }
        public int? ReviewScore { get; set; }
        public string ReviewText { get; set; }

    }


    public enum ReviewTypes
    {
        Property,
        Sight,
        City,
        Owner
    }
}