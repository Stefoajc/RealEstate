using System;
using System.Configuration;
using System.Linq;
using AutoMapper;
using RealEstate.Model;
using RealEstate.ViewModels.WebMVC;

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
            CreateMap<CreatePropertyViewModel, Properties>();
            CreateMap<Properties, PropertyBriefInfoViewModel>()
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Address.City.Country.CountryNameBG + " " + s.Address.City.CityName + " " +
                        s.Address.FullAddress))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault()));

            CreateMap<AttributesKeyValueViewModel, KeyValuePairs>();

            //PropertyId = p.PropertyId,
            //PropertyType = p.PropertyType.PropertyTypeName,
            //Status = "Sell",
            //Views = p.Views,
            //PropertyName = p.PropertyName,
            //FullAddress = p.Address.City.Country.CountryNameBG + " " + p.Address.City.CityName + " " + p.Address.FullAddress,
            //Price = p.SellingInfo.Select(s => s.SellingPrice).FirstOrDefault() ?? p.Rentals.Select(r => r.RentalPrices.Average(r => r.Price)).FirstOrDefault(),
            //ImagePath = p.Images.Select(i => i.ImagePath).FirstOrDefault(),
            //Info = p.AdditionalDescription,
            //BathroomsCount = p.SellingInfo.Select(s => s.BathroomsCount).FirstOrDefault(), //Figure it out
            //AreaInSquareFt = p.SellingInfo.Select(s => s.AreaInSquareFt).FirstOrDefault(),
            //BedroomsCount = p.SellingInfo.Select(s => s.BedroomsCount).FirstOrDefault() // Figure it out

            CreateMap<Properties, PropertyInfoViewModel>()
                .ForMember(p => p.PropertyType, opt => opt.MapFrom(s => s.PropertyType.PropertyTypeName))
                .ForMember(p => p.Status, opt => opt.ResolveUsing(new PropertyTypeResolver()))
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Address.City.Country.CountryNameBG + " " + s.Address.City.CityName + " " +
                        s.Address.FullAddress))
                .ForMember(p => p.Price, opt => opt.ResolveUsing(new PropertyPriceResolver()))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault()))
                .ForMember(p => p.Info, opt => opt.MapFrom(s => s.AdditionalDescription))
                .ForMember(p => p.BottomRight, opt => opt.ResolveUsing(new PropertyBottomRightResolver()))
                .ForMember(p => p.BottomLeft, opt => opt.ResolveUsing(new PropertyBottomLeftResolver()));

        }

    }

    #region PropertyInfoResolvers

    public class PropertyTypeResolver : IValueResolver<Properties, PropertyInfoViewModel, string>
    {
        public string Resolve(Properties source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            return source.SellingPrice != null ? "Sell" : "Rent";
        }
    }

    public class PropertyPriceResolver : IValueResolver<Properties, PropertyInfoViewModel, decimal>
    {
        public decimal Resolve(Properties source, PropertyInfoViewModel destination, decimal member, ResolutionContext context)
        {
            return source.SellingPrice ?? source.Rentals.Average(ss => ss.RentalPrice);
        }
    }

    public class PropertyBathroomsResolver : IValueResolver<Properties, PropertyInfoViewModel, string>
    {
        public string Resolve(Properties source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            return "";
        }
    }

    public class PropertyBedroomsResolver : IValueResolver<Properties, PropertyInfoViewModel, string>
    {
        public string Resolve(Properties source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            return "";
        }
    }

    public class PropertyBottomRightResolver : IValueResolver<Properties, PropertyInfoViewModel, string>
    {
        public string Resolve(Properties source, PropertyInfoViewModel destination, string member, ResolutionContext context)
        {
            return source.PropertyType.PropertyTypeName;

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
            return
                $"<li><strong>Площ:</strong> {source.AreaInSquareFt}<sup>m2</sup></li>";
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

        }
    }

    public class AgentProfile : Profile
    {

        //AgentId = a.Id,
        //FullName = a.FirstName + " " + a.LastName,
        //Description = a.Description,
        //PhoneNumber = a.PhoneNumber,
        //OfficePhone = ConfigurationManager.AppSettings.Get("OfficePhone"),
        //Email = a.Email,
        //ImagePath = a.Images.FirstOrDefault()?.ImagePath

        public AgentProfile()
        {
            CreateMap<AgentUsers, AgentListViewModel>()
                .ForMember(p => p.AgentId,
                    opt => opt.MapFrom(s => s.Id))
                .ForMember(p => p.FullName,
                    opt => opt.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(p => p.OfficePhone,
                    opt => opt.UseValue(ConfigurationManager.AppSettings.Get("OfficePhone")))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault()));

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
                    opt => opt.MapFrom(s => s.Images.Select(i => i.ImagePath).FirstOrDefault()));
        }
    }
}