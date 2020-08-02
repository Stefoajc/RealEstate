using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using RealEstate.Extentions;
using RealEstate.Services;
using RealEstate.Services.AgentMaterials;
using RealEstate.ViewModels.WebMVC.AgentMaterials;

namespace RealEstate.WebAppMVC.Controllers.AgentMaterials
{
    [Authorize(Roles = "Agent, Administrator")]
    public class AgentMaterialsController : Controller
    {
        private readonly FileServices _filesManager;
        private readonly FolderServices _folderManager;
        private readonly AgentServices _agentServices;

        public AgentMaterialsController(FileServices filesManager, FolderServices folderManager, AgentServices agentServices)
        {
            _filesManager = filesManager;
            _folderManager = folderManager;
            _agentServices = agentServices;
        }

        // GET: AgentMaterials
        public async Task<ActionResult> Index()
        {
            //Filters
            ViewBag.Agents = await _agentServices.GetAgentsForDropDown();
            //-------

            var files = await _filesManager.List(User.Identity.GetUserId());
            var folders = await _folderManager.List(User.Identity.GetUserId());

            return View(new AgentMaterialsIndexViewModel
            {
                Files = files,
                Folders = folders
            });
        }

        #region Folders

        public async Task<ActionResult> CreateFolder(FolderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var createdFolder = await _folderManager.Create(model, User.Identity.GetUserId());

            return Json(createdFolder);
        }

        public async Task<ActionResult> EditFolder(FolderEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var editedFolder = await _folderManager.Edit(model, User.Identity.GetUserId());

            return Json(editedFolder);
        }

        public async Task<ActionResult> DeleteFolder(int id)
        {
            await _folderManager.Delete(id, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }
        #endregion

        #region Files 

        [HttpPost]
        public async Task<ActionResult> UploadFile(FileCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var createdFile = await _filesManager.Create(model, User.Identity.GetUserId());

            return Json(createdFile);
        }

        [HttpPost]
        public async Task<ActionResult> EditFile(FileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(ModelState.ToDictionary());
            }

            var editedFile = await _filesManager.Edit(model, User.Identity.GetUserId());

            return Json(editedFile);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteFile(int id)
        {
            await _filesManager.Delete(id, User.Identity.GetUserId());

            return Json("STATUS_OK");
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(int id, bool forceDownload = true)
        {
            var file = await _filesManager.DownloadFile(id);

            Response.AppendHeader("Content-Disposition",
                forceDownload ? $"attachment; filename=\"{file.FileName}\"" : $"inline; filename=\"{file.FileName}\"");

            return File(file.FileData, file.FileType);

        }

        #endregion
    }
}