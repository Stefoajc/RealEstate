#define DEVELOPEMENT

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Ninject;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.Services.Payments;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.Payments;

namespace RealEstate.Services
{
    /// <summary>
    /// Only dayly rentals will be reserved
    /// </summary>
    public class ReservationServices : BaseService
    {
        private PaymentServices PaymentsManager { get; set; }
        private PropertiesServices PropertiesManager { get; set; }
        private IEmailService EmailManager { get; }
        private ISmsService SmsManager { get; }

        private Dictionary<PaymentStatus, string> PaymentStatusToString = new Dictionary<PaymentStatus, string>
        {
            {PaymentStatus.CaparoPayed, "Платено капаро"},
            {PaymentStatus.Pending, "Очаква плащане"},
            {PaymentStatus.FullPayed, "Платено"},
            {PaymentStatus.PassedUnpaid, "Изтекла" }
        };

        [Inject]
        public ReservationServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr
            , PropertiesServices propertiesManager, PaymentServices paymentServices
            , IEmailService emailManager, ISmsService smsManager) : base(unitOfWork, userMgr)
        {
            PropertiesManager = propertiesManager;
            PaymentsManager = paymentServices;
            EmailManager = emailManager;
            SmsManager = smsManager;
        }

        #region Reservations count

        public int GetReservationsCount(int? propertyId = null, DateTime? from = null, DateTime? to = null, List<PaymentStatus> paymentStatuses = null)
        {
            var reservationsQuery = unitOfWork.ReservationsRepository.GetAll();

            if (propertyId != null && propertyId != 0)
            {
                reservationsQuery = reservationsQuery.Where(r => r.PropertyId == propertyId);
            }
            if (from != null && to != null)
            {
                reservationsQuery = reservationsQuery.Where(res =>
                    (res.From >= from && res.From < to) || (res.To > from && res.To <= to) ||
                    (to > res.From && to <= res.To) || (from >= res.From && from < res.To));
            }

            if (paymentStatuses != null)
            {
                if (!paymentStatuses.Any())
                {
                    reservationsQuery = reservationsQuery.Where(r => paymentStatuses.Any(p => p == r.PaymentStatus));
                }
            }

            return reservationsQuery.Count();
        }


        #endregion

        #region PaymentHelpers

        public async Task<string> PayReservation(int id, bool isCaparoPayed, PaymentMethodInfoViewModel paymentMethod, string urlOk = null, string urlCancel = null)
        {
            var price = await GetReservationPrice(id, isCaparoPayed);

            var payedItem = new PayedItemViewModel
            {
                PayedItemId = id.ToString(),
                PayedItemCode = isCaparoPayed ? "ReservationCaparo" : "ReservationFull"
            };

            var paymentResult = await PaymentsManager.PaymentFacade(paymentMethod, payedItem, price, urlOk, urlCancel);


#if DEVELOPEMENT

#else 
            //Notify for payment
            var contactInfo = await GetReservationCreatorContactInformation(id);
            //IsEasyPayCode
            if (Regex.IsMatch(paymentResult, @"^[0-9]+$"))
            {

                await SmsManager.SendSmsAsync(contactInfo.PhoneNumber,
                    "Код за плащане в EasyPay: " + paymentResult + "\r\nsProperties");

                await EmailManager.SendEmailAsync(contactInfo.Email, "Код за плащане чрез EasyPay"
                    , $"<p>Здравейте,</p><p>Вашият номер за плащане в EasyPay е: {paymentResult}</p><p><br></p><p>За да извършите плащането моля посетете някой от <a href=\"https://www.easypay.bg/site/?p=offices\" target=\"_blank\" rel=\"noopener\">офисите на EasyPay</a> и заплатете по посоченият по по-горе код.</p><p><br/></p><p>Поздрави,<br>екипът на&nbsp;<strong>еТемида.</strong></p><hr><p>!&nbsp; Това е автоматично генериран е-мейл от системата на&nbsp;<strong>еТемида.<br></strong>&nbsp; &nbsp; &nbsp;Моля не отговаряйте и не изпращайте други съобщения на този е-мейл.</p>"
                    , true);
            }
            else
            {

            }
#endif


            return paymentResult;
        }

