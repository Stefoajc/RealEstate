using Ninject;
using RealEstate.Services.Forum;

namespace RealEstate.WebAppMVC.Controllers.Forum
{
    public class ForumReviewsController
    {
        [Inject]
        public ReviewServices ReviewsManager { get; set; }



    }
}