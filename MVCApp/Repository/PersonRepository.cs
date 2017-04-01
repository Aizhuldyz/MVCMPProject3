using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Microsoft.Ajax.Utilities;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class PersonRepository
    {
        private readonly ApplicationDbContext _context;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PersonRepository()
        {
            _context = new ApplicationDbContext();
        }

        public IEnumerable<Person> GetAll()
        {
            return _context.Persons.ToList();
        }

        public List<string> GetAllNames()
        {
            return _context.Persons.Select(x=>x.Name).ToList();
        }

        public void Add(Person person)
        {
            try
            {
                _context.Persons.Add(person);
                _context.SaveChanges();
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while adding an entry to Person with name {person.Name}: {e.Message}");
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var person = _context.Persons.FirstOrDefault(x => x.Id == id);
                _context.Persons.Remove(person);
                _context.SaveChanges();
                return true;
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while deleting an entry from Person with id {id}: {e.Message}");
                return false;
            }
        }

    }
}