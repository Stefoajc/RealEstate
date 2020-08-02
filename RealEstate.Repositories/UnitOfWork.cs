using System;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.Repositories.Forum;
using RealEstate.Repositories.Forum.Interfaces;
using RealEstate.Repositories.Interfaces;
using IReviewsRepository = RealEstate.Repositories.Interfaces.IReviewsRepository;

namespace RealEstate.Repositories
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly RealEstateDbContext _dbContext;

        public UnitOfWork()
        {
            _dbContext = new RealEstateDbContext();
        }

        // Add all the repository handles here
        private IAddressesRepository _addressesRepository;
        public IAddressesRepository AddressesRepository => _addressesRepository ?? (_addressesRepository = new AddressesRepository(_dbContext));

        private INotificationsRepository _notificationsRepository;
        public INotificationsRepository NotificationsRepository => _notificationsRepository ?? (_notificationsRepository = new NotificationsRepository(_dbContext));

        private IAppointmentsRepository _appointmentsRepository;
        public IAppointmentsRepository AppointmentsRepository => _appointmentsRepository ?? (_appointmentsRepository = new AppointmentsRepository(_dbContext));

        private IPropertiesBaseRepository _propertiesBaseRepository;
        public IPropertiesBaseRepository PropertiesBaseRepository => _propertiesBaseRepository ?? (_propertiesBaseRepository = new PropertiesBaseRepository(_dbContext));

        private IPropertiesRepository _propertiesRepository;
        public IPropertiesRepository PropertiesRepository => _propertiesRepository ?? (_propertiesRepository = new PropertiesRepository(_dbContext));

        private IRentalsRepository _rentalsRepository;
        public IRentalsRepository RentalsRepository => _rentalsRepository ?? (_rentalsRepository = new RentalsRepository(_dbContext));

        private IReservationsRepository _reservationsRepository;
        public IReservationsRepository ReservationsRepository => _reservationsRepository ?? (_reservationsRepository = new ReservationsRepository(_dbContext));

        private IReviewsRepository _reviewsRepository;
        public IReviewsRepository ReviewsRepository => _reviewsRepository ?? (_reviewsRepository = new ReviewsRepository(_dbContext));

        private IImagesRepository _imagesRepository;
        public IImagesRepository ImagesRepository => _imagesRepository ?? (_imagesRepository = new ImagesRepository(_dbContext));

        private ISightsRepository _sightsRepository;
        public ISightsRepository SightsRepository => _sightsRepository ?? (_sightsRepository = new SightsRepository(_dbContext));

        private ICitiesRepository _citiesRepository;
        public ICitiesRepository CitiesRepository => _citiesRepository ?? (_citiesRepository = new CitiesRepository(_dbContext));

        private IExtrasRepository _extrasRepository;
        public IExtrasRepository ExtrasRepository => _extrasRepository ?? (_extrasRepository = new ExtrasRepository(_dbContext));

        private IOwnerRegisterCodesRepository _ownerRegisterCodesRepository;
        public IOwnerRegisterCodesRepository OwnerRegisterCodesRepository => _ownerRegisterCodesRepository ?? (_ownerRegisterCodesRepository = new OwnerRegisterCodesRepository(_dbContext));

        private ILikesRepository _likesRepository;
        public ILikesRepository LikesRepository => _likesRepository ?? (_likesRepository = new LikesRepository(_dbContext));

        private IUsersRepository _usersRepository;
        public IUsersRepository UsersRepository => _usersRepository ?? (_usersRepository = new UsersRepository(_dbContext));

        private ISearchTrackingRepository _searchTrackingRepository;
        public ISearchTrackingRepository SearchTrackingRepository => _searchTrackingRepository ?? (_searchTrackingRepository = new SearchTrackingRepository(_dbContext));

        //PAYMENTS
        private IInvoiceRepository _invoicesRepository;
        public IInvoiceRepository InvoicesRepository => _invoicesRepository ?? (_invoicesRepository = new InvoicesRepository(_dbContext));

        private IPayedItemsRepository _payedItemsRepository;
        public IPayedItemsRepository PayedItemsRepository => _payedItemsRepository ?? (_payedItemsRepository = new PayedItemsRepository(_dbContext));


        //Forum
        private IForumCategoriesRepository _forumCategoriesRepository;
        public IForumCategoriesRepository ForumCategoriesRepository => _forumCategoriesRepository ?? (_forumCategoriesRepository = new ForumCategoriesRepo(_dbContext));

        private IThemesRepository _themesRepository;
        public IThemesRepository ThemesRepository => _themesRepository ?? (_themesRepository = new ThemesRepo(_dbContext));

        private IPostsRepository _postsRepository;
        public IPostsRepository PostsRepository => _postsRepository ?? (_postsRepository = new PostsRepo(_dbContext));

        private ICommentsRepository _commentsRepository;
        public ICommentsRepository CommentsRepository => _commentsRepository ?? (_commentsRepository = new CommentsRepo(_dbContext));

        private Forum.Interfaces.IReviewsRepository _forumReviewsRepository;
        public Forum.Interfaces.IReviewsRepository ForumReviewsRepository => _forumReviewsRepository ?? (_forumReviewsRepository = new ReviewsRepo(_dbContext));

        private ITagsRepository _tagsRepository;
        public ITagsRepository TagsRepository => _tagsRepository ?? (_tagsRepository = new TagsRepo(_dbContext));

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        private bool _disposed;


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
