using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using RealEstate.Model;

namespace RealEstate.Repositories.Interfaces
{
    public interface IUsersRepository:IGenericRepository<ApplicationUser>
    {
        List<IdentityRole> GetRoles();
        string GetRoleId(string role);
    }
}