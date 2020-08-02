using System;
using System.ComponentModel.DataAnnotations;
using RealEstate.ViewModels.CustomDataAnnotations;

namespace RealEstate.ViewModels.WebMVC
{

    public class ClientAppointmentsViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsApprovedByAgent { get; set; }

        public PropertyInfoViewModel Property { get; set; }
        public AppointmentAgentViewModel Agent { get; set; }
    }

    public class FullCalendarEventViewModel
    {
        public string Id { get; set; }
        public long StartDateInMilliseconds { get; set; }
        public long EndDateInMilliseconds { get; set; }
        public string Title { get; set; }
    }

    public class AgentAppointmentsViewModel
    {
        public long Start { get; set; }
        public long End { get; set; }
        public string Title { get; set; }
    }

    public class AgentOwnAppointmentsViewModel
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AdditionalDescription { get; set; }
        public bool IsApprovedByAgent { get; set; }

        public string ClientName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientEmail { get; set; }

        public PropertyInfoViewModel Property { get; set; }
    }

    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Изберете имот!")]
        public int PropertyId { get; set; }
        [Required(ErrorMessage = "Изберете агент!")]
        public string AgentId { get; set; }

        [Required(ErrorMessage = "Въведете име!")]
        public string ClientName { get; set; }
        [Required(ErrorMessage = "Въведете телефонен номер!")]
        public string ClientPhoneNumber { get; set; }
        [EmailAddress]
        public string ClientEmail { get; set; }

        [Required(ErrorMessage = "Въведете дата и час!")]
        public DateTime? AppointmentDate { get; set; }
        
        public string AppointmentDescription { get; set; }

        [Required(ErrorMessage = "Празен Recaptcha код!")]
        [ValidateRecaptcha(ErrorMessage = "Невалиден Recaptcha код!")]
        public string reCaptcha { get; set; }
    }

    public class AppointmentCreateDTO
    {
        public AppointmentCreateDTO(AppointmentCreateViewModel model)
        {
            PropertyId = model.PropertyId;
            AgentId = model.AgentId;
            AppointmentDate = (DateTime)model.AppointmentDate;
            AppointmentDescription = model.AppointmentDescription;
        }

        [Required(ErrorMessage = "Изберете имот!")]
        public int PropertyId { get; set; }
        [Required(ErrorMessage = "Изберете агент!")]
        public string AgentId { get; set; }
        [Required(ErrorMessage = "Въведете дата и час!")]
        public DateTime AppointmentDate { get; set; }
        public string AppointmentDescription { get; set; }
    }

    public class AppointmentEditViewModels
    {
        public int Id { get; set; }

        public bool IsApprovedByAgent { get; set; }

        [Required(ErrorMessage = "Въведете дата и час!")]
        public DateTime AppointmentDate { get; set; }
    }


    public class AppointmentCreatePartialViewModel
    {
        public int PropertyId { get; set; }
        public string AgentId { get; set; }
    }
}