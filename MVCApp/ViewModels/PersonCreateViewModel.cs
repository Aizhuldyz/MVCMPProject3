using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.UI.WebControls;
using MVCApp.Validation;

namespace MVCApp.ViewModels
{
    public class PersonCreateViewModel
    {
        public int Id { get; set; }
        [DisplayName("Name")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        [StringLength(50, ErrorMessage = "Name should not be more than 50 chars")]
        public string Name { get; set; }

        [DisplayName("BirthDate")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/YYYY}")]
        [Required]
        [BirthDateValidation]
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        [DataType(DataType.Upload)]
        [FileContentTypeValidation]
        public HttpPostedFileBase Photo { get; set; }
    }
}