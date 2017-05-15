using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class CandidateBadgeRepository : IBadgeRepository
    {
        private readonly BadgeRepository _badgeRepository;
        private Dictionary<int, Tuple<OperationType, Badge>> _modifiedBadges;

        public CandidateBadgeRepository()
        {
            _modifiedBadges = new Dictionary<int, Tuple<OperationType, Badge>>();
            _badgeRepository = new BadgeRepository();
        }
        public void SetAppContext(ApplicationDbContext context)
        {
            _badgeRepository.SetAppContext(context);
        }

        public Badge Get(int id)
        {
            if (_modifiedBadges.Keys.Contains(id))
            {
                var pair = _modifiedBadges[id];
                if (pair.Item1 != OperationType.Delete)
                {
                    return pair.Item2;
                }
                    return null;
            }
            return _badgeRepository.Get(id);
        }

        public List<Badge> GetAll()
        {
            var list = _badgeRepository.GetAll();
            foreach (var info in _modifiedBadges)
            {
                var id = info.Key;
                var operationType = info.Value.Item1;
                var modifiedBadge = info.Value.Item2;
                if (operationType == OperationType.Delete)
                {
                    var badge = list.FirstOrDefault(x => x.Id == id);
                    if (badge != null)
                    {
                        list.Remove(badge);
                    }
                }
                else if (operationType == OperationType.Create)
                {
                    list.Add(modifiedBadge);
                }
                else
                {
                    var badge = list.FirstOrDefault(x => x.Id == id);
                    if (badge != null)
                    {
                        var index = list.IndexOf(badge);
                        list[index] = modifiedBadge;
                    }
                }
            }
            return list;
        }

        public void Add(Badge badge)
        {
            var existingMaxId = _badgeRepository.GetAll().Select(x => x.Id).DefaultIfEmpty().Max();        
            var maxId = _modifiedBadges.Keys.DefaultIfEmpty().Max(); 
            if (existingMaxId > maxId)
            {
                maxId = existingMaxId;
            }

            badge.Id = maxId + 1;

            _modifiedBadges[badge.Id] = Tuple.Create(OperationType.Create, badge);
        }

        public bool Update(Badge badge)
        {
            var oldPerson = Get(badge.Id);
            if (oldPerson == null) return false;

            if (_modifiedBadges.ContainsKey(badge.Id))
            {
                var operationType = _modifiedBadges[badge.Id].Item1;
                if (operationType == OperationType.Create)
                {
                    _modifiedBadges[badge.Id] = Tuple.Create(OperationType.Create, badge);
                }
                else if (operationType == OperationType.Update)
                {
                    _modifiedBadges[badge.Id] = Tuple.Create(OperationType.Update, badge);
                }
            }
            else
            {
                _modifiedBadges[badge.Id] = Tuple.Create(OperationType.Update, badge);
            }
            return true;
        }

        public List<Badge> FindAll(Func<Badge, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Badge FindSingle(Func<Badge, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            if (_modifiedBadges.ContainsKey(id))
            {
                _modifiedBadges.Remove(id);
            }
            else
            {
                var badge = _badgeRepository.Get(id);
                if (badge == null) return false;
                _modifiedBadges[id] = Tuple.Create(OperationType.Delete, badge);
            }
            return true;
        }

        public void SaveChanges()
        {
            foreach (var info in _modifiedBadges)
            {
                var id = info.Key;
                var operationType = info.Value.Item1;
                var modifiedBadge = info.Value.Item2;
                if (operationType == OperationType.Delete)
                {
                    _badgeRepository.Delete(id);
                }
                else if (operationType == OperationType.Create)
                {
                    _badgeRepository.Add(modifiedBadge);
                }
                else
                {
                    _badgeRepository.Update(modifiedBadge);
                }
            }
            _modifiedBadges = new Dictionary<int, Tuple<OperationType, Badge>>();
        }

        public bool HasChanges()
        {
            return _modifiedBadges.Any();
        }
    }
    
    public enum OperationType
    {
        Create = 0,
        Update = 1,
        Delete = 2
    }
}