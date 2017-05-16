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
        [StringLength(50, ErrorMessage = "Badge {0} cannot be more than {1} chars")]
        [RegularExpression("[a-zA-z0-9 -]+", ErrorMessage = "Title is not valid")]
        public string Title { get; set; }

        [DisplayName("Description")]
        [StringLength(100, ErrorMessage = "The {0} should not be more than {1} chars")]
        public string Description { get; set; }

        [DisplayName("Image")]
        [DataType(DataType.Upload)]
        [Required]
        public HttpPostedFileBase Image { get; set; }
    }
}