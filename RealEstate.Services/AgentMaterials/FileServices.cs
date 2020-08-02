using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.AgentMaterials;
using RealEstate.Model.Notifications;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.AgentMaterials;

namespace RealEstate.Services.AgentMaterials
{
    public class FileServices
    {
        private readonly RealEstateDbContext _dbContext;
        private readonly ApplicationUserManager _userManager;
        private readonly INotificationCreator _notificationCreator;
        private readonly string _baseMaterialsPath = ConfigurationManager.AppSettings["MaterialsPath"];
        private readonly int _timeWindowForChangesInMinutes = int.Parse(ConfigurationManager.AppSettings["TimeWindowForChangesInMinutes"]);

        public FileServices(RealEstateDbContext dbContext, ApplicationUserManager userManager, INotificationCreator notificationCreator)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _notificationCreator = notificationCreator;
        }


        public async Task<List<FileListViewModel>> List(string issuerAgentId)
        {
            var files = await _dbContext.Files
                .Include(f => f.Agent)
                .Select(f => new FileListViewModel
                {
                    Id = f.Id,
                    AgentId = f.AgentId,
                    AgentName = f.Agent.FirstName + " " + f.Agent.LastName,
                    CreatedOn = f.CreatedOn,
                    Name = f.Name,
                    Path = f.RelativePath,
                    Size = f.SizeInBytes,
                    Type = f.Type,
                    FolderId = f.FolderId
                })
                .ToListAsync();

            var isAdmin = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Administrator));
            var isMaintenance = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Maintenance));

            foreach (var file in files)
            {
                var isOwner = file.AgentId == issuerAgentId;
                var isInTimeForChange = file.CreatedOn > DateTime.Now.AddMinutes(-_timeWindowForChangesInMinutes);

                file.IsAllowedToEdit = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
                file.IsAllowedToDelete = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
                file.IsAllowedToCut = isAdmin || isMaintenance;
                file.IsAllowedToCopy = isAdmin || isMaintenance;
            }

            return files;
        }


        public async Task<FileListViewModel> Get(int id, string issuerAgentId)
        {
            var file = await _dbContext.Files
                .Include(f => f.Agent)
                .Where(f => f.Id == id)
                .Select(f => new FileListViewModel
                {
                    Id = f.Id,
                    AgentId = f.AgentId,
                    AgentName = f.Agent.FirstName + " " + f.Agent.LastName,
                    CreatedOn = f.CreatedOn,
                    Name = f.Name,
                    Path = f.RelativePath,
                    Size = f.SizeInBytes,
                    Type = f.Type,
                    FolderId = f.FolderId
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен търсеният файл! Опитайте отново!");

            var isAdmin = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Administrator));
            var isMaintenance = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Maintenance));            
            var isOwner = file.AgentId == issuerAgentId;
            var isInTimeForChange = file.CreatedOn > DateTime.Now.AddMinutes(-_timeWindowForChangesInMinutes);

            file.IsAllowedToEdit = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
            file.IsAllowedToDelete = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
            file.IsAllowedToCut = isAdmin || isMaintenance;
            file.IsAllowedToCopy = isAdmin || isMaintenance;

            return file;
        }

        public async Task<bool> Exist(string fileName = null, int? fileId = null, int? folderId = null)
        {
            var fileExistanceQuery = _dbContext.Files.AsQueryable();

            if (!string.IsNullOrEmpty(fileName))
            {
                fileExistanceQuery = fileExistanceQuery.Where(f => f.Name == fileName);
            }

            if (fileId != null)
            {
                fileExistanceQuery = fileExistanceQuery.Where(f => f.Id == fileId);
            }

            if (folderId != null)
            {
                fileExistanceQuery = fileExistanceQuery.Where(f => f.FolderId == folderId);
            }

            return await fileExistanceQuery.AnyAsync();
        }

        public async Task<FileListViewModel> Create(FileCreateViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new NotAuthorizedUserException("Само потребители могат да качват файлове!");
            }

            if (!await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedUserException("Нямате право да извършвате това действие!");
            }

            if (await Exist(fileName: model.Name, folderId: model.FolderId))
            {
                throw new ArgumentException("Съществува файл с това име в тази папка!");
            }


            string filePath = Path.GetFileNameWithoutExtension(model.Name) + Path.GetExtension(model.File.FileName);

            if (model.FolderId != null)
            {
                filePath = Path.Combine(await _dbContext.Folders
                               .Where(f => f.Id == model.FolderId).Select(f => f.RelativePath)
                               .FirstOrDefaultAsync() ?? "", filePath);
            }
            else
            {
                filePath = Path.Combine(_baseMaterialsPath, filePath);
            }

            var physicalFilePath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), filePath.Replace('/', '\\').TrimStart('\\'));

            try
            {
                model.File.SaveAs(physicalFilePath);
            }
            catch (Exception)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(physicalFilePath) ?? throw new ArgumentException("Пътят е грешен"));
                model.File.SaveAs(physicalFilePath);
            }

            var file = new Files
            {
                Name = Path.GetFileName(filePath),
                FolderId = model.FolderId,
                AgentId = agentId,
                SizeInBytes = model.File.ContentLength,
                Type = MimeMapping.GetMimeMapping(model.File.FileName),
                RelativePath = filePath
            };
            try
            {
                _dbContext.Files.Add(file);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                File.Delete(physicalFilePath);
                throw;
            }


            var createdFile = await Get(file.Id, agentId);

            #region Notifications

            var notificationToCreate = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Material,
                NotificationPicture = "",
                NotificationLink = "/agentmaterials/index?fileId=" + createdFile.Id + (createdFile.FolderId != null ? "&folderId=" + createdFile.FolderId : "" ),
                NotificationText = createdFile.AgentName + " добави материал: " + createdFile.Name
            };

            await _notificationCreator.CreateGlobalNotification(notificationToCreate, agentId);

            #endregion

            return createdFile;
        }

        public async Task<FileListViewModel> Edit(FileEditViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new NotAuthorizedUserException("Само потребители могат да редактират файлове!");
            }

            if (!await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedUserException("Нямате право да извършвате това действие!");
            }

            if (!await Exist(fileId: model.FileId))
            {
                throw new ArgumentException("Файлът, който искате да редактирате не съществува в системата!");
            }

            var fileToEdit = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == model.FileId)
                ?? throw new ContentNotFoundException("Не е намерен файлът, който искате да редактирате!");

            if (fileToEdit.AgentId != agentId && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Трябва да сте собственик на файла или админ за да го редактирате!");
            }

            if (await Exist(fileName: model.FileName, folderId: fileToEdit.FolderId))
            {
                throw new ArgumentException("Съществува файл с това име в папката!");
            }

            //Extentions are not editable
            //Only the name without the extention
            var newFileName = Path.GetFileNameWithoutExtension(model.FileName) + Path.GetExtension(fileToEdit.Name);
            var currentDir = Path.GetDirectoryName(fileToEdit.RelativePath);

            var newRelativePath = Path.Combine(currentDir ?? _baseMaterialsPath, newFileName);
            var oldRelativePath = fileToEdit.RelativePath;
            var systemRoot = HttpRuntime.AppDomainAppPath;
            var newPhysicalPath = Path.Combine(systemRoot.TrimEnd('\\', '/'), newRelativePath.TrimStart('\\', '/'));
            var oldPhysicalPath = Path.Combine(systemRoot.TrimEnd('\\', '/'), oldRelativePath.TrimStart('\\', '/'));

            try
            {
                //Change the name in the file system
                File.Move(oldPhysicalPath, newPhysicalPath);
            }
            catch (Exception)
            {
                throw new Exception("Проблем в системата при преименуване на файл!");
            }

            try
            {
                // change name in DB
                _dbContext.Files.Attach(fileToEdit);
                fileToEdit.Name = newFileName;
                fileToEdit.RelativePath = newRelativePath;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Change the name in the file system
                File.Move(newPhysicalPath, oldPhysicalPath);

                throw new Exception("Проблем при в системата, свържете се с Администратор!");
            }


            return await Get(model.FileId, agentId);
        }

        public async Task Delete(int fileId, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new NotAuthorizedUserException("Само потребители могат да Изтриват файлове!");
            }

            if (!await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedUserException("Нямате право да извършвате това действие!");
            }

            if (!await Exist(fileId: fileId))
            {
                throw new ArgumentException("Не съществува файл с това име в системата!");
            }

            var fileToDelete = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == fileId)
                             ?? throw new ContentNotFoundException("Не е намерен файлът, който искате да Изтриете!");

            if (fileToDelete.AgentId != agentId && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Трябва да сте собственик на файла или админ за да го изтриете!");
            }

            var systemRoot = HttpRuntime.AppDomainAppPath;
            var filePhysicalPath = Path.Combine(systemRoot.TrimEnd('\\', '/'),
                fileToDelete.RelativePath.TrimStart('\\', '/'));

            File.Delete(filePhysicalPath);

            _dbContext.Files.Remove(fileToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<FileDownloadViewModel> DownloadFile(int id)
        {
            var fileToDownload = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == id) ?? throw new ContentNotFoundException("Не е намерен файлът, който искате да свалите!");

            var physicalPath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), fileToDownload.RelativePath.Replace('/', '\\').TrimStart('\\'));

            return new FileDownloadViewModel
            {
                FileName = fileToDownload.Name,
                FileType = fileToDownload.Type,
                FileData = File.ReadAllBytes(physicalPath)
            };
        }

        public async Task<List<FileListViewModel>> CopyFiles(List<int> filesToCopy, int? destinationFolderId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FileListViewModel>> CutFiles(List<int> filesToCut, int? destinationFolderId)
        {
            throw new NotImplementedException();
        }
    }
}