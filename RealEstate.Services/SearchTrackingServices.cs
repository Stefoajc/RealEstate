using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Helpers;

namespace RealEstate.Services
{
    public class SearchTrackingServices : BaseService
    {
        public SearchTrackingServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) : base(unitOfWork, userMgr)
        {
        }

        public async Task AddSearchParameters(string userId, object searchParams)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var searchParamsCollectionToAdd = new SearchParamsTracking();

                foreach (var searchParam in searchParams.AsDictionary())
                {
                    if (searchParam.Value is IEnumerable && !(searchParam.Value is string))
                    {
                        foreach (var value in (IEnumerable)searchParam.Value)
                        {
                            searchParamsCollectionToAdd.SearchParams.Add(new SearchParams
                            {
                                ParamName = searchParam.Key,
                                ParamValue = value.ToString()
                            });
                        }
                    }
                    if (searchParam.Value != null)
                    {
                        searchParamsCollectionToAdd.SearchParams.Add(new SearchParams
                        {
                            ParamName = searchParam.Key,
                            ParamValue = searchParam.Value.ToString()
                        });
                    }
                }

                user.SearchParamsTrackings.Add(searchParamsCollectionToAdd);
                await userManager.UpdateAsync(user);
            }
        }

        public async Task<Dictionary<string, string>> GetLatestSearchParameters(string userId)
        {
            var searchParamsUsed = (await unitOfWork.SearchTrackingRepository
                .Where(u => u.UserId == userId)
                .OrderByDescending(p => p.CreatedOn)
                .Select(p => p.SearchParams)
                .FirstOrDefaultAsync());



            return searchParamsUsed?.ToDictionary(sp => sp.ParamName, sp => sp.ParamValue, StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, string>();
        }

        public async Task<Dictionary<string, string>> GetMostUsedSearchParameters(string userId, int parametersCount = 5)
        {
            var searchParamsUsed = (await unitOfWork.SearchTrackingRepository
                .Where(u => u.UserId == userId)
                .OrderByDescending(p => p.CreatedOn)
                .SelectMany(p => p.SearchParams)
                .ToListAsync());


            List<SearchParamCounter> searchedParamsWithUsedCount = new List<SearchParamCounter>();
            foreach (var searchParam in searchParamsUsed)
            {

                var searchParamToIncrease = searchedParamsWithUsedCount.FirstOrDefault(s => s.ParamName == searchParam.ParamName & s.ParamValue == searchParam.ParamValue);

                if (searchParamToIncrease != null)
                {
                    searchParamToIncrease.Count++;
                }
                else
                {
                    searchedParamsWithUsedCount.Add(new SearchParamCounter
                    {
                        ParamName = searchParam.ParamName,
                        ParamValue = searchParam.ParamValue,
                        Count = 1
                    });
                }
            }

            return searchedParamsWithUsedCount
                .OrderByDescending(sp => sp.Count)
                .Take(parametersCount)
                .ToDictionary(sp => sp.ParamName, 
                sp => sp.ParamValue,
                StringComparer.OrdinalIgnoreCase);
        }

        private class SearchParamCounter
        {
            public string ParamName { get; set; }
            public string ParamValue { get; set; }
            public int Count { get; set; }
        }
    }
}
