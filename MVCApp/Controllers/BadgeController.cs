using System;
using System.Collections.Generic;
using System.Globalization;
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

            return View(badgeViewModels);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (_badgeRepository.Delete(id))
                return Json(new { success = "true" });
            return Json(new { error = "true" });
        }

        [HttpPost]
        public ActionResult Add(Badge badge)
        {
            _badgeRepository.Add(badge);

            if (badge.Id == 0) return Json(new { error = "true" });
            var rowHtml = CommonHelper.GetRowHtml(badge);
            return Json(new { rowHtml });
        }
    }
}