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
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC.AgentMaterials;

namespace RealEstate.Services.AgentMaterials
{
    public class FolderServices
    {
        private readonly RealEstateDbContext _dbContext;
        private readonly ApplicationUserManager _userManager;
        private readonly string _baseMaterialsPath = ConfigurationManager.AppSettings["MaterialsPath"];
        private readonly int _timeWindowForChangesInMinutes = int.Parse(ConfigurationManager.AppSettings["TimeWindowForChangesInMinutes"]);

        public FolderServices(RealEstateDbContext dbContext, ApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        public async Task<List<FolderListViewModel>> List(string issuerAgentId)
        {
            var folders = await _dbContext.Folders
                .Include(f => f.Agent)
                .Select(f => new FolderListViewModel
                {
                    Id = f.Id,
                    AgentId = f.AgentId,
                    AgentName = f.Agent.FirstName + " " + f.Agent.LastName,
                    CreatedOn = f.CreatedOn,
                    Name = f.Name,
                    Path = f.RelativePath,
                    ParentId = f.ParentId
                })
                .ToListAsync();

            var isAdmin = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Administrator));
            var isMaintenance = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Maintenance));

            foreach (var folder in folders)
            {
                var isOwner = folder.AgentId == issuerAgentId;
                var isInTimeForChange = folder.CreatedOn > DateTime.Now.AddMinutes(-_timeWindowForChangesInMinutes);

                folder.IsAllowedToEdit = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
                folder.IsAllowedToDelete = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
            }

            return folders;
        }

        private async Task<List<Folders>> GetAll(int? folderId = null, string folderRelativePath = null)
        {
            var foldersQuery = _dbContext.Folders.AsQueryable();

            foldersQuery = folderId == null ? foldersQuery : foldersQuery.Where(f => f.ParentId == folderId);
            foldersQuery = string.IsNullOrEmpty(folderRelativePath)
                ? foldersQuery
                : foldersQuery.Where(f => f.RelativePath.StartsWith(folderRelativePath));

            return await foldersQuery.ToListAsync();
        }

        public async Task<FolderListViewModel> Get(int id, string issuerAgentId)
        {
            var folder = await _dbContext.Folders
                .Include(f => f.Agent)
                .Where(f => f.Id == id)
                .Select(f => new FolderListViewModel
                {
                    Id = f.Id,
                    AgentId = f.AgentId,
                    AgentName = f.Agent.FirstName + " " + f.Agent.LastName,
                    CreatedOn = f.CreatedOn,
                    Name = f.Name,
                    Path = f.RelativePath,
                    ParentId = f.ParentId
                })
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена папката!");

            var isAdmin = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Administrator));
            var isMaintenance = await _userManager.IsInRoleAsync(issuerAgentId, Enum.GetName(typeof(Role), Role.Maintenance));
            var isOwner = folder.AgentId == issuerAgentId;
            var isInTimeForChange = folder.CreatedOn > DateTime.Now.AddMinutes(-_timeWindowForChangesInMinutes);

            folder.IsAllowedToEdit = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;
            folder.IsAllowedToDelete = (isOwner && isInTimeForChange) || isAdmin || isMaintenance;

            return folder;
        }

        public async Task<bool> Exist(string folderName = null, int? folderId = null, int? parentId = null)
        {
            var folderExistanceQuery = _dbContext.Folders.AsQueryable();

            if (!string.IsNullOrEmpty(folderName))
            {
                folderExistanceQuery = folderExistanceQuery.Where(f => f.Name == folderName);
            }

            if (folderId != null)
            {
                folderExistanceQuery = folderExistanceQuery.Where(f => f.Id == folderId);
            }

            if (parentId != null)
            {
                folderExistanceQuery = folderExistanceQuery.Where(f => f.ParentId == parentId);
            }

            return await folderExistanceQuery.AnyAsync();
        }

        public async Task<FolderListViewModel> Create(FolderCreateViewModel model, string agentId)
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

            if (await Exist(folderName: model.Name, parentId: model.ParentId))
            {
                throw new ArgumentException("Съществува файл с това име в системата!");
            }

            var folderPath = model.Name;

            if (model.ParentId != null)
            {
                folderPath = Path.Combine(await _dbContext.Folders
                                            .Where(f => f.Id == model.ParentId).Select(f => f.RelativePath)
                                            .FirstOrDefaultAsync() ?? "", folderPath);
            }
            else
            {
                folderPath = Path.Combine(_baseMaterialsPath, folderPath);
            }

            var physicalFilePath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), folderPath.Replace('/', '\\').TrimStart('\\'));

            Directory.CreateDirectory(physicalFilePath);


            var folder = new Folders
            {
                Name = Path.GetFileName(folderPath),
                ParentId = model.ParentId,
                AgentId = agentId,
                RelativePath = folderPath
            };

            try
            {
                _dbContext.Folders.Add(folder);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                File.Delete(physicalFilePath);
                throw;
            }


            return await Get(folder.Id, agentId);
        }

        public async Task<FolderListViewModel> Edit(FolderEditViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new NotAuthorizedUserException("Само потребители могат да редактират папки!");
            }

            if (!await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedUserException("Нямате право да извършвате това действие!");
            }

            if (!await Exist(folderId: model.Id))
            {
                throw new ArgumentException("Не съществува папка с това име в системата!");
            }


            var folderToEdit = await _dbContext.Folders.FirstOrDefaultAsync(f => f.Id == model.Id)
                ?? throw new ContentNotFoundException("Не е намерена папката, която искате да редактирате!");

            if (folderToEdit.AgentId != agentId && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Трябва да сте собственик на папката или админ за да я редактирате!");
            }

            if (await Exist(folderName: model.Name, parentId: folderToEdit.ParentId))
            {
                throw new ArgumentException("Съществува папка с това име в папка!");
            }

            if (folderToEdit.Name != model.Name)
            {
                var newFolderName = model.Name;
                var parentDirOfCurrentDir = Path.GetDirectoryName(folderToEdit.RelativePath);

                var newRelativePath = Path.Combine(parentDirOfCurrentDir ?? _baseMaterialsPath, newFolderName);
                var oldRelativePath = folderToEdit.RelativePath;

                await MoveAllChildren(oldRelativePath, newRelativePath);

                // change name in DB
                _dbContext.Folders.Attach(folderToEdit);
                folderToEdit.Name = newFolderName;
                folderToEdit.RelativePath = newRelativePath;                
                await _dbContext.SaveChangesAsync();



                var systemRoot = HttpRuntime.AppDomainAppPath;

                var newPhysicalPath = Path.Combine(systemRoot.TrimEnd('\\', '/'), newRelativePath.TrimStart('\\', '/'));
                var oldPhysicalPath = Path.Combine(systemRoot.TrimEnd('\\', '/'), oldRelativePath.TrimStart('\\', '/'));

                //Change the name in the file system
                Directory.Move(oldPhysicalPath, newPhysicalPath);
            }

            return await Get(model.Id, agentId);
        }

        private async Task MoveAllChildren(string oldRelativePath, string newRelativePath)
        {
            var allFilesInFolder = await _dbContext.Files
                .Where(f => f.RelativePath.StartsWith(oldRelativePath + "\\")) // append slashes to prevent name collisions like (f1 and f1234)
                .ToListAsync();

            foreach (var fileToChangePathTo in allFilesInFolder)
            {
                _dbContext.Files.Attach(fileToChangePathTo);
                fileToChangePathTo.RelativePath = fileToChangePathTo.RelativePath.Replace(oldRelativePath, newRelativePath);
            }

            var allFoldersInFolder = await _dbContext.Folders
                .Where(f => f.RelativePath.StartsWith(oldRelativePath + "\\"))
                .ToListAsync();

            foreach (var folderToChangePathTo in allFoldersInFolder)
            {
                _dbContext.Folders.Attach(folderToChangePathTo);
                folderToChangePathTo.RelativePath = folderToChangePathTo.RelativePath.Replace(oldRelativePath, newRelativePath);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int folderId, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new NotAuthorizedUserException("Само потребители могат да Изтриват папките!");
            }

            if (!await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Maintenance)))
            {
                throw new NotAuthorizedUserException("Нямате право да извършвате това действие!");
            }

            if (!await Exist(folderId: folderId))
            {
                throw new ArgumentException("Не съществува папка с това име в системата!");
            }

            var folderToDelete = await _dbContext.Folders
                .Include(f => f.ChildFiles)
                .Include(f => f.ChildFolders)
                .FirstOrDefaultAsync(f => f.Id == folderId)
                               ?? throw new ContentNotFoundException("Не е намерен файлът, който искате да редактирате!");

            if (folderToDelete.AgentId != agentId && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Трябва да сте собственик на файла или админ за да го изтриете!");
            }

            if (folderToDelete.ChildFolders.Any() || folderToDelete.ChildFiles.Any())
            {
                throw new NotAuthorizedException("Не може да се изтриват папки съдържащи информация !");
            }

            var systemRoot = HttpRuntime.AppDomainAppPath;
            var folderPhysicalPath = Path.Combine(systemRoot.TrimEnd('\\', '/'), folderToDelete.RelativePath.TrimStart('\\', '/'));

            Directory.Delete(folderPhysicalPath);

            _dbContext.Folders.Remove(folderToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}