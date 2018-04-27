using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.ViewModels.CustomDataAnnotations
{
    public class PropertyDependOn : ValidationAttribute
    {
        private readonly string _dependantProperty;

        public PropertyDependOn(string dependantProperty) : base("Грешка")
        {
            _dependantProperty = dependantProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value == null)
            {
                // Get the dependent property 
                var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(_dependantProperty) ?? throw new ArgumentException(_dependantProperty + " не е намерено");
                // Get the value of the dependent property 
                var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);

                if (otherPropertyValue != null)
                {
                    return new ValidationResult(
                        FormatErrorMessage(ErrorMessage));
                }
            }

            return ValidationResult.Success;
        }
    }
}
