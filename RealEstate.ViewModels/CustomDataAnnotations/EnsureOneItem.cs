using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.CustomDataAnnotations
{
    public class EnsureOneItem : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            IList list = (IList) value;

            return list?.Count > 0;
        }
    }
}
