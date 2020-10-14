using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using RazorEngine;
using RazorEngine.Templating;
using RealEstate.Data;
using RealEstate.Model;
using RealEstate.Model.Notifications;
using RealEstate.Model.Reports;
using RealEstate.Services.Exceptions;
using RealEstate.Services.Interfaces;
using RealEstate.ViewModels.WebMVC;
using RealEstate.ViewModels.WebMVC.Reports;
using Encoding = System.Text.Encoding;

namespace RealEstate.Services.Reports
{
    public class ReportServices
    {
        private readonly RealEstateDbContext _dbContext;
        private readonly ApplicationUserManager _userManager;
        private readonly WebPlatformServices _webPlatformServices;
        private readonly NoReplyMailService _noReplayMail;
        private readonly INotificationCreator _notificationCreator;
        private readonly string _baseReportsPath = ConfigurationManager.AppSettings["ReportsPath"];

        public ReportServices(RealEstateDbContext dbContext, ApplicationUserManager userManager, WebPlatformServices webPlatformServices, NoReplyMailService noReplayMail, INotificationCreator notificationCreator)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _webPlatformServices = webPlatformServices;
            _noReplayMail = noReplayMail;
            _notificationCreator = notificationCreator;
        }

        public async Task ListReports(string agentId, string propertyId)
        {
            return;
        }


        public async Task CreateReport(ReportCreateViewModel model, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът, който пише 'Докладът' в системата!");
            }

            if (!(await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                  || await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат право да правят Доклади !");
            }

            if (!await _webPlatformServices.Exist(model.WebPlatformViews.Select(w => w.PlatformId)))
            {
                throw new ArgumentException("Някоя от платформите, която сте избрали не съществува в системата!");
            }

            var propertyToCreateReportTo =
                await _dbContext.Properties.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == model.PropertyId);

            if (propertyToCreateReportTo == null)
            {
                throw new ContentNotFoundException("Не е намерен имотът, за който пишете доклад!");
            }

            if (propertyToCreateReportTo.PropertyState != PropertyState.Available || !propertyToCreateReportTo.IsActive)
            {
                throw new ArgumentException("Имотът, на който пишете доклад не активен !");
            }

            if (propertyToCreateReportTo.AgentId != agentId &&
                !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Нямате право да пишете доклад на имот, които не под ваше представителство !");
            }

            var brokersSharedWith =
                await _dbContext.Partners
                .Where(p => model.ColleguesIds.Any(ci => ci == p.Id))
                .ToListAsync();

            var promotianMediaeUsed = await _dbContext.PromotionMediae
                .Where(pm => model.PromotionMediaUsedIds.Any(pmu => pmu == pm.Id))
                .ToListAsync();

            var report = new Model.Reports.Reports
            {
                PathToReport = await CreateReportRelativePath(agentId, model.PropertyId),
                TotalViews = model.TotalViews,
                TotalCalls = model.TotalCalls,
                TotalInspections = model.TotalInspections,
                TotalOffers = model.TotalOffers,
                ActionsConclusion = model.ActionsConclusion,
                ChangeArguments = model.ChangeArguments,
                IsMarketingChangeIssued = model.IsMarketingChangeIssued,
                IsPriceChangeIssued = model.IsPriceChangeIssued,
                PropertyId = model.PropertyId,
                PromotionMediae = new HashSet<PromotionMedia>(promotianMediaeUsed),
                ColleaguesPartners = new HashSet<Partners>(brokersSharedWith),
                WebPlatformViews = new HashSet<WebPlatformViews>(model.WebPlatformViews
                    .Select(p => new WebPlatformViews
                    {
                        Views = p.Views,
                        WebPlatformId = p.PlatformId
                    })),
                CustomPromotionMediae = new HashSet<CustomPromotionMedia>(model.CustomPromotionMediae.Select(c => new CustomPromotionMedia { CustomMedia = c })),
                CustomRecommendedActions = new HashSet<CustomRecommendedActions>(model.CustomRecommendedActions.Select(c => new CustomRecommendedActions { RecommendedAction = c })),
                Offers = new HashSet<Offers>(model.Offers.Select(o => new Offers { Offer = o })),
                AgentId = agentId
            };

            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();

            var htmlReport = await CreateReportAsHtmlFile(report.Id, propertyToCreateReportTo.Id, agentId);

