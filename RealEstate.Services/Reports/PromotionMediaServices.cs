using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.ViewModels.WebMVC.Reports;

namespace RealEstate.Services.Reports
{
    public class PromotionMediaServices
    {
        private readonly RealEstateDbContext _dbContext;

        public PromotionMediaServices(RealEstateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PromotionMediaCheckKeyValueViewModel>> ListAsKeyValue()
        {
            return await _dbContext.PromotionMediae
                .Select(pm => new PromotionMediaCheckKeyValueViewModel
                {
                    Id = pm.Id,
                    Media = pm.Media,
                    IsChecked = false
                })
                .ToListAsync();
        }
    }
}