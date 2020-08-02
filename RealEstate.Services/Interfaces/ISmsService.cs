using System.Threading.Tasks;

namespace RealEstate.Services.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string To, string Message, string encoding = "utf-8");
    }
}