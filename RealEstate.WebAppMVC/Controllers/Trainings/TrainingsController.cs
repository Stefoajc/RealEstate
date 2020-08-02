using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Services.Trainings;
using RealEstate.ViewModels.WebMVC.Trainings;

namespace RealEstate.WebAppMVC.Controllers.Trainings
{
    [Authorize(Roles = "Administrator,Agent,Maintenance")]
    public class TrainingsController : Controller
    {
        private readonly TrainingServices _trainingServices;

        public TrainingsController(TrainingServices trainingServices)
        {
            _trainingServices = trainingServices;
        }

        // GET: Trainings
        public async Task<ActionResult> Index()
        {
            var trainings = await _trainingServices.ListAsync();            

            return View(trainings);
        }
        
        // POST: /Trainings/Create
        [Authorize(Roles = "Administrator,Maintenance")]
        [HttpPost]
        public async Task<ActionResult> Create(TrainingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var createdTraining = await _trainingServices.CreateAsync(model, User.Identity.GetUserId());

            return Json(createdTraining);
        }

        // POST: /Trainings/Edit
        [Authorize(Roles = "Administrator,Maintenance")]
        [HttpPost]
        public async Task<ActionResult> Edit(TrainingEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var createdTraining = await _trainingServices.EditAsync(model, User.Identity.GetUserId());

            return Json(createdTraining);
        }


        // POST: /Trainings/Delete
        [Authorize(Roles = "Administrator,Maintenance")]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _trainingServices.DeleteAsync(id, User.Identity.GetUserId());
            return Json("STATUS_OK");
        }
    }
}