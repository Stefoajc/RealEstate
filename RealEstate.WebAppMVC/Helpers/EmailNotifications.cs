using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using RealEstate.Services;

namespace RealEstate.WebAppMVC.Helpers
{
    public static class EmailNotifications
    {
        public static NoReplyMailService NoReplyMailService { get; set; } = new NoReplyMailService();
        public static OfficeMailService OfficeMailService { get; set; } = new OfficeMailService();

        public static async Task RegiteredUserNotification(string userName, string userMail, string userPhoneNumber)
        {
            string titleTemplate = $"Клиент с потербителско име {userName} се регистрира!";
            string bodyTempalate = $"<h1 align=\"center\">Потребител беше регистриран на {DateTime.Now.ToString("dd.MM.yyyy HH:MM")} </h1> " +
                                   $"<div>Потребителско име:{userName}<div>" +
                                   $"<div>Email адрес: {userMail ?? "Не е зададен" }<div> " +
                                   $"<div>Телефонен номер:{userPhoneNumber ?? "Няма"}<div>";

            //Notification on company email
            await OfficeMailNotification(titleTemplate, bodyTempalate);

            string userMailTitle = "Регистрацията ви в \"sproperties.net\" е успешна!";            
            string userMailBody = $"Успешна регистрация<br> Може да влезете в системата от <a href=\"https://www.sproperties.net/Account/Login\">тук.</a>";

            await NoReplyMailService.SendHtmlEmailAsync(userMail, userMailTitle, userMailBody, true);
        }

        private static async Task OfficeMailNotification(string title, string body)
        {
            await NoReplyMailService.SendHtmlEmailAsync(ConfigurationManager.AppSettings["OfficeEmail"], title, body, true);
        }
    }
}