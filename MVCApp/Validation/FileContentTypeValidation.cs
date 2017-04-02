using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace MVCApp.Validation
{
    public class FileContentTypeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult("Please choose an image to upload");

            var file = (HttpPostedFileWrapper)value;
            var validImageTypes = new[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };
            if (!validImageTypes.Contains(file.ContentType))
            {
                return new ValidationResult("Please choose either a GIF, JPG or PNG image.");
            }

            return ValidationResult.Success;

        }
    }
}