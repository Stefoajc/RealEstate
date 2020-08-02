using System.Collections.Generic;
using RealEstate.Model;

namespace RealEstate.Repositories.Interfaces
{
    public interface IReservationsRepository:IGenericRepository<Reservations>
    {
        List<Reservations> GetOwnerReservations(string ownerId);
    }
}