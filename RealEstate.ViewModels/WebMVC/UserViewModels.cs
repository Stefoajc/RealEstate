using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RealEstate.ViewModels.WebMVC
{
    class UserViewModels
    {
    }


    public class NonRegisteredUserCreateDTO
    {
        public NonRegisteredUserCreateDTO(string clientName, string clientPhoneNumber, string clientEmail)
        {
            ClientName = clientName;
            ClientPhoneNumber = clientPhoneNumber;
            ClientEmail = clientEmail;
        }

        public NonRegisteredUserCreateDTO(AppointmentCreateViewModel model)
        {
            ClientName = model.ClientName;
            ClientPhoneNumber = model.ClientPhoneNumber;
            ClientEmail = model.ClientEmail;
        }

        public string ClientName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ClientEmail { get; set; }
    }


    public class AgentReviewCreateViewModel
    {
        [Required(ErrorMessage = "Задължително е да се въведе рейтинг точки!")]
        public int ReviewScore { get; set; }
        [Required(ErrorMessage = "Задължително е да се въведе обосновка!")]
        [StringLength(50,ErrorMessage = "Обосновете се с поне 50 символа!")]
        public string ReviewText { get; set; }

        [Required(ErrorMessage = "Задължително е да се избере Агент за който е ревюто!")]
        public string AgentId { get; set; }
    }

    public class EditUserPersonalInfoViewModel
    {
        [StringLength(50, ErrorMessage = "Максималната дължина на името е 50 символа")]
        public string FirstName { get; set; }
        [StringLength(50, ErrorMessage = "Максималната дължина на фамилията е 50 символа")]
        public string LastName { get; set; }
        [AllowHtml]
        public string AdditionalInformation { get; set; }
    }

    public class ClientReviewListViewModel
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }   
    }


    public class AppointmentAgentViewModel
    {
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class UserIdNameViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
