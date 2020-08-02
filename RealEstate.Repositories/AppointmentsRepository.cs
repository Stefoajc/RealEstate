using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class AppointmentsRepository : GenericRepository<RealEstateDbContext, Appointments>, IAppointmentsRepository
    {
        public AppointmentsRepository(RealEstateDbContext db) : base(db)
        {
        }
    }
}
