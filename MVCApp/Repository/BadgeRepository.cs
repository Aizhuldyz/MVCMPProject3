using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class BadgeRepository
    {
        public ApplicationDbContext Context;

        public BadgeRepository()
        {
            Context = new ApplicationDbContext();
        }

        public List<Badge> GetAll()
        {
            return Context.Badges.ToList();
        }

        public void Add(Badge person)
        {
            Context.Badges.Add(person);
            Context.SaveChanges();
        }

        public void Delete(int id)
        {
            var badge = Context.Badges.FirstOrDefault(x => x.Id == id);
            Context.Badges.Remove(badge);
            Context.SaveChanges();
        }
    }
}