using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.AgentQuestions;

namespace RealEstate.Services.AgentQuestions
{
    public class AgentQuestionServices
    {
        private readonly RealEstateDbContext _dbContext;
        private readonly ApplicationUserManager _userManager;
        private readonly INotificationCreator _notificationCreator;

        public AgentQuestionServices(RealEstateDbContext dbContext, ApplicationUserManager userManager, INotificationCreator notificationCreator)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _notificationCreator = notificationCreator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIssuingTheQuestionsId">The user which makes the query to get the questions</param>
        /// <returns></returns>
        public async Task<List<AgentQuestionListViewModel>> List(string userIssuingTheQuestionsId)
        {
            var questions = await _dbContext.AgentQuestions
                .Include(q => q.Agent)
                .Include(q => q.Agent.Images)
                .Include(q => q.Answer)
                .Include(q => q.Answer.Agent)
                .Include(q => q.Answer.Agent.Images)
                .Select(q => new AgentQuestionListViewModel
                {
                    Id = q.Id,
                    CreatedOn = q.CreatedOn,
                    Question = q.Question,
                    AskingAgentId = q.AgentId,
                    AskingAgentName = q.Agent.FirstName + " " + q.Agent.LastName,
                    AskingAgentImage = q.Agent.Images
                        .Select(i =>i.ImagePath)
                        .FirstOrDefault(),

                    AnswerId = q.Answer != null ? (int?)q.Answer.Id : null,
                    Answer = q.Answer != null ? q.Answer.Answer : null,
                    AnswerCreatedOn = q.Answer != null ? (DateTime?)q.Answer.CreatedOn : null,
                    AnswerModifiedOn = q.Answer != null ? (DateTime?)q.Answer.ModifiedOn : null,
                    IsAccepted = q.Answer != null ? (bool?)q.Answer.IsAcceptedAsCorrect : null,
                    AnsweringAgentId = q.Answer != null ? q.Answer.AgentId : null,
                    AnsweringAgentName = q.Answer != null ? q.Answer.Agent.FirstName + " " + q.Answer.Agent.LastName : null,
                    AnsweringAgentImage = q.Answer != null ?  q.Answer.Agent.Images
                        .Select(i => i.ImagePath)
                        .FirstOrDefault() : null,
                })
                .ToListAsync();


            foreach (var question in questions)
            {
                //Only other agents can accept the answer as correct
                question.IsAllowedToAccept = question.IsAccepted == true || question.AskingAgentId != userIssuingTheQuestionsId;
                //Only admin or (creator if it's unaccepted) can delete answer
                question.IsAllowedToDeleteAnswer = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                       Enum.GetName(typeof(Role), Role.Administrator))
                                                   || (question.IsAccepted != true && question.AnsweringAgentId ==
                                                       userIssuingTheQuestionsId);

                question.IsAllowedToEditAnswer = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                     Enum.GetName(typeof(Role), Role.Administrator))
                                                 || question.IsAccepted != true;

                question.IsAllowedToDeleteQuestion = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                         Enum.GetName(typeof(Role), Role.Administrator))
                                                     || (question.AnswerId == null && question.AskingAgentId ==
                                                         userIssuingTheQuestionsId);

