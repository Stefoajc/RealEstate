using System.Configuration;
using System.Threading.Tasks;

namespace RealEstate.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendHtmlEmailAsync(string to, string subject, string body);
        Task SendPlainTextEmailAsync(string to, string subject, string body);
    }
}