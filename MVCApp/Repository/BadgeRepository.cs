using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class BadgeRepository : IBadgeRepository
    {
        private ApplicationDbContext _context;

        public virtual void SetAppContext(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual Badge Get(int id)
        {
            return _context.Badges.Find(id);
        }

        public virtual List<Badge> GetAll()
        {
            return _context.Badges.ToList();
        }

        public virtual void Add(Badge badge)
        {
            _context.Badges.Add(badge);
            _context.SaveChanges();
        }

        public virtual bool Update(Badge badge)
        {
            var oldPerson = Get(badge.Id);
            if (oldPerson == null) return false;
            _context.Badges.AddOrUpdate(badge);
            _context.SaveChanges();
            return true;
        }

        public virtual List<Badge> FindAll(Func<Badge, bool> predicate)
        {
            var badges = _context.Badges.Where(predicate).ToList();
            return badges;
        }

        public virtual Badge FindSingle(Func<Badge, bool> predicate)
        {
            var badge = _context.Badges.Where(predicate).FirstOrDefault();
            return badge;
        }

        public virtual bool Delete(int id)
        {
            var badge = Get(id);
            if (badge == null) return false;
            _context.Badges.Remove(badge);
            _context.SaveChanges();
            return true;
        }

        public virtual  bool HasChanges()
        {
            return false;
        }
    }
}