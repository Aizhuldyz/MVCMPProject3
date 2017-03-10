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

        public int Add(Person person)
        {
            return Context.Persons.Add(person).Id;
        }

        public void Delete(int id)
        {
            var person = Context.Persons.FirstOrDefault(x => x.Id == id);
            Context.Persons.Remove(person);
        }

    }
}