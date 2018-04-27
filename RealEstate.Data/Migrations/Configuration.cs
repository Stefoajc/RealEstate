using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RealEstate.Extentions;
using RealEstate.Model;

namespace RealEstate.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<RealEstateDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(RealEstateDbContext context)
        {
            SeedCountries(context);
            SeedCities(context);

            SeedPropertyExtras(context);
            SeedRentalExtras(context);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            if (!context.PropertyTypes.Any())
            {
                SeedPropertyTypes(context);
            }
            if (!context.PropertyRentPeriods.Any())
            {
                context.PropertyRentPeriods.SeedEnumValues<PropertySeason, RentPeriod>(@enum => @enum);
            }
            if (!context.UnitTypes.Any())
            {
                context.UnitTypes.SeedEnumValues<UnitTypes, UnitType>(@enum => @enum);
            }
            context.SaveChanges();
            SeedUsers(context);
            SeedProperties(context);
            context.SaveChanges();
        }

        private void SeedPropertyTypes(RealEstateDbContext context)
        {
            string[] propertyTypes = new[] { "Хотел", "Къща", "Мотел", "Станция", "Офис сграда", "Офис", "Вила", "Етаж", "Паркинг", "Парко място", "Гараж", "Земя", "Магазин", "Търговски център", "Сграда", "Склад", "Стаи", "Друго" };

            for (int i = 0; i < propertyTypes.Length; i++)
            {
                var propertyType = propertyTypes[i];
                if (!context.PropertyTypes.Any(pt => pt.PropertyTypeName == propertyType))
                {
                    context.PropertyTypes.Add(new PropertyTypes
                    {
                        PropertyTypeId = i,
                        PropertyTypeName = propertyTypes[i]
                    });
                }
            }

            context.SaveChanges();
        }


        private static void SeedUsers(RealEstateDbContext context)
        {

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            List<IdentityRole> roles = new List<IdentityRole>()
                {
                    new IdentityRole("Client"),
                    new IdentityRole("Owner"),
                    new IdentityRole("Administrator"),
                    new IdentityRole("Maintenance"),
                    new IdentityRole("Agent")
                };

            foreach (var role in roles)
            {
                if (!context.Roles.Any(r => r.Name == role.Name))
                {
                    roleManager.Create(role);
                }
            }


            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(u => u.UserName == "Owner"))
            {
                var user = new OwnerUsers
                {
                    UserName = "Owner",
                    Email = "owner@abv.bg",
                    FirstName = "Owner",
                    LastName = "Owner",
                    EmailConfirmed = true,
                    Description = "Owner of Properties",
                    PhoneNumber = "0987654321",
                    Images = new HashSet<UserImages> { new UserImages { ImageRatio = 1, ImageType = "image/jpeg", ImagePath = @"\Resources\Users\Broker\team-1.jpg" } }
                };

                userManager.Create(user, "123qwe!@#");
                userManager.AddToRole(user.Id, "Owner");
            }

            if (!context.Users.Any(u => u.UserName == "Maintenance"))
            {
                var user = new ApplicationUser()
                {
                    UserName = "Maintenance",
                    Email = "Maintenance@abv.bg",
                    FirstName = "Maintenance",
                    LastName = "Maintenance",
                    EmailConfirmed = true,
                    Description = "Maintenance of Properties",
                    PhoneNumber = "0987654321",
                    Images = new HashSet<UserImages> { new UserImages { ImageRatio = 1, ImageType = "image/jpeg", ImagePath = @"\Resources\Users\Broker\team-1.jpg" } }
                };

                userManager.Create(user, "123qwe!@#");
                userManager.AddToRole(user.Id, "Maintenance");
            }

            if (!context.Users.Any(u => u.UserName == "Client"))
            {
                var user = new ClientUsers()
                {
                    UserName = "Client",
                    Email = "Client@abv.bg",
                    FirstName = "Client",
                    LastName = "Client",
                    EmailConfirmed = true,
                    Description = "Client of Properties",
                    PhoneNumber = "0987654321",
                    Images = new HashSet<UserImages> { new UserImages { ImageRatio = 1, ImageType = "image/jpeg", ImagePath = @"\Resources\Users\Broker\team-1.jpg" } }
                };

                userManager.Create(user, "123qwe!@#");
                userManager.AddToRole(user.Id, "Client");
            }

            if (!context.Users.Any(u => u.UserName == "Agent"))
            {
                var user = new AgentUsers()
                {
                    UserName = "Agent",
                    Email = "Agent@abv.bg",
                    FirstName = "Agent",
                    LastName = "Agent",
                    EmailConfirmed = true,
                    Description = "Agent of Properties",
                    PhoneNumber = "0987654321",
                    Images = new HashSet<UserImages> { new UserImages { ImageRatio = 1, ImageType = "image/jpeg", ImagePath = @"\Resources\Users\Broker\team-1.jpg" } }
                };

                userManager.Create(user, "123qwe!@#");
                userManager.AddToRole(user.Id, "Agent");
            }

            if (!context.Users.Any(u => u.UserName == "Administrator"))
            {
                var user = new ApplicationUser()
                {
                    UserName = "Administrator",
                    Email = "Administrator@abv.bg",
                    FirstName = "Administrator",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    Description = "Administrator of Properties",
                    PhoneNumber = "0987654321",
                    Images = new HashSet<UserImages> { new UserImages { ImageRatio = 1, ImageType = "image/jpeg", ImagePath = @"\Resources\Users\Broker\team-1.jpg" } }
                };

                userManager.Create(user, "123qwe!@#");
                userManager.AddToRole(user.Id, "Administrator");
            }
        }

        private void SeedCountries(RealEstateDbContext context)
        {
            if (context.Countries.Any())
            {
                return;
            }
            using (StreamReader fs = new StreamReader(@"D:\RealEstate\Resources\Countries.csv"))
            {
                string line;
                while ((line = fs.ReadLine()) != null)
                {
                    string[] countryProperties = line.Trim(' ', '\r', '\n').Split(';');
                    Countries country = new Countries
                    {
                        CountryId = int.Parse(countryProperties[0]),
                        CountryNameBG = countryProperties[1],
                        CountryNameEN = countryProperties[2],
                        IsoCode = countryProperties[3],
                    };

                    context.Countries.Add(country);
                }
                context.SaveChanges();
            }
        }

        private void SeedCities(RealEstateDbContext context)
        {
            if (context.Cities.Any())
            {
                return;
            }
            using (StreamReader fs = new StreamReader(@"D:\RealEstate\Resources\Cities.csv"))
            {
                string line;
                while ((line = fs.ReadLine()) != null)
                {
                    string[] cityProperties = line.Trim(' ', '\r', '\n').Split(';');
                    Cities city = new Cities
                    {
                        CityId = int.Parse(cityProperties[0]),
                        CityName = cityProperties[1],
                        PostalCode = cityProperties[2],
                        PhoneCode = cityProperties[3],
                        CityCode = cityProperties[4],
                        CountryId = int.Parse(cityProperties[5])
                    };

                    context.Cities.Add(city);
                }
                context.SaveChanges();
            }
        }

        private void SeedRentalExtras()
        {

        }

        private void SeedPropertyExtras()
        {

        }

        private void SeedSights()
        {

        }


        private void SeedPropertyExtras(RealEstateDbContext context)
        {
            List<PropertyExtras> extras = new List<PropertyExtras>()
            {
                new PropertyExtras("Басейн"),
                new PropertyExtras("СПА"),
                new PropertyExtras("Двор"),
                new PropertyExtras("Двор"),
                new PropertyExtras("Голф игрище"),
                new PropertyExtras("Закрит Басейн"),
                new PropertyExtras("Веранда"),
                new PropertyExtras("Кът за отдих"),
            };

            foreach (var extra in extras)
            {
                if (!context.PropertyExtras.Any(r => r.ExtraName == extra.ExtraName))
                {
                    context.PropertyExtras.AddOrUpdate(extra);
                }
            }
        }


        private void SeedRentalExtras(RealEstateDbContext context)
        {
            List<RentalExtras> extras = new List<RentalExtras>()
            {
                new RentalExtras("Климатик"),
                new RentalExtras("Wi-Fi"),
                new RentalExtras("Самостоятелна Кухня"),
                new RentalExtras("Обща Кухня"),
                new RentalExtras("Самостоятелен санитарен възел"),
                new RentalExtras("Общ Санитарен възел"),
                new RentalExtras("Камина"),
                new RentalExtras("Изглед към море"),
                new RentalExtras("Изглед към планината")
            };

            foreach (var extra in extras)
            {
                if (!context.RentalExtras.Any(r => r.ExtraName == extra.ExtraName))
                {
                    context.RentalExtras.AddOrUpdate(extra);
                }
            }
        }

        private void SeedProperties(RealEstateDbContext context)
        {

            var properties = new Properties
            {
                PropertyName = "Lorem Ipsum Dolor 1",
                IsActive = true,
                AreaInSquareFt = 150,
                AdditionalDescription =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin rutrum nisi eu ante mattis.",
                AgentId = context.Users.OfType<AgentUsers>().Select(a => a.Id).FirstOrDefault(),
                OwnerId = context.Users.OfType<OwnerUsers>().Select(a => a.Id).FirstOrDefault(),
                Address = new Addresses
                {
                    CityId = context.Cities.Select(c => c.CityId).FirstOrDefault(),
                    FullAddress = "бул. Константин Величков 2",
                    Coordinates = new Coordinates
                    {
                        Latitude = 42.15D,
                        Longtitude = 24.75D
                    }
                },
                PropertySeason = context.PropertyRentPeriods.FirstOrDefault(),
                PropertyType = context.PropertyTypes.FirstOrDefault(),
                Images = new HashSet<PropertyImages>
                {
                    new PropertyImages
                    {
                        ImageRatio = 1.5F,
                        ImageType = "image/jpeg",
                        ImagePath = @"\Images\properties\property-detail-1.jpg"
                    },
                    new PropertyImages
                    {
                        ImageRatio = 1.5F,
                        ImageType = "image/jpeg",
                        ImagePath = @"\Images\properties\property-detail-2.jpg"
                    }
                },
                PropertyExtras = new HashSet<PropertyExtras>(context.PropertyExtras.Take(3).ToList()),
                SellingPrice = 150000,
                RentalPrice = 500M,
                Rentals = new HashSet<RentalsInfo>
                {
                    new RentalsInfo
                    {
                        AdditionalInfo = "Три легла, две стаи",
                        UnitTypeId = (int)UnitType.Стая,
                        UnitCount = 3,
                        RentalExtras = new HashSet<RentalExtras>(context.RentalExtras.Take(3).ToList()),
                        RentalPrice = 10M,
                        RentPricePeriod = RentalPeriod.PerBedDaily,
                        RentalAttributes = new HashSet<KeyValuePairs>
                        {
                            new KeyValuePairs
                            {
                                Key = "Брой легла",
                                Value = "3"
                            }
                        }
                    }
                }
            };

            context.Properties.Add(properties);
            context.SaveChanges();
        }

    }
}
