using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MVCApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        [JsonIgnore]
        public string PhotoUrl { get; set; }

        [JsonIgnore]
        public virtual ICollection<Recognition> Badges { get; set; }
    }
}