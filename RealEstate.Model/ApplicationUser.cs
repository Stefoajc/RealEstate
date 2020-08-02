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
        }

        public ApplicationUser(string userName) : base(userName)
        {
            UserName = userName;
            CreatedOn = DateTime.Now;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string AdditionalDescription { get; set; }

        public DateTime? LastActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ISet<Notifications.Notifications> Notifications { get; set; } = new HashSet<Notifications.Notifications>();
        public virtual ISet<UserImages> Images { get; set; } = new HashSet<UserImages>();
        public virtual ISet<SearchParamsTracking> SearchParamsTrackings { get; set; } = new HashSet<SearchParamsTracking>();
        public virtual ISet<SocialMediaAccounts> SocialMediaAccounts { get; set; } = new HashSet<SocialMediaAccounts>();

        public virtual ISet<Notifications.Notifications> NotificationsToBeNotifiedFor { get; set; } = new HashSet<Notifications.Notifications>();
        public virtual ISet<Notifications.Notifications> NotificationsCreated { get; set; } = new HashSet<Notifications.Notifications>();

        public virtual ISet<Notifications.Notifications> NotificationsSeen { get; set; } = new HashSet<Notifications.Notifications>();
        public virtual ISet<Notifications.Notifications> NotificationsClicked { get; set; } = new HashSet<Notifications.Notifications>();

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Type", this.GetType().BaseType?.Name));
            userIdentity.AddClaim(new Claim("ProfileImage", this.Images?.Select(i => i.ImagePath).FirstOrDefault() ?? "/Resources/no-image-person.png"));
            userIdentity.AddClaim(new Claim("PhoneNumber", this.PhoneNumber ?? "NULL"));
            userIdentity.AddClaim(new Claim("IsPhoneNumberConfirmed", this.PhoneNumberConfirmed.ToString()));

            return userIdentity;
        }
    }

    public class AgentUsers : ApplicationUser
    {
        public int PropertiesSold { get; set; }
        //Properties which this agent is selling
        public virtual ISet<Properties> Properties { get; set; } = new HashSet<Properties>();
        //Reviews from users
        public virtual ISet<AgentReviews> Reviews { get; set; } = new HashSet<AgentReviews>();
        /// <summary>
        /// Appointments made to the agent
        /// </summary>
        public virtual ISet<Appointments> Appointments { get; set; } = new HashSet<Appointments>();
    }


    public class OwnerUsers : ApplicationUser
    {
        public decimal MoneyEarned { get; set; }

        public DateTime? BirthDate { get; set; }

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
        public virtual ISet<Likes> Likes { get; set; } = new HashSet<Likes>();
        public virtual ISet<Reservations> Reservations { get; set; } = new HashSet<Reservations>();
        /// <summary>
        /// Given Reviews
        /// </summary>
        public virtual ISet<Reviews> Reviews { get; set; } = new HashSet<Reviews>();

        /// <summary>
        /// Appointments made by the user
        /// </summary>
        public virtual ISet<Appointments> Appointments { get; set; } = new HashSet<Appointments>();
    }


    public enum Role
    {
        Client,
        Owner,
        Administrator,
        Maintenance,
        Marketer,
        Agent,
        TeamMember
    }
}
