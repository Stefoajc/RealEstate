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

        void Save();
    }
}
