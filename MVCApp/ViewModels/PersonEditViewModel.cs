using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVCApp.Validation;

namespace MVCApp.ViewModels
{
    public class PersonEditViewModel
    {
        public int Id { get; set; }
        [DisplayName("Name")]
        [StringLength(50, ErrorMessage = "Name should not be more than 50 chars")]
        [Required]
        public string Name { get; set; }

        [DisplayName("BirthDate")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/YYYY}")]
        [Required]
        [BirthDateValidation]
        public DateTime BirthDate { get; set; }
        public string PhotoUrl { get; set; }

        [DisplayName("Delete Photo")]
        public bool DeletePhoto { get; set; }

        [DataType(DataType.Upload)]
        [FileContentTypeValidation]
        [DisplayName("Photo")]
        public HttpPostedFileBase Photo { get; set; }
    }
}