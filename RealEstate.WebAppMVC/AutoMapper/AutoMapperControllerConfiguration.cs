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
            CreateMap<Properties, PropertySliderViewModel>()
                .ForMember(p => p.FullAddress,
                    opt => opt.MapFrom(s =>
                        s.Address.City.Country.CountryNameBG + ", \n" + s.Address.City.CityName +
                        s.Address.FullAddress))
                .ForMember(p => p.ImagePath,
                    opt => opt.MapFrom(s => s.Images.Select(ss => ss.ImagePath).FirstOrDefault()))
                .ForMember(p => p.SellingPrice,
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
}