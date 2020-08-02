using System.Linq;
using RealEstate.Model;

namespace RealEstate.Repositories.Interfaces
{
    public interface IRentalsRepository:IGenericRepository<RentalsInfo>
    {
        IQueryable<PropertyTypes> GetRentalTypes();
        IQueryable<RentalHirePeriodsTypes> GetRentalPeriodTypes();
    }
}