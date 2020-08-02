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

namespace RealEstate.WebAppMVC.Controllers.AgentQuestions
{

    [Authorize(Roles = "Administrator,Agent")]
    public class AgentAnswersController : Controller
    {

        private readonly AgentServices _agentServices;
        private readonly AgentAnswerServices _agentAnswerService;
        private readonly AgentQuestionServices _agentQuestionServices;

        public AgentAnswersController(AgentServices agentServices, AgentAnswerServices agentAnswerService, AgentQuestionServices agentQuestionServices)
        {
            _agentServices = agentServices;
            _agentAnswerService = agentAnswerService;
            _agentQuestionServices = agentQuestionServices;
        }

        [HttpPost]
        public async Task<ActionResult> Create(AnswerCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            await _agentAnswerService.Create(model.Answer, model.QuestionId, User.Identity.GetUserId());

            var questionWithCreatedAnswer = await _agentQuestionServices.Get(model.QuestionId, User.Identity.GetUserId());

            return Json(questionWithCreatedAnswer);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AnswerEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            await _agentAnswerService.Edit(model.Id, model.Answer, User.Identity.GetUserId());

            var questionWithEditedAnswer = await _agentQuestionServices.Get(model.Id, User.Identity.GetUserId());

            return Json(questionWithEditedAnswer);
        }


        [HttpPost]
        public async Task<ActionResult> Accept(int id)
        {
            await _agentAnswerService.Accept(id, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _agentAnswerService.Delete(id, User.Identity.GetUserId());

            var questionWithEditedAnswer = await _agentQuestionServices.Get(id, User.Identity.GetUserId());

            return Json(questionWithEditedAnswer);
        }
    }
}