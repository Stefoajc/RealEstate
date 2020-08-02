using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Services;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.WebAppMVC.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ImageServices _imagesManager;

        [Inject]
        public ImagesController(ImageServices imageServices)
        {
            _imagesManager = imageServices;
        }


        public ActionResult AddImage(ImageCreateViewModel model)
        {
            throw new NotImplementedException();
        }

        //
        //POST Images/DeleteImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteImage(int id)
        {
            await _imagesManager.DeleteImage(id, User.Identity.GetUserId());
            return Json("STATUS_OK");
        }
    }
}