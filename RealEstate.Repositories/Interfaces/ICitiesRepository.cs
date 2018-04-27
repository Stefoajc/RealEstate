using System.Linq;
using RealEstate.Model;

namespace RealEstate.Repositories.Interfaces
{
    public interface ICitiesRepository:IGenericRepository<Cities>
    {
        IQueryable<Countries> GetAllCountries();
    }
}