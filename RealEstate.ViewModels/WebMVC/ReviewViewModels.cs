using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class ReviewViewModels
    {
        
    }

    public class ReviewStarsPartialViewModel
    {
        public ReviewStarsPartialViewModel(string reviewedItemType, object reviewForeignKey)
        {
            ReviewForeignKey = reviewForeignKey;
            ReviewedItemType = reviewedItemType;
        }

        [Required]
        public string ReviewedItemType { get; set; }
        [Required(ErrorMessage = "Задължително е да се въведе относно какво е ревюто!")]
        public object ReviewForeignKey { get; set; }
    }

    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "Задължително е да се въведе рейтинг точки!")]
        public int ReviewScore { get; set; }
        public string ReviewText { get; set; }

        [Required(ErrorMessage = "Задължително е да се въведе относно какво е ревюто!")]
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

    public class PropertyReviewCreateViewModel
    {
        [Required(ErrorMessage = "Изберете имот за даване на Ревю!")]
        public int PropertyId { get; set; }
        public string ReviewText { get; set; }
        [Required(ErrorMessage = "Въведете резултат на ревюто!")]
        public int Score { get; set; }
    }


    public class ReviewsStarsPartialViewModel
    {
        public ReviewsStarsPartialViewModel() {}

        public double? AverageScore { get; set; }
        public int ReviewsCount { get; set; }
    }


    public class ReviewListViewModel
    {
        public int? ReviewScore { get; set; }
        public string ReviewText { get; set; }
        public DateTime CreatedOn { get; set; }
        public ClientReviewListViewModel ClientReviewer { get; set; }
    }


    public enum ReviewTypes
    {
        Property,
        Sight,
        City,
        Owner,
        Agent
    }
}