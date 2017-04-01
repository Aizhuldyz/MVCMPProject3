﻿using System;

namespace MVCApp.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public string PhotoUrl { get; set; }
    }
}