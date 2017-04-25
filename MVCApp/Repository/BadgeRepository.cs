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

        public BadgeRepository()
        {
            _context = new ApplicationDbContext();
        }

        public Badge Get(int id)
        {
            return _context.Badges.FirstOrDefault(x => x.Id == id);
        }

        public List<Badge> GetAll()
        {
            return _context.Badges.ToList();
        }

        public void Add(Badge badge)
        {
            _context.Badges.Add(badge);
            _context.SaveChanges();
        }

        public bool Update(Badge badge)
        {
            var oldPerson = Get(badge.Id);
            if (oldPerson == null) return false;
            _context.Badges.AddOrUpdate(badge);
            _context.SaveChanges();
            return true;
        }

        public List<Badge> FindAll(Func<Badge, bool> predicate)
        {
            var badges = _context.Badges.Where(predicate).ToList();
            return badges;
        }

        public Badge FindSingle(Func<Badge, bool> predicate)
        {
            var badge = _context.Badges.Where(predicate).FirstOrDefault();
            return badge;
        }

        public bool Delete(int id)
        {
            var badge = _context.Badges.FirstOrDefault(x => x.Id == id);
            if (badge == null) return false;
            _context.Badges.Remove(badge);
            _context.SaveChanges();
            return true;
        }
    }
}