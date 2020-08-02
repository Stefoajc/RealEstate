using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RealEstate.Model.Forum;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.Services.Forum.AutoMapper
{
    public static class AutoMapperForumConfiguration
    {
        public static readonly Action<IMapperConfigurationExpression> ConfigAction = cfg =>
        {
            cfg.AddProfile(new ForumCategoriesProfile());
            cfg.AddProfile(new ThemesProfile());
            cfg.AddProfile(new PostsProfile());
            cfg.AddProfile(new CommentsProfile());
            cfg.AddProfile(new ForumReviewsProfile());
        };

        public static void Configure()
        {
            Mapper.Initialize(ConfigAction);
        }
    }

    #region Forum

    public class ForumCategoriesProfile : Profile
    {
        public ForumCategoriesProfile()
        {
            CreateMap<ForumCategoryCreateViewModel, ForumCategories>()
                .ForMember(l => l.CreatorId, opt => opt.ResolveUsing((theme, dst, arg3, res) => (string)res.Options.Items["CreatorId"]));

            CreateMap<ForumCategoryEditViewModel, ForumCategories>();

            CreateMap<ForumCategories, ForumCategoryDetailViewModel>();
        }
    }

    public class ThemesProfile : Profile
    {
        public ThemesProfile()
        {
            CreateMap<ThemeCreateViewModel, Themes>()
                .ForMember(p => p.CreatorId, opt => opt.ResolveUsing((post, dest, arg3, res) => (string)res.Options.Items["UserId"]));
            CreateMap<ThemeEditViewModel, Themes>();
            CreateMap<Themes, ThemeDetailsViewModel>()
                .ForMember(t => t.CategoryName, opt => opt.MapFrom(o => o.Category != null ? o.Category.Name : null));
        }
    }

    public class PostsProfile : Profile
    {
        public PostsProfile()
        {
            CreateMap<PostCreateViewModel, Posts>()
                .ForMember(p => p.Tags, opt => opt.Ignore())
                .ForMember(p => p.Images, opt => opt.Ignore())
                .ForMember(p => p.Title, opt => opt.MapFrom(o => o.Title.Trim(' ')))
                .ForMember(p => p.CreatorId, opt => opt.ResolveUsing((post, dest, arg3, res) => (string)res.Options.Items["UserId"]));
            CreateMap<PostEditViewModel, Posts>()
                .ForMember(p => p.Title, opt => opt.MapFrom(o => o.Title.Trim(' ')));

            CreateMap<Posts, ReviewsStarsPartialViewModel>()
                .ForMember(r => r.AverageScore, opt => opt.MapFrom(o => o.PostReviews.Average(p => p.Score)))
                .ForMember(r => r.ReviewsCount, opt => opt.MapFrom(o => o.PostReviews.Count));
            CreateMap<Posts, PostDetailViewModel>()
                .ForMember(p => p.AuthorId, opt => opt.MapFrom(post => post.PostCreator.Id))
                .ForMember(p => p.AuthorName, opt => opt.MapFrom(post => string.IsNullOrEmpty(post.PostCreator.FirstName) && string.IsNullOrEmpty(post.PostCreator.LastName) ? post.PostCreator.UserName : post.PostCreator.FirstName + " " + post.PostCreator.LastName ))
                .ForMember(p => p.ThemeId, opt => opt.MapFrom(post => post.Theme.ThemeId))
                .ForMember(p => p.ThemeName, opt => opt.MapFrom(post => post.Theme.Name))
                .ForMember(p => p.CommentsCount, opt => opt.MapFrom(post => post.Comments.Count))
                .ForMember(p => p.ImageUrls, opt => opt.MapFrom(o => o.Images.Select(i => i.ImagePath).ToList()))
                .ForMember(p => p.ReviewsInfo, opt => opt.MapFrom(post => post.PostReviews.Any() ? post : null));

            CreateMap<Posts, PostForumIndexViewModel>()
                .ForMember(p => p.ThemeName, opt => opt.MapFrom(o => o.Theme.Name))
                .ForMember(p => p.AuthorName, opt => opt.MapFrom(post => string.IsNullOrEmpty(post.PostCreator.FirstName) && string.IsNullOrEmpty(post.PostCreator.LastName) ? post.PostCreator.UserName : post.PostCreator.FirstName + " " + post.PostCreator.LastName))
                .ForMember(p => p.ImageUrls, opt => opt.MapFrom(o => o.Images.Select(i => i.ImagePath).ToList()));

            CreateMap<Posts, PostSideViewModel>()
                .ForMember(p => p.MediaUrl,
                    opt => opt.MapFrom(o =>
                        o.VideoUrl ?? o.Images.Select(i => i.ImagePath).FirstOrDefault() ?? @"\Resources\noimage.png"));
        }
    }

    public class CommentsProfile : Profile
    {
        public CommentsProfile()
        {
            CreateMap<CommentCreateViewModel, Comments>()
                .ForMember(p => p.UserId, opt => opt.ResolveUsing((post, dest, arg3, res) => (string)res.Options.Items["UserId"]));
            CreateMap<CommentEditViewModel, Comments>();
            CreateMap<Comments, CommentListViewModel>();
            CreateMap<Comments, CommentSideViewModel>()
                .ForMember(c => c.Views, opt => opt.MapFrom(com => com.Post.Views))
                .ForMember(c => c.Comment, opt => opt.MapFrom(com => com.Body))
                .ForMember(c => c.UserImageUrl,
                    opt => opt.MapFrom(u =>
                        u.CommentWriter.Images.Select(i => i.ImagePath).FirstOrDefault() ?? @"\Resources\noimage.png"));


            CreateMap<Comments, CommentPostDetailsViewModel>()
                .ForMember(c => c.Body, opt => opt.MapFrom(com => com.Body))
                .ForMember(c => c.Author, opt => opt.MapFrom(com => com.CommentWriter.UserName))
                .ForMember(c => c.CreationDate, opt => opt.MapFrom(com => com.CreatedOn))
                .ForMember(c => c.UserImageUrl,
                    opt => opt.MapFrom(u =>
                        u.CommentWriter.Images.Select(i => i.ImagePath).FirstOrDefault() ?? @"\Resources\noimage.png"));
        }
    }

    public class ForumReviewsProfile : Profile
    {
        public ForumReviewsProfile()
        {
            CreateMap<ForumReviewCreateViewModel, CommentReviews>()
                .ForMember(p => p.UserId, opt => opt.ResolveUsing((post, dest, arg3, res) => (string)res.Options.Items["UserId"]))
                .ForMember(fr => fr.CommentId, map => map.MapFrom(o => o.ReviewedItemId))
                .ForMember(fr => fr.ReviewText, map => map.MapFrom(o => o.ReviewText))
                .ForMember(fr => fr.Score, map => map.MapFrom(o => o.Score));
            CreateMap<ForumReviewCreateViewModel, PostReviews>()
                .ForMember(p => p.UserId, opt => opt.ResolveUsing((post, dest, arg3, res) => (string)res.Options.Items["UserId"]))
                .ForMember(fr => fr.PostId, map => map.MapFrom(o => o.ReviewedItemId))
                .ForMember(fr => fr.ReviewText, map => map.MapFrom(o => o.ReviewText))
                .ForMember(fr => fr.Score, map => map.MapFrom(o => o.Score));
        }
    }

    public class TagsProfile : Profile
    {
        public TagsProfile()
        {
            CreateMap<Tags, TagDetailViewModel>();
        }
    }
    #endregion
}