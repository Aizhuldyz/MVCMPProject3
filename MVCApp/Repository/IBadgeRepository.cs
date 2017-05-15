using System;
using System.Collections.Generic;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public interface IBadgeRepository
    {
        void SetAppContext(ApplicationDbContext context);

        Badge Get(int id);

        List<Badge> GetAll();

        void Add(Badge badge);

        bool Update(Badge badge);

        List<Badge> FindAll(Func<Badge, bool> predicate);

        Badge FindSingle(Func<Badge, bool> predicate);

        bool Delete(int id);

        bool HasChanges();
    }
}