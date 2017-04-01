using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCApp.ViewModels
{
    public class PersonCreateViewModel
    {
        public int Id { get; set; }
        [DisplayName("Name")]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true)]
        public string Name { get; set; }

        [DisplayName("BirthDate")]
        [Required]
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        [DataType(DataType.Upload)]
        [Required]
        public HttpPostedFileBase Photo { get; set; }
    }
}