        private async Task<ContactInfoViewModel> GetReservationCreatorContactInformation(int reservationId)
        {
            ContactInfoViewModel contactInfo = await unitOfWork.ReservationsRepository
                              .Include(r => r.NonRegisteredUser, r => r.ClientUser)
                              .Where(r => r.ReservationId == reservationId)
                              .Select(r => new ContactInfoViewModel
                              {
                                  Email = r.ClientUserId == null ? r.NonRegisteredUser.ClientEmail : r.ClientUser.Email,
                                  PhoneNumber = r.ClientUserId == null ? r.NonRegisteredUser.ClientPhoneNumber : r.ClientUser.PhoneNumber
                              })
                              .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Потребителя не е намерен!");

            return contactInfo;
        }

        private async Task<decimal> GetReservationPrice(int id, bool selectCaparoPrice = false)
        {
            var reservationPriceQuery = unitOfWork.ReservationsRepository
                .Where(r => r.ReservationId == id);

            decimal reservationPrice = selectCaparoPrice
                ? await reservationPriceQuery.Select(r => r.CaparoPrice).FirstOrDefaultAsync()
                : await reservationPriceQuery.Select(r => r.FullPrice).FirstOrDefaultAsync();


            if (reservationPrice == 0.0M)
            {
                throw new ArgumentException("Проблем с резервацията, свържете се с екипът ни за помощ!");
            }

            return reservationPrice;
        }

        #endregion


        #region List Reservations

        public async Task<List<ListPropertyReservationsViewModel>> ListReservations(int propertyId)
        {
            return await unitOfWork.ReservationsRepository
                .Include(r => r.Property, r => r.Property.UnitType)
                .Where(r => r.PropertyId == propertyId)
                .Select(r => new ListPropertyReservationsViewModel
                {
                    PropertyId = r.PropertyId,
                    PropertyName = r.Property is Properties ? ((Properties)r.Property).PropertyName : ((RentalsInfo)r.Property).Property.PropertyName,
                    RentalType = r.Property.UnitType.PropertyTypeName,
                    From = r.From,
                    To = r.To
                })
                .ToListAsync();

        }

        /// <summary>
        /// List Reservations of the currentUser
        /// Client user : Reservations Made
        /// Owner user : Reservations made on his Properties
        /// Other : All Reservations on all Properties
        /// </summary>
        /// <returns></returns>
        public async Task<List<ListReservationViewModel>> ListUserReservations(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Не е въведен потребител, за който да се покажат резервациите!");
            }

            List<ListReservationViewModel> reservations;
            if (await userManager.IsInRoleAsync(userId, ClientRole))
            {
                reservations = await ListClientReservations(userId);
            }
            else if (await userManager.IsInRoleAsync(userId, OwnerRole))
            {
                reservations = await ListOwnerReservations(userId);
            }
            else if (await userManager.IsInRoleAsync(userId, AgentRole))
            {
                reservations = await ListAgentReservations(userId);
            }
            else // Happens when the user is other role
            {
                reservations = await ListAllReservations();
            }

