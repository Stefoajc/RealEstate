using System;
using RealEstate.Data.Migrations;

namespace RealEstate.Data
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using RealEstate.Model;
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


        public IDbSet<Properties> Properties { get; set; }
        public IDbSet<RentalsInfo> RentalsInfo { get; set; }
        public IDbSet<SellingInfo> SellingInfo { get; set; }
        public IDbSet<PropertySeason> PropertyRentPeriods { get; set; }
        public IDbSet<PropertyTypes> PropertyTypes { get; set; }
        public IDbSet<Countries> Countries { get; set; }
        public IDbSet<Cities> Cities { get; set; }
        public IDbSet<Addresses> Addresses { get; set; }
        public IDbSet<Reviews> Reviews { get; set; }
        public IDbSet<CityReviews> CityReviews { get; set; }
        public IDbSet<SightReviews> SightReviews { get; set; }
        public IDbSet<PropertyReviews> PropertyReviews { get; set; }
        public IDbSet<Sights> Sights { get; set; }
        public IDbSet<Coordinates> Coordinates { get; set; }
        public IDbSet<Extras> Extras { get; set; }
        public IDbSet<RentalExtras> RentalExtras { get; set; }
        public IDbSet<PropertyExtras> PropertyExtras { get; set; }
        public IDbSet<UnitTypes> UnitTypes { get; set; }
        public IDbSet<OwnerRegisterCodes> OwnerRegisterCodes { get; set; }


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

            modelBuilder.Entity<Extras>()
                .HasKey(r => r.ExtraId)
                .ToTable("Extras");
            modelBuilder.Entity<RentalExtras>()
                .HasKey(r => r.ExtraId)
                .ToTable("RentalExtras");
            modelBuilder.Entity<PropertyExtras>()
                .HasKey(r => r.ExtraId)
                .ToTable("PropertyExtras");


            modelBuilder.Entity<Reservations>()
                .HasOptional(r => r.Property)
                .WithMany(r => r.Reservations);

            modelBuilder.Entity<Reservations>()
                .HasOptional(r => r.Rental)
                .WithMany(r => r.Reservations);

            modelBuilder.Entity<KeyValuePairs>()
                .HasOptional(kp => kp.Property)
                .WithMany(p => p.PropertyAttributes);

            modelBuilder.Entity<KeyValuePairs>()
                .HasOptional(kp => kp.Rental)
                .WithMany(p => p.RentalAttributes);

            base.OnModelCreating(modelBuilder);
        }
    }

    
    
    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}