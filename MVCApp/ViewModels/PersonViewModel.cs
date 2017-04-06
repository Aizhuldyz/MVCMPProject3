using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MVCApp.ViewModels
{
    public class PersonViewModel
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("BirthDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/YYYY}")]
        public DateTime BirthDate { get; set; }

        public int Age { get; set; }

        [DisplayName("Photo")]
        public string PhotoUrl { get; set; }
    }
}