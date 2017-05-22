using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using MVCApp.Models;

namespace MVCApp.Repository
{
    public class RecognitionRepository
    {
        private readonly ApplicationDbContext _context;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _cacheKey = ConfigurationManager.AppSettings["personsCacheKey"];

        public RecognitionRepository()
        {
            _context = new ApplicationDbContext();
        }

        public RecognitionRepository(ApplicationDbContext context)
        {
            _context =context;
        }

        public bool Exists(int personId, int badgeId)
        {
            if (_context.Recognitions.FirstOrDefault(x => x.PersonId == personId && x.BadgeId == badgeId) != null)
            {
                return true;
            }
            return false;
        }

        public bool Add(int personId, int badgeId)
        {
            var recognition = new Recognition
            {
                PersonId = personId,
                BadgeId = badgeId
            };
            try
            {
                _context.Recognitions.Add(recognition);
                _context.SaveChanges();
                MemoryCache.Default.Remove(_cacheKey);
                return true;
            }
            catch (DbException e)
            {
                Log.Error($"Error Occured while adding a recognition to Person with id: {personId}: {e.Message}");
            }
            return false;
        }

        public IEnumerable<Recognition> GetByPersonId(int id)
        {
            return _context.Recognitions.Where(x => x.PersonId == id).ToList();
        }

        public IEnumerable<Recognition> GetAll()
        {
            return _context.Recognitions.ToList();
        }
    }
}