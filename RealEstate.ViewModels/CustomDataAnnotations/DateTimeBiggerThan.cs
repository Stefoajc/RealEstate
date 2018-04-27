using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.ViewModels.CustomDataAnnotations
{
    public class DateTimeBiggerThan : ValidationAttribute
    {
        private readonly string _dependentProperty;

        public DateTimeBiggerThan(string propertyName) : base(propertyName)
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

            var dateBigger = (DateTime)value;
            var dateSmaller = (DateTime)otherPropertyValue;

            return dateBigger > dateSmaller ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(ErrorMessage));
        }
    }
}
