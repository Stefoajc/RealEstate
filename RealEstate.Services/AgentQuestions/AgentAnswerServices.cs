using System.Threading.Tasks;
using RealEstate.Data;
using System;
using System.Data.Entity;
using RealEstate.Model;
using RealEstate.Model.AgentQuestions;
using RealEstate.Services.Exceptions;

namespace RealEstate.Services.AgentQuestions
{
    public class AgentAnswerServices
    {
        private readonly RealEstateDbContext _dbContext;
        private readonly ApplicationUserManager _userManager;

        public AgentAnswerServices(RealEstateDbContext dbContext, ApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        public async Task Create(string answer, int questionId, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът задаващ въпроса в системата!");
            }

            if (!(await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent)) 
                || await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат достъп !");
            }

            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentException("Въведете отговор!");
            }

            if (!await _dbContext.AgentQuestions.AnyAsync(q => q.Id == questionId))
            {
                throw new ContentNotFoundException("Не е намерен въпросът, на който отговаряте!");
            }

            var answerToCreate = new AgentAnswers
            {
                Answer = answer,
                AgentId = agentId,
                Id = questionId
            };

            _dbContext.AgentAnswers.Add(answerToCreate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Edit(int answerId, string answer, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът задаващ въпроса в системата!");
            }

            if (!(await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                  || await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Само админи и брокери могат да редактират отговори !");
            }

            if (string.IsNullOrEmpty(answer))
            {
                throw new ArgumentException("Въведете отговор!");
            }

            var answerToEdit = await _dbContext.AgentAnswers.FirstOrDefaultAsync(a => a.Id == answerId)
                               ?? throw new ContentNotFoundException("Не е намерен отговорът, който искате да редактирате!");

            if (answerToEdit.IsAcceptedAsCorrect &&
                !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Само Админи могат да редактират приет отговор !");
            }

            _dbContext.AgentAnswers.Attach(answerToEdit);
            answerToEdit.Answer = answer;
            answerToEdit.ModifiedOn = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        public async Task Accept(int answerId, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът задаващ въпроса в системата!");
            }

            if (!(await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                  || await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат достъп !");
            }

            var answerToEdit = await _dbContext.AgentAnswers.FirstOrDefaultAsync(a => a.Id == answerId)
                               ?? throw new ContentNotFoundException("Не е намерен отговорът, който искате да редактирате!");

            if (answerToEdit.AgentId == agentId)
            {
                throw new NotAuthorizedUserException("Нямате право да приемате собствен отговор за верен!");
            }

            _dbContext.AgentAnswers.Attach(answerToEdit);
            answerToEdit.IsAcceptedAsCorrect = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int answerId, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е установен брокерът задаващ въпроса!");
            }

            var answerToEdit = await _dbContext.AgentAnswers
                .FirstOrDefaultAsync(a => a.Id == answerId);

            if (answerToEdit == null) return;

            if (!await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)) && answerToEdit.AgentId != agentId)
            {
                throw new NotAuthorizedUserException("Нямате права за изтриване на този отговор.");
            }

            if (answerToEdit.AgentId == agentId && answerToEdit.IsAcceptedAsCorrect)
            {
                throw new NotAuthorizedUserException("Само Администратор може да изтрие приет отговор.");
            }

            _dbContext.AgentAnswers.Remove(answerToEdit);
            await _dbContext.SaveChangesAsync();            
        }
    }
}