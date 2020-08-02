using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Extentions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class AppointmentServices : BaseService
    {
        private readonly IEmailService emailService;
        private readonly ISmsService smsService;
        private readonly UserServices usersManager;
        private readonly NotificationServices notificationServices;

        [Inject]
        public AppointmentServices(IUnitOfWork unitOfWork
            , ApplicationUserManager userMgr
            , IEmailService emailService
            , UserServices usersManager
            , NotificationServices notificationServices) 
            : base(unitOfWork, userMgr)
        {
            this.emailService = emailService;
            this.usersManager = usersManager;
            this.notificationServices = notificationServices;
        }


        public async Task<bool> HasPassedApprovedAppointmentForProperty(string userId, int propertyId)
        {
            var appointment = await unitOfWork.AppointmentsRepository
                .Where(a => a.IssuerUserId == userId && a.PropertyId == propertyId 
                    && a.IsApprovedByAgent && a.AppointmentDate < DateTime.Now)
                .FirstOrDefaultAsync();

            return appointment != null;
        }

        public async Task<bool> HasPassedApprovedAppointmentWithAgent(string userId, string agentId)
        {
            var appointment = await unitOfWork.AppointmentsRepository
                .Where(a => a.IssuerUserId == userId && a.AgentId == agentId 
                    && a.IsApprovedByAgent && a.AppointmentDate < DateTime.Now)
                .FirstOrDefaultAsync();

            return appointment != null;
        }

        public async Task<List<FullCalendarEventViewModel>> GetAgentAppointmentsForClient(string agentId, DateTime? dateStart, DateTime? dateEnd)
        {
            var appointments = (await List(agentId: agentId, dateStart: dateStart, dateEnd: dateEnd, approved: true)
                .ToListAsync())
                .Select(a => new FullCalendarEventViewModel
                {
                    Id = a.Id.ToString(),
                    StartDateInMilliseconds = a.AppointmentDate.GetMiliseconds(),
                    EndDateInMilliseconds = a.AppointmentDate.AddHours(1).GetMiliseconds(),
                    Title = "Среща с клиент"
                })
                .ToList();

            return appointments;
        }

        public async Task<List<AgentOwnAppointmentsViewModel>> GetAgentOwnAppointments(string agentId, DateTime? dateStart = null, DateTime? dateEnd = null)
        {
            var appointments = (await List(agentId: agentId, dateStart: dateStart, dateEnd: dateEnd, approved: null)
                .Include(a => a.Property)
                .Include(a => a.IssuerUser)
                .Include(a => a.NonRegisteredUser)
                .ToListAsync())
                .Select(a => new AgentOwnAppointmentsViewModel
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AdditionalDescription = a.AppointmentDescription,
                    ClientEmail = a.NonRegisteredUser != null ? a.NonRegisteredUser.ClientEmail : a.IssuerUser.Email,
                    ClientName = a.NonRegisteredUser != null ? a.NonRegisteredUser.ClientName : a.IssuerUser.UserName,
                    ClientPhoneNumber = a.NonRegisteredUser != null ? a.NonRegisteredUser.ClientPhoneNumber : a.IssuerUser.PhoneNumber,
                    IsApprovedByAgent = a.IsApprovedByAgent,
                    Property = Mapper.Map<PropertyInfoViewModel>(a.Property)
                })
                .ToList();

            return appointments;
        }

        public async Task<List<ClientAppointmentsViewModel>> GetClientAppointments(string clientId, DateTime? dateStart = null, DateTime? dateEnd = null, bool? isApproved = null)
        {
            var appointments = (await List(clientId: clientId, dateStart: dateStart, dateEnd: dateEnd, approved: isApproved)
                .Include(a => a.Agent)
                .Include(a => a.Property)
                .Include(a => a.Property.UnitType)
                .ToListAsync())
                .Select(a => new ClientAppointmentsViewModel
                {
                    AppointmentId = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    IsApprovedByAgent = a.IsApprovedByAgent,

                    Property = Mapper.Map<PropertyInfoViewModel>(a.Property),
                    Agent = Mapper.Map<AppointmentAgentViewModel>(a.Agent)
                })
                .ToList();

            return appointments;
        }


        #region Helpers
        private IQueryable<Appointments> List(string agentId = null, string clientId = null, int? propertyId = null, DateTime? dateStart = null, DateTime? dateEnd = null, bool? approved = null, bool? deleted = false)
        {
            var appointments = unitOfWork.AppointmentsRepository.GetAll();

            appointments = string.IsNullOrEmpty(agentId) ? appointments : appointments.Where(a => a.AgentId == agentId);
            appointments = string.IsNullOrEmpty(clientId) ? appointments : appointments.Where(a => a.IssuerUserId == clientId);
            appointments = propertyId == null ? appointments : appointments.Where(a => a.PropertyId == propertyId);
            appointments = dateStart == null
                ? appointments
                : appointments.Where(a => a.AppointmentDate > dateStart);

            appointments = dateEnd == null
                ? appointments
                : appointments.Where(a => a.AppointmentDate < dateEnd);

            appointments = approved == null ? appointments : appointments.Where(a => a.IsApprovedByAgent == approved);
            appointments = appointments.Where(a => a.isDeleted == deleted);

            return appointments;
        }

        #endregion

        public async Task<int> Create(AppointmentCreateDTO model, NonRegisteredUserCreateDTO user)
        {
            if (!await userManager.IsInRoleAsync(model.AgentId, AgentRole))
            {
                throw new NotAuthorizedException("Потребителят на който заявявате среща не е Брокер или не е намерен!");
            }
            if (!await unitOfWork.PropertiesBaseRepository.GetAll().AnyAsync(p => p.Id == model.PropertyId))
            {
                throw new ContentNotFoundException("Имотът, за който заявявате среща не е намерен в системата!");
            }
            
            Appointments appointment = new Appointments
            {
                AgentId = model.AgentId,
                PropertyId = model.PropertyId,
                AppointmentDate = model.AppointmentDate,
                AppointmentDescription = model.AppointmentDescription,
                NonRegisteredUser = new NonRegisteredAppointmentUsers
                {
                    ClientEmail  = user.ClientEmail,
                    ClientName = user.ClientName,
                    ClientPhoneNumber = user.ClientPhoneNumber
                }
            };
            var appointmentId = await CreateAppointment(appointment);
            //Notify Agent for the appointment
            if (ConfigurationManager.AppSettings["AppStatus"] != "Developement")
            {
                await SendEmailAndSmsNotificationToAgent(model.AgentId, model.PropertyId, user.ClientName, user.ClientEmail, user.ClientPhoneNumber);
            }
            return appointmentId;
        }

        public async Task<int> Create(AppointmentCreateDTO model, string userId)
        {
            if (!await userManager.IsInRoleAsync(model.AgentId, AgentRole))
            {
                throw new NotAuthorizedException("Потребителят на който заявявате среща не е Брокер или не е намерен!");
            }
            if (!await unitOfWork.PropertiesBaseRepository.GetAll().AnyAsync(p => p.Id == model.PropertyId))
            {
                throw new ContentNotFoundException("Имотът, за който заявявате среща не е намерен в системата!");
            }
            if (!await userManager.IsInRoleAsync(userId, ClientRole))
            {
                throw new NotAuthorizedException("Потребителят от който заявявате среща не е намерен!");
            }

            Appointments appointment = new Appointments
            {
                AgentId = model.AgentId,
                PropertyId = model.PropertyId,
                AppointmentDate = model.AppointmentDate,
                AppointmentDescription = model.AppointmentDescription,
                IssuerUserId = userId
            };

            var appointmentId = await CreateAppointment(appointment);
            //Notify Agent for the appointment
            if (ConfigurationManager.AppSettings["AppStatus"] != "Developement")
            {
                var userInfo = await userManager.FindByIdAsync(userId);
                await SendEmailAndSmsNotificationToAgent(model.AgentId, model.PropertyId, userInfo.UserName, userInfo.Email, userInfo.PhoneNumber);
            }
            return appointmentId;
        }

        private async Task<int> CreateAppointment(Appointments appointment)
        {
            unitOfWork.AppointmentsRepository.Add(appointment);
            await unitOfWork.SaveAsync();
            return appointment.Id;
        }



        public async Task Delete(int id, string userId)
        {
            var appointmentToDelete = await unitOfWork.AppointmentsRepository
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (appointmentToDelete != null)
            {
                if (appointmentToDelete.AgentId != userId && appointmentToDelete.IssuerUserId != userId)
                {
                    throw new NotAuthorizedUserException("Не сте собственик на тази среща за да е изтриете!");
                }

                appointmentToDelete.isDeleted = true;
                unitOfWork.AppointmentsRepository.Edit(appointmentToDelete);
                await unitOfWork.SaveAsync();
            }

            //Uncomment for permanent deletion
            //UnitOfWork.AppointmentsRepository.Delete(appointmentToDelete);
            //await UnitOfWork.SaveAsync();
        }


        public async Task ApproveAppointment(int appointmentId, string agentId)
        {
            var isAdmin = await userManager.IsInRoleAsync(agentId, "Administrator");

            if (agentId == null && !isAdmin) throw new ArgumentException("Изберете агент");

            var appointment = await unitOfWork.AppointmentsRepository
                .Where(a => a.Id == appointmentId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена срещата!");
            if (appointment.AgentId != agentId && !isAdmin)
            {
                throw new NotAuthorizedException("Не сте оторизиран за одобряване на среща!");
            }

            appointment.IsApprovedByAgent = true;
            unitOfWork.AppointmentsRepository.Edit(appointment);
            await unitOfWork.SaveAsync();

            //Notify Agent for the appointment
            if (ConfigurationManager.AppSettings["AppStatus"] != "Developement")
            {
                await SendEmailAndSmsNotificationToClient(appointment);
            }
        }

        public async Task ChangeAppointmentDate(int appointmentId, string agentId, DateTime newDate)
        {
            if (agentId == null)
            {
                throw new ArgumentException("Изберете агент");
            }
            var appointment = await unitOfWork.AppointmentsRepository
                                  .Where(a => a.Id == appointmentId)
                                  .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена срещата!");
            if (appointment.AgentId != agentId)
            {
                throw new NotAuthorizedException("Не сте оторизиран за одобряване на среща!");
            }

            appointment.AppointmentDate = newDate;
            unitOfWork.AppointmentsRepository.Edit(appointment);
            await unitOfWork.SaveAsync();

        }

        public async Task Edit(AppointmentEditViewModels model)
        {
            var appointmentToEdit = await unitOfWork.AppointmentsRepository
                .Where(a => a.Id == model.Id)
                .FirstOrDefaultAsync() 
                ?? throw new ContentNotFoundException("Резервацията не е намерена!");

            appointmentToEdit.AppointmentDate = model.AppointmentDate;
            appointmentToEdit.IsApprovedByAgent = model.IsApprovedByAgent;

            unitOfWork.AppointmentsRepository.Edit(appointmentToEdit);
            await unitOfWork.SaveAsync();
        }

        
        #region Helpers

        private async Task SendEmailAndSmsNotificationToClient(Appointments appointment)
        {
            var agentName = appointment.Agent.FirstName + " " + appointment.Agent.LastName;
            var clientPhoneNumber = appointment.NonRegisteredUser != null ? appointment.NonRegisteredUser.ClientPhoneNumber : appointment.IssuerUser.PhoneNumber;
            if (!string.IsNullOrEmpty(clientPhoneNumber) && !string.IsNullOrEmpty(agentName))
            {
                await smsService.SendSmsAsync(clientPhoneNumber, $"Потвърдена среща от {agentName} за {appointment.AppointmentDate:DD.MM.YYYY HH:mm}");
            }

            var clientEmail = appointment.NonRegisteredUser != null ? appointment.NonRegisteredUser.ClientEmail : appointment.IssuerUser.Email;
            if (!string.IsNullOrEmpty(clientEmail) && !string.IsNullOrEmpty(agentName))
            {
                var emailTitle = $"Срещата ви с {agentName} е потвърдена";
                var emailBody = $"<div style=\"text-align:center\"> Агент <a href=\"www.sProperties.com/Agents/Details?agentId={appointment.AgentId}\">{agentName}</a> потвърди заявката ви за среща <br/> Срещата е на {appointment.AppointmentDate:DD.MM.YYYY HH:mm} за имот <a href=\"www.sProperties.com/Properties/Details/{appointment.PropertyId}\">{appointment.PropertyId}</a>  </div>";

                await emailService.SendEmailAsync(clientEmail, emailTitle, emailBody, true);
            }
        }

        private async Task SendEmailAndSmsNotificationToAgent(string agentId, int propertyId, string clientName, string clientEmail, string clientPhoneNumber)
        {
            var agentEmailAndPhone = await unitOfWork.UsersRepository
                .Where(u => u.Id == agentId)
                .Select(u => new { u.Email, u.PhoneNumber })
                .FirstOrDefaultAsync();

            if (agentEmailAndPhone == null) return;

            //Send mail if agent has email address
            if (!string.IsNullOrEmpty(agentEmailAndPhone.Email))
            {
                var propertyInfo = await unitOfWork.PropertiesRepository
                    .Include(p => p.Address)
                    .Where(p => p.Id == propertyId)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Address = p.Address.City.CityName + ", " + p.Address.FullAddress
                    }).FirstOrDefaultAsync();

                if (propertyInfo == null) return;

                string mailTemplate = $"<div style=\"text-align:center\">Заявена среща от Клиент : <h3>{clientName}</h3> тел.:<h4>{clientPhoneNumber}</h4> е-поща:<h4>{clientEmail}</h4> Имот: <h3><a href=\"sProperties.com/Properties/Details/{propertyInfo.Id}\">{propertyInfo.Id}</a> - {propertyInfo.Address} </h3></div>";

                await emailService.SendEmailAsync(agentEmailAndPhone.Email, "Заявка за оглед на имот", mailTemplate, true);
            }
            //Send sms if agent has phoneNumber
            if (!string.IsNullOrEmpty(agentEmailAndPhone.PhoneNumber))
            {
                await smsService.SendSmsAsync(agentEmailAndPhone.PhoneNumber, $"Заявена среща от тел.{clientPhoneNumber} за имот {propertyId}");
            }
        }
        #endregion

        #region Notify for appointment

        public async Task NotifyForAppointments()
        {
            var appointmentsToBeNotified = await unitOfWork.AppointmentsRepository
                .Include(a => a.IssuerUser)
                .Where(a => a.CreatedOn.Year == DateTime.Now.Year
                            && a.CreatedOn.Month == DateTime.Now.Month
                            && a.CreatedOn.Day == DateTime.Now.Day)
                .ToListAsync();

            foreach (var appointment in appointmentsToBeNotified)
            {
                var appointmentIssuerName =
                    string.IsNullOrEmpty(appointment.IssuerUser.FirstName) &&
                    string.IsNullOrEmpty(appointment.IssuerUser.FirstName)
                        ? appointment.IssuerUser.UserName
                        : appointment.IssuerUser.FirstName + appointment.IssuerUser.LastName;

                NotificationCreateViewModel notification = new NotificationCreateViewModel
                {
                    NotificationTypeId = (int) NotificationType.Calendar,
                    NotificationPicture = null,
                    NotificationText = "Имате уговорка в " + appointment.AppointmentDate.ToString("dd.MM.yyyy hh:mm") +
                                       " с " + appointmentIssuerName + " телефон:" + appointment.IssuerUser.PhoneNumber
                };

                await notificationServices.CreateIndividualNotification(notification, appointment.AgentId);
            }
        }

        #endregion
    }
}