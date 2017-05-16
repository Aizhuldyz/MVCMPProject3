using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Web;
using Microsoft.Ajax.Utilities;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class PersonRepository
    {
        private readonly ApplicationDbContext _context;
        
        public PersonRepository()
        {
            _context = new ApplicationDbContext();
        }

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Person> GetAll()
        {
            var persons = MemoryCache.Default["persons"] as IEnumerable<Person>;
            if (persons == null)
            {
                persons = _context.Persons.ToList();
                var cacheTimeOut = int.Parse(ConfigurationManager.AppSettings["cacheTimeOut"]);
                MemoryCache.Default.Add("persons", persons, new DateTimeOffset(DateTime.Now.AddMinutes(cacheTimeOut)));
            }
            return persons;
        }

        public Person Get(int id)
        {
            return _context.Persons.FirstOrDefault(x => x.Id == id);
        }

        public List<string> GetAllNames()
        {
            return _context.Persons.Select(x => x.Name).ToList();
        }

        public void Add(Person person)
        {
            _context.Persons.Add(person);
            _context.SaveChanges();
        }


        public bool Update(Person person)
        {
            var oldPerson = Get(person.Id);
            if (oldPerson == null) return false;
            _context.Persons.AddOrUpdate(person);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var person = _context.Persons.FirstOrDefault(x => x.Id == id);
            if (person == null) return false;
            _context.Persons.Remove(person);
            _context.SaveChanges();
            return true;
        }

        public List<Person> FindAll(Func<Person, bool> predicate)
        {
            var persons = _context.Persons.Where(predicate).ToList();
            return persons;
        }

        public Person FindByFullName(Func<Person, bool> predicate)
        {
            var person = _context.Persons.Where(predicate).OrderBy(x => x.BirthDate).FirstOrDefault();
            return person;
        }

    }
}