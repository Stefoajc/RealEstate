using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using RealEstate.Data;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{
    public class PropertySearchListViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Views { get; set; }

        public decimal? PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public double? AreaInSquareMeters { get; set; }
        public bool IsRentSearch { get; set; } = false;
        public string AdditionalInformation { get; set; }

        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Areas { get; set; }

        public PersonSearcherListViewModel PersonSearcher { get; set; }

        public List<string> UnitTypes { get; set; } = new List<string>();
    }

    public class PropertySearchDetailViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Views { get; set; }

        public decimal? PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public double? AreaInSquareMeters { get; set; }
        public bool IsRentSearch { get; set; } = false;
        public string AdditionalInformation { get; set; }

        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Areas { get; set; }

        public PersonSearcherListViewModel PersonSearcher { get; set; }

        public List<string> UnitTypes { get; set; } = new List<string>();

        public TeamUserListViewModel TeamUser { get; set; }
    }

    public class PropertySearchCreateViewModel : IValidatableObject
    {
        public decimal? PriceFrom { get; set; }
        [Required(ErrorMessage = "Цената на търсенето е задължителна!")]
        public decimal PriceTo { get; set; }
        public double? AreaInSquareMeters { get; set; }
        public bool IsRentSearch { get; set; } = false;
        public string AdditionalInformation { get; set; }

        public int CityId { get; set; }
        public string Areas { get; set; }

        [Required(ErrorMessage = "Търсещият не е въведен!")]
        public PersonSearcherCreateViewModel PersonSearcher { get; set; }
        [EnsureOneItem(ErrorMessage = "Трябва да изберете поне един тип имот, които се търси!")]
        public ICollection<int> UnitTypeIds { get; set; } = new List<int>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new RealEstateDbContext();

            if (dbContext.PropertyTypes
                    .Count(pt => UnitTypeIds.Any(ut => pt.PropertyTypeId == ut)) != UnitTypeIds.Count)
            {
                yield return new ValidationResult("Някои от избраните типове имоти е грешен! Свържете се с администратор!", new []{ "UnitTypeIds" });
            }
            
            if (!dbContext.Cities.Any(c => c.CityId == CityId))
            {
                yield return new ValidationResult("Избраният град не съществува!", new[] { "CityId" });
            }
        }
    }

    public class PersonSearcherCreateViewModel
    {
        [Required(ErrorMessage = "Името е задължително!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Телефонният номер е задължителен!")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AdditionalInformation { get; set; }
    }

    public class PersonSearcherListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AdditionalInformation { get; set; }
    }


    public class PropertySearchEditViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Идентификационният номер е задължителен")]
        public int Id { get; set; }
        public decimal? PriceFrom { get; set; }
        [Required(ErrorMessage = "Цената на търсенето е задължителна!")]
        public decimal PriceTo { get; set; }
        public double? AreaInSquareMeters { get; set; }
        public bool IsRentSearch { get; set; } = false;
        public string AdditionalInformation { get; set; }

        public int CityId { get; set; }
        public string Areas { get; set; }

        [Required(ErrorMessage = "Търсещият не е въведен!")]
        public PersonSearcherCreateViewModel PersonSearcher { get; set; }
        [EnsureOneItem(ErrorMessage = "Трябва да изберете поне един тип имот, които се търси!")]
        public ICollection<DropDownPropertyTypesViewModel> UnitTypes { get; set; } = new List<DropDownPropertyTypesViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var dbContext = new RealEstateDbContext();

            var unitTypeIds = UnitTypes.Select(u => u.PropertyTypeId);

            if (dbContext.PropertyTypes
                    .Count(pt => unitTypeIds.Any(ut => pt.PropertyTypeId == ut)) != UnitTypes.Count)
            {
                yield return new ValidationResult("Някои от избраните типове имоти е грешен!", new[] { "UnitTypeIds" });
            }

            if (!dbContext.Cities.Any(c => c.CityId == CityId))
            {
                yield return new ValidationResult("Избраният град не съществува!", new[] { "CityId" });
            }
        }
    }

}