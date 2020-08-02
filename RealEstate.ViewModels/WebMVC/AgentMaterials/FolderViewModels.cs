using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RealEstate.ViewModels.WebMVC.AgentMaterials
{

    public class FolderListViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string Path { get; set; }

        public int? ParentId { get; set; }

        public bool? IsAllowedToEdit { get; set; }
        public bool? IsAllowedToDelete { get; set; }
    }

    public class FolderCreateViewModel : IValidatableObject
    {
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "Името на папката е задължително")]
        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new Data.RealEstateDbContext();

            if (dbContext.Folders.Any(f => f.Name == Name && f.ParentId == ParentId))
            {
                yield return new ValidationResult("Има папка с това име!", new[] { "Name" });
            }
        }
    }

    public class FolderEditViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Не е селектирана папката която се редактира!")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Името на папката е задължително")]
        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new Data.RealEstateDbContext();

            var parentFolderId = dbContext.Folders
                .Where(f => f.Id == Id)
                .Select(f => f.ParentId)
                .FirstOrDefault();

            if (dbContext.Folders.Any(f => f.Name == Name && f.ParentId == parentFolderId))
            {
                yield return new ValidationResult("Има папка с това име!", new[] { "Name" });
            }
        }
    }
}