            if (model.SendToOwnersViaEmail)
            {
                var ownerEmail = propertyToCreateReportTo.Owner.Email;
                // send mail to owner
                await _noReplayMail.SendHtmlEmailAsync(ownerEmail, "Доклад за имот от СПРОПЪРТИС", htmlReport);
            }
        }

        private async Task<string> CreateReportRelativePath(string agentId, int propertyId)
        {
            var agentUserNameCreatingReport = await _userManager.Users
                                                  .Where(u => u.Id == agentId)
                                                  .Select(u => u.UserName)
                                                  .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Брокерът правещ докладът не е намерен!");

            var reportName = "Report for Property-" + propertyId + "- from " + DateTime.Now.ToString("dd.MM.yyyy") + ".html";
            var reportDirectoryRelativePath = Path.Combine(_baseReportsPath, agentUserNameCreatingReport);

            var reportRelativePath = Path.Combine(reportDirectoryRelativePath, reportName);

            return reportRelativePath;
        }

        private async Task<string> CreateReportAsHtmlFile(int reportId, int propertyId, string agentId)
        {
            var systemRoot = HttpRuntime.AppDomainAppPath;
            var agentUserNameCreatingReport = await _userManager.Users
                .Where(u => u.Id == agentId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync() ?? throw new ContentNotFoundException("Брокерът правещ докладът не е намерен!");

            var reportName = "Report for Property-" + propertyId + "- from " + DateTime.Now.ToString("dd.MM.yyyy") + ".html";
            var reportDirectoryRelativePath = Path.Combine(_baseReportsPath, agentUserNameCreatingReport);
            var reportDirectoryPhysicalPath = Path.Combine(systemRoot.TrimEnd('\\', '/'), reportDirectoryRelativePath.TrimStart('\\', '/'));

            var reportPhysicalPath = Path.Combine(reportDirectoryPhysicalPath, reportName);

            Directory.CreateDirectory(reportDirectoryPhysicalPath);
            if (File.Exists(reportPhysicalPath))
            {
                throw new ArgumentException("Вече има файл с репорт от тази дата за този имот!");
            }

            var reportAsHtmlPage = await CreateEmailReportAsHtmlAsync(reportId);
            var htmlFileAsBytes = Encoding.UTF8.GetBytes(reportAsHtmlPage);
            using (var fileStream = new FileStream(reportPhysicalPath, FileMode.Create))
            {
                await fileStream.WriteAsync(htmlFileAsBytes, 0, htmlFileAsBytes.Length);
            }

            return reportAsHtmlPage;
        }

        #region Report to Html Page Helpers

        private async Task<string> CreateEmailReportAsHtmlAsync(int reportId)
        {
            var reportTemplateViewModel = await CreateTemplateViewModelAsync(reportId);
            return CreateEmailTemplate(reportTemplateViewModel);
        }

        public async Task<ReportTemplateViewModel> CreateTemplateViewModelAsync(int reportId)
        {
            var report = await _dbContext.Reports
                .Include(p => p.PromotionMediae)
                .Include(p => p.WebPlatformViews)
                .Include(p => p.WebPlatformViews.Select(wpv => wpv.WebPlatform))
                .Include(p => p.CustomRecommendedActions)
                .Include(p => p.CustomPromotionMediae)
                .Include(p => p.ColleaguesPartners)
                .Include(p => p.Offers)
                .Include(p => p.Agent)
                .FirstOrDefaultAsync(r => r.Id == reportId) ?? throw new ContentNotFoundException("Не е намерен докладът !");

            var reportEmailModel = new ReportTemplateViewModel
            {
                PromotionMediae = await _dbContext.PromotionMediae
                    .Select(p => new PromotionMediaForEmail
                    {
                        Id = p.Id,
                        MediaType = p.Media
                    })
                    .ToListAsync()
            };

            reportEmailModel.PromotionMediae.ForEach((p) => p.IsChecked = report.PromotionMediae.Any(pm => pm.Id == p.Id));
            reportEmailModel.CustomPromotionMediae =
                report.CustomPromotionMediae.Select(cpm => cpm.CustomMedia).ToList();
            reportEmailModel.WebPlatformViews = report.WebPlatformViews
                .Select(wpv => new WebPlatformViewEmailViewModel
                {
                    Views = wpv.Views,
                    PlatformName = wpv.WebPlatform.WebPlatform
                })
                .ToList();
            reportEmailModel.PartnersSharedWith = report.ColleaguesPartners
                .Select(p => p.PartnerName + (string.IsNullOrEmpty(p.PartnerCompanyName) ? "" : " от " + p.PartnerCompanyName)).ToList();

            reportEmailModel.TotalViews = report.TotalViews;
            reportEmailModel.TotalCalls = report.TotalCalls;
            reportEmailModel.TotalInspections = report.TotalInspections;
            reportEmailModel.TotalOffers = report.TotalOffers;
            reportEmailModel.Offers = report.Offers.Select(o => o.Offer).ToList();

            reportEmailModel.ActionsConclusion = report.ActionsConclusion;

            reportEmailModel.IsMarketingChangeIssued = report.IsMarketingChangeIssued;
            reportEmailModel.IsPriceChangeIssued = report.IsPriceChangeIssued;
            reportEmailModel.CustomRecommendedActions =
                report.CustomRecommendedActions.Select(cra => cra.RecommendedAction).ToList();
            reportEmailModel.ChangeArguments = report.ChangeArguments;

            reportEmailModel.CreatedOn = report.CreatedOn;
            reportEmailModel.LinkToProperty = Path.Combine(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), "/Properties/Details/" + report.PropertyId);
            reportEmailModel.AgentCreator = report.Agent.FirstName + " " + report.Agent.LastName;

            return reportEmailModel;
        }

        private string CreateEmailTemplate(ReportTemplateViewModel templateModel)
        {
            string template = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, @"Resources\EmailTemplates\HTMLPage1.cshtml"));

            if (Engine.Razor.IsTemplateCached("ReportsTemplate", typeof(ReportTemplateViewModel)))
            {
                return Engine.Razor.Run("ReportsTemplate", typeof(ReportTemplateViewModel), templateModel);
            }

            return Engine.Razor.RunCompile(template, "ReportsTemplate", null, templateModel);
        }

        #endregion


        public async Task<ReportFileDownloadViewModel> DownloadReport(int reportId, string agentId)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Не е намерен брокерът, който пише 'Докладът' в системата!");
            }

            if (!(await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Agent))
                  || await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator))))
            {
                throw new NotAuthorizedUserException("Потребителят няма право на това действие! Само админи и брокери имат право да правят Доклади !");
            }

            var report = await _dbContext.Reports.FirstOrDefaultAsync(r => r.Id == reportId) ??
                         throw new ContentNotFoundException("Не е намерен Докладът, който искате да свалите!");

            if (report.AgentId != agentId && !await _userManager.IsInRoleAsync(agentId, Enum.GetName(typeof(Role), Role.Administrator)))
            {
                throw new NotAuthorizedUserException("Нямате право да свалите този доклад!");
            }

            var systemRoot = HttpRuntime.AppDomainAppPath;
            var reportPhysicalPath =
                Path.Combine(systemRoot.TrimEnd('\\', '/'), report.PathToReport.TrimStart('\\', '/'));


            return new ReportFileDownloadViewModel
            {
                ReportName = Path.GetFileName(report.PathToReport),
                ReportData = File.ReadAllBytes(reportPhysicalPath)
            };
        }


        public async Task NotifyForReports()
        {
            var propertiesForReportCreation = await _dbContext.Properties
                .Include(p => p.Reports)
                .Include(p => p.Owner)
                .Where(p => p.IsActive && p.PropertyState == PropertyState.Available)
                .ToListAsync();

            if (!propertiesForReportCreation.Any()) return;

            var propertiesWithoutReportYet = propertiesForReportCreation.Where(p => p.Reports.Count == 0);

            foreach (var property in propertiesWithoutReportYet)
            {
                var propertyCreationDate = property.CreatedOn;

                if (propertyCreationDate < DateTime.Now.AddDays(-14))
                {
                    var ownerName = property.Owner.FirstName + " " + property.Owner.LastName;
                    var propertyImage = property.Images.Select(i => i.ImagePath).FirstOrDefault();
                    await CreateReportNotification(property.Id, property.PropertyName, propertyImage, ownerName, property.AgentId);
                }
            }

            var propertiesWithReports = propertiesForReportCreation.Where(p => p.Reports.Count > 0);

            foreach (var property in propertiesWithReports)
            {
                var getLastReportDate = property.Reports
                    .OrderByDescending(r => r.CreatedOn)
                    .Select(p => p.CreatedOn)
                    .FirstOrDefault();

                if (getLastReportDate < DateTime.Now.AddDays(-14))
                {
                    var ownerName = property.Owner.FirstName + " " + property.Owner.LastName;
                    var propertyImage = property.Images.Select(i => i.ImagePath).FirstOrDefault();
                    await CreateReportNotification(property.Id, property.PropertyName, propertyImage, ownerName, property.AgentId);
                }
            }
        }

        #region NotifyForReports Helpers

        private async Task CreateReportNotification(int propertyId, string propertyName, string propertyImage
            , string propertyOwnerName, string propertyAgentId)
        {
            await _notificationCreator.CreateIndividualNotification(
                new NotificationCreateViewModel
                {
                    NotificationTypeId = (int)NotificationType.Report,
                    NotificationPicture = propertyImage,
                    NotificationLink = "/Reports/Create?propertyId=" + propertyId,
                    NotificationText = "Напомняне за Доклад на имот: " + propertyName + " на " + propertyOwnerName
                }
                , propertyAgentId);
        }

        #endregion


    }
}