using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.Trainings;

namespace RealEstate.Services.Trainings
{
    public class TrainingServices
    {
        private readonly RealEstateDbContext _dbContext;
        private readonly INotificationCreator _notificationCreator;
        private readonly ApplicationUserManager _userManager;

        public TrainingServices(RealEstateDbContext dbContext, INotificationCreator notificationCreator, ApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _notificationCreator = notificationCreator;
            _userManager = userManager;
        }

        public async Task<List<TrainingListViewModel>> ListAsync()
        {
            var trainings = await _dbContext.Trainings
                .OrderByDescending(t => t.TrainingDate)
                .Select(t => new TrainingListViewModel
                {
                    Id = t.Id,
                    CreatedOn = t.CreatedOn,
                    TrainingTheme = t.TrainingTheme,
                    TrainingDate = t.TrainingDate,
                    AdditionalDescription = t.AdditionalDescription,
                    TrainingMaterialsFolderLink = t.TrainingMaterialsFolderLink
                })
                .ToListAsync();

            return trainings;
        }

        public async Task<TrainingListViewModel> Get(int id)
        {
            var training = await _dbContext.Trainings
                .Where(t => t.Id == id)
                .Select(t => new TrainingListViewModel
                {
                    Id = t.Id,
                    CreatedOn = t.CreatedOn,
                    TrainingTheme = t.TrainingTheme,
                    TrainingDate = t.TrainingDate,
                    AdditionalDescription = t.AdditionalDescription,
                    TrainingMaterialsFolderLink = t.TrainingMaterialsFolderLink
                })
                .FirstOrDefaultAsync();

            return training;
        }

        public async Task<TrainingListViewModel> CreateAsync(TrainingCreateViewModel model, string adminId)
        {
            if (string.IsNullOrEmpty(adminId))
            {
                throw new ArgumentException("Не е намерен администартора, който създава Обучение!");
            }

            if (!await _userManager.IsInRoleAsync(adminId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи имат право да създават обучения !");
            }

            if (model.TrainingDate == null)
            {
                throw new ArgumentException("Не е въведена дата на обучението!");
            }

            var training = new Model.Trainings
            {
                TrainingDate = (DateTime)model.TrainingDate,
                TrainingTheme = model.TrainingTheme,
                AdditionalDescription = model.AdditionalDescription,
                TrainingMaterialsFolderLink = model.TrainingMaterialsFolderLink
            };

            _dbContext.Trainings.Add(training);
            await _dbContext.SaveChangesAsync();

            #region notification

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int) NotificationType.Learning,
                NotificationLink = "/trainings/index?trainingId=" + training.Id,
                NotificationText = "Ще се проведе обучение на тема: " + training.TrainingTheme + " в " +
                                   training.TrainingDate.ToString("dddd, dd.MM.yyyyг. hh:mmч.")
            };

            await _notificationCreator.CreateGlobalNotification(notificationToCreate, adminId);

            #endregion

            return await Get(training.Id);
        }

        public async Task<TrainingListViewModel> EditAsync(TrainingEditViewModel model, string adminId)
        {
            if (string.IsNullOrEmpty(adminId))
            {
                throw new ArgumentException("Не е намерен администартора, който редактира Обучение!");
            }

            if (!await _userManager.IsInRoleAsync(adminId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи имат право да редактират обучения !");
            }

            var trainingToEdit = await _dbContext.Trainings
                .FirstOrDefaultAsync(t => t.Id == model.Id) 
                ?? throw new ContentNotFoundException("Не е намерено обучението, което искате да редактирате!");

            if (model.TrainingDate == null)
            {
                throw new ArgumentException("Не е въведена дата на обучението!");
            }

            var oldThemeName = trainingToEdit.TrainingTheme;
            var oldThemeDate = trainingToEdit.TrainingDate;

            _dbContext.Trainings.Attach(trainingToEdit);
            trainingToEdit.TrainingDate = (DateTime)model.TrainingDate;
            trainingToEdit.AdditionalDescription = model.AdditionalDescription;
            trainingToEdit.TrainingTheme = model.TrainingTheme;
            trainingToEdit.TrainingMaterialsFolderLink = model.TrainingMaterialsFolderLink;
            await _dbContext.SaveChangesAsync();

            #region notification

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Learning,
                NotificationLink = "/trainings/index?trainingId=" + model.Id,
                NotificationText = "Обучението на тема: " + oldThemeName + " от " +
                                   oldThemeDate.ToString("dddd, dd.MM.yyyyг. hh:mmч.")
                                   + "е преместено в " + ((DateTime)(model.TrainingDate)).ToString("dddd, dd.MM.yyyyг. hh:mmч.")
                                   + " с тема: " + model.TrainingTheme
            };

            await _notificationCreator.CreateGlobalNotification(notificationToCreate, adminId);

            #endregion

            return await Get(trainingToEdit.Id);
        }

        public async Task DeleteAsync(int id, string adminId)
        {
            if (string.IsNullOrEmpty(adminId))
            {
                throw new ArgumentException("Не е намерен администартора, който изтриват Обучение!");
            }

            if (!await _userManager.IsInRoleAsync(adminId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи имат право да изтриват обучения !");
            }

            var trainingToDelete = await _dbContext.Trainings
                                     .FirstOrDefaultAsync(t => t.Id == id)
                                 ?? throw new ContentNotFoundException("Не е намерено обучението, което искате да изтриете!");

            _dbContext.Trainings.Remove(trainingToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}
