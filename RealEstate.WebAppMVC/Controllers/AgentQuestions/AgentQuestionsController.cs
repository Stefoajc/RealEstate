using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.AgentQuestions;
using RealEstate.ViewModels.WebMVC.AgentQuestions;
using RealEstate.WebAppMVC.Helpers;

namespace RealEstate.WebAppMVC.Controllers.AgentQuestions
{
    [Authorize(Roles = "Administrator,Agent")]
    public class AgentQuestionsController : Controller
    {
        private readonly AgentServices _agentServices;
        private readonly AgentQuestionServices _agentQuestionServices;

        public AgentQuestionsController(AgentQuestionServices agentQuestionServices, AgentServices agentServices)
        {
            _agentQuestionServices = agentQuestionServices;
            _agentServices = agentServices;
        }


        // GET: AgentQuestions
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var currentlyLoggedUserId = User.Identity.GetUserId();
            var questions = await _agentQuestionServices.List(userIssuingTheQuestionsId: currentlyLoggedUserId);

            //Filters setup
            ViewBag.Agents = await _agentServices.GetAgentsForDropDown();
            //------------

            return View(questions);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AgentQuestionCreateViewModel questionToCreate)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var createdQuestion = await _agentQuestionServices.Create(questionToCreate.Question, User.Identity.GetUserId());

            return Json(createdQuestion);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AgentQuestionEditViewModel questionToEdit)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var editedQuestion =
                await _agentQuestionServices.Edit(questionToEdit.QuestionId, questionToEdit.Question, User.Identity.GetUserId());

            return Json(editedQuestion);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _agentQuestionServices.Delete(id, User.Identity.GetUserId());
            
            return Json("STATUS_OK");
        }
    }
}