            return reservations;
        }
        #region List Reservations Helpers

        /// <summary>
        /// List reservertion of all owner's properties
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        private async Task<List<ListReservationViewModel>> ListOwnerReservations(string ownerId)
        {
            var ownerReservations = (await unitOfWork.ReservationsRepository
                .Include(p => p.Property, p => p.Property.UnitType)
                .Select(r => new
                {
                    OwnerId = r.Property is Properties ? ((Properties)r.Property).OwnerId : ((RentalsInfo)r.Property).Property.OwnerId,
                    Property = r.Property,
                    CreatedOn = r.CreatedOn,
                    From = r.From,
                    To = r.To,
                    ReservationId = r.ReservationId,
                    CaparoPrice = r.CaparoPrice,
                    FullPrice = r.FullPrice,
                    PaymentStatus = r.PaymentStatus
                })
                .Where(r => r.OwnerId == ownerId)
                .ToListAsync())
                .Select(r => new ListReservationViewModel
                {
                    ReservationId = r.ReservationId,
                    CreatedOn = r.CreatedOn,
                    From = r.From,
                    To = r.To,
                    CaparoPrice = r.CaparoPrice,
                    FullPrice = r.FullPrice,
                    PaymentStatus = PaymentStatusToString[r.PaymentStatus],
                    Property = Mapper.Map<PropertyInfoViewModel>(r.Property, opt => opt.Items["IsForRent"] = true)
                })
                .ToList();

            return ownerReservations;

            //var ownerReservations = (await PropertiesManager
            //    .ListPropertiesAggregated(ownerId: ownerId, isShortPeriodRent: true)
            //    .Properties.Join(UnitOfWork.ReservationsRepository.GetAll(),
            //        property => property.Id,
            //        reservation => reservation.PropertyId,
            //        (property, reservation) => new { property, reservation })
            //    .Select(p => new
            //    {
            //        Property = p.property,
            //        CreatedOn = p.reservation.CreatedOn,
            //        From = p.reservation.From,
            //        To = p.reservation.To,
            //        ReservationId = p.reservation.ReservationId,
            //        CaparoPrice = p.reservation.CaparoPrice,
            //        FullPrice = p.reservation.FullPrice,
            //        PaymentStatus = p.reservation.PaymentStatus
            //    })
            //    .ToListAsync());

            //var propertyIds = ownerReservations.Select(r => r.Property.Id).ToList();
            //var images = UnitOfWork.ImagesRepository.GetAll()
            //    .OfType<PropertyImages>()
            //    .Where(i => propertyIds.Any(pId => pId == i.PropertyId))
            //    .Select(i => new { i.PropertyId, i.ImagePath });

            //var resultReservations = ownerReservations.Select(p => new ListReservationViewModel
            //{
            //    CreatedOn = p.CreatedOn,
            //    From = p.From,
            //    To = p.To,
            //    ReservationId = p.ReservationId,
            //    CaparoPrice = p.CaparoPrice,
            //    FullPrice = p.FullPrice,
            //    PaymentStatus = PaymentStatusToString[p.PaymentStatus],
            //    Property = Mapper.Map<PropertyInfoDTO, PropertyInfoViewModel>(p.Property, opt =>
            //    {
            //        opt.Items["IsForRent"] = true;
            //        opt.AfterMap((src, dst) => dst.ImagePath = images
            //            .Where(i => i.PropertyId == src.Id)
            //            .Select(i => i.ImagePath)
            //            .FirstOrDefault());
            //    })
            //})
            //    .ToList();

            //return resultReservations;
        }


        private async Task<List<ListReservationViewModel>> ListAgentReservations(string agentId)
        {



            var agentReservations = (await unitOfWork.ReservationsRepository
                    .Include(p => p.Property, p => p.Property.UnitType)
                    .ToListAsync())
                    .Select(r => new
                    {
                        AgentId = r.Property is Properties ? ((Properties)r.Property).AgentId : ((RentalsInfo)r.Property).Property.AgentId,
                        Property = r.Property,
                        CreatedOn = r.CreatedOn,
                        From = r.From,
                        To = r.To,
                        ReservationId = r.ReservationId,
                        CaparoPrice = r.CaparoPrice,
                        FullPrice = r.FullPrice,
                        PaymentStatus = r.PaymentStatus
                    })
                    .Where(r => r.AgentId == agentId)
                    .ToList()
                    .Select(r => new ListReservationViewModel
                    {
                        ReservationId = r.ReservationId,
                        CreatedOn = r.CreatedOn,
                        From = r.From,
                        To = r.To,
                        CaparoPrice = r.CaparoPrice,
                        FullPrice = r.FullPrice,
                        PaymentStatus = PaymentStatusToString[r.PaymentStatus],
                        Property = Mapper.Map<PropertyInfoViewModel>(r.Property, opt => opt.Items["IsForRent"] = true)
                    })
                    .ToList();

            return agentReservations;

            //var agentReservations = (await PropertiesManager
            //    .ListPropertiesAggregated(agentId: agentId, isShortPeriodRent: true)
            //    .Properties.Join(UnitOfWork.ReservationsRepository.GetAll(),
            //        property => property.Id,
            //        reservation => reservation.PropertyId,
            //        (property, reservation) => new { property, reservation })
            //    .Select(p => new
            //    {
            //        Property = p.property,
            //        CreatedOn = p.reservation.CreatedOn,
            //        From = p.reservation.From,
            //        To = p.reservation.To,
            //        ReservationId = p.reservation.ReservationId,
            //        CaparoPrice = p.reservation.CaparoPrice,
            //        FullPrice = p.reservation.FullPrice,
            //        PaymentStatus = p.reservation.PaymentStatus
            //    })
            //    .ToListAsync());


            //var propertyIds = agentReservations.Select(r => r.Property.Id).ToList();
            //var images = UnitOfWork.ImagesRepository.GetAll()
            //    .OfType<PropertyImages>()
            //    .Where(i => propertyIds.Any(pId => pId == i.PropertyId))
            //    .Select(i => new { i.PropertyId, i.ImagePath });

            //var resultReservations = agentReservations.Select(p => new ListReservationViewModel
            //{
            //    CreatedOn = p.CreatedOn,
            //    From = p.From,
            //    To = p.To,
            //    ReservationId = p.ReservationId,
            //    CaparoPrice = p.CaparoPrice,
            //    FullPrice = p.FullPrice,
            //    PaymentStatus = PaymentStatusToString[p.PaymentStatus],
            //    Property = Mapper.Map<PropertyInfoDTO, PropertyInfoViewModel>(p.Property, opt =>
            //    {
            //        opt.Items["IsForRent"] = true;
            //        opt.AfterMap((src, dst) => dst.ImagePath = images
            //            .Where(i => i.PropertyId == src.Id)
            //            .Select(i => i.ImagePath)
            //            .FirstOrDefault());
            //    })
            //})
            //    .ToList();

            //return resultReservations;
        }

        /// <summary>
        /// List all reservations for a client
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<List<ListReservationViewModel>> ListClientReservations(string userId)
        {
            var reservations = (await unitOfWork.ReservationsRepository
                .Include(r => r.Property, r => r.Property.UnitType)
                .Where(r => r.ClientUserId == userId)
                .Select(r => new
                {
                    Property = r.Property,
                    ReservationId = r.ReservationId,
                    CreatedOn = r.CreatedOn,
                    From = r.From,
                    To = r.To,
                    CaparoPrice = r.CaparoPrice,
                    FullPrice = r.FullPrice,
                    PaymentStatus = r.PaymentStatus
                })
                .ToListAsync())
                .Select(r => new ListReservationViewModel
                {
                    ReservationId = r.ReservationId,
                    CreatedOn = r.CreatedOn,
                    From = r.From,
                    To = r.To,
                    CaparoPrice = r.CaparoPrice,
                    FullPrice = r.FullPrice,
                    PaymentStatus = PaymentStatusToString[r.PaymentStatus],
                    Property = Mapper.Map<PropertyInfoViewModel>(r.Property, opt => opt.Items["IsForRent"] = true)
                })
                .ToList();

            return reservations;
        }

        /// <summary>
        /// List all reservations
        /// </summary>
        /// <returns></returns>
        private async Task<List<ListReservationViewModel>> ListAllReservations(int? propertyId = null)
        {
            var reservations = unitOfWork.ReservationsRepository
                .Include(r => r.Property, r => r.Property.UnitType);

            if (propertyId != null)
            {
                reservations = reservations.Where(r => r.PropertyId == propertyId);
            }


            return (await reservations
                    .Select(r => new
                    {
                        ReservationId = r.ReservationId,
                        Property = r.Property,
                        CreatedOn = r.CreatedOn,
                        From = r.From,
                        To = r.To,
                        CaparoPrice = r.CaparoPrice,
                        FullPrice = r.FullPrice,
                        r.PaymentStatus
                    })
                    .ToListAsync())
                    .Select(r => new ListReservationViewModel
                    {
                        ReservationId = r.ReservationId,
                        CreatedOn = r.CreatedOn,
                        From = r.From,
                        To = r.To,
                        CaparoPrice = r.CaparoPrice,
                        FullPrice = r.FullPrice,
                        PaymentStatus = PaymentStatusToString[r.PaymentStatus],
                        Property = Mapper.Map<PropertyInfoViewModel>(r.Property, opt => opt.Items["IsForRent"] = true)
                    }).ToList();
        }
        #endregion

        #endregion


        /// <summary>
        /// Create Reservation only for dayly available properties
        /// </summary>
        /// <param name="model"></param>
        public async Task CreateReservation(CreateReservationDTO model)
        {
            var property = await unitOfWork.PropertiesBaseRepository
                .Where(r => r.Id == model.PropertyId)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерен имота, който искате да резервирате!");

            if (!property.RentalHirePeriodType.IsTimePeriodSearchable)
            {
                throw new InvalidReservationException("Само имоти с дневни наемни цени могат да се резервират!");
            }

            if (property.RentalPrice == null)
            {
                throw new InvalidReservationException("Имотът, който искате да резервирате няма наемна цена !");
            }

            if (string.IsNullOrEmpty(model.UserId) && model.NonRegisteredUser == null)
            {
                throw new ArgumentException("Не е попълнен потребителя правещ резервацията!");
            }

            //Todal day of the reservation, floored, converted to decimal and multiplied by the rental price (ex. 7,1 days -> 7 * RentalPrice)
            decimal totalReservationPrice = (decimal)property.RentalPrice * Convert.ToDecimal(Math.Floor((model.To - model.From).TotalDays));

            Reservations reservation = new Reservations
            {
                From = model.From,
                To = model.To,
                AdditionalDescription = model.AdditionalDescription,
                PropertyId = model.PropertyId,
                ApproveStatus = ApproveStatus.Pending,
                ClientUserId = model.UserId,
                NonRegisteredUser = model.NonRegisteredUser == null ? null : new NonRegisteredReservationUsers
                {
                    ClientName = model.NonRegisteredUser.ClientName,
                    ClientEmail = model.NonRegisteredUser.ClientEmail,
                    ClientPhoneNumber = model.NonRegisteredUser.ClientPhoneNumber
                },
                PaymentStatus = PaymentStatus.Pending,
                CaparoPrice = totalReservationPrice / 10,
                FullPrice = totalReservationPrice,
            };

            unitOfWork.ReservationsRepository.Add(reservation);
            await unitOfWork.SaveAsync();
        }

        /// <summary>
        /// This should be accessed only by Maintenance or creator of reservation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        public async Task DeleteReservation(int id, string userId)
        {
            var reservation = unitOfWork.ReservationsRepository
                .Where(r => r.ReservationId == id)
                .FirstOrDefault() ?? throw new ContentNotFoundException("Не е намерена резервацията!");

            if (!await userManager.IsInRoleAsync(userId, TeamUserRole)
                && reservation.ClientUserId != userId
                && !await userManager.IsInRoleAsync(userId, OwnerRole))
            {
                throw new NotAuthorizedException("Нямате право да изтривате резервацията!");
            }

            if (reservation.From < DateTime.Now)
            {
                throw new InvalidOperationException("Нe може да изтривате резервация, която вече е минала!");
            }

            reservation.IsDeleted = true;
            unitOfWork.ReservationsRepository.Edit(reservation);
            await unitOfWork.SaveAsync();

            //Uncomment for permanent deletion
            //UnitOfWork.ReservationsRepository.Delete(reservation);
            //await UnitOfWork.SaveAsync();
        }


        public async Task<ReservationDetailsViewModel> GetReservation(int id, string userId)
        {
            var reservation = await unitOfWork.ReservationsRepository
                .Include(r => r.Property)
                .Where(r => r.ReservationId == id)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Не е намерена резервацията!");



            if (reservation.ClientUserId != userId)
            {
                throw new NotAuthorizedException("Резервацията не е на потребителят, който използвате!");
            }


            return new ReservationDetailsViewModel
            {
                PropertyId = reservation.PropertyId,
                To = reservation.To,
                From = reservation.From,
                PropertyName = reservation.Property is Properties ? ((Properties)reservation.Property).PropertyName : ((RentalsInfo)reservation.Property).Property.PropertyName,
                PropertyImage = reservation.Property is Properties ? ((Properties)reservation.Property).Images.Select(p => p.ImagePath).FirstOrDefault() : ((RentalsInfo)reservation.Property).Property.Images.Select(p => p.ImagePath).FirstOrDefault(),
                CaparoParice = reservation.CaparoPrice,
                Price = reservation.FullPrice,
                PropertyType = reservation.Property.UnitType.PropertyTypeName,
                PaymentStatus = PaymentStatusToString[reservation.PaymentStatus],
                ReservationStatus = reservation.To < DateTime.Now ? "Предстояща" : "Минала"
            };

        }

        #region Reservation status

        public async Task ClientCancel(int reservationId, string userId)
        {
            await ChangeApproveStatus(reservationId, userId, ApproveStatus.CanceledByClient);
        }

        public async Task OwnerCancel(int reservationId, string userId)
        {
            await ChangeApproveStatus(reservationId, userId, ApproveStatus.CanceledByOwner);
        }

        public async Task ApproveReservation(int reservationId, string userId)
        {
            await ChangeApproveStatus(reservationId, userId, ApproveStatus.Approved);
        }

        private async Task ChangeApproveStatus(int reservationId, string userId, ApproveStatus status)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Изберете потребител, чиято консултация ще променяте");
            }
            var reservationToEdit = unitOfWork.ReservationsRepository
                                        .Where(r => r.ReservationId == reservationId)
                                        .FirstOrDefault() ?? throw new ContentNotFoundException("Резервацията не е намерена");

            //only Client can cancel the reservation
            if (status == ApproveStatus.CanceledByClient)
            {
                if (reservationToEdit.ClientUserId != userId)
                {
                    throw new NotAuthorizedException("Нямате право да редактирате статусът на резервацията!");
                }
            }
            //Only team member can change to other statuses
            else if (!await userManager.IsInRoleAsync(userId, TeamUserRole))
            {
                throw new NotAuthorizedException("Нямате право да редактирате статусът на резервацията!");
            }

            reservationToEdit.ApproveStatus = status;

            unitOfWork.ReservationsRepository.Edit(reservationToEdit);
            unitOfWork.Save();
        }

        #endregion

        #region Payment

        /// <summary>
        /// Execute when the reservation is fully paid
        /// </summary>
        /// <param name="reservationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task PayedFully(int reservationId, string userId)
        {
            await ChangePaymentStatusAsync(reservationId, userId, PaymentStatus.FullPayed);
        }

        /// <summary>
        /// Execute when the reservation caparo is paid
        /// </summary>
        /// <param name="reservationId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task PayedCaparo(int reservationId, string userId)
        {
            await ChangePaymentStatusAsync(reservationId, userId, PaymentStatus.CaparoPayed);
        }

        private async Task ChangePaymentStatusAsync(int reservationId, string userId, PaymentStatus status)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Изберете потребител, на който принадлежи резервацията");
            }

            var reservationToEdit = await unitOfWork.ReservationsRepository
                                        .Where(r => r.ReservationId == reservationId && r.ClientUserId == userId)
                                        .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Резервацията не е намерена");

            if (reservationToEdit.PaymentStatus == PaymentStatus.FullPayed)
            {
                throw new InvalidReservationException("Не може да промените платена резервация!");
            }
            if (reservationToEdit.PaymentStatus == PaymentStatus.CaparoPayed && status == PaymentStatus.Pending)
            {
                throw new InvalidReservationException("Не може резервация с платено капаро да бъде на изчакване!");
            }

            reservationToEdit.PaymentStatus = status;
            unitOfWork.ReservationsRepository.Edit(reservationToEdit);
            await unitOfWork.SaveAsync();
        }

        #endregion
    }
}