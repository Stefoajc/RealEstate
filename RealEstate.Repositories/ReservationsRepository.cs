using System;
using System.Collections.Generic;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public class ReservationsRepository : GenericRepository<RealEstateDbContext, Reservations>, IReservationsRepository
    {
        public ReservationsRepository(RealEstateDbContext db) : base(db)
        {
        }

        public List<Reservations> GetOwnerReservations(string ownerId)
        {
            //var reservation = Context.Database.SqlQuery()
            throw new NotImplementedException();


            //            SELECT r.ReservationId,
            //	 r.CreatedOn, 

            //     [From],
            //	 [To], 
            //	 r.CaparoPrice, 
            //	 r.PaymentStatus, 
            //	 p.Id, 
            //	 CAST(
            //        CASE
            //            WHEN p.Discriminator = 'Properties'
            //                THEN p.PropertyName
            //            ELSE pp.PropertyName
            //        END AS nvarchar(255)) as PropertyName,
            //	 pt.PropertyTypeName,
            //	 p.Views,
            //	 CAST(
            //        CASE
            //            WHEN p.Discriminator = 'Properties'
            //                THEN rh.PeriodName
            //            ELSE rhrh.PeriodName
            //        END AS nvarchar(255)) as [STATUS],
            //	 p.RentalPrice,
            //	 CAST(
            //        CASE
            //            WHEN p.Discriminator = 'Properties'
            //                THEN i.ImagePath
            //            ELSE ii.ImagePath
            //        END AS nvarchar(255)) as ImagePath,
            //	 p.AdditionalDescription,
            //	 CAST(
            //        CASE
            //            WHEN p.Discriminator = 'Properties'
            //                THEN c.CityName
            //            ELSE cc.CityName
            //        END AS nvarchar(255)) as CityName,
            //	CAST(
            //        CASE
            //            WHEN p.Discriminator = 'Properties'

            //                THEN a.FullAddress
            //            ELSE aa.FullAddress
            //        END AS nvarchar(255)) as PropertyName,
            //	 p.AreaInSquareMeters,
            //	 'BottomLeft',
            //	 'BottomRight',
            //	 'IsPartlyRented',
            //	 CAST(
            //             CASE
            //                  WHEN p.Discriminator = 'Properties'
            //                     THEN 0 
            //                  ELSE 1
            //             END AS bit) as IsRentalProperty,
            //	 1 as IsRentable,
            //	 CAST(
            //        CASE
            //            WHEN p.Discriminator = 'Properties'
            //                THEN AVG(rew.ReviewScore)
            //            ELSE AVG(rewrew.ReviewScore)
            //        END AS float) as ReviewScore,
            //	 p.CreatedOn
            //FROM Reservations r
            //JOIN PropertiesBases p ON r.PropertyId = p.Id
            //LEFT JOIN AspNetUsers u ON p.AgentId = u.Id
            //LEFT JOIN Addresses a ON a.AddressId = p.AddressId
            //LEFT JOIN Cities c ON c.CityId = a.CityId
            //LEFT JOIN RentalHirePeriodsTypes rh ON rh.Id = p.RentalPricePeriodId
            //LEFT JOIN Images i ON i.PropertyId = p.Id
            //LEFT JOIN PropertyReviews pr ON pr.PropertyId = p.Id
            //LEFT JOIN Reviews rew ON rew.ReviewId = pr.ReviewId

            //LEFT JOIN PropertiesBases pp ON p.PropertyId = pp.Id
            //LEFT JOIN AspNetUsers uu ON pp.AgentId = uu.Id
            //LEFT JOIN Addresses aa ON aa.AddressId = pp.AddressId
            //LEFT JOIN Cities cc ON cc.CityId = aa.CityId
            //LEFT JOIN RentalHirePeriodsTypes rhrh ON rhrh.Id = pp.RentalPricePeriodId
            //LEFT JOIN Images ii ON ii.PropertyId = pp.Id
            //LEFT JOIN PropertyReviews prpr ON prpr.PropertyId = pp.Id
            //LEFT JOIN Reviews rewrew ON rewrew.ReviewId = prpr.ReviewId

            //LEFT JOIN PropertyTypes pt ON p.UnitTypeId = pt.PropertyTypeId
            //WHERE CAST(CASE
            //    WHEN p.Discriminator = 'Properties'

            //        THEN u.Id
            //    ELSE uu.Id
            //    END as nvarchar(255)) = 'b7e8c336-667a-495b-99f0-06fe3b55c07b'

        }
    }
}
