using System;
using System.Collections.Generic;

namespace MVCApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhotoUrl { get; set; }

        public virtual ICollection<Recognition> Badges { get; set; }
    }
}