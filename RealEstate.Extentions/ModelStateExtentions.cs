using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RealEstate.Extentions
{
    public static class ModelStateExtentions
    {

        public static Dictionary<string, string[]> ToJson(this ModelStateDictionary modelState)
        {
            var errorList = modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            errorList.Add("success", new []{"false"});

            return errorList;
        }
    }
}
