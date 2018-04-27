using System.Configuration;
using System.Threading.Tasks;

namespace RealEstate.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string To, string Subject, string Body, bool isBodyHtml = true);
    }
}