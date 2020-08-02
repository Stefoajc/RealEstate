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
                    PropertyTypeName = "�����",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 2,
                    PropertyTypeName = "����",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 3,
                    PropertyTypeName = "�����",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 4,
                    PropertyTypeName = "�������",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 5,
                    PropertyTypeName = "���� ������",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 6,
                    PropertyTypeName = "����",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 7,
                    PropertyTypeName = "����������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 8,
                    PropertyTypeName = "����",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 9,
                    PropertyTypeName = "����",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 10,
                    PropertyTypeName = "�������",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 11,
                    PropertyTypeName = "����� �����",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 12,
                    PropertyTypeName = "�����",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 13,
                    PropertyTypeName = "����",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 14,
                    PropertyTypeName = "�������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 15,
                    PropertyTypeName = "��������� ������",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 16,
                    PropertyTypeName = "������",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 18,
                    PropertyTypeName = "�����",
                    IsPropertyOnly = true
                },
                new PropertyTypes
                {
                    PropertyTypeId = 19,
                    PropertyTypeName = "����",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 20,
                    PropertyTypeName = "������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 21,
                    PropertyTypeName = "�����",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 22,
                    PropertyTypeName = "���",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 23,
                    PropertyTypeName = "�������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 24,
                    PropertyTypeName = "���������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 25,
                    PropertyTypeName = "���",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 26,
                    PropertyTypeName = "������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 27,
                    PropertyTypeName = "����������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 28,
                    PropertyTypeName = "����������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 29,
                    PropertyTypeName = "���������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 30,
                    PropertyTypeName = "��������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 31,
                    PropertyTypeName = "��������",
                    IsPropertyOnly = false
                },
                new PropertyTypes
                {
                    PropertyTypeId = 32,
                    PropertyTypeName = "����������",
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
                new Extras("������"),
                new Extras("���"),
                new Extras("������"),
                new Extras("����"),
                new Extras("���� ������"),
                new Extras("������ ������"),
                new Extras("�������"),
                new Extras("��� �� �����"),
                new Extras("��������"),
                new Extras("����"),
                new Extras("�������"),
                new Extras("�����"),
                new Extras("�������"),
                new Extras("��������"),
                new Extras("���"),
                new Extras("���������"),
                new Extras("��������������"),
                new Extras("�����������������"),
                new Extras("��������� �����")
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
                new PropertyExtras("����������"),
                new PropertyExtras("� ������ (������)"),
                new PropertyExtras("������")
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
                new RentalExtras("��������"),
                new RentalExtras("������"),
                new RentalExtras("������������� �����"),
                new RentalExtras("���� �����"),
                new RentalExtras("������������� ��������� �����"),
                new RentalExtras("��� ��������� �����"),
                new RentalExtras("������")
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
                    FullAddress = "���. ���������� �������� 2",
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
                        AdditionalDescription = "��� �����, ��� ����",
                        UnitTypeId = 1,
                        UnitCount = 3,
                        //Extras = new HashSet<Extras>(context.RentalExtras.Take(3).ToList()),
                        RentalPrice = 10M,
                        RentalHirePeriodType = context.RentalHirePeriodsTypes.FirstOrDefault(),
                        Attributes = new HashSet<KeyValuePairs>
                        {
                            new KeyValuePairs
                            {
                                Key = "���� �����",
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
                    Description = "������ �� ���������� �� ��������.",
                    Code = "ReservationCaparo",
                    PaymentActionHandle = "CaparoPaymentCommand",
                    Amount = 0.0M,
                    PayedItemType = "Reservation",
                    PayedItemValue = null
                },
                new PayedItemsMeta
                {
                    Description = "����� ���� �� ���������� �� ��������.",
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
                    PeriodName = "�������",
                    IsTimePeriodSearchable = false
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "������",
                    IsTimePeriodSearchable = true
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "�� �����",
                    IsTimePeriodSearchable = true
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "��������",
                    IsTimePeriodSearchable = false
                },
                new RentalHirePeriodsTypes
                {
                    PeriodName = "�������",
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
                "������� ����",
                "���������� ���� �����������",
                "���������� ���� ������������",
                "������� �������",
                "���������� �������"
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
                "�� ��������� �������",
                "���������� �������� �� �����������",
                "���������� �������� �� �����",
                "������� �� ������ � ���",
                "������ �� ������ � ���"
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
                "���������� �����������",
                "���� � ����� �����",
                "���� � ���� ����",
                "���������",
                "��������� �������",
                "��������",
                "����������",
                "�������",
                "��������"
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
                "��� ���������",
                "�����",
                "�������",
                "��������� � ��������",
                "������������",
                "����� ���������",
                "�������� �����",
                "��������� � �������"
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
                "���",
                "�������",
                "��������",
                "������ ��������",
                "������",
                "�������",
                "�������� ����������",
                "�������� ����������",
                "������������",
                "��������"
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
                    Type = "����",     //���� �����������
                    ImageAssociation = "/Resources/NotificationTypes/logo-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Birthday,
                    Type = "������ ���", //������ ��� �� ������/��������
                    ImageAssociation = "/Resources/NotificationTypes/birthday-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Holiday,
                    Type = "�������",  //������� ����������
                    ImageAssociation = "/Resources/NotificationTypes/holiday-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Report,
                    Type = "������",   //������ �� ���� ��� �����������
                    ImageAssociation = "/Resources/NotificationTypes/report-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Learning,
                    Type = "��������", //�������� � �����
                    ImageAssociation = "/Resources/NotificationTypes/learning-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Property,
                    Type = "����",     //�������/����������/������ ����
                    ImageAssociation = "/Resources/NotificationTypes/property-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Search,
                    Type = "�������",  //������� �� ����
                    ImageAssociation = "/Resources/NotificationTypes/search-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Post,
                    Type = "����",     //���� ��� ������
                    ImageAssociation = "/Resources/NotificationTypes/post-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Calendar,
                    Type = "��������", //��������
                    ImageAssociation = "/Resources/NotificationTypes/calendar-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Question,
                    Type = "������",   //������ ��� ������� � ��������
                    ImageAssociation = "/Resources/NotificationTypes/question-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Material,
                    Type = "��������", //�������� � ������
                    ImageAssociation = "/Resources/NotificationTypes/material-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Contact,
                    Type = "�������",  //������� � ������
                    ImageAssociation = "/Resources/NotificationTypes/contact-notification.png"
                },
                new NotificationTypes
                {
                    Id = (int)NotificationType.Inquery,
                    Type = "���������", //�� ����������� �����
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
                    Name = "���� ������"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 14,
                    Name = "��.��������, ����� �� ���������"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 1,
                    Month = 3,
                    Name = "���� �����"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 3,
                    Month = 3,
                    Name = "��� �� �������������� �� �������� �� �������� ���"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 1,
                    Month = 5,
                    Name = "��� �� ����� � �� �������������� ������������ �����������"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 6,
                    Month = 5,
                    Name = "��� �� ���������� � ������� �� ����������� �����"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 24,
                    Month = 5,
                    Name = "��� �� ����������� �������� � ������� � �� ����������� ���������"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 6,
                    Month = 9,
                    Name = "��� �� ������������ �� ��������"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 22,
                    Month = 9,
                    Name = "��� �� �������������� �� ��������"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 1,
                    Month = 10,
                    Name = "��� �� ��������� ��������"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 24,
                    Month = 12,
                    Name = "����� �����"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 25,
                    Month = 12,
                    Name = "��������� �������� (������)"
                },
                new ConstantHolidays
                {
                    DayOfMonth = 26,
                    Month = 12,
                    Name = "��������� �������� (������)"
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
                   Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 2,
                    Name = "���� ��� - �op��, �op���"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 6,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 7,
                    Name = "���� ��� - ����"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 12,
                    Name = "���� ��� - ����, ������"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 17,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 18,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 20,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 21,
                    Name = "���� ��� - ������, ������"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 22,
                    Name = "���� ��� - �������, �������"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 24,
                    Name = "���� ��� - �������, ������"
                },
                new ConstantHolidays
                {
                    Month = 1,
                    DayOfMonth = 25,
                    Name = "���� ��� - ������, ��������"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 1,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 3,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 5,
                    Name = "���� ��� - �����, ������"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 6,
                    Name = "���� ��� - �������, �����"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 10,
                    Name = "���� ��� - ���������, �����, ��������"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 13,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 2,
                    DayOfMonth = 21,
                    Name = "���� ��� - �������, ��������"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 4,
                    Name = "���� ��� - �������, ���������"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 9,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 10,
                    Name = "���� ��� - ������, ����"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 16,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 19,
                    Name = "���� ��� - ������, �����"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 21,
                    Name = "���� ��� - ����"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 23,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 25,
                    Name = "���� ��� (������������) - ������, �����"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 26,
                    Name = "���� ��� - �������, ������"
                },
                new ConstantHolidays
                {
                    Month = 3,
                    DayOfMonth = 28,
                    Name = "���� ��� - ����, �����, �����, ������"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 4,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 14,
                    Name = "���� ��� - �������, �������"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 20,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 4,
                    DayOfMonth = 25,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 1,
                    Name = "���� ��� - �������"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 2,
                    Name = "���� ��� - �����, ���������"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 3,
                    Name = "���� ��� (������ �����, ����������� ��������) - �����, �����"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 5,
                    Name = "���� ��� - �����, �����"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 6,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 11,
                    Name = "���� ��� - ������, ��������"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 12,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 1,
                    Name = "���� ��� - "
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 21,
                    Name = "���� ��� - ����������, �����"
                },
                new ConstantHolidays
                {
                    Month = 5,
                    DayOfMonth = 30,
                    Name = "���� ��� - ����, ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 6,
                    Name = "���� ��� - ����"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 7,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 17,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 20,
                    Name = "���� ��� - �����, ������"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 23,
                    Name = "���� ��� - ����, ���, �������, ���������, �������, ����, �����, ������"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 24,
                    Name = "���� ��� - ����"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 29,
                    Name = "���� ��� - �����, �����"
                },
                new ConstantHolidays
                {
                    Month = 6,
                    DayOfMonth = 30,
                    Name = "���� ��� - �������"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 1,
                    Name = "���� ��� - ������, �����, ��������, ���������"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 5,
                    Name = "���� ��� - ������, ��������, �����"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 7,
                    Name = "���� ��� - �������, �������"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 15,
                    Name = "���� ��� - ��������, ���������, �����"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 16,
                    Name = "���� ��� - ����, ���, ����, �����"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 17,
                    Name = "���� ��� - �����, ������"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 20,
                    Name = "���� ��� (�������) - ����, �����, ������, ����, ����"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 22,
                    Name = "���� ��� - �����, ���������"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 25,
                    Name = "���� ��� - ���, ����"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 26,
                    Name = "���� ��� - ��������, ���������"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 27,
                    Name = "���� ��� (�������� ������) - ��������"
                },
                new ConstantHolidays
                {
                    Month = 7,
                    DayOfMonth = 31,
                    Name = "���� ��� - �������, �������"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 8,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 15,
                    Name = "���� ��� (������ ����������) - �����, �������, �������, �����, �������"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 20,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 26,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 8,
                    DayOfMonth = 30,
                    Name = "���� ��� - ����������, �����"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 1,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 3,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 5,
                    Name = "���� ��� - ������, �������, ����, ���������"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 14,
                    Name = "���� ��� (����������) - �������"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 16,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 17,
                    Name = "���� ��� - ����, �������, ����, �����, ����, �����, �������, �����, �����"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 22,
                    Name = "���� ��� - �������, ��������"
                },
                new ConstantHolidays
                {
                    Month = 9,
                    DayOfMonth = 25,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 1,
                    Name = "���� ��� - �����, ������"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 6,
                    Name = "���� ��� - ����, ������"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 7,
                    Name = "���� ��� - ������ 2"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 9,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 14,
                    Name = "���� ��� - �����, �����"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 18,
                    Name = "���� ��� - ������, ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 26,
                    Name = "���� ��� - �������"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 27,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 10,
                    DayOfMonth = 28,
                    Name = "���� ��� - �������, ��������"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 1,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 8,
                    Name = "���� ��� (�������������) - �����, ��������"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 11,
                    Name = "���� ��� - ����, �����"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 13,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 14,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 16,
                    Name = "���� ��� - �����"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 24,
                    Name = "���� ��� - ���������, ����"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 25,
                    Name = "���� ��� - �������, ����������"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 26,
                    Name = "���� ��� - �������, ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 11,
                    DayOfMonth = 30,
                    Name = "���� ��� - ������, ������, �������, ������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 4,
                    Name = "���� ��� - �������, �������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 5,
                    Name = "���� ��� - ����, �����, ���������, ������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 6,
                    Name = "���� ��� (��������) - �������, ��������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 8,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 8,
                    Name = "���� ��� - ���"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 12,
                    Name = "���� ��� - ��������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 14,
                    Name = "���� ��� - �����, �������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 15,
                    Name = "���� ��� - �������, �������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 17,
                    Name = "���� ��� - ������, �������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 18,
                    Name = "���� ��� - ������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 20,
                    Name = "���� ��� - ������, �����, �����, ���"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 22,
                    Name = "���� ��� - ���������"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 26,
                    Name = "���� ��� - �����, �����"
                },
                new ConstantHolidays
                {
                    Month = 12,
                    DayOfMonth = 27,
                    Name = "���� ��� - ������, �����, ������, ���������, ������, ������"
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
