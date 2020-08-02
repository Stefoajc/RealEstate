using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Ninject;
using RealEstate.Model.Forum;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC.Forum;

namespace RealEstate.Services.Forum
{
    public class CommentServices : BaseService
    {
        public CommentServices(IUnitOfWork unitOfWork,  ApplicationUserManager userMgr) : base(unitOfWork, userMgr) {}

        [Inject]
        public PostServices PostsManager { get; set; }

        public async Task<List<CommentSideViewModel>> ListLatest()
        {
            var comments = await unitOfWork.CommentsRepository
                .Include(c => c.Post, c => c.CommentWriter, c => c.CommentWriter.Images)
                .OrderByDescending(c => c.CreatedOn)
                .Take(5)
                .ToListAsync();

            return comments.Select(Mapper.Map<CommentSideViewModel>)
            .ToList();
        }        

        public async Task<List<CommentListViewModel>> Get(int postId)
        {
            if (!await PostsManager.Exists(postId))
            {
                throw new ArgumentException("Постът, на който търсите коментарите не е намерен!");
            }

            var comments = await unitOfWork.CommentsRepository.Where(c => c.PostId == postId).ToListAsync();

            return comments.Select(c => Mapper.Map<CommentListViewModel>(c)).ToList();
        }

        public async Task<List<CommentListViewModel>> UserComments(string userId)
        {
            if (!await IsUserExisting(userId))
            {
                throw new ArgumentException("Потребителят не е намерен!");
            }

            var userPosts = await unitOfWork.CommentsRepository
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return userPosts.Select(c => Mapper.Map<CommentListViewModel>(c)).ToList();
        }        

        public async Task<CommentPostDetailsViewModel> Create(CommentCreateViewModel model, string userId)
        {
            if (!await PostsManager.Exists(model.PostId))
            {
                throw new ArgumentException("Постът, който искате да коментирате не съществува!");
            }

            Comments commentToCreate = Mapper.Map<Comments>(model, opts => opts.Items.Add("UserId", userId));

            unitOfWork.CommentsRepository.Add(commentToCreate);
            unitOfWork.Save();

            var createdComment = await unitOfWork.CommentsRepository
                .Include(c => c.CommentWriter)
                .Where(c => c.CommentId == commentToCreate.CommentId)
                .FirstOrDefaultAsync();

            return Mapper.Map<CommentPostDetailsViewModel>(createdComment);
        }

        public async Task<CommentListViewModel> Edit(CommentEditViewModel model, string userId)
        {
            Comments comment = await unitOfWork.CommentsRepository.GetAll()
                .FirstOrDefaultAsync(c => c.CommentId == model.CommentId)
                ?? throw new ArgumentException("Коментарът, който искате да редактирате не съществува!"); ;

            if (!await IsAdminOrOwner(userId, comment))
            {
                throw new NotAuthorizedUserException("Коментарър, който искате да редактирате не е създаден от вас!");
            }

            Mapper.Map(model, comment);
            unitOfWork.CommentsRepository.Edit(comment);
            unitOfWork.Save();

            return Mapper.Map<CommentListViewModel>(comment);
        }

        public async Task Delete(int id, string userId)
        {
            Comments comment = await unitOfWork.CommentsRepository.GetAll()
                .FirstOrDefaultAsync(c => c.CommentId == id)
                ?? throw new ArgumentException("Коментарът, който искате да изтриете не съществува!");
            
            if (!await IsAdminOrOwner(userId, comment))
            {
                throw new NotAuthorizedUserException("Коментарър, който искате да изтриете не е създаден от вас!");
            }

            unitOfWork.CommentsRepository.Delete(comment);
            await unitOfWork.SaveAsync();
        }        


        public async Task<bool> Exists(int id) 
            => await unitOfWork.CommentsRepository.GetAll().AnyAsync(c => c.CommentId == id);


        #region Private methods

        private async Task<bool> IsAdminOrOwner(string userId, Comments comment)
            => await IsAdministrator(userId) || IsOwnerOfComment(userId, comment);

        private async Task<bool> IsAdministrator(string userId) => await userManager.IsInRoleAsync(userId, AdminRole);

        private static bool IsOwnerOfComment(string userId, Comments commentToDelete) => commentToDelete.UserId == userId;

        private async Task<bool> IsUserExisting(string userId) => await userManager.Users.AnyAsync(u => u.Id == userId);

        #endregion
    }
}