using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class ReservationsRepository:GenericRepository<RealEstateDbContext,Reservations>,IReservationsRepository
    {
        public ReservationsRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}
