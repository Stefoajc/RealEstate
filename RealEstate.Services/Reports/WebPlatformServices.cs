using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RealEstate.Data;
using RealEstate.ViewModels.WebMVC.Reports;

namespace RealEstate.Services.Reports
{
    public class WebPlatformServices
    {
        private readonly RealEstateDbContext _dbContext;

        public WebPlatformServices(RealEstateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Exist(int platformId)
        {
            return await _dbContext.WebPlatforms.AnyAsync(p => p.Id == platformId);
        }

        public async Task<bool> Exist(IEnumerable<int> platformIds)
        {
            return await _dbContext.WebPlatforms.AnyAsync(p => platformIds.Any(pId => pId == p.Id));
        }

        public async Task<List<WebPlatformKeyValueViewModel>> ListAsync()
        {
            return await _dbContext.WebPlatforms
                .Select(wp => new WebPlatformKeyValueViewModel
                {
                    Id = wp.Id,
                    WebPlatform = wp.WebPlatform
                })
                .ToListAsync();
        }
    }
}