using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Migrations;
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

        public Person Get(int id)
        {
            try
            {
                return _context.Persons.FirstOrDefault(x => x.Id == id);
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while retrieving a Person with id {id}: {e.Message}");
                return null;
            }

        }

        public List<string> GetAllNames()
        {
            try
            {
                return _context.Persons.Select(x => x.Name).ToList();
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while retrieving all person names: {e.Message}");
                return null;
            }
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


        public bool Update(Person person)
        {
            var oldPerson = Get(person.Id);
            if (oldPerson != null)
            {
                try
                {
                    _context.Persons.AddOrUpdate(person);
                    _context.SaveChanges();
                }
                catch (DbException e)
                {
                    Log.Error($"Error Occured while updating Person with id: {person.Id}: {e.Message}");
                    return false;
                }
                return true;
            }
            return false;
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