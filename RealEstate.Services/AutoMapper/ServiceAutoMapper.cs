using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AutoMapper;
using RealEstate.Model;
using RealEstate.Model.Payment;
using RealEstate.Services.Helpers;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.Payments;

namespace RealEstate.Services.AutoMapper
{

    public class AutoMapperServiceConfiguration
    {
        public static readonly Action<IMapperConfigurationExpression> ConfigAction = cfg =>
        {
            cfg.AddProfile(new PropertiesProfile());
            cfg.AddProfile(new RentalPriceForPeriodProfile());
            cfg.AddProfile(new RentalInfoProfile());
            cfg.AddProfile(new AgentProfile());
            cfg.AddProfile(new ClientProfile());
            cfg.AddProfile(new TeamMemberProfile());
            cfg.AddProfile(new InvoiceProfile());
            cfg.AddProfile(new AddressProfile());
            cfg.AddProfile(new AppointmentProfile());
            cfg.AddProfile(new OwnersProfile());
        };

        public static void Configure()
        {
            Mapper.Initialize(ConfigAction);
        }
    }

    public class PropertiesProfile : Profile
    {
        public PropertiesProfile()
        {
            CreateMap<CreatePropertyViewModel, Properties>()
                .ForMember(p => p.PropertyName, opt => opt.MapFrom(o => o.PropertyName.Trim(' ')));

            CreateMap<Properties, PropertyBriefInfoViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(o => o.Id))
                .ForMember(p => p.PropertyName, opt => opt.MapFrom(o => o.UnitType.PropertyTypeName))
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s => s.Address.City.CityName + " " +
                        s.Address.FullAddress))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault() ?? "/Resources/noimage.png"));

            CreateMap<AttributesKeyValueViewModel, KeyValuePairs>();

            //CreateMap<Properties, PropertyInfoViewModel>()
            //    .ForMember(p => p.PropertyId, opt => opt.MapFrom(s => s.Id))
            //    .ForMember(p => p.PropertyType, opt => opt.MapFrom(s => s.UnitType.PropertyTypeName))
            //    .ForMember(p => p.Status, opt => opt.ResolveUsing(new PropertyTypeResolver()))
            //    .ForMember(p => p.IsPartlyRented, opt => opt.MapFrom(r => r.Rentals.Any()))
            //    .ForMember(p => p.FullAddress,
            //        opt => opt.MapFrom(s =>
            //            s.Address.City.Country.CountryNameBG + " " + s.Address.City.CityName + " " +
            //            s.Address.FullAddress))
            //    .ForMember(p => p.Price, opt => opt.ResolveUsing(new PropertyPriceResolver()))
            //    .ForMember(p => p.ImagePath,
            //        opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault() ?? @"/Resources/noimage.png"))
            //    .ForMember(p => p.Info, opt => opt.MapFrom(s => s.AdditionalDescription))
            //    .ForMember(p => p.ReviewsAverage, opt => opt.MapFrom(s => s.Reviews.Average(r => r.ReviewScore)))
            //    .ForMember(p => p.BottomRight, opt => opt.ResolveUsing(new PropertyBottomRightResolver()))
            //    .ForMember(p => p.BottomLeft, opt => opt.ResolveUsing(new PropertyBottomLeftResolver()));

            CreateMap<PropertyInfoDTO, PropertyInfoViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.PropertyState, opt => opt.MapFrom(s => (int)s.PropertyState))
                .ForMember(p => p.PropertyType, opt => opt.MapFrom(s => s.UnitType.PropertyTypeName))
                .ForMember(p => p.Status, opt => opt.ResolveUsing(new PropertyTypeDTOResolver()))
                .ForMember(p => p.IsPartlyRented, opt => opt.MapFrom(r => r.IsPartlyRented))
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Address.City.Country.CountryNameBG + " " + s.Address.City.CityName + " " +
                        s.Address.FullAddress))
                .ForMember(p => p.Price, opt => opt.ResolveUsing(new PropertyPriceDTOResolver()))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => /*s.Images.Select(i => i.ImagePath).FirstOrDefault() ??*/ @"/Resources/noimage.png"))
                .ForMember(p => p.Info, opt => opt.MapFrom(s => s.AdditionalDescription))
                .ForMember(p => p.BottomRight, opt => opt.MapFrom(o => o.UnitType.PropertyTypeName))
                .ForMember(p => p.BottomLeft, opt => opt.MapFrom(o => o.AreaInSquareMeters != null ? $"<li><strong>Площ: </strong>{o.AreaInSquareMeters}<sup>m2</sup></li>" : ""));


            CreateMap<Properties, PropertyInfoViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.PropertyState, opt => opt.MapFrom(s => (int)s.PropertyState))
                .ForMember(p => p.PropertyType, opt => opt.MapFrom(s => s.UnitType.PropertyTypeName))
                .ForMember(p => p.Status, opt => opt.ResolveUsing(new PropertyTypeResolver()))
                .ForMember(p => p.IsPartlyRented, opt => opt.MapFrom(r => r.Rentals.Any()))
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Address.City.Country.CountryNameBG + " " + s.Address.City.CityName + " " +
                        s.Address.FullAddress))
                .ForMember(p => p.Price, opt => opt.ResolveUsing(new PropertyPriceResolver()))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault() ?? @"/Resources/noimage.png"))
                .ForMember(p => p.Info, opt => opt.MapFrom(s => s.AdditionalDescription))
                .ForMember(p => p.ReviewsAverage, opt => opt.MapFrom(s => s.Reviews.Average(r => r.ReviewScore)))
                .ForMember(p => p.BottomRight, opt => opt.ResolveUsing(new PropertyBottomRightResolver()))
                .ForMember(p => p.BottomLeft, opt => opt.ResolveUsing(new PropertyBottomLeftResolver()));

            CreateMap<KeyValuePairs,AttributesKeyValueViewModel>()
                .ForMember(k => k.Key, opt => opt.MapFrom(a => Helpers.AttributesResolvers.AttributeKeyReverseResolver(a.Key)));

            CreateMap<Properties, ReviewsStarsPartialViewModel>()
                .ForMember(r => r.AverageScore, opt => opt.MapFrom(o => o.Reviews.Average(pr => pr.ReviewScore)))
                .ForMember(r => r.ReviewsCount, opt => opt.MapFrom(o => o.Reviews.Count()));

            CreateMap<ClientUsers, ClientReviewListViewModel>()
                .ForMember(p => p.Name, opt => opt.MapFrom(s => s.UserName))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault()));

            CreateMap<PropertyReviews, ReviewListViewModel>()
                .ForMember(p => p.CreatedOn, opt => opt.MapFrom(s => s.CreatedOn))
                .ForMember(p => p.ReviewScore, opt => opt.MapFrom(s => s.ReviewScore))
                .ForMember(p => p.ReviewText, opt => opt.MapFrom(s => s.ReviewText))
                .ForMember(p => p.ClientReviewer, opt => opt.MapFrom(s => s.User));

            CreateMap<Properties, DetailsPropertyViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.PropertyState, opt => opt.MapFrom(s => (int)s.PropertyState))
                .ForMember(p => p.PropertyType,
                    opt => opt.MapFrom(s => s.UnitType != null ? s.UnitType.PropertyTypeName : null))
                .ForMember(p => p.BedroomsCount, opt => opt.ResolveUsing(new PropertyBedroomsCountResolver()))
                .ForMember(p => p.BathroomsCount, opt => opt.ResolveUsing(new PropertyBathroomsCountResolver()))
                .ForMember(p => p.RoomsCount, opt => opt.ResolveUsing(new PropertyRoomsCountResolver()))
                .ForMember(p => p.LikesCount,
                    opt => opt.MapFrom(s => s.PropertyLikes != null ? s.PropertyLikes.Count : 0))
                .ForMember(p => p.PropertySeason,
                    opt => opt.MapFrom(s => s.PropertySeason != null ? s.PropertySeason.PropertySeasonName : null))
                .ForMember(p => p.Extras, opt => opt.MapFrom(s => s.Extras.Select(e => e.ExtraName).ToList()))
                .ForMember(p => p.Images, opt => opt.ResolveUsing(new PropertyImagesResolver()))
                .ForMember(p => p.Attributes, opt => opt.MapFrom(s => s.Attributes))
                .ForMember(p => p.Rentals, opt => opt.MapFrom(s => s.Rentals)) //Have to write RentalInfo -> RentalInfoDetails Mapper
                .ForMember(p => p.PropertyParent, opt => opt.UseValue((PropertyInfoViewModel)null))
                .ForMember(p => p.TeamUser, opt => opt.MapFrom(s => s.Agent))
                .ForMember(p => p.Owner, opt => opt.MapFrom(s => s.Owner))
                .ForMember(p => p.RentalPeriod, opt => opt.MapFrom(s => s.RentalHirePeriodType.PeriodName))
                .ForMember(p => p.ReviewsInfo, opt => opt.MapFrom(s => s.Reviews.Any() ? s : null))
                .ForMember(p => p.ClientReviews, opt => opt.MapFrom(s => s.Reviews));


            CreateMap<Properties, PropertyMapViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(o => o.Id))
                .ForMember(p => p.PropertyName, opt => opt.MapFrom(o => o.PropertyName))
                .ForMember(p => p.PropertyType,
                    opt => opt.MapFrom(s => s.UnitType != null ? s.UnitType.PropertyTypeName : null))
                .ForMember(p => p.ImagePaths, opt => opt.MapFrom(o => o.Images.Select(i => i.ImagePath).ToList()))
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Address.City.Country.CountryNameBG + " " + s.Address.City.CityName + " " +
                        s.Address.FullAddress))
                .ForMember(p => p.Latitude, opt => opt.MapFrom(o => o.Address.Coordinates.Latitude))
                .ForMember(p => p.Longitude, opt => opt.MapFrom(o => o.Address.Coordinates.Longtitude))
                .ForMember(p => p.Price, opt => opt.ResolveUsing(new PropertyMapPriceResolver()))
                .ForMember(p => p.Period, opt => opt.ResolveUsing(new PropertyMapPricePeriodResolver()));
                //.ForMember(p => p.HasRentals, opt => opt.MapFrom(o => o.PropertyRentals.Any()));

            //CreateMap<PropertyInfoDTO, PropertiesRelatedViewModel>()
            //    .ForMember(p => p.PropertyId, opt => opt.MapFrom(o => o.Id))
            //    .ForMember(p => p.PropertyName, opt => opt.MapFrom(o => o.PropertyName))
            //    .ForMember(p => p.FullAddress, opt => opt.MapFrom(s =>
            //        s.Address.City.Country.CountryNameBG + ", " + s.Address.City.CityName + " " +
            //        s.Address.FullAddress))
            //    .ForMember(p => p.Price, opt => opt.ResolveUsing(o => o.Price))
            //    .ForMember(p => p.Status, opt => opt.ResolveUsing(o => o.Status))
            //    .ForMember(p => p.ImagePath, opt => opt.MapFrom(s => s.ImagePath))
            //    .ForMember(p => p.BottomRight, opt => opt.MapFrom(o => o.BottomRight))
            //    .ForMember(p => p.BottomLeft, opt => opt.MapFrom(o => o.BottomRight));
        }

    }

    #region PropertyMapResolvers

    public class PropertyMapPriceResolver : IValueResolver<Properties, PropertyMapViewModel, decimal>
    {
        public decimal Resolve(Properties source, PropertyMapViewModel destination, decimal member, ResolutionContext context)
        {
            bool isForRent = (bool)(context.Items.ContainsKey("IsForRent") ? context.Items["IsForRent"] : false);
            if (isForRent)
            {
                return source.RentalPrice ?? 0.0M;
            }
            return source.SellingPrice ?? 0.0M;
        }
    }

    public class PropertyMapPricePeriodResolver : IValueResolver<Properties, PropertyMapViewModel, string>
    {
        public string Resolve(Properties source, PropertyMapViewModel destination, string member, ResolutionContext context)
        {
            bool isForRent = (bool)(context.Items.ContainsKey("IsForRent") ? context.Items["IsForRent"] : false);
            if (isForRent)
            {
                return source.RentalHirePeriodType.PeriodName;
            }
            return null;
        }
    }

    #endregion

    #region PropertyInfoResolvers

    public class PropertyTypeDTOResolver : IValueResolver<PropertyInfoDTO, PropertyInfoViewModel, string>
    {
        public string Resolve(PropertyInfoDTO source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            bool isForRent = (bool)(context.Items.ContainsKey("IsForRent") ? context.Items["IsForRent"] : false);
            return isForRent ? source.RentalHirePeriodType?.PeriodName ?? "Продажна" : "Продажна";
        }
    }

    public class PropertyTypeResolver : IValueResolver<Properties, PropertyInfoViewModel, string>
    {
        public string Resolve(Properties source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            bool isForRent = (bool)(context.Items.ContainsKey("IsForRent") ? context.Items["IsForRent"] : false);
            return isForRent ? source.RentalHirePeriodType.PeriodName : "Продажна";
        }
    }

    public class PropertyPriceDTOResolver : IValueResolver<PropertyInfoDTO, PropertyInfoViewModel, decimal>
    {
        public decimal Resolve(PropertyInfoDTO source, PropertyInfoViewModel destination, decimal member, ResolutionContext context)
        {
            bool isForRent = (bool)(context.Items.ContainsKey("IsForRent") ? context.Items["IsForRent"] : false);
            if (isForRent)
            {
                return source.RentalPrice ?? 0.0M;
            }
            return source.SellingPrice ?? 0.0M;
        }
    }

    public class PropertyPriceResolver : IValueResolver<Properties, PropertyInfoViewModel, decimal>
    {
        public decimal Resolve(Properties source, PropertyInfoViewModel destination, decimal member, ResolutionContext context)
        {
            bool isForRent = (bool)(context.Items.ContainsKey("IsForRent") ? context.Items["IsForRent"] : false);
            if (isForRent)
            {
                return source.RentalPrice ?? 0.0M;
            }
            return source.SellingPrice ?? 0.0M;
        }
    }

    public class PropertyBottomRightResolver : IValueResolver<Properties, PropertyInfoViewModel, string>
    {
        public string Resolve(Properties source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            return source.UnitType.PropertyTypeName;

            //if (source.SellingInfo != null)
            //{
            //    string bedrooms = source.SellingInfo.BedroomsCount.ToString();
            //    string bathroom = source.SellingInfo.BathroomsCount.ToString();
            //    return
            //        $"<li title=\"Брой спални\"><i class=\"icons icon-bedroom\"></i> {bedrooms}</li>" +
            //        $"<li title=\"Брой бани\"><i class=\"icons icon-bathroom\"></i> {bathroom}</li>";
            //}
            //else // rental info 
            //{
            //    string bedrooms = string.Join(",", source.Rentals.Select(r => r.BedsCount).Distinct().ToList());
            //    return $"<li title=\"Легла в стая\"><i class=\"icons icon-bedroom\"></i> {bedrooms}</li>";
            //}

        }
    }

    public class PropertyBottomLeftResolver : IValueResolver<Properties, PropertyInfoViewModel, string>
    {
        public string Resolve(Properties source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            return source.AreaInSquareMeters != null ?
                $"<li><strong>Площ: </strong>{source.AreaInSquareMeters}<sup>m2</sup></li>" : "";
        }
    }

    #endregion

    #region PropertyDetailsResolvers

    public class PropertyBedroomsCountResolver : IValueResolver<Properties, DetailsPropertyViewModel, string>
    {
        public string Resolve(Properties source, DetailsPropertyViewModel destination, string member, ResolutionContext context)
        {
            return source.Attributes?.Where(a => a.Key == AttributesResolvers.BedroomsCountAttribute).Select(a => a.Value).FirstOrDefault();
        }
    }

    public class PropertyBathroomsCountResolver : IValueResolver<Properties, DetailsPropertyViewModel, string>
    {
        public string Resolve(Properties source, DetailsPropertyViewModel destination, string member, ResolutionContext context)
        {
            return source.Attributes?.Where(a => a.Key == AttributesResolvers.BathroomsCountAttribute).Select(a => a.Value).FirstOrDefault();
        }
    }

    public class PropertyRoomsCountResolver : IValueResolver<Properties, DetailsPropertyViewModel, string>
    {
        public string Resolve(Properties source, DetailsPropertyViewModel destination, string member, ResolutionContext context)
        {
            return source.Attributes?.Where(a => a.Key == AttributesResolvers.RoomsCountAttribute).Select(a => a.Value).FirstOrDefault();
        }
    }

    public class PropertyImagesResolver : IValueResolver<Properties, DetailsPropertyViewModel, List<string>>
    {
        public List<string> Resolve(Properties source, DetailsPropertyViewModel destination, List<string> member, ResolutionContext context)
        {
            return source.Images?.Where(i => Math.Abs(i.ImageRatio - 1.5F) < 0.1).Select(i => i.ImagePath).ToList();
        }
    }

    #endregion



    public class RentalPriceForPeriodProfile : Profile
    {
        public RentalPriceForPeriodProfile()
        {

        }
    }


    public class RentalInfoProfile : Profile
    {
        public RentalInfoProfile()
        {
            CreateMap<RentalsInfo, PropertyInfoViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(r => r.Id))
                .ForMember(p => p.PropertyName, opt => opt.MapFrom(r => r.Property.PropertyName))
                .ForMember(p => p.PropertyType, opt => opt.MapFrom(r => r.UnitType.PropertyTypeName))
                .ForMember(p => p.Status, opt => opt.MapFrom(r => r.RentalHirePeriodType.PeriodName))
                .ForMember(p => p.IsPartlyRented, opt => opt.UseValue(false))
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Property.Address.City.Country.CountryNameBG + " " + s.Property.Address.City.CityName + " " +
                        s.Property.Address.FullAddress))
                .ForMember(p => p.Price, opt => opt.MapFrom(r => r.RentalPrice))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s =>
                        s.Property.Images.Select(i => i.ImagePath).FirstOrDefault() ?? @"/Resources/noimage.png"))
                .ForMember(p => p.Info, opt => opt.MapFrom(s => s.AdditionalDescription))
                .ForMember(p => p.ReviewsAverage, opt => opt.MapFrom(s => s.Property.Reviews.Average(r => r.ReviewScore)))
                .ForMember(p => p.IsRentalProperty, opt => opt.UseValue(true))
                .ForMember(p => p.BottomRight, opt => opt.MapFrom(r => r.UnitType.PropertyTypeName))
                .ForMember(p => p.BottomLeft, opt => opt.ResolveUsing(new RentalBottomLeftResolver()));

            CreateMap<RentalsInfo, RentalInfoDetails>()
                .ForMember(r => r.UnitType,
                    opt => opt.MapFrom(o => o.UnitType != null ? o.UnitType.PropertyTypeName : null))
                .ForMember(r => r.Price, opt => opt.MapFrom(o => o.RentalPrice))
                .ForMember(r => r.PricePeriodType,
                    opt => opt.MapFrom(o => o.RentalHirePeriodType != null ? o.RentalHirePeriodType.PeriodName : null));

            //        UnitType = r.UnitType.RentalTypeName,
            //        UnitCount = r.UnitCount,
            //        Price = r.RentalPrice,
            //        PricePeriodType = r.RentalHirePeriodType?.PeriodName,
            //        AdditionalInfo = r.AdditionalInfo,
            //        RentalAttributes = r.RentalAttributes?.Select(Mapper.Map<AttributesKeyValueViewModel>).ToList()

            CreateMap<RentalsInfo, ReviewsStarsPartialViewModel>()
                .ForMember(r => r.AverageScore, opt => opt.MapFrom(o => o.Reviews.Average(pr => pr.ReviewScore)))
                .ForMember(r => r.ReviewsCount, opt => opt.MapFrom(o => o.Reviews.Count()));

            CreateMap<RentalsInfo, DetailsPropertyViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.Attributes, opt => opt.MapFrom(s => s.Property.PropertyName))
                .ForMember(p => p.PropertyType, opt => opt.MapFrom(s => s.UnitType != null ? s.UnitType.PropertyTypeName : null))
                .ForMember(p => p.RentalPeriod, opt => opt.MapFrom(s => s.RentalHirePeriodType.PeriodName))
                .ForMember(p => p.RentalPrice, opt => opt.MapFrom(s => s.RentalPrice))
                .ForMember(p => p.SellingPrice, opt => opt.UseValue<decimal?>(null))
                .ForMember(p => p.BedroomsCount, opt => opt.ResolveUsing(new RentalBedroomsCountResolver()))
                .ForMember(p => p.BathroomsCount, opt => opt.ResolveUsing(new RentalBathroomsCountResolver()))
                .ForMember(p => p.RoomsCount, opt => opt.ResolveUsing(new RentalRoomsCountResolver()))
                .ForMember(p => p.AreaInSquareMeters, opt => opt.ResolveUsing(new RentalAreaResolver()))
                .ForMember(p => p.Views, opt => opt.MapFrom(s => s.Views))
                .ForMember(p => p.Address, opt => opt.MapFrom(s => s.Property.Address))
                .ForMember(p => p.LikesCount,opt => opt.MapFrom(s => s.Property.PropertyLikes != null ? s.Property.PropertyLikes.Count : 0))
                .ForMember(p => p.PropertySeason, opt => opt.MapFrom(s => s.Property.PropertySeason != null ? s.Property.PropertySeason.PropertySeasonName : null))
                .ForMember(p => p.Extras, opt => opt.MapFrom(s => s.Extras.Select(e => e.ExtraName).ToList()))
                .ForMember(p => p.Images, opt => opt.ResolveUsing(new RentalImagesResolver()))
                .ForMember(p => p.Attributes, opt => opt.MapFrom(s => s.Attributes))
                .ForMember(p => p.Rentals, opt => opt.UseValue(new List<PropertyInfoViewModel>())) //Have to write RentalInfo -> RentalInfoDetails Mapper
                .ForMember(p => p.PropertyParent, opt => opt.MapFrom(r => r.Property))
                .ForMember(p => p.TeamUser, opt => opt.MapFrom(s => s.Property.Agent))
                .ForMember(p => p.ReviewsInfo, opt => opt.MapFrom(s => s.Reviews.Any() ? s : null))
                .ForMember(p => p.IsRentSearch, opt => opt.UseValue(true));           
        }


        #region RentalDetailsResolvers

        private class RentalAreaResolver : IValueResolver<RentalsInfo, DetailsPropertyViewModel, int?>
        {
            public int? Resolve(RentalsInfo source, DetailsPropertyViewModel destination, int? member, ResolutionContext context)
            {
                return source.Attributes == null
                    ? null
                    : source.Attributes.Any(a => a.Key == AttributesResolvers.AreaInSquareMetersAttribute)
                        ? int.Parse(source.Attributes
                                        .Where(a => a.Key == AttributesResolvers.AreaInSquareMetersAttribute).Select(a => a.Value)
                                        .FirstOrDefault())
                        : (int?) null;
            }
        }

        private class RentalBedroomsCountResolver : IValueResolver<RentalsInfo, DetailsPropertyViewModel, string>
        {
            public string Resolve(RentalsInfo source, DetailsPropertyViewModel destination, string member, ResolutionContext context)
            {
                return source.Attributes?.Where(a => a.Key == AttributesResolvers.BedroomsCountAttribute).Select(a => a.Value).FirstOrDefault();
            }
        }

        private class RentalBathroomsCountResolver : IValueResolver<RentalsInfo, DetailsPropertyViewModel, string>
        {
            public string Resolve(RentalsInfo source, DetailsPropertyViewModel destination, string member, ResolutionContext context)
            {
                return source.Attributes?.Where(a => a.Key == AttributesResolvers.BathroomsCountAttribute).Select(a => a.Value).FirstOrDefault();
            }
        }

        private class RentalRoomsCountResolver : IValueResolver<RentalsInfo, DetailsPropertyViewModel, string>
        {
            public string Resolve(RentalsInfo source, DetailsPropertyViewModel destination, string member, ResolutionContext context)
            {
                return source.Attributes?.Where(a => a.Key == AttributesResolvers.RoomsCountAttribute).Select(a => a.Value).FirstOrDefault();
            }
        }

        private class RentalImagesResolver : IValueResolver<RentalsInfo, DetailsPropertyViewModel, List<string>>
        {
            public List<string> Resolve(RentalsInfo source, DetailsPropertyViewModel destination, List<string> member, ResolutionContext context)
            {
                return source.Property.Images?.Where(i => Math.Abs(i.ImageRatio - 1.5F) < 0.1).Select(i => i.ImagePath).ToList();
            }
        }

        private class RentalBottomLeftResolver : IValueResolver<RentalsInfo, PropertyInfoViewModel, string>
        {
            public string Resolve(RentalsInfo source, PropertyInfoViewModel destination, string member, ResolutionContext context)
            {
                var rentalArea = source.Attributes
                    .Where(a => a.Key == "Area")
                    .Select(a => a.Value)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(rentalArea))
                {
                    return "";
                }

                return
                    $"<li><strong>Площ: </strong>{rentalArea}<sup>m2</sup></li>";
            }
        }

        #endregion



    }

    public class AgentProfile : Profile
    {

        public AgentProfile()
        {
            CreateMap<ApplicationUser, TeamUserListViewModel>()
                .ForMember(p => p.AgentId,
                    opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.FullName,
                    opt => opt.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(p => p.OfficePhone,
                    opt => opt.UseValue(ConfigurationManager.AppSettings.Get("OfficePhone")))
                .ForMember(p => p.ReviewsInfo, opt => opt.Ignore())
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault() ?? "/Resources/no-image-agent.jpg"));


            CreateMap<AgentUsers, AppointmentAgentViewModel>()
                .ForMember(p => p.AgentId, opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.AgentName, opt => opt.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(p => p.PhoneNumber, opt => opt.MapFrom(s => s.PhoneNumber))
                .ForMember(p => p.Email, opt => opt.MapFrom(s => s.Email));
        }

    }

    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<ClientUsers, ClientListViewModel>()
                .ForMember(p => p.FullName,
                    opt => opt.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault() ?? "/Resources/no-image-person.png"));
        }
    }


    public class TeamMemberProfile : Profile
    {
        public TeamMemberProfile()
        {

            CreateMap<SocialMediaAccounts, SocialMediaAccountViewModel>();

            CreateMap<ApplicationUser, TeamMemberViewModel>()
                .ForMember(u => u.Name, opt => opt.MapFrom(o => o.FirstName + " " + o.LastName))
                .ForMember(u => u.AdditionalDescription, opt => opt.MapFrom(o => o.AdditionalDescription ?? "Този потребител още не е въвел допълнителна информация."))
                .ForMember(u => u.FacebookAccount, opt => opt.MapFrom(o => o.SocialMediaAccounts.Where(s => s.SocialMedia == "Facebook").Select(s => s.SocialMediaAccount).FirstOrDefault()))
                .ForMember(u => u.TwitterAccount, opt => opt.MapFrom(o => o.SocialMediaAccounts.Where(s => s.SocialMedia == "Twitter").Select(s => s.SocialMediaAccount).FirstOrDefault()))
                .ForMember(u => u.SkypeAccount, opt => opt.MapFrom(o => o.SocialMediaAccounts.Where(s => s.SocialMedia == "Skype").Select(s => s.SocialMediaAccount).FirstOrDefault()))
                .ForMember(u => u.ImageSrc, opt => opt.MapFrom(o => o.Images.Select(i => i.ImagePath).FirstOrDefault() ?? "/Resources/no-image-agent.jpg"));

            //ImagePath = o.Images.Select(i => i.ImagePath).FirstOrDefault(),
            //Email = o.Email,
            //AgentId = o.Id,
            //PhoneNumber = o.PhoneNumber,
            //Description = o.AdditionalDescription,
            //FullName = o.FirstName + " " + o.LastName,
            //OfficePhone = "359876717000"

            //Id = u.Id,
            //Name = u.FirstName + " " + u.LastName,
            //AdditionalDescription = u.Description,
            //Role = "",
            //ImageSrc = u.Images.Select(i => i.ImagePath).FirstOrDefault(),
            //FacebookAccount = "",
            //SkypeAccount = "",
            //TwitterAccount = ""
        }
    }

    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<ePayQueryViewModel, Invoices>();
        }
    }

    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Addresses, DetailsAddressViewModel>()
                .ForMember(a => a.FullAddress,
                    opt => opt.MapFrom(
                        o => o.City.Country.CountryNameBG + ", " + o.City.CityName + ", " + o.FullAddress))
                .ForMember(a => a.Latitude,
                    opt => opt.MapFrom(o => o.Coordinates == null ? (double?)null : o.Coordinates.Latitude))
                .ForMember(a => a.Longitude,
                    opt => opt.MapFrom(o => o.Coordinates == null ? (double?)null : o.Coordinates.Longtitude));
        }
    }


    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<AppointmentCreateViewModel, Appointments>();
        }
    }

    public class OwnersProfile : Profile
    {
        public OwnersProfile()
        {
            CreateMap<OwnerUsers, OwnerViewModel>()
                .ForMember(o => o.Name, opt => opt.MapFrom(map => map.FirstName + " " + map.LastName))
                .ForMember(o => o.Email, opt => opt.MapFrom(map => map.Email))
                .ForMember(o => o.PhoneNumber, opt => opt.MapFrom(map => map.PhoneNumber))
                .ForMember(o => o.ImagePath,
                    opt => opt.MapFrom(map => map.Images.Select(i => i.ImagePath).FirstOrDefault() ?? "/Resources/no-image-agent.jpg"));
        }
    }
}