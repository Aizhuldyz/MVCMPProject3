using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ASP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MVCApp.ViewModels;
using RazorGenerator.Testing;

namespace MVCAppTest
{
    [TestClass]
    public class ViewTests
    {
        [TestMethod]
        public void IndexViewAttributeTest()
        {
            var view = new _Views_Person_Index_cshtml();
            var viewModels = new List<PersonViewModel>() { new PersonViewModel
            {
                Name = "name",
                BirthDate = new DateTime(1960, 12, 23),
                PhotoUrl = "photo",
                Id = 1
            } };
            var html = view.RenderAsHtml(viewModels);
            var node = html.DocumentNode.Element("div");
            Assert.AreEqual(node.Attributes["class"].Value, "col-md-6");
        }

        [TestMethod]
        public void PersonTablePartialTest()
        {
            var view = new _Views_Person_Partial__PersonTable_cshtml();
            var viewModels = new List<PersonViewModel>() { new PersonViewModel
            {
                Name = "name",
                BirthDate = new DateTime(1960, 12, 23),
                PhotoUrl = "photo",
                Id = 1, 
                Badges = new List<BadgeViewModel>()
            } };
            var html = view.RenderAsHtml(viewModels);
            var node = html.GetElementbyId("person_table");
            Assert.AreEqual(node.Attributes["class"].Value, "table table-striped");
        }

        [TestMethod]
        public void PersonDetailsPartialViewTest()
        {
            var view = new _Views_Person_Partial__PersonDetails_cshtml();
            var viewModel = new PersonViewModel
            {
                Name = "name",
                BirthDate = new DateTime(1960, 12, 23),
                PhotoUrl = "photo",
                Id = 1,
                Badges = new List<BadgeViewModel>()
            };

            var html = view.RenderAsHtml(viewModel);
            var node = html.DocumentNode.ChildNodes[3];
            Assert.AreEqual(node.Attributes["id"].Value, "badgeModal");
            Assert.AreEqual(node.Attributes["class"].Value, "modal");
        }
    }
}
