using System;
using System.Linq;
using AutoMapper;
using RealEstate.Model;
using RealEstate.Services.AutoMapper;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.WebAppMVC.AutoMapper
{
    public class AutoMapperControllerConfiguration
    {
        public static readonly Action<IMapperConfigurationExpression> ConfigAction = cfg =>
        {
            cfg.AddProfile(new PropertiesControllerProfile());
            cfg.AddProfile(new ExtrasControllerProfile());
            cfg.AddProfile(new ApplicationUserControllerProfile());
            cfg.AddProfile(new SearchParamsProfile());
            cfg.AddProfile(new ReservationProfile());
        };

        public static void Configure()
        {
            Mapper.Initialize(ConfigAction);
        }



    }


    public class PropertiesControllerProfile : Profile
    {
        public PropertiesControllerProfile()
        {
            CreateMap<PropertyInfoDTO, PropertySliderViewModel>()
                .ForMember(p => p.PropertyId, opt => opt.MapFrom(o => o.Id))
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Address.City.Country.CountryNameBG + ", " + s.Address.City.CityName + "(<a href=\"https://www.ekatte.com/search/node/" + s.Address.City.CityCode + "\" target=\"_blank\">" + s.Address.City.CityCode + "</a>)" + " <br/>" +
                        s.Address.FullAddress))
                //.ForMember(p => p.ImagePath,
                //    opt => opt.MapFrom(s => s.Images.Select(ss => ss.ImagePath).FirstOrDefault()))
                .ForMember(p => p.PriceDescription,
                    opt => opt.MapFrom(s => s.SellingPrice));
        }
    }


    public class ExtrasControllerProfile : Profile
    {
        public ExtrasControllerProfile()
        {
            CreateMap<Extras, ExtraCheckBoxViewModel>()
                .ForMember(e => e.IsChecked, opt => opt.UseValue(false));
        }
    }


    public class ApplicationUserControllerProfile : Profile
    {
        public ApplicationUserControllerProfile()
        {
            CreateMap<ApplicationUser, UsersIdInfoViewModel>()
                .ForMember(u => u.Info,
                    opt => opt.MapFrom(o => "Име: " + o.FirstName + " " + o.LastName + ", Телефон: " + o.PhoneNumber));
        }
    }


    public class SearchParamsProfile : Profile
    {
        public SearchParamsProfile()
        {
        }
    }


    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<NonRegisteredUserCreateDTO, NonRegisteredReservationUsers>();

            CreateMap<CreateReservationViewModel, CreateReservationDTO>()
                .ForMember(e => e.UserId, opt => opt.ResolveUsing((src, dst, s, ctx) => dst.UserId = (string)ctx.Items["UserId"]));

            CreateMap<NonRegisteredReservationUsers, CreateReservationDTO>();
            CreateMap<CreateReservationForNonRegisteredUserViewModel, CreateReservationDTO>()
                .ForMember(r => r.NonRegisteredUser, opt => opt.MapFrom(o => new NonRegisteredUserCreateDTO(o.ClientName, o.ClientEmail, o.ClientPhoneNumber)));
        }
    }
}