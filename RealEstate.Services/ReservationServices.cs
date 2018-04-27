using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Exceptions;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ReservationServices : BaseService
    {
        public ReservationServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }


        public int GetReservationsCount(DateTime from, DateTime to)
        {
            var count = UnitOfWork.ReservationsRepository
                .GetAll()
                .Count(res =>
                    (res.From >= from && res.From < to) || (res.To > from && res.To <= to) ||
                    (to > res.From && to <= res.To) || (from >= res.From && from < res.To));

            return count;
        }

        public int GetReservationsCount(int propertyId, DateTime from, DateTime to)
        {
            var count = UnitOfWork.ReservationsRepository
                .FindBy(r => r.Rental.PropertyId == propertyId)
                .Count(res =>
                (res.From >= from && res.From < to) || (res.To > from && res.To <= to) ||
                (to > res.From && to <= res.To) || (from >= res.From && from < res.To));

            return count;
        }

        public int GetReservationsCount(int propertyId)
        {
            var count = UnitOfWork.ReservationsRepository
                .Include(r => r.Rental)
                .Count(r => r.Rental.PropertyId == propertyId);

            return count;
        }

        public int GetAllSuccessFullyPassedReservationsCount()
        {
            var count = UnitOfWork.ReservationsRepository
                .FindBy(r => (r.PaymentStatus == PaymentStatus.FullPayed || r.PaymentStatus == PaymentStatus.CaparoPayed)
                             && r.To <= DateTime.Now)
                             .Count();

            return count;
        }

        /// <summary>
        /// List Reservations of the currentUser
        /// Client user : Reservations Made
        /// Owner user : Reservations made on his Properties
        /// Other : All Reservations on all Properties
        /// </summary>
        /// <returns></returns>
        public List<ListReservationViewModel> ListCurrentUserReservetions()
        {
            var userId = User.Identity.GetUserId();

            List<ListReservationViewModel> reservations;
            if (User.IsInRole("Client"))
            {
                reservations = ListClientReservations(userId);
            }
            else if (User.IsInRole("Owner"))
            {
                reservations = ListOwnerReservations(userId);
            }
            else // Happens when the user is in Agent or Maintenance Role
            {
                reservations = ListAllReservations();
            }


            return reservations;
        }
        #region ListOwnReservations

        /// <summary>
        /// List reservertion of all owner's properties
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<ListReservationViewModel> ListOwnerReservations(string userId)
        {
            var reservations = UnitOfWork.PropertiesRepository
                .Include(p => p.Rentals, p => p.Rentals.Select(r => r.Reservations))
                .Where(p => p.OwnerId == userId)
                .Join(UnitOfWork.RentalsRepository.GetAll(),
                p => p.PropertyId,
                r => r.PropertyId,
                (p, r) => new { PropertyName = p.PropertyName, RentalId = r.RentalId })
                .Join(UnitOfWork.ReservationsRepository.GetAll(),
                p_r => p_r.RentalId,
                res => res.RentalId,
                (p_r, res) => new { PropertiesRentals = p_r, Reservations = res })
                .Select(p => new ListReservationViewModel
                {
                    PropertyName = p.PropertiesRentals.PropertyName,
                    From = p.Reservations.From,
                    To = p.Reservations.To,
                    ReservationId = p.Reservations.ReservationId
                })
                .ToList();

            return reservations;
        }

        /// <summary>
        /// List all reservations for a client
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<ListReservationViewModel> ListClientReservations(string userId)
        {
            var reservations = UnitOfWork.ReservationsRepository
                .Include(r => r.Rental, r => r.Rental.Property)
                .Where(r => r.ClientUserId == userId)
                .Select(r => new ListReservationViewModel
                {
                    ReservationId = r.ReservationId,
                    PropertyName = r.Rental.Property.PropertyName,
                    From = r.From,
                    To = r.To
                })
                .ToList();

            return reservations;
        }

        /// <summary>
        /// List all reservations
        /// </summary>
        /// <returns></returns>
        private List<ListReservationViewModel> ListAllReservations()
        {
            var reservations = UnitOfWork.ReservationsRepository
                .Include(r => r.Rental, r => r.Rental.Property)
                .Select(r => new ListReservationViewModel
                {
                    ReservationId = r.ReservationId,
                    PropertyName = r.Rental.Property.PropertyName,
                    From = r.From,
                    To = r.To
                })
                .ToList();

            return reservations;
        }
        #endregion


        public void CreateReservation(CreateReservationViewModel model)
        {
            var rentalToReserve = UnitOfWork.RentalsRepository
                .FindBy(r => r.RentalId == model.RentalId)
                .Select(r => r.RentalPrice)?
                .FirstOrDefault() ?? throw new ContentNotFoundException("SellingPrice is not set for this period of that Rental");

            Reservations reservation = new Reservations
            {
                From = model.From,
                To = model.To,
                RentalId = model.RentalId,
                ApproveStatus = ApproveStatus.Pending,
                ClientUserId = User.Identity.GetUserId(),
                PaymentStatus = PaymentStatus.Pending,
                CaparoPrice = rentalToReserve / 10,
                FullPrice = rentalToReserve,
            };


            UnitOfWork.ReservationsRepository.Add(reservation);
            UnitOfWork.Save();
        }

        /// <summary>
        /// This can be accessed only by Maintenance
        /// </summary>
        /// <param name="id"></param>
        public void DeleteReservation(int id)
        {
            var reservation = UnitOfWork.ReservationsRepository.FindBy(r => r.ReservationId == id).FirstOrDefault();

            UnitOfWork.ReservationsRepository.Delete(reservation);
            UnitOfWork.Save();
        }

        public void ChangeApproveStatus(int reservationId, ApproveStatus status)
        {
            var userId = User.Identity.GetUserId();

            var reservationToEdit = UnitOfWork.ReservationsRepository
                .FindBy(r => r.ReservationId == reservationId && r.ClientUserId == userId)
                .FirstOrDefault() ?? throw new ContentNotFoundException("Reservation not found");

            reservationToEdit.ApproveStatus = status;

            UnitOfWork.ReservationsRepository.Edit(reservationToEdit);
            UnitOfWork.Save();
        }

        public void ChangePaymentStatus(int reservationId, PaymentStatus status)
        {
            var userId = User.Identity.GetUserId();

            var reservationToEdit = UnitOfWork.ReservationsRepository
                                        .FindBy(r => r.ReservationId == reservationId && r.ClientUserId == userId)
                                        .FirstOrDefault() ?? throw new ContentNotFoundException("Reservation not found");

            reservationToEdit.PaymentStatus = status;

            UnitOfWork.ReservationsRepository.Edit(reservationToEdit);
            UnitOfWork.Save();
        }


    }
}