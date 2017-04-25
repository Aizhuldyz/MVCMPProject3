using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MVCApp.FilterAttributes;
using MVCApp.Helper;
using MVCApp.Models;
using MVCApp.Repository;
using MVCApp.ViewModels;

namespace MVCApp.Controllers
{
    public class BadgeController : Controller
    {
        private readonly BadgeRepository _badgeRepository;
        public BadgeController()
        {
            _badgeRepository = new BadgeRepository();
        }

        public ActionResult Index()
        {
            var badges = _badgeRepository.GetAll();
            var badgeViewModels = Mapper.Map<IEnumerable<Badge>, IEnumerable<BadgeViewModel>>(badges);

            ViewBag.PageName = "Badge";
            return View(badgeViewModels);
        }

        [Route("award/{id:decimal}/delete")]
        [HttpPost]
        [LogAction]
        [HandleException]
        public ActionResult Delete(int id)
        {
            if (_badgeRepository.Delete(id))
                return Json(new { success = "true" });
            return Json(new { error = "true" });
        }


        [Route("create-award")]
        [HttpGet]
        [LogAction]
        public ActionResult Add()
        {
            ViewBag.PageName = "Create Badge";
            return View(new BadgeCreateViewModel());
        }

        [Route("create-award")]
        [HttpPost]
        [LogAction]
        [HandleException]
        public ActionResult Add(BadgeCreateViewModel badge)
        {
            var newBadge = Mapper.Map<BadgeCreateViewModel, Badge>(badge);

            if (badge.Image == null)
            {
                _badgeRepository.Add(newBadge);
            }
            else
            {
                var fileName = Path.GetFileName(badge.Image.FileName);
                newBadge.ImageUrl = fileName;
                _badgeRepository.Add(newBadge);
                var path = GetImagePath(newBadge.Id);
                var imagePath = path + "/" + fileName;
                Directory.CreateDirectory(path);
                badge.Image.SaveAs(imagePath);
            }

            return RedirectToAction("Index");
        }

        [LogAction]
        public ActionResult GetImage(int badgeId, string fileName)
        {
            var photoPath = GetImagePath(badgeId) + "/" + fileName;
            return File(photoPath, MimeMapping.GetMimeMapping(fileName));
        }

        [Route("award/{id:decimal}/edit")]
        [HttpGet]
        [LogAction]
        public ActionResult Edit(int id)
        {
            ViewBag.PageName = "Edit";
            var badge = _badgeRepository.Get(id);
            if (badge == null)
            {
                return RedirectToAction("Index");
            }
            var badgeViewModel = Mapper.Map<Badge, BadgeEditViewModel>(badge);
            return View(badgeViewModel);
        }

        [Route("award/{id:decimal}/edit")]
        [HttpPost]
        [LogAction]
        [HandleException]
        public ActionResult Edit(BadgeEditViewModel badge)
        {
            if (badge.Image != null)
            {
                var fileName = Path.GetFileName(badge.Image.FileName);
                badge.ImageUrl = fileName;
                var path = GetImagePath(badge.Id);
                var imagePath = path + "/" + fileName;
                Directory.CreateDirectory(path);
                badge.Image.SaveAs(imagePath);
            }
            else
            {
                badge.ImageUrl = _badgeRepository.Get(badge.Id).ImageUrl;
            }
            
            var editedBadge = Mapper.Map<BadgeEditViewModel, Badge>(badge);
            _badgeRepository.Update(editedBadge);
            return RedirectToAction("Index");
        }

        [LogAction]
        [HandleException]
        public ActionResult GetBadgeInfo(int id)
        {
            var badge = _badgeRepository.Get(id);
            var badgeViewModel = Mapper.Map<Badge, BadgeViewModel>(badge);
            return PartialView("Partial/_BadgeInfoModal", badgeViewModel);
        }

        [LogAction]
        public ActionResult FindAll()
        {
            var badges = _badgeRepository.GetAll().ToList();
            var badgeViewModels = Mapper.Map<List<Badge>, List<BadgeViewModel>>(badges);
            return View("Partial/_BadgeList", badgeViewModels);
        }

        [LogAction]
        public ActionResult FindByTitle(string title)
        {
            var badges = _badgeRepository.FindAll(badge => badge.Title.Contains(title)).ToList();
            var badgeViewModels = Mapper.Map<List<Badge>, List<BadgeViewModel>>(badges);
            return View("Partial/_BadgeDetails", badgeViewModels);
        }

        [LogAction]
        public ActionResult FindByFullTitle(string title)
        {
            var fullTitle = title.Replace("_", " ");
            var badge = _badgeRepository.FindSingle(b => b.Title.Equals(fullTitle));
            var badgeViewModel = Mapper.Map<Badge, BadgeViewModel>(badge);
            return View("Partial/_BadgeInfo", badgeViewModel);
        }

        [LogAction]
        public ActionResult FindById(int id)
        {
            var badge = _badgeRepository.FindSingle(b => b.Id.Equals(id));
            var badgeViewModel = Mapper.Map<Badge, BadgeViewModel>(badge);
            return View("Partial/_BadgeInfo", badgeViewModel);
        }

        private string GetImagePath(int badgeId)
        {
            return Server.MapPath(ConfigurationManager.AppSettings["BadgeImageUploadPath"]) + badgeId;
        }

    }
}