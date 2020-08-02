using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.Holidays;
using RealEstate.Model.Notifications;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services.Holidays
{
    public class HolidayServices
    {
        private readonly RealEstateDbContext dbContext;
        private readonly UserServices userServices;
        private readonly NotificationServices notificationServices;

        public HolidayServices(RealEstateDbContext dbContext
            , NotificationServices notificationServices
            , UserServices userServices)
        {
            this.dbContext = dbContext;
            this.notificationServices = notificationServices;
            this.userServices = userServices;
        }

        public async Task NotifyForHolidays()
        {
            var holidayNow = await dbContext.ConstantHolidays
                .Where(h => h.Month == DateTime.Now.Month && h.DayOfMonth == DateTime.Now.Day)
                .FirstOrDefaultAsync();

            if (holidayNow != null)
            {
                await CreateNotificationForHoliday(holidayNow.Name);
            }
            
            var holidayDateNow = GetMovingHolidays(DateTime.Now.Year)
                .FirstOrDefault(h => h.Date.Year == DateTime.Now.Year 
                            && h.Date.Month == DateTime.Now.Month 
                            && h.Date.Day == DateTime.Now.Day);

            if (holidayDateNow != null)
            {
                await CreateNotificationForHoliday(holidayDateNow.Name);
            }
        }

        private async Task CreateNotificationForHoliday(string holidayName)
        {
            var admin = await userServices
                                .GetUsersInRole(Enum.GetName(typeof(Role), Role.Administrator))
                                .FirstOrDefaultAsync();

            NotificationCreateViewModel notification = new NotificationCreateViewModel
            {
                NotificationTypeId = (int)NotificationType.Holiday,
                NotificationText = "Днес се празнува - " + holidayName
            };

            if (admin != null)
            {
                await notificationServices.CreateGlobalNotification(notification, admin.Id);
            }
            else
            {
                var agentIds = await dbContext.Users.OfType<AgentUsers>()
                    .Select(a => a.Id)
                    .ToListAsync();

                foreach (var agentId in agentIds)
                {
                    await notificationServices.CreateIndividualNotification(notification, agentId);
                }
            }
        }


        private List<ChangingHolidays> GetMovingHolidays(int year)
        {
            return new List<ChangingHolidays>
            {
                new ChangingHolidays
                {
                    Date = GetEasterSunday(year),
                    Name = "Великден"
                },
                new ChangingHolidays
                {
                    Date = GetGoodFriday(year),
                    Name = "Разпети петък"
                },
                new ChangingHolidays
                {
                    Date = GetAscensionDay(year),
                    Name = "Възнесение господне"
                },
                new ChangingHolidays
                {
                    Date = GetWhitSunday(year),
                    Name = "Петдесетница"
                },
                new ChangingHolidays
                {
                    Date = GetPalmDate(year),
                    Name = "Цветница"
                }
            };
        }

        //Великден
        private DateTime GetEasterSunday(int year)
        {
            int month = 0;
            int day = 0;
            int g = year % 19;
            int c = year / 100;
            int h = h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }

        //Разпети петък
        private DateTime GetGoodFriday(int year)
        {
            return GetEasterSunday(year).AddDays(-2);
        }

        //Възнесение господне
        private DateTime GetAscensionDay(int year)
        {
            return GetEasterSunday(year).AddDays(39);
        }

        //Петдесетница
        private DateTime GetWhitSunday(int year)
        {
            return GetEasterSunday(year).AddDays(49);
        }

        //Цветница
        private DateTime GetPalmDate(int year)
        {
            return GetEasterSunday(year).AddDays(-7);
        }
    }
}