using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Model;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Repositories.Interfaces
{
    public interface IPropertiesRepository:IGenericRepository<Properties>
    {
        Task<List<PropertyTypes>> ListPropertyTypesAsync();
        List<PropertyTypes> ListPropertyTypes();
        Task<List<PropertySeason>> ListPropertySeasons();
        IQueryable<PropertyInfoDTO> GetAll(bool excludeRentals);
    }
}