using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVCApp.Validation;

namespace MVCApp.ViewModels
{
    public class BadgeCreateViewModel
    {
        public int Id { get; set; }

        [DisplayName("Title")]
        [Required]
        public string Title { get; set; }

        [DisplayName("Description")]
        [StringLength(100, ErrorMessage = "Description should not be more than 100 chars")]
        [Required]
        public string Description { get; set; }

        [DisplayName("Image")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }
    }
}