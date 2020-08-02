using System;
using RealEstate.Data.Migrations;
using RealEstate.Model.AgentMaterials;
using RealEstate.Model.AgentQuestions;
using RealEstate.Model.Contact;
using RealEstate.Model.ContactDiary;
using RealEstate.Model.Forum;
using RealEstate.Model.Holidays;
using RealEstate.Model.MailList;
using RealEstate.Model.Notifications;
using RealEstate.Model.Payment;
using RealEstate.Model.PropertySearches;
using RealEstate.Model.Reports;
using RealEstate.Model.SmsSubSystem;

namespace RealEstate.Data
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Model;
    using System.Data.Entity;

    //public class RealEstateDbContext : DbContext
    //{
    //    // Your context has been configured to use a 'RealEstateDbContext' connection string from your application's 
    //    // configuration file (App.config or Web.config). By default, this connection string targets the 
    //    // 'RealEstate.Data.RealEstateDbContext' database on your LocalDb instance. 
    //    // 
    //    // If you wish to target a different database and/or database provider, modify the 'RealEstateDbContext' 
    //    // connection string in the application configuration file.
    //    public RealEstateDbContext()
    //        : base("name=RealEstateDbContext")
    //    {
    //    }


    //    // Add a DbSet for each entity type that you want to include in your model. For more information 
    //    // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

    //    // public virtual DbSet<MyEntity> MyEntities { get; set; }
    //}

    public class RealEstateDbContext : IdentityDbContext<ApplicationUser>
    {
        public RealEstateDbContext()
            : base("RealEstate", false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RealEstateDbContext, Configuration>());
        }

        public static RealEstateDbContext Create()
        {
            return new RealEstateDbContext();
        }

        public virtual IDbSet<PropertySearches> PropertySearches { get; set; }
        public virtual IDbSet<Properties> Properties { get; set; }
        public virtual IDbSet<RentalsInfo> RentalsInfo { get; set; }
        public virtual IDbSet<OffersForProperty> OffersForProperties { get; set; }
        public virtual IDbSet<PropertySeason> PropertyRentPeriods { get; set; }
        public virtual IDbSet<PropertyTypes> PropertyTypes { get; set; }
        public virtual IDbSet<Countries> Countries { get; set; }
        public virtual IDbSet<Cities> Cities { get; set; }
        public virtual IDbSet<Addresses> Addresses { get; set; }
        public virtual IDbSet<Reviews> Reviews { get; set; }
        public virtual IDbSet<CityReviews> CityReviews { get; set; }
        public virtual IDbSet<SightReviews> SightReviews { get; set; }
        public virtual IDbSet<PropertyReviews> PropertyReviews { get; set; }
        public virtual IDbSet<AgentReviews> AgentReviews { get; set; }
        public virtual IDbSet<OwnerReviews> OwnerReviews { get; set; }
        public virtual IDbSet<Sights> Sights { get; set; }
        public virtual IDbSet<Coordinates> Coordinates { get; set; }
        public virtual IDbSet<Extras> Extras { get; set; }
        public virtual IDbSet<RentalExtras> RentalExtras { get; set; }
        public virtual IDbSet<PropertyExtras> PropertyExtras { get; set; }
        public virtual IDbSet<OwnerRegisterCodes> OwnerRegisterCodes { get; set; }
        public virtual IDbSet<Images> Images { get; set; }
        public virtual IDbSet<SearchParamsTracking> SearchParamsTrackings { get; set; }
        public virtual IDbSet<RentalHirePeriodsTypes> RentalHirePeriodsTypes { get; set; }
        public virtual IDbSet<Appointments> Appointments { get; set; }
        public virtual IDbSet<NonRegisteredAppointmentUsers> NonRegisteredUsers { get; set; }

        #region Sms returned Statuses

        public virtual IDbSet<SmsRequests> SmsRequests { get; set; }
        public virtual IDbSet<SmsDeliveryStatuses> SmsDeliveryStatuses { get; set; }
        #endregion


        #region Payments

        public virtual IDbSet<Invoices> Invoices { get; set; }
        public virtual IDbSet<PayedItemsMeta> PayedItems { get; set; }

        #endregion

        #region User Tables

        public virtual IDbSet<SocialMediaAccounts> SocialMediaAccounts { get; set; }

        #endregion

        #region MailList
        public virtual IDbSet<EmailList> EmailList { get; set; }
        #endregion

        #region ContactMessages
        public virtual IDbSet<ContactMessages> ContactMessages { get; set; }
        #endregion

        #region Forum

        public IDbSet<ForumCategories> ForumCategories { get; set; }
        public IDbSet<Themes> Themes { get; set; }
        public IDbSet<Posts> Posts { get; set; }
        public IDbSet<Comments> Comments { get; set; }
        public IDbSet<ForumReviews> ForumReviews { get; set; }
        public IDbSet<PostImages> PostImages { get; set; }
        public IDbSet<Tags> Tags { get; set; }

        #endregion

        #region ContactsDiary

        public virtual IDbSet<ContactsDiary> ContactsDiary { get; set; }
        public virtual IDbSet<DealTypes> DealTypes { get; set; }
        public virtual IDbSet<NegotiationStates> NegotiationStates { get; set; }
        public virtual IDbSet<ContactedPersonTypes> ContactedPersonTypes { get; set; }

        #endregion

        #region AgentQuestions

        public virtual IDbSet<AgentQuestions> AgentQuestions { get; set; }
        public virtual IDbSet<AgentAnswers> AgentAnswers { get; set; }

        #endregion

        #region AgentMaterials

        public virtual IDbSet<Folders> Folders { get; set; }
        public virtual IDbSet<Files> Files { get; set; }

        #endregion


        #region Reports

        public virtual IDbSet<Partners> Partners { get; set; }
        public virtual IDbSet<PartnerTypes> PartnerTypes { get; set; }

        public virtual IDbSet<PromotionMedia> PromotionMediae { get; set; }

        public virtual IDbSet<WebPlatforms> WebPlatforms { get; set; }
        public virtual IDbSet<WebPlatformViews> WebPlatformViews { get; set; }
        public virtual IDbSet<Reports> Reports { get; set; }

        public virtual IDbSet<CustomPromotionMedia> CustomPromotionMediae { get; set; }
        public virtual IDbSet<CustomRecommendedActions> CustomRecommendedActions { get; set; }
        public virtual IDbSet<Offers> Offers { get; set; }

        #endregion


        #region Notifications

        public virtual IDbSet<Notifications> Notifications { get; set; }
        public virtual IDbSet<NotificationTypes> NotificationTypes { get; set; }

        #endregion

        #region Holidays

        public virtual IDbSet<ConstantHolidays> ConstantHolidays { get; set; }

        #endregion

        #region Trainings

        public virtual IDbSet<Trainings> Trainings { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reviews>()
                .HasKey(r => r.ReviewId)
                .ToTable("Reviews");
            modelBuilder.Entity<CityReviews>()
                .HasKey(r => r.ReviewId)
                .ToTable("CityReviews");
            modelBuilder.Entity<SightReviews>()
                .HasKey(r => r.ReviewId)
                .ToTable("SightReviews");
            modelBuilder.Entity<PropertyReviews>()
                .HasKey(r => r.ReviewId)
                .ToTable("PropertyReviews");
            modelBuilder.Entity<AgentReviews>()
                .HasKey(r => r.ReviewId)
                .ToTable("AgentReviews");
            modelBuilder.Entity<OwnerReviews>()
                .HasKey(r => r.ReviewId)
                .ToTable("OwnerReviews");

            modelBuilder.Entity<Appointments>()
                .HasKey(a => a.Id)
                .HasOptional(a => a.NonRegisteredUser)
                .WithRequired(n => n.Appointment);

            modelBuilder.Entity<Folders>()
                .HasOptional(item => item.Parent)
                .WithMany(item => item.ChildFolders)
                .HasForeignKey(item => item.ParentId);

            modelBuilder.Entity<PropertySearches>()
                .HasRequired(p => p.PersonSearcher)
                .WithRequiredPrincipal(p => p.PropertySearch)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<PropertySearches>()
                .HasMany(ps => ps.UnitTypes)
                .WithMany(ps => ps.PropertySearches);                


            modelBuilder.Entity<Notifications>()
                .HasOptional(n => n.UserCreator)
                .WithMany(u => u.NotificationsCreated)
                .HasForeignKey(u => u.UserCreatorId);

            modelBuilder.Entity<Notifications>()
                .HasOptional(n => n.UserToNotify)
                .WithMany(u => u.NotificationsToBeNotifiedFor)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<Notifications>()
                .HasMany(n => n.UsersSawNotifications)
                .WithMany(u => u.NotificationsSeen);

            modelBuilder.Entity<Notifications>()
                .HasMany(n => n.UsersClickedNotifications)
                .WithMany(n => n.NotificationsClicked);

            base.OnModelCreating(modelBuilder);
        }
    }

    
    
    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}