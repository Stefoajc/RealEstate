using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services.Forum;
using RealEstate.ViewModels.WebMVC.Forum;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers.Forum
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly PostServices postsManager;
        private readonly CommentServices commentsManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public CommentsController(PostServices postServices, CommentServices commentServices)
        {
            postsManager = postServices;
            commentsManager = commentServices;
        }

        //
        // POST: Comments/Create
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(CommentCreateViewModel model)
        {
            _logger.Info("Creating comment! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Info("Invalid comment Form! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }
            if (!await postsManager.Exists(model.PostId))
            {
                ModelState.AddModelError("PostId", "Постът, който искате да коментирате не съществува.");
                return Json(ModelState.ToDictionary());
            }
            try
            {
                var createdComment = await commentsManager.Create(model, User.Identity.GetUserId());
                _logger.Info("Comment created successfully!");

                return Json(new
                {
                    CreationDate = createdComment.CreationDate.ToString("MMMM dd, yyyy"),
                    UserImageUrl = createdComment.UserImageUrl,
                    Author = createdComment.Author,
                    Body = createdComment.Body,
                    CommentId = createdComment.CommentId
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Creating comment failed!");
                throw;
            }
        }

        //
        // POST: Comments/Edit
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(CommentEditViewModel model)
        {
            _logger.Info("Editing comment! Params: " + model.ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Editing comment Form Invalid! Error: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                await commentsManager.Edit(model, User.Identity.GetUserId());
                _logger.Info("Comment edited successfully!");

                return Json(model.Body);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Editing comment failed!");
                throw;
            }

        }

        //
        // POST: Comments/Delete
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.Info("Deleting Comment With Id: " + id);

            try
            {
                await commentsManager.Delete(id, User.Identity.GetUserId());
                _logger.Info("Deleting comment successfully!");

                return Json("STATUS_OK");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Deleting comment failed");
                throw;
            }
        }



    }
}