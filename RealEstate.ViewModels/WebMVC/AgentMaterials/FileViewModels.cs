using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RealEstate.ViewModels.WebMVC.AgentMaterials
{
    public class AgentMaterialsIndexViewModel
    {
        public List<FileListViewModel> Files { get; set; }
        public List<FolderListViewModel> Folders { get; set; }
    }

    public class FileListViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public long Size { get; set; }

        public bool IsAllowedToEdit { get; set; }
        public bool IsAllowedToDelete { get; set; }
        public bool IsAllowedToCopy { get; set; }
        public bool IsAllowedToCut { get; set; }

        public int? FolderId { get; set; }
    }

    public class FileCreateViewModel : IValidatableObject
    {
        public int? FolderId { get; set; }

        [Required(ErrorMessage = "Въведете име на файлът")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не е въведен файл за качване!")]
        public HttpPostedFileBase File { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new Data.RealEstateDbContext();

            if (dbContext.Files.Any(f => f.Name == Name && f.FolderId == FolderId))
            {
                yield return new ValidationResult("Има файл с това име!", new []{ "Name" });
            }

        }
    }


    public class FileEditViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Не е избран файл, който да бъде редактиран!")]
        public int FileId { get; set; }

        [Required(ErrorMessage = "Въведете име на файлът")]
        public string FileName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new Data.RealEstateDbContext();
            var parentFolderId = dbContext.Files
                .Where(f => f.Id == FileId)
                .Select(f => f.FolderId)
                .FirstOrDefault();

            if (dbContext.Files.Any(f => f.Name == FileName && f.FolderId == parentFolderId))
            {
                yield return new ValidationResult("Има файл с това име!", new[] { "Name" });
            }

        }
    }

    public class FileDownloadViewModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileData { get; set; }
    }
}