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
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public string Name { get; set; }

        [DisplayName("BirthDate")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/YYYY}")]
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase Photo { get; set; }
    }
}