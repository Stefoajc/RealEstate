using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using RealEstate.ViewModels.CustomDataAnnotations;
using RealEstate.WebAppMVC.Helpers.DataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Forum
{
    public class VideoPostCreateViewModel
    {
        public string VideoUrl { get; set; }
    }

    public class PostCreateViewModel
    {
        [Required(ErrorMessage = "Въведете заглавие на поста!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Въведете тяло на поста!")]
        [AllowHtml]
        public string Body { get; set; }
        [Required(ErrorMessage = "Изберете тема в която да сложите този пост!")]
        public int ThemeId { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
        
        [FileType("JPG,JPEG,PNG,WEBP,GIF,BMP")]
        public List<HttpPostedFileBase> ImageFiles { get; set; } = new List<HttpPostedFileBase>();

        public string VideoUrl { get; set; }
    }

    public class PostEditViewModel
    {
        [Required(ErrorMessage = "Изберете пост който да редактирате!")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Въведете заглавие на поста!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Въведете тяло на поста!")]
        [AllowHtml]
        public string Body { get; set; }
    }

    public class PostDetailViewModel
    {
        public int PostId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int ThemeId { get; set; }
        public string ThemeName { get; set; }
        public int CommentsCount { get; set; }
        public string VideoUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public ReviewsStarsPartialViewModel ReviewsInfo { get; set; }

        public List<TagDetailViewModel> Tags { get; set; } = new List<TagDetailViewModel>();
        public List<CommentPostDetailsViewModel> Comments { get; set; } = new List<CommentPostDetailsViewModel>();
    }

    public class PostForumIndexViewModel
    {
        public int PostId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string CreatorId { get; set; }
        public string AuthorName { get; set; }
        public int ThemeId { get; set; }
        public string ThemeName { get; set; }
        public int CommentsCount { get; set; }
        public string VideoUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class PostSideViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        //Url depending what kind of post it is (video/image)
        public string MediaUrl { get; set; }

        public long Views { get; set; }
    }

}