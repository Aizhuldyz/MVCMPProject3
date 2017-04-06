using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class BadgeRepository
    {
        private readonly ApplicationDbContext _context;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public BadgeRepository()
        {
            _context = new ApplicationDbContext();
        }

        public Badge Get(int id)
        {
            try
            {
                return _context.Badges.FirstOrDefault(x => x.Id == id);
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while retrieving a Badge with id {id}: {e.Message}");
                return null;
            }

        }

        public List<Badge> GetAll()
        {
            return _context.Badges.ToList();
        }

        public void Add(Badge badge)
        {
            try
            {
                _context.Badges.Add(badge);
                _context.SaveChanges();
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while adding an entry to Badge with title {badge.Title}: {e.Message}");
            }
        }

        public bool Update(Badge badge)
        {
            var oldPerson = Get(badge.Id);
            if (oldPerson != null)
            {
                try
                {
                    _context.Badges.AddOrUpdate(badge);
                    _context.SaveChanges();
                }
                catch (DbException e)
                {
                    Log.Error($"Error Occured while updating Person with id: {badge.Id}: {e.Message}");
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
                var badge = _context.Badges.FirstOrDefault(x => x.Id == id);
                _context.Badges.Remove(badge);
                _context.SaveChanges();
                return true;
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while deleting an entry from Badge with id {id}: {e.Message}");
                return false;
            }
        }
    }
}