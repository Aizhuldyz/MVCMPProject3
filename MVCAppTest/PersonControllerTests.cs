using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVCApp.Controllers;
using MVCApp.Models;
using MVCApp.Repository;

namespace MVCAppTest
{
    [TestClass]
    public class PersonControllerTests
    {
        private Mock<PersonRepository> _personRepo;
        private Mock<BadgeRepository> _badgeRepo;
        private Mock<RecognitionRepository> _recognitionRepo;
        private Mock<ApplicationDbContext> _context;
        [TestInitialize]
        public void Init()
        {
            _context = new Mock<ApplicationDbContext>();
            _personRepo = new Mock<PersonRepository>(_context.Object);
            _badgeRepo = new Mock<BadgeRepository>();
            _recognitionRepo = new Mock<RecognitionRepository>(_context.Object);
            _badgeRepo.Object.SetAppContext(_context.Object);
            MVCApp.AutoMapperConfig.RegisterMappings();
        }

        [TestMethod]
        public void GetCreateShouldReturnView()
        {
            var controller = new PersonController();
            var result = controller.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void IndexShouldReturnViewWhenUserIsAnAdmin()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.User.IsInRole("Admin")).Returns(true);
            var controller = new PersonController(_context.Object, _badgeRepo.Object,
                _personRepo.Object,_recognitionRepo.Object );
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            _personRepo.Setup(x => x.GetAll()).Returns(new List<Person>());
            var result = controller.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void IndexShouldReturnViewWhenUserIsInUsers()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.User.IsInRole("Users")).Returns(true);
            context.Setup(ctx => ctx.User.IsInRole("Admin")).Returns(false);
            var controller = new PersonController(_context.Object, _badgeRepo.Object,
                _personRepo.Object, _recognitionRepo.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            _personRepo.Setup(x => x.GetAll()).Returns(new List<Person>());
            var result = controller.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void DeleteShouldReturnJsonResult()
        {
            var controller = new PersonController(_context.Object, _badgeRepo.Object,
                _personRepo.Object, _recognitionRepo.Object);
            _personRepo.Setup(x => x.Delete(2)).Returns(true);
            var result = controller.Delete(2);
            Assert.IsInstanceOfType(result, typeof(JsonResult));
        }

        [TestMethod]
        public void EditShouldReturlHttpResultWhenIdNotFound()
        {
            var controller = new PersonController(_context.Object, _badgeRepo.Object,
                _personRepo.Object, _recognitionRepo.Object);
            Person notFound = null;
            _personRepo.Setup(x => x.Get(2)).Returns(notFound);

            var result = controller.Edit(2);
            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));

        }

        [TestMethod]
        public void GetAddrecognitionShouldReturnPartialView()
        {
            var controller = new PersonController(_context.Object, _badgeRepo.Object,
                _personRepo.Object, _recognitionRepo.Object);
            _badgeRepo.Setup(x => x.GetAll())
                .Returns(new List<Badge>
                {
                    new Badge
                    {
                        Title = "title",
                        Description = "desctiption",
                        Id = 1,
                        ImageUrl = "image"
                    }
                });
            var result = controller.AddRecognition(1, "personName");
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual("Partial/_AddNewBadge", ((PartialViewResult)result).ViewName);
        }

    }
}
