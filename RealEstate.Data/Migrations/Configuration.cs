using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RealEstate.Extentions;
using RealEstate.Model;
using RealEstate.Model.ContactDiary;
using RealEstate.Model.Holidays;
using RealEstate.Model.Notifications;
using RealEstate.Model.Payment;
using RealEstate.Model.Reports;

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
            //SeedCountries(context);
            //SeedCities(context);

            //SeedExtras(context);
            //SeedPropertyExtras(context);
            //SeedRentalExtras(context);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            if (!context.PropertyTypes.Any())
            {
                SeedPropertyTypes(context);
            }
            //if (!context.PropertyRentPeriods.Any())
            //{
            //    context.PropertyRentPeriods.SeedEnumValues<PropertySeason, RentPeriod>(@enum => @enum);
            //}

            //SeedRentalHirePeriodsTypes(context);
            //context.SaveChanges();


            //SeedPayedItems(context);

            SeedDealTypes(context);
            SeedNegotiationStates(context);
            SeedContactedPersonTypes(context);


            //reports
            SeedWebPlatforms(context);
            SeedPromotionMediae(context);
            SeedPartnerTypes(context);


            //notifications
            SeedNotificationTypes(context);


            //Holidays
            SeedConstantHolidays(context);
        }

        private void SeedPropertyTypes(RealEstateDbContext context)
        {
            PropertyTypes[] propertyTypes =
            {
                new PropertyTypes
                {
                    PropertyTypeId = 1,
                    PropertyTypeName = "Хотел",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 2,
                    PropertyTypeName = "Къща",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 3,
                    PropertyTypeName = "Мотел",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 4,
                    PropertyTypeName = "Станция",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 5,
                    PropertyTypeName = "Офис сграда",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 6,
                    PropertyTypeName = "Офис",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 7,
                    PropertyTypeName = "Апартамент",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 8,
                    PropertyTypeName = "Вила",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 9,
                    PropertyTypeName = "Етаж",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 10,
                    PropertyTypeName = "Паркинг",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 11,
                    PropertyTypeName = "Парко място",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 12,
                    PropertyTypeName = "Гараж",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 13,
                    PropertyTypeName = "Земя",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 14,
                    PropertyTypeName = "Магазин",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 15,
                    PropertyTypeName = "Търговски център",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 16,
                    PropertyTypeName = "Сграда",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 18,
                    PropertyTypeName = "Склад",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 19,
                    PropertyTypeName = "Стая",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 20,
                    PropertyTypeName = "Студио",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 21,
                    PropertyTypeName = "Друго",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 22,
                    PropertyTypeName = "Цех",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 23,
                    PropertyTypeName = "Бунгало",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 24,
                    PropertyTypeName = "Ресторант",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 25,
                    PropertyTypeName = "Бар",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 26,
                    PropertyTypeName = "Кафене",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 27,
                    PropertyTypeName = "Гарсониера",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 28,
                    PropertyTypeName = "Боксониера",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 29,
                    PropertyTypeName = "Едностаен",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 30,
                    PropertyTypeName = "Двустаен",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 31,
                    PropertyTypeName = "Тристаен",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 32,
                    PropertyTypeName = "Многостаен",
                    IsPropertyOnly = false
                }
            };

            foreach (PropertyTypes propertyType in propertyTypes)
            {
                if (!context.PropertyTypes.Any(pt => pt.PropertyTypeName == propertyType.PropertyTypeName))
                {
                    context.PropertyTypes.Add(propertyType);
                }
            }

            context.SaveChanges();
        }

        private static void SeedUsers(RealEstateDbContext context)
        {

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);


            var roles = Enum.GetNames(typeof(Role)).Select(r => new IdentityRole(r));

            foreach (var role in roles)
            {
                if (!context.Roles.Any(r => r.Name == role.Name))
                {
                    roleManager.Create(role);
                }
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            List<string> ownerUserNames = new List<string> { "Owner", "Owner1", "Owner2", "Owner3" };
            foreach (var ownerUserName in ownerUserNames)
            {
                if (!context.Users.Any(u => u.UserName == ownerUserName))
                {
                    AddTeamUser(ownerUserName, ownerUserName + "@abv.bg", ownerUserName, ownerUserName, "Owner of properties", "0987654321",
                        @"\Resources\Users\Broker\team-1.jpg", Enum.GetName(typeof(Role), Role.Owner), userManager);
                }
            }

            List<string> maintenanceUserNames = new List<string> { "Maintenance", "Maintenance1", "Maintenance2", "Maintenance3" };
            foreach (var maintenanceUserName in maintenanceUserNames)
            {
                if (!context.Users.Any(u => u.UserName == maintenanceUserName))
                {
                    AddTeamUser(maintenanceUserName, maintenanceUserName + "@abv.bg", maintenanceUserName, maintenanceUserName, "Maintenance of properties", "0987654321",
                        @"\Resources\Users\Broker\team-1.jpg", Enum.GetName(typeof(Role), Role.Maintenance), userManager);
                }
            }

            List<string> marketerUserNames = new List<string> { "Marketer", "Marketer1", "Marketer2", "Marketer3" };
            foreach (var marketerUserName in marketerUserNames)
            {
                if (!context.Users.Any(u => u.UserName == marketerUserName))
                {
                    AddTeamUser(marketerUserName, marketerUserName + "@abv.bg", marketerUserName, marketerUserName, "Marketer of properties", "0987654321",
                        @"\Resources\Users\Broker\team-1.jpg", Enum.GetName(typeof(Role), Role.Marketer), userManager);
                }
            }

            List<string> clientUserNames = new List<string> { "Client", "Client2", "Client2", "Client3" };
            foreach (var clientUserName in clientUserNames)
            {
                if (!context.Users.Any(u => u.UserName == clientUserName))
                {
                    AddTeamUser(clientUserName, clientUserName + "@abv.bg", clientUserName, clientUserName, "Client of properties", "0987654321",
                        @"\Resources\Users\Broker\team-1.jpg", Enum.GetName(typeof(Role), Role.Client), userManager);
                }
            }

            List<string> agentUserNames = new List<string> { "Agent", "Agent2", "Agent2", "Agent3" };
            foreach (var agentUserName in agentUserNames)
            {
                if (!context.Users.Any(u => u.UserName == agentUserName))
                {
                    AddTeamUser(agentUserName, agentUserName + "@abv.bg", agentUserName, agentUserName, "Agent of properties", "0987654321",
                        @"\Resources\Users\Broker\team-1.jpg", Enum.GetName(typeof(Role), Role.Agent), userManager);
                }
            }

            List<string> adminUserNames = new List<string> { "Administrator", "Administrator1", "Administrator2", "Administrator3" };

            foreach (string adminUserName in adminUserNames)
            {
                if (!context.Users.Any(u => u.UserName == adminUserName))
                {
                    AddTeamUser(adminUserName, adminUserName + "@abv.bg", adminUserName, adminUserName, "Admin of the system", "0987654321",
                        @"\Resources\Users\Broker\team-1.jpg", Enum.GetName(typeof(Role), Role.Administrator), userManager);
                }
            }
            if (!context.Users.Any(u => u.UserName == "Administrator"))
            {
                var user = new ApplicationUser
                {
                    UserName = "Administrator",
                    Email = "Administrator@abv.bg",
                    FirstName = "Administrator",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    AdditionalDescription = "Administrator of Properties",
                    PhoneNumber = "0987654321",
                    Images = new HashSet<UserImages> { new UserImages { ImageRatio = 1, ImageType = "image/jpeg", ImagePath = @"\Resources\Users\Broker\team-1.jpg" } }
                };



                userManager.Create(user, "123qwe!@#");
                userManager.AddToRole(user.Id, "Administrator");
                userManager.AddToRole(user.Id, "TeamMember");
            }
        }

        private static void AddTeamUser(string userName, string email, string firstName, string lastName, string description,
            string phoneNumbers, string imagePath, string role, UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user;
            if (role == "Agent")
            {
                user = new AgentUsers
                {
                    SocialMediaAccounts = new HashSet<SocialMediaAccounts>
                    {
                        new SocialMediaAccounts
                        {
                            SocialMedia = "Facebook",
                            SocialMediaAccount = @"https://www.facebook.com/stefan.peev.587"
                        },new SocialMediaAccounts
                        {
                            SocialMedia = "Twitter",
                            SocialMediaAccount = @"https://twitter.com/login?lang=en"
                        },new SocialMediaAccounts
                        {
                            SocialMedia = "Skype",
                            SocialMediaAccount = @"ajc.4efo"
                        }
                    }
                };
            }
            else if (role == "Client")
            {
                user = new ClientUsers();
            }
            else if (role == "Owner")
            {
                user = new OwnerUsers();
            }
            else
            {
                user = new ApplicationUser();
            }

            user.UserName = userName;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.EmailConfirmed = true;
            user.PhoneNumber = phoneNumbers;
            user.PhoneNumberConfirmed = true;
            user.AdditionalDescription = description;
            user.Images = new HashSet<UserImages>
            {
                new UserImages {ImageRatio = 1, ImageType = "image/jpeg", ImagePath = imagePath}
            };

            userManager.Create(user, "123qwe!@#");
            userManager.AddToRole(user.Id, role);
            userManager.AddToRole(user.Id, "TeamMember");
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

        private void SeedSights()
        {

        }

        //Extras for all properties
        private void SeedExtras(RealEstateDbContext context)
        {
            List<Extras> extras = new List<Extras>
            {
                new Extras("Басейн"),
                new Extras("СПА"),
                new Extras("Фитнес"),
                new Extras("Двор"),
                new Extras("Голф игрище"),
                new Extras("Закрит Басейн"),
                new Extras("Веранда"),
                new Extras("Кът за отдих"),
                new Extras("Интернет"),
                new Extras("Море"),
                new Extras("Планина"),
                new Extras("Гараж"),
                new Extras("Паркинг"),
                new Extras("Асансьор"),
                new Extras("СОТ"),
                new Extras("Обзаведен"),
                new Extras("Водоснабдяване"),
                new Extras("Електроснабдяване"),
                new Extras("Санитарен възел")
            };

            foreach (var extra in extras)
            {
                if (!context.Extras.Any(r => r.ExtraName == extra.ExtraName))
                {
                    context.Extras.AddOrUpdate(extra);
                }
            }
        }

        private void SeedPropertyExtras(RealEstateDbContext context)
        {
            List<PropertyExtras> extras = new List<PropertyExtras>
            {
                new PropertyExtras("Ипотекиран"),
                new PropertyExtras("В строеж (зелено)"),
                new PropertyExtras("Лизинг")
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
            List<RentalExtras> extras = new List<RentalExtras>
            {
                new RentalExtras("Климатик"),
                new RentalExtras("Камина"),
                new RentalExtras("Самостоятелна Кухня"),
                new RentalExtras("Обща Кухня"),
                new RentalExtras("Самостоятелен санитарен възел"),
                new RentalExtras("Общ Санитарен възел"),
                new RentalExtras("Тераса")
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
                AreaInSquareMeters = 150,
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
                UnitType = context.PropertyTypes.FirstOrDefault(),
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
                //Extras = new HashSet<Extras>(context.PropertyExtras.Take(3).ToList()),
                SellingPrice = 150000,
                RentalPrice = 500M,
                RentalPricePeriodId = context.RentalHirePeriodsTypes.Select(r => r.Id).FirstOrDefault(),
                Rentals = new HashSet<RentalsInfo>
                {
                    new RentalsInfo
                    {
                        AdditionalDescription = "Три легла, две стаи",
                        UnitTypeId = 1,
                        UnitCount = 3,
                        //Extras = new HashSet<Extras>(context.RentalExtras.Take(3).ToList()),
                        RentalPrice = 10M,
                        RentalHirePeriodType = context.RentalHirePeriodsTypes.FirstOrDefault(),
                        Attributes = new HashSet<KeyValuePairs>
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

            if (!context.Properties.Any(c => c.PropertyName == properties.PropertyName))
            {
                context.Properties.Add(properties);
                context.SaveChanges();
            }
        }

        private void SeedPayedItems(RealEstateDbContext context)
        {
            var items = new List<PayedItemsMeta>
            {
                new PayedItemsMeta
                {
                    Description = "Капаро за резервация на квартира.",
                    Code = "ReservationCaparo",
                    PaymentActionHandle = "CaparoPaymentCommand",
                    Amount = 0.0M,
                    PayedItemType = "Reservation",
                    PayedItemValue = null
                },
                new PayedItemsMeta
                {
                    Description = "Пълна цена за резервация на квартира.",
                    Code = "ReservationFull",
                    PaymentActionHandle = "FullPaymentCommand",
                    Amount = 0.0M,
                    PayedItemType = "Reservation",
                    PayedItemValue = null
                }
            };

            foreach (var payedItem in items)
            {
                if (!context.PayedItems.Any(p => p.Code == payedItem.Code))
                {
                    context.PayedItems.Add(payedItem);
                }
            }

            context.SaveChanges();
        }
        private void SeedRentalHirePeriodsTypes(RealEstateDbContext context)
        {
            List<RentalHirePeriodsTypes> rentalPeriods = new List<RentalHirePeriodsTypes>
            {
                new RentalHirePeriodsTypes
                {
                    PeriodName = "Месечно",
                    IsTimePeriodSearchable = false
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "Дневно",
                    IsTimePeriodSearchable = true
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "На човек",
                    IsTimePeriodSearchable = true
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "Седмично",
                    IsTimePeriodSearchable = false
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "Годишно",
                    IsTimePeriodSearchable = false
                }
            };

            foreach (var rentalPeriod in rentalPeriods)
            {
                if (!context.RentalHirePeriodsTypes.Any(r => r.PeriodName == rentalPeriod.PeriodName))
                {
                    context.RentalHirePeriodsTypes.Add(rentalPeriod);
                }
            }

            context.SaveChanges();
        }

        private void SeedDealTypes(RealEstateDbContext context)
        {
            var dealTypes = new List<string>
            {
                "Търсене наем",
                "Предлагане наем дългосрочен",
                "Предлагане наем краткосрочен",
                "Търсене покупка",
                "Предлагане покупка"
            };
            foreach (var dealType in dealTypes)
            {
                if (context.DealTypes.Any(d => d.DealType == dealType)) continue;

                context.DealTypes.Add(new DealTypes { DealType = dealType });
            }
            context.SaveChanges();
        }

        private void SeedNegotiationStates(RealEstateDbContext dbContext)
        {
            var negotiationStates = new List<string>
            {
                "Не установен контакт",
                "Последващо обаждане за разясняване",
                "Последващо обаждане за среща",
                "Отказва да работи с нас",
                "Приема да работи с нас"
            };
            foreach (var negotiationState in negotiationStates)
            {
                if (dbContext.NegotiationStates.Any(d => d.State == negotiationState)) continue;

                dbContext.NegotiationStates.Add(new NegotiationStates { State = negotiationState });
            }
            dbContext.SaveChanges();
        }

        private void SeedContactedPersonTypes(RealEstateDbContext dbContext)
        {
            var contactedPersonTypes = new List<string>
            {
                "Стройтелен предприемач",
                "Лице с много имоти",
                "Лице с един имот",
                "Бизнесмен",
                "Влиятелна личност",
                "Наемател",
                "Наемодател",
                "Купувач",
                "Продавач"
            };
            foreach (var contactedPersonType in contactedPersonTypes)
            {
                if (dbContext.ContactedPersonTypes.Any(d => d.ContactedPersonType == contactedPersonType)) continue;

                dbContext.ContactedPersonTypes.Add(new ContactedPersonTypes { ContactedPersonType = contactedPersonType });
            }
            dbContext.SaveChanges();
        }


        private void SeedWebPlatforms(RealEstateDbContext dbContext)
        {
            var propertyPlatforms = new List<string>
            {
                "sproperties.net",
                "bazar.bg",
                "olx.bg",
                "alo.bg",
                "marica.bg",
                "plovdiv24.bg",
                "bezplatno.net",
                "pozvanete.bg",
                "bulgarian-offers.com",
                "imot.bg",
                "imoti.bg",
                "imoti.net",
                "imoti.com",
                "bulgarianproperties.bg",
                "homes.bg",
                "Facebook Marketplace"
            };

            foreach (var platform in propertyPlatforms)
            {
                if (!dbContext.WebPlatforms.Any(p => p.WebPlatform == platform))
                {
                    dbContext.WebPlatforms.Add(new WebPlatforms
                    {
                        WebPlatform = platform
                    });
                }
            }

            dbContext.SaveChanges();
        }

        private void SeedPromotionMediae(RealEstateDbContext dbContext)
        {
            var promotionMediae = new List<string>
            {
                "Уеб платформи",
                "Фаери",
                "Билборд",
                "Споделяне с контакти",
                "Транспаранти",
                "Емейл маркетинг",
                "Социални мрежи",
                "Споделяне с Брокери"
            };

            foreach (var promotionMedia in promotionMediae)
            {
                if (!dbContext.PromotionMediae.Any(p => p.Media == promotionMedia))
                {
                    dbContext.PromotionMediae.Add(new PromotionMedia
                    {
                        Media = promotionMedia
                    });
                }
            }

            dbContext.SaveChanges();
        }

        private void SeedPartnerTypes(RealEstateDbContext dbContext)
        {
            var partnerTypes = new List<string>
            {
                "ЧСИ",
                "Адвокат",
                "Нотариус",
                "Банков служител",
                "Брокер",
                "Агенция",
                "Кредитен консултант",
                "Финансов консултант",
                "Счетоводител",
                "Архитект"
            };

            foreach (var partnerType in partnerTypes)
            {
                if (!dbContext.PartnerTypes.Any(p => p.Type == partnerType))
                {
                    dbContext.PartnerTypes.Add(new PartnerTypes
                    {
                        Type = partnerType
                    });
                }
            }

            dbContext.SaveChanges();
        }

        private void SeedNotificationTypes(RealEstateDbContext dbContext)
        {
            var notificationTypes = new List<NotificationTypes>
            {
                new NotificationTypes
                {
                    Id = (int)NotificationType.Basic,
                    Type = "Обща",     //Обща нотификация
                    ImageAssociation = "/Resources/NotificationTypes/logo-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Birthday,
                    Type = "Рожден ден", //Рожден ден на клиент/партньор
                    ImageAssociation = "/Resources/NotificationTypes/birthday-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Holiday,
                    Type = "Празник",  //Празник национален
                    ImageAssociation = "/Resources/NotificationTypes/holiday-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Report,
                    Type = "Доклад",   //Доклад за имот към собственика
                    ImageAssociation = "/Resources/NotificationTypes/report-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Learning,
                    Type = "Обучение", //Обучение в офиса
                    ImageAssociation = "/Resources/NotificationTypes/learning-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Property,
                    Type = "Имот",     //Добавен/редактиран/изтрит имот
                    ImageAssociation = "/Resources/NotificationTypes/property-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Search,
                    Type = "Търсене",  //Търсене на имот
                    ImageAssociation = "/Resources/NotificationTypes/search-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Post,
                    Type = "Пост",     //Пост във форума
                    ImageAssociation = "/Resources/NotificationTypes/post-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Calendar,
                    Type = "Календар", //Календар
                    ImageAssociation = "/Resources/NotificationTypes/calendar-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Question,
                    Type = "Въпрос",   //Въпрос във въпроси и отговори
                    ImageAssociation = "/Resources/NotificationTypes/question-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Material,
                    Type = "Материал", //Материал в базата
                    ImageAssociation = "/Resources/NotificationTypes/material-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Contact,
                    Type = "Контакт",  //Контакт с клиент
                    ImageAssociation = "/Resources/NotificationTypes/contact-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Inquery,
                    Type = "Запитване", //От контактната форма
                    ImageAssociation = "/Resources/NotificationTypes/logo-notification.png"
                }
            };

            foreach (var notificationType in notificationTypes)
            {
                if (!dbContext.NotificationTypes.Any(p => p.Type == notificationType.Type))
                {
                    dbContext.NotificationTypes.Add(new NotificationTypes
                    {
                        Id = notificationType.Id,
                        Type = notificationType.Type
                    });
                }
            }

            dbContext.SaveChanges();
        }

        private void SeedConstantHolidays(RealEstateDbContext dbContext)
        {
            List<ConstantHolidays> nationalHolidays = new List<ConstantHolidays>
            {
                new ConstantHolidays
                {
                    DayOfMonth = 1,
                    Month = 1,
                    Name = "Нова година"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 14,
                    Name = "св.Валентин, денят на влюбените"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 1,
                    Month = 3,
                    Name = "Баба Марта"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 3,
                    Month = 3,
                    Name = "Ден на Освобождението на България от османско иго"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 1,
                    Month = 5,
                    Name = "Ден на труда и на международната работническа солидарност"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 6,
                    Month = 5,
                    Name = "Ден на храбростта и празник на Българската армия"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 24,
                    Month = 5,
                    Name = "Ден на българската просвета и култура и на славянската писменост"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 6,
                    Month = 9,
                    Name = "Ден на Съединението на България"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 22,
                    Month = 9,
                    Name = "Ден на Независимостта на България"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 1,
                    Month = 10,
                    Name = "Ден на народните будители"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 24,
                    Month = 12,
                    Name = "Бъдни вечер"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 25,
                    Month = 12,
                    Name = "Рождество Христово (Коледа)"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 26,
                    Month = 12,
                    Name = "Рождество Христово (Коледа)"
                }
            };

            foreach (var constantHoliday in nationalHolidays)
            {
                if (!dbContext.ConstantHolidays.Any(c => c.Name == constantHoliday.Name))
                {
                    dbContext.ConstantHolidays.Add(constantHoliday);
                }
            }


            List<ConstantHolidays> nameDays = new List<ConstantHolidays>
            {
                new ConstantHolidays
                {
                   Month = 1,
                   DayOfMonth = 1,
                   Name = "Имен ден - Васил"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 2,
                    Name = "Имен ден - Гopан, Гopица"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 6,
                    Name = "Имен ден - Йордан"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 7,
                    Name = "Имен ден - Иван"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 12,
                    Name = "Имен ден - Таня, Татяна"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 17,
                    Name = "Имен ден - Антон"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 18,
                    Name = "Имен ден - Атанас"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 20,
                    Name = "Имен ден - Евтим"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 21,
                    Name = "Имен ден - Агнеса, Максим"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 22,
                    Name = "Имен ден - Тимотей, Тимофей"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 24,
                    Name = "Имен ден - Аксения, Ксения"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 25,
                    Name = "Имен ден - Григор, Григории"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 1,
                    Name = "Имен ден - Трифон"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 3,
                    Name = "Имен ден - Симеон"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 5,
                    Name = "Имен ден - Агата, Агатия"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 6,
                    Name = "Имен ден - Пламена, Огнян"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 10,
                    Name = "Имен ден - Харалампи, Ламби, Валентин"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 13,
                    Name = "Имен ден - Евлоги, Евлогия"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 21,
                    Name = "Имен ден - Евстати, Евстатия"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 4,
                    Name = "Имен ден - Герасим, Герасимка"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 9,
                    Name = "Имен ден - Младен, Младена"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 10,
                    Name = "Имен ден - Галина, Галя"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 16,
                    Name = "Имен ден - Тодор"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 19,
                    Name = "Имен ден - Дарина, Дария"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 21,
                    Name = "Имен ден - Яков"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 23,
                    Name = "Имен ден - Лидия"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 25,
                    Name = "Имен ден (Благовещение) - Благой, Благо"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 26,
                    Name = "Имен ден - Гавраил, Гаврил"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 28,
                    Name = "Имен ден - Боян, Бояна, Бойко, Албена"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 4,
                    Name = "Имен ден - Аврам"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 14,
                    Name = "Имен ден - Мартина, Мартина"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 20,
                    Name = "Имен ден - Лазар"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 25,
                    Name = "Имен ден - Марко"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 1,
                    Name = "Имен ден - Йеремия"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 2,
                    Name = "Имен ден - Борис, Борислава"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 3,
                    Name = "Имен ден (Светли петък, Живоприемни източник) - Живко, Живка"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 5,
                    Name = "Имен ден - Ирена, Ирина"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 6,
                    Name = "Имен ден - Георги, Гергана"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 11,
                    Name = "Имен ден - Томина, Томислав"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 12,
                    Name = "Имен ден - Герман"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 1,
                    Name = "Имен ден - "
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 21,
                    Name = "Имен ден - Константин, Елена"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 30,
                    Name = "Имен ден - Емил, Емилия, Емилиян"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 6,
                    Name = "Имен ден - Спас"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 7,
                    Name = "Имен ден - Валери, Валерия"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 17,
                    Name = "Имен ден - Мануил, Емануил"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 20,
                    Name = "Имен ден - Бисер, Бисера"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 23,
                    Name = "Имен ден - Асен, Ася, Аспарух, Десислава, Панайот, Крум, Румен, Чавдар"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 24,
                    Name = "Имен ден - Еньо"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 29,
                    Name = "Имен ден - Петър, Петра"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 30,
                    Name = "Имен ден - Апостол"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 1,
                    Name = "Имен ден - Кузман, Дамян, Красимир, Красимира"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 5,
                    Name = "Имен ден - Атанас, Атанаска, Наско"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 7,
                    Name = "Имен ден - Недялко, Недялка"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 15,
                    Name = "Имен ден - Владимир, Владимира, Влади"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 16,
                    Name = "Имен ден - Олга, Оля, Юлия, Юлиан"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 17,
                    Name = "Имен ден - Марин, Марина"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 20,
                    Name = "Имен ден (Илинден) - Илия, Илиян, Илияна, Илко, Илка"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 22,
                    Name = "Имен ден - Магда, Магдалена"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 25,
                    Name = "Имен ден - Ана, Анна"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 26,
                    Name = "Имен ден - Параскев, Параскева"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 27,
                    Name = "Имен ден (Пантелей пътник) - Пантелей"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 31,
                    Name = "Имен ден - Евдоким, Евдокия"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 8,
                    Name = "Имен ден - Емилия, Емилиян"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 15,
                    Name = "Имен ден (Голяма Богородица) - Мария, Мариана, Марияна, Марко, Деспина"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 20,
                    Name = "Имен ден - Самуил"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 26,
                    Name = "Имен ден - Адриан, Адриана"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 30,
                    Name = "Имен ден - Александър, Алекс"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 1,
                    Name = "Имен ден - Симеон"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 3,
                    Name = "Имен ден - Антим"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 5,
                    Name = "Имен ден - Захари, Захария, Зара, Елисавета"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 14,
                    Name = "Имен ден (Кръстовден) - Кръстьо"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 16,
                    Name = "Имен ден - Людмил, Людмила"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 17,
                    Name = "Имен ден - Вяра, Надежда, Надя, Любов, Люба, Любен, Любомир, София, Софка"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 22,
                    Name = "Имен ден - Гълъбин, Гълъбина"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 25,
                    Name = "Имен ден - Сергей"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 1,
                    Name = "Имен ден - Анани, Анания"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 6,
                    Name = "Имен ден - Тома, Томина"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 7,
                    Name = "Имен ден - Сергей 2"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 9,
                    Name = "Имен ден - Аврам"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 14,
                    Name = "Имен ден - Петко, Петка"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 18,
                    Name = "Имен ден - Златка, Златко, Златина"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 26,
                    Name = "Имен ден - Димитър"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 27,
                    Name = "Имен ден - Нестор"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 28,
                    Name = "Имен ден - Лъчезар, Лъчезара"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 1,
                    Name = "Имен ден - Аргир"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 8,
                    Name = "Имен ден (Архангеловден) - Ангел, Архангел"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 11,
                    Name = "Имен ден - Мина, Минка"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 13,
                    Name = "Имен ден - Евлоги, Евлогия"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 14,
                    Name = "Имен ден - Филип"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 16,
                    Name = "Имен ден - Матей"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 24,
                    Name = "Имен ден - Екатерина, Катя"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 25,
                    Name = "Имен ден - Климент, Климентина"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 26,
                    Name = "Имен ден - Стилиан, Стилян, Стиляна"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 30,
                    Name = "Имен ден - Андрей, Андреа, Андриан, Първан"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 4,
                    Name = "Имен ден - Варвара, Варадин"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 5,
                    Name = "Имен ден - Сава, Сафка, Елисавета, Славчо"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 6,
                    Name = "Имен ден (Никулден) - Николай, Николета"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 8,
                    Name = "Имен ден - Стамат"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 8,
                    Name = "Имен ден - Ана"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 12,
                    Name = "Имен ден - Спиридон"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 14,
                    Name = "Имен ден - Снежа, Снежана"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 15,
                    Name = "Имен ден - Свобода, Елевтер"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 17,
                    Name = "Имен ден - Данаил, Данаила"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 18,
                    Name = "Имен ден - Модест"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 20,
                    Name = "Имен ден - Пламен, Огнян, Игнат, Иго"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 22,
                    Name = "Имен ден - Анастасия"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 26,
                    Name = "Имен ден - Давид, Йосиф"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 27,
                    Name = "Имен ден - Стефан, Стоян, Станчо, Венцислав, Запрян, Стамен"
                }
            };

            foreach (var nameDay in nameDays)
            {
                if (!dbContext.ConstantHolidays.Any(c => c.Name == nameDay.Name))
                {
                    dbContext.ConstantHolidays.Add(nameDay);
                }
            }


            dbContext.SaveChanges();
        }
    }
}
