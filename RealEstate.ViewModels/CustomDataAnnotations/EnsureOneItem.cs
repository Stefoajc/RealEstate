using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