                question.IsAllowedToEditQuestion = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                       Enum.GetName(typeof(Role), Role.Administrator))
                                                   || (question.AnswerId == null && question.AskingAgentId ==
                                                      userIssuingTheQuestionsId);
            }


            return questions;
        }

        public async Task<AgentQuestionListViewModel> Get(int id, string userIssuingTheQuestionsId)
        {
            var question = await _dbContext.AgentQuestions
                .Include(q => q.Agent)
                .Include(q => q.Agent.Images)
                .Include(q => q.Answer)
                .Include(q => q.Answer.Agent)
                .Include(q => q.Answer.Agent.Images)
                .Where(q => q.Id == id)
                .Select(q => new AgentQuestionListViewModel
                {
                    Id = q.Id,
                    CreatedOn = q.CreatedOn,
                    Question = q.Question,
                    AskingAgentId = q.AgentId,
                    AskingAgentName = q.Agent.FirstName + " " + q.Agent.LastName,
                    AskingAgentImage = q.Agent.Images
                        .Select(i => i.ImagePath)
                        .FirstOrDefault(),

                    AnswerId = q.Answer != null ? (int?)q.Answer.Id : null,
                    Answer = q.Answer != null ? q.Answer.Answer : null,
                    AnswerCreatedOn = q.Answer != null ? (DateTime?)q.Answer.CreatedOn : null,
                    AnswerModifiedOn = q.Answer != null ? (DateTime?)q.Answer.ModifiedOn : null,
                    IsAccepted = q.Answer != null ? (bool?)q.Answer.IsAcceptedAsCorrect : null,
                    AnsweringAgentId = q.Answer != null ? q.Answer.AgentId : null,
                    AnsweringAgentName = q.Answer != null ? q.Answer.Agent.FirstName + " " + q.Answer.Agent.LastName : null,
                    AnsweringAgentImage = q.Answer != null ? q.Answer.Agent.Images
                        .Select(i => i.ImagePath)
                        .FirstOrDefault() : null,
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен въпросът!");

            //Only other agents can accept the answer as correct
            question.IsAllowedToAccept = question.IsAccepted == true || question.AskingAgentId != userIssuingTheQuestionsId;
            //Only admin or (creator if it's unaccepted) can delete answer
            question.IsAllowedToDeleteAnswer = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                   Enum.GetName(typeof(Role), Role.Administrator))
                                               || (question.IsAccepted != true && question.AnsweringAgentId ==
                                                   userIssuingTheQuestionsId);

            question.IsAllowedToEditAnswer = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                 Enum.GetName(typeof(Role), Role.Administrator))
                                             || question.IsAccepted != true;

            question.IsAllowedToDeleteQuestion = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                     Enum.GetName(typeof(Role), Role.Administrator))
                                                 || (question.AnswerId == null && question.AskingAgentId ==
                                                     userIssuingTheQuestionsId);

            question.IsAllowedToEditQuestion = await _userManager.IsInRoleAsync(userIssuingTheQuestionsId,
                                                   Enum.GetName(typeof(Role), Role.Administrator))
                                               || (question.AnswerId == null && question.AskingAgentId ==
                                                   userIssuingTheQuestionsId);

            return question;
        }

        public async Task<AgentQuestionListViewModel> Create(string question, string agentId)
        {
            if (string.IsNullOrEmpty(question))
            {
                throw new ArgumentException("Не е въведен въпрос!");
            }

            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът задаващ въпроса в системата!");
            }

            var questionToCreate = new Model.AgentQuestions.AgentQuestions
            {
                Question = question,
                AgentId = agentId
            };

            _dbContext.AgentQuestions.Add(questionToCreate);
            await _dbContext.SaveChangesAsync();

            var createdQuestion = await Get(questionToCreate.Id, agentId);

            #region Create Notification

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Question,
                NotificationPicture = "",
                NotificationLink = "/agentquestions/index?questionId=" + createdQuestion.Id,
                NotificationText = createdQuestion.AskingAgentName + " пита: " + createdQuestion.Question                
            };

            await _notificationCreator.CreateGlobalNotification(notificationToCreate, agentId);

            #endregion

            return createdQuestion;
        }

        public async Task<AgentQuestionListViewModel> Edit(int questionId, string question, string agentId)
        {
            if (string.IsNullOrEmpty(question))
            {
                throw new ArgumentException("Не е въведен въпрос!");
            }

            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е установен брокерът задаващ въпроса!");
            }

            if (!(await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                  || await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат достъп !");
            }

            var questionToEdit = await _dbContext.AgentQuestions
                .FirstOrDefaultAsync(q => q.Id == questionId)
                ?? throw new ContentNotFoundException("Не е намерен въпросът, който искате да редактирате!");

            if (questionToEdit.AgentId != agentId && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                //User is not the creator, neither is administrator
                throw new NotAuthorizedUserException("Трябва да сте създател или администратор за да редактирате този въпрос!");
            }

            if (questionToEdit.AgentId == agentId && questionToEdit.Answer != null)
            {
                throw new NotAuthorizedUserException("Нямате право да редактирате отговорен въпрос! Свържете се с админ !");
            }

            _dbContext.AgentQuestions.Attach(questionToEdit);
            questionToEdit.Question = question;
            await _dbContext.SaveChangesAsync();


            return await Get(questionId, agentId);
        }

        public async Task Delete(int questionId, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е установен брокерът задаващ въпроса!");
            }

            var questionToDelete = await _dbContext.AgentQuestions.Include(q => q.Answer)
                .FirstOrDefaultAsync(q => q.Id == questionId)
                ?? throw new ContentNotFoundException("Не е намерен въпросът, който искате да редактирате!");



            if ( !(questionToDelete.Answer == null && questionToDelete.AgentId == agentId) 
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Нямате права да изтривате този въпрос!");
            }


            //User is Admin or The Question doesnt have Answers and the Agent issuing is the creator
            _dbContext.AgentQuestions.Remove(questionToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}