using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
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

            Mapper.Initialize(cfg => cfg.CreateMap<Badge, BadgeViewModel>());
            var badgeViewModels = Mapper.Map<IEnumerable<Badge>, IEnumerable<BadgeViewModel>>(badges);

            ViewBag.PageName = "Badge";
            return View(badgeViewModels);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (_badgeRepository.Delete(id))
                return Json(new { success = "true" });
            return Json(new { error = "true" });
        }


        public ActionResult Add()
        {
            ViewBag.PageName = "Create Badge";
            return View(new BadgeCreateViewModel());
        }

        [HttpPost]
        public ActionResult Add(BadgeCreateViewModel badge)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<BadgeCreateViewModel, Badge>());
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
                var path = Server.MapPath(ConfigurationManager.AppSettings["BadgeImageUploadPath"]) + newBadge.Id;
                var imagePath = path + "/" + fileName;
                Directory.CreateDirectory(path);
                badge.Image.SaveAs(imagePath);
            }

            return RedirectToAction("Index");
        }

        public ActionResult GetImage(int badgeId, string fileName)
        {
            var photoPath = ConfigurationManager.AppSettings["BadgeImageUploadPath"] + badgeId + "/" + fileName;
            return File(photoPath, MimeMapping.GetMimeMapping(fileName));
        }


        public ActionResult Edit(int id)
        {
            ViewBag.PageName = "Edit";
            var badge = _badgeRepository.Get(id);
            if (badge == null)
            {
                return RedirectToAction("Index");
            }
            Mapper.Initialize(cfg => cfg.CreateMap<Badge, BadgeEditViewModel>());
            var badgeViewModel = Mapper.Map<Badge, BadgeEditViewModel>(badge);
            return View(badgeViewModel);
        }

        [HttpPost]
        public ActionResult Edit(BadgeEditViewModel badge)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<BadgeEditViewModel, Badge>());
            var editedBadge = Mapper.Map<BadgeEditViewModel, Badge>(badge);

            if (badge.DeleteImage)
            {
                editedBadge.ImageUrl = null;
            }
            else if(badge.Image != null)
            {
                var fileName = Path.GetFileName(badge.Image.FileName);
                editedBadge.ImageUrl = fileName;
                var path = Server.MapPath(ConfigurationManager.AppSettings["BadgeImageUploadPath"]) + badge.Id;
                var imagePath = path + "/" + fileName;
                Directory.CreateDirectory(path);
                badge.Image.SaveAs(imagePath);
            }
            _badgeRepository.Update(editedBadge);
            return RedirectToAction("Index");
        }


        public ActionResult GetBadgeInfo(int id)
        {
            var badge = _badgeRepository.Get(id);

            Mapper.Initialize(cfg => cfg.CreateMap<Badge, BadgeViewModel>());
            var badgeViewModel = Mapper.Map<Badge, BadgeViewModel>(badge);
            return PartialView("Partial/_BadgeInfo", badgeViewModel);
        }
    }
}