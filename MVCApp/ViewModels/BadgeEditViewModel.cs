﻿using System;
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
        public string Title { get; set; }

        [DisplayName("Description")]
        [Required]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        [DisplayName("Delete Image")]
        public bool DeleteImage { get; set; }

        [DisplayName("Image")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }
    }
}