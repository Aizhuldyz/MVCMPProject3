using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCApp.ViewModels
{
    public class SelectBadgesViewModel
    {
        public int PersonId { get; set; }
        [DisplayName("Name")]
        [ReadOnly(true)]
        public string PersonName { get; set; }

        public List<BadgeViewModel> Badges { get; set; }
    }
}