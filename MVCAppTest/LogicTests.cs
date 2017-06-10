using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVCApp;
using MVCApp.Models;
using MVCApp.Repository;

/* Badgerepository tests*/
namespace MVCAppTest
{
    [TestClass]
    public class LogicTests
    {
        private Mock<ApplicationDbContext> _context;
        private BadgeRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _context = new Mock<ApplicationDbContext>();
            _repo = new BadgeRepository();
            _repo.SetAppContext(_context.Object);
        }
        [TestMethod]
        public void CreateBadgeShouldReturnId()
        {
            var badge = new Badge
            {
                Title = "title",
                Description = "description",
                ImageUrl = "image"
            };
            var addedBadge = badge;
            addedBadge.Id = 1;
            _context.Setup(x => x.Badges.Add(badge)).Returns(addedBadge);

            _repo.Add(badge);
            Assert.AreEqual(1, badge.Id);
        }

        [TestMethod]
        public void GetShouldReturnABadge()
        {
            var badge = new Badge
            {
                Id = 1,
                Title = "title",
                Description = "description",
                ImageUrl = "image"
            };
            _context.Setup(x => x.Badges.Find(badge.Id)).Returns(badge);

            var returnedBadge = _repo.Get(1);
            Assert.AreEqual("title", returnedBadge.Title);
            Assert.AreEqual(1, returnedBadge.Id);
        }

        [TestMethod]
        public void UpdateInvalidBadgeShouldReturnFalse()
        {
            var badge = new Badge
            {
                Id = 1,
                Title = "title",
                Description = "description",
                ImageUrl = "image"
            };

            Badge returnedBadge = null;
            _context.Setup(x => x.Badges.Find(badge.Id)).Returns(returnedBadge);
            var result = _repo.Update(badge);
            Assert.AreEqual(result, false);
        }


        [TestMethod]
        public void HasChangesAllwaysReturnsFalse()
        {
            var result = _repo.HasChanges();
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DeleteBadgeWithInvalidIdShouldReturnFalse()
        {
            var badge = new Badge
            {
                Id = 1,
                Title = "title",
                Description = "description",
                ImageUrl = "image"
            };
            Badge returnedBadge = null;
            _context.Setup(x => x.Badges.Find(badge.Id)).Returns(returnedBadge);
            var result = _repo.Delete(badge.Id);
            Assert.AreEqual(result, false);
        }
    }
}
