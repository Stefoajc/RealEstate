using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class UsersRepository:GenericRepository<RealEstateDbContext,ApplicationUser>,IUsersRepository
    {
        public UsersRepository(RealEstateDbContext db) : base(db)
        {
        }

        public List<IdentityRole> GetRoles()
        {
            return Context.Roles.ToList();
        }

        public string GetRoleId(string role)
        {
            return Context.Roles.Where(r => r.Name == role).Select(r => r.Id).FirstOrDefault();
        }
    }
}