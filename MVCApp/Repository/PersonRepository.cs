using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class PersonRepository
    {
        public ApplicationDbContext Context;

        public PersonRepository()
        {
            Context = new ApplicationDbContext();
        }

        public List<Person> GetAll()
        {
            return Context.Persons.ToList();
        }

    }
}