using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC.Forum
{

    public class CommentListViewModel
    {
        public int CommentId { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserId { get; set; }
    }

    public class CommentCreateViewModel
    {
        [AllowHtml, HtmlValidation(ErrorMessage = "Опитвате се да въведете опасно съдържание!"), Required(ErrorMessage = "Въведете тяло на коментарър!")]
        public string Body { get; set; }
        [Required(ErrorMessage = "Изберете пост!")]
        public int PostId { get; set; }
    }

    public class CommentEditViewModel
    {
        [Required(ErrorMessage = "Изберете коментар, който да редактираш!")]
        public int CommentId { get; set; }
        [Required(ErrorMessage = "Въведете тяло на коментарър!")]
        public string Body { get; set; }
    }

    public class CommentSideViewModel
    {
        public int PostId { get; set; }
        public string UserImageUrl { get; set; }
        public string Comment { get; set; }
        public long Views { get; set; }
    }

    public class CommentPostDetailsViewModel
    {
        public int CommentId { get; set; }
        public string UserImageUrl { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public string Body { get; set; }
    }
}