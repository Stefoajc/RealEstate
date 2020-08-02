using System.Threading.Tasks;
using RealEstate.Repositories.Forum.Interfaces;

namespace RealEstate.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAddressesRepository AddressesRepository { get; }
        INotificationsRepository NotificationsRepository { get; }
        IPropertiesRepository PropertiesRepository { get; }
        IRentalsRepository RentalsRepository { get; }
        IReviewsRepository ReviewsRepository { get; }
        IReservationsRepository ReservationsRepository { get; }
        IImagesRepository ImagesRepository { get; }
        ISightsRepository SightsRepository { get; }
        ICitiesRepository CitiesRepository { get; }
        IExtrasRepository ExtrasRepository { get; }
        IOwnerRegisterCodesRepository OwnerRegisterCodesRepository { get; }
        ILikesRepository LikesRepository { get; }
        IUsersRepository UsersRepository { get; }
        ISearchTrackingRepository SearchTrackingRepository { get; }
        IPropertiesBaseRepository PropertiesBaseRepository { get; }
        IAppointmentsRepository AppointmentsRepository { get; }

        //PAYMENTS
        IInvoiceRepository InvoicesRepository { get; }
        IPayedItemsRepository PayedItemsRepository { get; }

        //FORUM
        IPostsRepository PostsRepository { get; }
        ICommentsRepository CommentsRepository { get; }
        Forum.Interfaces.IReviewsRepository ForumReviewsRepository { get; }
        IForumCategoriesRepository ForumCategoriesRepository { get; }
        IThemesRepository ThemesRepository { get; }
        ITagsRepository TagsRepository { get; }

        void Save();
        Task SaveAsync();
    }
}
