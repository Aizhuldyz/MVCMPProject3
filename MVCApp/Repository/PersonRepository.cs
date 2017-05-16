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
        private readonly string _cacheKey = ConfigurationManager.AppSettings["personsCacheKey"];
        private readonly int _cacheTimeOut = int.Parse(ConfigurationManager.AppSettings["cacheTimeOut"]);

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
            var persons = MemoryCache.Default[_cacheKey] as IEnumerable<Person>;
            if (persons == null)
            {
                persons = _context.Persons.ToList();
                MemoryCache.Default.Add(_cacheKey, persons, new DateTimeOffset(DateTime.Now.AddMinutes(_cacheTimeOut)));
            }
            return persons;
        }

        public Person Get(int id)
        {
            if (IsInCache(id))
            {
                return ((List<Person>)MemoryCache.Default[_cacheKey]).FirstOrDefault(x => x.Id == id);
            }
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
           AddToCache(person);
        }


        public bool Update(Person person)
        {
            var oldPerson = Get(person.Id);
            if (oldPerson == null) return false;
            _context.Persons.AddOrUpdate(person);
            _context.SaveChanges();
            var persons = (List<Person>)MemoryCache.Default[_cacheKey];
            if (IsInCache(person.Id))
            {
                var outdated = persons.FirstOrDefault(x => x.Id == person.Id);
                persons.Remove(outdated);
                persons.Add(person);
            }
            else if (persons == null)
            {
                persons = new List<Person> {person};
                MemoryCache.Default.Add(_cacheKey, persons, new DateTimeOffset(DateTime.Now.AddMinutes(_cacheTimeOut)));
            }
            return true;
        }

        public bool Delete(int id)
        {
            var person = _context.Persons.FirstOrDefault(x => x.Id == id);
            if (person == null) return false;
            _context.Persons.Remove(person);
            _context.SaveChanges();
            if (IsInCache(id))
            {
                var persons = (List<Person>) MemoryCache.Default[_cacheKey];
                persons.Remove(person);
            }
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

        private bool IsInCache(int personId)
        {
            var cache = MemoryCache.Default[_cacheKey] as IEnumerable<Person>;
            return cache?.ToList().FirstOrDefault(x => x.Id == personId) != null;
        }

        private void AddToCache(Person person)
        {
            var persons = (MemoryCache.Default[_cacheKey] as IEnumerable<Person>)?.ToList() ?? new List<Person> { person };
            MemoryCache.Default.Add(_cacheKey, persons, new DateTimeOffset(DateTime.Now.AddMinutes(_cacheTimeOut)));
            persons.Add(person);
            MemoryCache.Default.Add(_cacheKey, persons, new DateTimeOffset(DateTime.Now.AddMinutes(_cacheTimeOut)));
        }

    }
}