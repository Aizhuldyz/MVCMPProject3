using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVCApp.Helper;

namespace MVCApp.Validation
{
    public class BirthDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var birthDate = (DateTime)value;
            if (CommonHelper.GetAge(birthDate) > 150)
            {
                return new ValidationResult("Person cannot be older than 150 years");
            }

            return ValidationResult.Success;

        }
    }
}