using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class RentalInfoServices : BaseService
    {
        private readonly ExtraServices _extrasManager;

        public RentalInfoServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr, ExtraServices extrasManager) : base(unitOfWork, user, userMgr)
        {
            _extrasManager = extrasManager;
        }

        public ISet<RentalsInfo> AddRentalInfo(List<CreateRentalInfoViewModel> rentalInfoCreateModels)
        {
            ISet<RentalsInfo> rentals = new HashSet<RentalsInfo>();
            foreach (var rentalInfoCreateModel in rentalInfoCreateModels)
            {
                var rentalInfoToAdd = new RentalsInfo()
                {
                    UnitTypeId = rentalInfoCreateModel.UnitTypeId,
                    AdditionalInfo = rentalInfoCreateModel.AdditionalInfo
                };

                //rentalInfoToAdd.RentalPrices = new HashSet<RentalPrice>(_rentalPeriodPricesManager.AddRentalPriceForPeriod(rentalInfoCreateModel.RentalPrices));
                rentalInfoToAdd.RentalExtras = new HashSet<RentalExtras>(_extrasManager.AddRentalExtras(rentalInfoCreateModel.RentalExtras.Where(e => e.IsChecked).Select(e => e.ExtraId).ToList()));

                rentals.Add(rentalInfoToAdd);
            }

            return rentals;
        }


        public void EditRentalInfo(IEnumerable<EditRentalInfoForPropertyViewModel> rentalInfoesToEdit)
        {
            foreach (EditRentalInfoForPropertyViewModel rentalInfoModel in rentalInfoesToEdit)
            {
                var rentalInfoToEdit = UnitOfWork.RentalsRepository
                    .FindBy(r => r.RentalId == rentalInfoModel.RentalInfoId)
                    .FirstOrDefault();

                if (rentalInfoToEdit != null)
                {
                    rentalInfoToEdit.UnitTypeId = rentalInfoModel.RentalTypeId;
                    rentalInfoToEdit.AdditionalInfo = rentalInfoModel.AdditionalInfo;

                    //Add New Extras
                    rentalInfoToEdit.RentalExtras.Clear();
                    rentalInfoToEdit.RentalExtras = new HashSet<RentalExtras>(_extrasManager.AddRentalExtras(rentalInfoModel.RentalExtrasIds));

                    //Add New PriceForPeriod
                    //rentalInfoToEdit.RentalPrices.Clear();
                    //rentalInfoToEdit.RentalPrices = new HashSet<RentalPrice>(
                    //    _rentalPeriodPricesManager.AddRentalPriceForPeriod(
                    //        rentalInfoModel.RentalPrices));

                    UnitOfWork.RentalsRepository.Edit(rentalInfoToEdit);
                }
            }
        }


        public void DeleteRentalInfo(IEnumerable<RentalsInfo> rentalsInfoToRemove)
        {
            //Deleting the rentalInfos which no longer exist in the collection
            foreach (var rentalInfo in rentalsInfoToRemove)
            {
                UnitOfWork.RentalsRepository.Delete(rentalInfo);
            }
        }


        public List<RentalTypeDropDownViewModel> GetRentalTypesForDropDown()
        {
            return UnitOfWork.RentalsRepository
                .GetRentalTypes()
                .Select(rt => new RentalTypeDropDownViewModel
                {
                    UnitTypeId = (int)rt.RentalTypeId,
                    RentalTypeName = rt.RentalTypeName
                }).ToList();
        }

        public List<RentalPeriodDropDownViewModel> GetRentalPeriods()
        {
            var rentalPeriodList = new List<RentalPeriodDropDownViewModel>
            {
                new RentalPeriodDropDownViewModel
                {
                    PeriodId = (int) RentalPeriod.Dayly,
                    Period = "Ден"
                },
                new RentalPeriodDropDownViewModel
                {
                    PeriodId = (int) RentalPeriod.Monthly,
                    Period = "Месец"
                },
                new RentalPeriodDropDownViewModel
                {
                    PeriodId = (int) RentalPeriod.PerBedDaily,
                    Period = "За легло на ден"
                },
                new RentalPeriodDropDownViewModel
                {
                    PeriodId = (int) RentalPeriod.PerPersonDaily,
                    Period = "За човек на ден"
                },
                new RentalPeriodDropDownViewModel
                {
                    PeriodId = (int) RentalPeriod.Weekly,
                    Period = "Седмица"
                },
                new RentalPeriodDropDownViewModel
                {
                    PeriodId = (int) RentalPeriod.Yearly,
                    Period = "Година"
                }
            };

            return rentalPeriodList;
        }


        public List<RentalInfoBedsRoomsAvailableViewModel> GetFreeRentalsByBedsCount(int propertyId, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
            //var propertyRentalsInPeriod = UnitOfWork.RentalsRepository
            //    .Include(r => r.Reservations, r => r.Property)
            //    .Where(r => r.PropertyId == propertyId)
            //    .Select(r => new RentalInfoBedsRoomsAvailableViewModel
            //    {
            //        BedsCount = r.BedsCount,
            //        AvailableRooms = r.RoomCount - r.Reservations.Count(res => (res.From >= from && res.From < to) || (res.To > from && res.To <= to) || (to > res.From && to <= res.To) || (from >=res.From && from < res.To))
            //    })
            //    .ToList();

            //return propertyRentalsInPeriod;
        }



    }
}