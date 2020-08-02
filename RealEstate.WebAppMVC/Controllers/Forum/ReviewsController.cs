using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NLog;
using RealEstate.Extentions;
using RealEstate.Services.Forum;
using RealEstate.ViewModels.WebMVC.Forum;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers.Forum
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ReviewServices _reviewsManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ReviewsController(ReviewServices reviewServices)
        {
            _reviewsManager = reviewServices;
        }

        //
        // POST: /Reviews/Create
        [HttpPost]
        public async Task<ActionResult> Create(ForumReviewCreateViewModel review, string reviewedItemType)
        {
            _logger.Info("Creating Review! Params: " + (new { review, reviewedItemType }).ToJson());

            if (!ModelState.IsValid)
            {
                _logger.Error("Creating Review Form Invalid! Errors: " + ModelState.ToJson());
                return Json(ModelState.ToDictionary());
            }

            try
            {
                var createdReview = await _reviewsManager.Create(review, (ReviewType)Enum.Parse(typeof(ReviewType), reviewedItemType), User.Identity.GetUserId());
                _logger.Info("Creating Review Successfully!");

                return Json(createdReview);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Creating Review Failed!");
                throw;
            }

        }
    }
}