using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVCApp.Validation;

namespace MVCApp.ViewModels
{
    public class BadgeEditViewModel
    {
        public int Id { get; set; }

        [DisplayName("Title")]
        [Required]
        [StringLength(50, ErrorMessage = "Badge title cannot be more than 50 chars")]
        [RegularExpression("^[A-Za-z0-9 -]+$]", ErrorMessage = "Title is not valid")]
        public string Title { get; set; }

        [DisplayName("Description")]
        [StringLength(250, ErrorMessage = "Description should not be more than 100 chars")]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        [DisplayName("Delete Image")]
        public bool DeleteImage { get; set; }

        [DisplayName("Image")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }
    }
}