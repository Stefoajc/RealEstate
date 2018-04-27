using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RealEstate.Model
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
            CreatedOn = DateTime.Now;
            LastActive = DateTime.Now;
        }

        public ApplicationUser(string userName):base(userName)
        {
            UserName = userName;
            CreatedOn = DateTime.Now;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Description { get; set; }

        public DateTime LastActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ISet<Notifications> Notifications { get; set; } = new HashSet<Notifications>();
        public virtual ISet<UserImages> Images { get; set; } = new HashSet<UserImages>();

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Type",this.GetType().BaseType?.Name));
            return userIdentity;
        }



    }

    public class AgentUsers : ApplicationUser
    {
        public AgentUsers():base()
        {                
        }

        public int PropertiesSold { get; set; }
        //Properties which this agent is selling
        public virtual ISet<Properties> Properties { get; set; } = new HashSet<Properties>();
        //Reviews from users
        public virtual ISet<AgentReviews> Reviews { get; set; } = new HashSet<AgentReviews>();
    }


    public class OwnerUsers : ApplicationUser
    {
        public decimal MoneyEarned { get; set; }

        public virtual ISet<Properties> Properties { get; set; } = new HashSet<Properties>();
        /// <summary>
        /// Received Reviews
        /// </summary>
        public virtual ISet<OwnerReviews> Reviews { get; set; } = new HashSet<OwnerReviews>();
    }

    public class ClientUsers : ApplicationUser
    {
        public decimal MoneySpent { get; set; }
        public string AdditionalInformation { get; set; }

        /// <summary>
        /// All things the user liked
        /// </summary>
        public virtual ISet<Likes> Likes { get; set; }
        public virtual ISet<Reservations> Reservations { get; set; }
        /// <summary>
        /// Given Reviews
        /// </summary>
        public virtual ISet<Reviews> Reviews { get; set; }

    }
}
