using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using Ninject;
using RealEstate.Extentions;
using RealEstate.Model;
using RealEstate.Services;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.WebAppMVC.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly string all = "Всички";
        private readonly string[] _propertyTypes = { "Хотели", "Къща", "Мотел", "Станция", "Офис сграда", "Вила", "Паркинг", "Гараж", "Земя", "Търговски център", "Сграда", "Склад", "Стаи", "Друго" };
        private readonly PropertiesServices _propertyManager;
        private readonly CityServices _cityManager;
        private readonly RentalInfoServices _rentalInfoManager;
        private readonly ExtraServices _extrasManager;
        private readonly UserServices _userServices;
        private readonly ClientServices _clientsManager;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        [Inject]
        public PropertiesController(ExtraServices extraServices, PropertiesServices propertyServices, ClientServices clientServices, CityServices cityServices, RentalInfoServices rentalInfoServices, UserServices userServices)
        {
            _propertyManager = propertyServices;
            _cityManager = cityServices;
            _rentalInfoManager = rentalInfoServices;
            _extrasManager = extraServices;
            _userServices = userServices;
            _clientsManager = clientServices;
        }

        [AllowAnonymous]
        //GET: Properties
        public ActionResult Index()
        {
            ViewBag.SliderProperties = _propertyManager.GetHighestRatedProperties(3)
                .AsEnumerable()
                .Select(p => Mapper.Map<PropertySliderViewModel>(p)).ToList();

            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.RelatedProperties = _propertyManager.GetRelatedPropertiesBriefInfo();

            ViewBag.PropertiesAll = _propertyManager.GetPropertiesForSell().Properties.Take(6).ToList();
            ViewBag.HappyClient = _clientsManager.GetHappyClients();

            //Agents List
            ViewBag.Agents = _userServices.GetTopTwoAgents();
            return View();
        }

        public ActionResult ListProperties(SearchSellViewModel model)
        {
            //Setup filters
            ViewBag.PropertiesTitle = model.propertyType == null ? all : _propertyTypes[(int)model.propertyType];
            ViewBag.SearchParams = model;
            //---- Setup End -----

            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: model.propertyType, cityId: model.cityId);

            var properties = _propertyManager.GetPropertiesForSell(model.priceFrom, model.priceTo, model.propertyType, model.cityId, model.distanceFromCity, model.pageCount, model.pageSize, model.sortBy, model.orderBy);

            //Pagination
            var propCount = properties.Count;
            ViewBag.LastPage = propCount / model.pageSize == 0 ? (propCount / model.pageSize) - 1 : (propCount / model.pageSize);
            //---------

            return View(properties.Properties);
        }


        public ActionResult ListPropertiesFullWidth(SearchSellViewModel model)
        {
            //Setup filters
            ViewBag.PropertiesTitle = model.propertyType == null ? all : _propertyTypes[(int)model.propertyType];
            ViewBag.SearchParams = model;
            //---- Setup End -----

            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: model.propertyType, cityId: model.cityId);

            var properties = _propertyManager.GetPropertiesForSell(model.priceFrom, model.priceTo, model.propertyType, model.cityId, model.distanceFromCity, model.pageCount, model.pageSize, model.sortBy, model.orderBy);

            //LastPage
            var propCount = properties.Count;
            ViewBag.LastPage = propCount / model.pageSize == 0 ? (propCount / model.pageSize) - 1 : (propCount / model.pageSize);

            return View(properties.Properties);
        }

        public ActionResult Map(int? propertyType = null)
        {
            //Setup filters
            ViewBag.propertyType = propertyType;

            //Setup Advanced Search
            ViewBag.PropertiesTitle = propertyType == null ? all : _propertyTypes[(int)propertyType];
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            //Advanced Search End

            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: propertyType);

            return View();
        }


        public ActionResult ListRentProperties(SearchRentalViewModel model)
        {
            //Setup filters
            ViewBag.PropertiesTitle = model.rentPropertyType == null ? all : _propertyTypes[(int)model.rentPropertyType];
            ViewBag.SearchParams = model;
            //---- Setup End -----

            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: model.rentPropertyType, cityId:model.rentCityId);

            var propertiesList = _propertyManager.GetPropertyForRent(model.from, model.to, model.rentPriceFrom, model.rentPriceTo, model.rentPropertyType, model.rentCityId, model.rentDistanceFromCity, model.pageCount, model.pageSize, model.sortBy, model.orderBy);

            //LastPage
            var propCount = propertiesList.Count;
            ViewBag.LastPage = propCount / model.pageSize == 0 ? (propCount / model.pageSize) - 1 : (propCount / model.pageSize);

            return View(propertiesList.Properties);
        }

        public ActionResult ListRentPropertiesFullWidth(SearchRentalViewModel model)
        {
            //Setup filters
            ViewBag.PropertiesTitle = model.rentPropertyType == null ? all : _propertyTypes[(int)model.rentPropertyType];
            ViewBag.SearchParams = model;
            //---- Setup End -----

            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: model.rentPropertyType, cityId: model.rentCityId);

            var propertiesList = _propertyManager.GetPropertyForRent(model.from, model.to, model.rentPriceFrom, model.rentPriceTo, model.rentPropertyType, model.rentCityId, model.rentDistanceFromCity, model.pageCount, model.pageSize, model.sortBy, model.orderBy);

            //LastPage
            var propCount = propertiesList.Count;
            ViewBag.LastPage = propCount / model.pageSize == 0 ? (propCount / model.pageSize) - 1 : (propCount / model.pageSize);


            return View(propertiesList.Properties);
        }


        public ActionResult Recommended(int? propertyType = null, int? cityId = null, int pageCount = 1, int pageSize = 6,
            string sortBy = "PropertyName", string orderBy = "Descending")
        {
            //Setup filters
            ViewBag.PropertiesTitle = propertyType == null ? all : _propertyTypes[(int)propertyType];
            ViewBag.propertyType = propertyType;
            ViewBag.cityId = cityId;
            ViewBag.pageCount = pageCount;
            ViewBag.pageSize = pageSize;
            ViewBag.sortBy = sortBy;
            ViewBag.orderBy = orderBy;
            //---- Setup End -----

            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: propertyType);
            var pagedProperties = _propertyManager.GetPropertiesForSell(null, null, propertyType, cityId, null, pageCount, pageSize, sortBy, orderBy);

            //LastPage
            var propCount = pagedProperties.Count;
            ViewBag.LastPage = propCount / pageSize == 0 ? (propCount / pageSize) - 1 : (propCount / pageSize);

            return View(pagedProperties.Properties);
        }

        /// <summary>
        /// Search View
        /// </summary>
        /// <returns></returns>
        //GET: Properties/Search
        [HttpGet]
        public ActionResult Search(SearchFullViewModel searchFull)
        {
            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo();

            return View(searchFull);
        }

        //POST Properties/SearchSell
        [HttpGet]
        public ActionResult SearchSell(SearchSellViewModel model)
        {
            //Setup filters
            ViewBag.SearchParams = model;
            ViewBag.PropertiesTitle = model.propertyType == null ? all : _propertyTypes[(int)model.propertyType];
            //---- Setup End -----


            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: model.propertyType, cityId: model.cityId);

            //Checking the modelState
            if (!ModelState.IsValid)
            {
                return View("Search", new SearchFullViewModel { SearchSell = model });
            }

            //Filtering the Properties
            var pagedProperties = _propertyManager.GetPropertiesForSell(model.priceFrom, model.priceTo, model.propertyType, model.cityId, model.distanceFromCity, model.pageCount, model.pageSize, model.sortBy, model.orderBy);

            //LastPage
            var propCount = pagedProperties.Count;
            ViewBag.LastPage = propCount / model.pageSize == 0 ? (propCount / model.pageSize) - 1 : (propCount / model.pageSize);

            return View("SearchResult", pagedProperties.Properties);
        }

        //POST Properties/SearchRental
        [HttpGet]
        public ActionResult SearchRental(SearchRentalViewModel model)
        {
            //Setup filters
            ViewBag.SearchParams = model;
            ViewBag.PropertiesTitle = model.rentPropertyType == null ? all : _propertyTypes[(int)model.rentPropertyType];


            //Setup Advanced Search
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.FeaturedProperties = _propertyManager.GetRelatedPropertiesBriefInfo(propertyType: model.rentPropertyType, cityId: model.rentCityId);

            //Checking the modelState
            if (!ModelState.IsValid)
            {
                return View("Search", new SearchFullViewModel { SearchRental = model });
            }

            //Getting Filtered Properties
            var pagedProperties = _propertyManager.GetPropertyForRent(model.from, model.to, model.rentPriceFrom, model.rentPriceTo,
                model.rentPropertyType, model.rentCityId, model.rentDistanceFromCity, model.pageCount, model.pageSize, model.sortBy, model.orderBy);

            //LastPage
            var propCount = pagedProperties.Count;
            ViewBag.LastPage = propCount / model.pageSize == 0 ? (propCount / model.pageSize) - 1 : (propCount / model.pageSize);
            //---- Setup End -----



            return View("SearchResult", pagedProperties.Properties);
        }


        // GET: Properties/Details
        [HttpGet]
        public ActionResult Details(int id)
        {
            ViewBag.Cities = _cityManager.GetCitiesForDropDown(40);
            ViewBag.PropertyTypes = _propertyManager.GetPropertyTypes();
            ViewBag.Agents = _userServices.GetTopTwoAgents();
            ViewBag.RelatedProperties = _propertyManager.GetPropertiesForSell().Properties.Take(6).ToList();
            ViewBag.HappyClient = _clientsManager.GetHappyClients();

            DetailsPropertyViewModel property = _propertyManager.Details(id);
            _propertyManager.IncrementViews(id);

            return View(property);
        }

        // GET: Properties/Create
        [Authorize(Roles = "Administrator,Agent")]
        public ActionResult Create(PropertyType? propertyType = null)
        {
            ViewBag.CountryId = new SelectList(_cityManager.GetCountiesForDropDown(), "CountryId", "CountryName");
            ViewBag.PropertySeasonId = new SelectList(_propertyManager.GetPropertySeasons(), "PropertySeasonId", "PropertySeasonName");
            ViewBag.PropertyTypeId = new SelectList(_propertyManager.GetPropertyTypes(), "PropertyTypeId", "PropertyTypeName", (int?)propertyType);
            ViewBag.UnitTypeId = new SelectList(_rentalInfoManager.GetRentalTypesForDropDown(), "UnitTypeId", "RentalTypeName");
            ViewBag.OwnerId = new SelectList(_userServices.GetUsersInRole("Owner").Select(Mapper.Map<UsersIdInfoViewModel>).ToList(), "Id", "Info");
            ViewBag.RentalPeriodId = _rentalInfoManager.GetRentalPeriods();


            ViewBag.RentalExtras = _extrasManager.GetExtras("RentalExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList();

            //Property Extras populated here
            CreatePropertyViewModel property = new CreatePropertyViewModel
            {
                PropertyExtrasCheckBoxes = _extrasManager.GetExtras("PropertyExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList()
            };


            //Property Additional Information
            string pageTitle = "Имот";
            List<KeyValuePair<string, string>> propertyDescriptors = new List<KeyValuePair<string, string>>();
            switch (propertyType)
            {
                case PropertyType.Хотел:
                    pageTitle = "Хотел";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брои стаи", "3"),
                        new KeyValuePair<string,string>("Апартаменти", "3"),
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.Къща:
                    pageTitle = "Къща";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брои стаи", "3"),
                        new KeyValuePair<string,string>("Брой спални", "3"),
                        new KeyValuePair<string,string>("Брой бани", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.Мотел:
                    pageTitle = "Мотел";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брои стаи", "3"),
                        new KeyValuePair<string,string>("Апартаменти", "3"),
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.Станция:
                    pageTitle = "Станция";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брои стаи", "3"),
                        new KeyValuePair<string,string>("Апартаменти", "3"),
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.ОфисСграда:
                    pageTitle = "Офис Сграда";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брои офиси", "3"),
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.Офис:
                    pageTitle = "Офис";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Етаж", "Първи"),
                    };
                    break;
                case PropertyType.Вила:
                    pageTitle = "Вила";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брои стаи", "3"),
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.Етаж:
                    pageTitle = "Етаж";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Брои стаи", "3"),
                        new KeyValuePair<string,string>("Етаж", "Първи"),
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.Паркинг:
                    pageTitle = "Паркинг";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Парко места", "3"),
                        new KeyValuePair<string,string>("Подземен", "да"),
                        new KeyValuePair<string,string>("Охраняем", "не")
                    };
                    break;
                case PropertyType.ПаркоМясто:
                    pageTitle = "Парко Място";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Подземен", "да"),
                        new KeyValuePair<string,string>("Охраняем", "не")
                    };
                    break;
                case PropertyType.Гараж:
                    pageTitle = "Гараж";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Самостоятелен", "да")
                    };
                    break;
                case PropertyType.Земя:
                    pageTitle = "Земя";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Обработваема", "не")
                    };
                    break;
                case PropertyType.Магазин:
                    pageTitle = "Магазин";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Отопление", "Климатик")
                    };
                    break;
                case PropertyType.ТърговскиЦентър:
                    pageTitle = "Търговски Център";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брои обекти", "3"),
                        new KeyValuePair<string,string>("Отопление", "Климатик"),
                        new KeyValuePair<string,string>("Стройтелство", "ново")
                    };
                    break;
                case PropertyType.Сграда:
                    pageTitle = "Сграда";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Етажи", "3"),
                        new KeyValuePair<string,string>("Брой Помещения", "3"),
                        new KeyValuePair<string,string>("Вид сграда", "Строеж")
                    };
                    break;
                case PropertyType.Склад:
                    pageTitle = "Склад";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Отопление", "няма")
                    };
                    break;
                case PropertyType.Стаи:
                    pageTitle = "Стаи";
                    propertyDescriptors = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string,string>("Брой", "няма"),
                        new KeyValuePair<string,string>("Отопление", "няма")
                    };
                    break;
                case PropertyType.Друго:
                    break;
                case null:
                    break;
                default:
                    break;
            }

            ViewBag.Title = "Създай " + pageTitle;
            ViewBag.PropertyDescriptors = propertyDescriptors;
            return View(property);
        }




        // POST: Properties/Create
        [HttpPost]
        [Authorize(Roles = "Administrator,Agent")]
        public ActionResult Create(CreatePropertyViewModel model)
        {
            if (model.RentalPrice != null && model.RentPricePeriod == null ||
                model.RentalPrice == null && model.RentPricePeriod != null)
            {
                ModelState.AddModelError("RentalPrice", "И двете полета трябва да са попълнени");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.CountryId = new SelectList(_cityManager.GetCountiesForDropDown(), "CountryId", "CountryName");
                ViewBag.PropertySeasonId = new SelectList(_propertyManager.GetPropertySeasons(), "PropertySeasonId", "PropertySeasonName");
                ViewBag.PropertyTypeId = new SelectList(_propertyManager.GetPropertyTypes(), "PropertyTypeId", "PropertyTypeName");
                ViewBag.UnitTypeId = new SelectList(_rentalInfoManager.GetRentalTypesForDropDown(), "UnitTypeId", "RentalTypeName");
                ViewBag.OwnerId = new SelectList(_userServices.GetUsersInRole("Owner").Select(Mapper.Map<UsersIdInfoViewModel>).ToList(), "Id", "Info");
                ViewBag.RentalPeriodId = _rentalInfoManager.GetRentalPeriods();
                ViewBag.RentalExtras = _extrasManager.GetExtras("RentalExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList();

                model.PropertyExtrasCheckBoxes = _extrasManager.GetExtras("PropertyExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList();

                ViewBag.Title = "Създай Имот";

                return Json(ModelState.ToJson());
            }

            _propertyManager.AddProperty(model);
            return Json("STATUS_OK");
        }


        [HttpGet]
        [Authorize(Roles = "Administrator,Agent")]
        public ActionResult Edit(int id)
        {
            ViewBag.CountyId = new SelectList(_cityManager.GetCountiesForDropDown(), "CountryId", "CountryName");
            ViewBag.PropertySeasonId = new SelectList(_propertyManager.GetPropertySeasons(), "PropertySeasonId", "PropertySeasonName");
            ViewBag.RentalTypeId = new SelectList(_rentalInfoManager.GetRentalTypesForDropDown(), "UnitTypeId", "RentalTypeName");

            ViewBag.RentalExtras = _extrasManager.GetExtras("RentalExtras").Select(Mapper.Map<ExtraCheckBoxViewModel>).ToList();

            var property = _propertyManager.GetProperty(id);

            return View(property);
        }

        //// POST: Properties/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "PropertyId,AddressId,PropertySeasonId,PropertyTypeId,PropertyName,AdditionalDescription,CreatedOn,Views,IsActive")] Properties properties)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Properties.Add(properties);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AddressId = new SelectList(db.Addresses, "AddressId", "FullAddress", properties.AddressId);
        //    ViewBag.PropertySeasonId = new SelectList(db.PropertySeasons, "PropertySeasonId", "PropertySeasonName", properties.PropertySeasonId);
        //    ViewBag.PropertyTypeId = new SelectList(db.PropertyTypes, "PropertyTypeId", "PropertyTypeName", properties.PropertyTypeId);
        //    return View(properties);
        //}

        //// GET: Properties/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Properties properties = await db.Properties.FindAsync(id);
        //    if (properties == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AddressId = new SelectList(db.Addresses, "AddressId", "FullAddress", properties.AddressId);
        //    ViewBag.PropertySeasonId = new SelectList(db.PropertySeasons, "PropertySeasonId", "PropertySeasonName", properties.PropertySeasonId);
        //    ViewBag.PropertyTypeId = new SelectList(db.PropertyTypes, "PropertyTypeId", "PropertyTypeName", properties.PropertyTypeId);
        //    return View(properties);
        //}

        //// POST: Properties/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "PropertyId,AddressId,PropertySeasonId,PropertyTypeId,PropertyName,AdditionalDescription,CreatedOn,Views,IsActive")] Properties properties)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(properties).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.AddressId = new SelectList(db.Addresses, "AddressId", "FullAddress", properties.AddressId);
        //    ViewBag.PropertySeasonId = new SelectList(db.PropertySeasons, "PropertySeasonId", "PropertySeasonName", properties.PropertySeasonId);
        //    ViewBag.PropertyTypeId = new SelectList(db.PropertyTypes, "PropertyTypeId", "PropertyTypeName", properties.PropertyTypeId);
        //    return View(properties);
        //}

        //// GET: Properties/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Properties properties = await db.Properties.FindAsync(id);
        //    if (properties == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(properties);
        //}

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var status = _propertyManager.DeleteProperty(id);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
