using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.ViewModels.CustomDataAnnotations
{
    public class IntBiggerThan : ValidationAttribute
    {
        private readonly string _dependentProperty;

        public IntBiggerThan(string propertyName) : base(propertyName)
        {
            _dependentProperty = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(_dependentProperty) ?? throw new ArgumentException(_dependentProperty + " не е намерено");
            var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

            if (otherPropertyValue == null || value == null)
            {
                return ValidationResult.Success;
            }

            var intBigger = (int)value;
            var intSmaller = (int)otherPropertyValue;

            return intBigger > intSmaller ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(ErrorMessage));

        }
    }
}
