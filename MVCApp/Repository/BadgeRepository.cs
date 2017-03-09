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
    }
}