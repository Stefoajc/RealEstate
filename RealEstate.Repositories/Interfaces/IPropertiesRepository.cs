using System.Collections.Generic;
using RealEstate.Model;

namespace RealEstate.Repositories.Interfaces
{
    public interface IPropertiesRepository:IGenericRepository<Properties>
    {
        List<PropertyTypes> ListPropertyTypes();
        List<PropertySeason> ListPropertySeasons();
    }
}