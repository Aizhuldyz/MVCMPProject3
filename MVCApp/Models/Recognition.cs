using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCApp.Models
{
    public class Recognition
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int BadgeId { get; set; }

        public Person Person { get; set; }
        public virtual Badge Badge { get; set; }
    }
}