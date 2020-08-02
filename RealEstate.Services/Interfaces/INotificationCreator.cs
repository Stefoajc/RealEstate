using System.Threading.Tasks;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services.Interfaces
{
    public interface INotificationCreator
    {
        Task CreateIndividualNotification(NotificationCreateViewModel model, string userToNotify);
        Task CreateGlobalNotification(NotificationCreateViewModel model, string userCreatorId);
    }
}