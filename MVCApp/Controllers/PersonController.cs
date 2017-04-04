﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
    public class PersonController : Controller
    {
        private readonly PersonRepository _personRepository;
        public PersonController()
        {
            _personRepository = new PersonRepository();
        }

        public ActionResult Index()
        {
            var persons = _personRepository.GetAll();

            Mapper.Initialize(cfg => cfg.CreateMap<Person, PersonViewModel>());
            var personViewModels = Mapper.Map<IEnumerable<Person>, IEnumerable<PersonViewModel>>(persons);

            return View(personViewModels);
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (_personRepository.Delete(id))
            {
                return Json(new {success=true});
            }

            return Json(new {error = true});
        }

        [HttpPost]
        [ValidateModelState]
        public ActionResult Create(PersonCreateViewModel person)
        {
            var newPerson = new Person
            {
                Name = person.Name,
                BirthDate = person.BirthDate,
                Age = CommonHelper.GetAge(person.BirthDate)
            };

            Mapper.Initialize(cfg => cfg.CreateMap<Person, PersonViewModel>());
            PersonViewModel newPersonViewModel;
            if (person.Photo == null)
            {
                _personRepository.Add(newPerson);
                newPersonViewModel = Mapper.Map<Person, PersonViewModel>(newPerson);
                return PartialView("Partial/_PersonTableRow", newPersonViewModel);
            }

            var fileName = Path.GetFileName(person.Photo.FileName);                        
            newPerson.PhotoUrl = fileName;
            _personRepository.Add(newPerson);

            var path = Server.MapPath(ConfigurationManager.AppSettings["PersonPhotoUploadPath"]) + newPerson.Id;
            var photoPath = path + "/" + fileName;
            Directory.CreateDirectory(path);
            person.Photo.SaveAs(photoPath);

            newPersonViewModel = Mapper.Map<Person, PersonViewModel>(newPerson);
            return PartialView("Partial/_PersonTableRow", newPersonViewModel);
        }


        [HttpPost]
        public ActionResult Edit(PersonEditViewModel person)
        {
            return PartialView("Partial/_EditPersonForm", person);
        }

        [HttpPost]
        public ActionResult SaveEdit(PersonEditViewModel person)
        {
            return RedirectToAction("Index");
        }


        public ActionResult GetPhoto(int personId, string fileName)
        {
            var photoPath = ConfigurationManager.AppSettings["PersonPhotoUploadPath"] + personId + "/" + fileName;
            return File(photoPath, MimeMapping.GetMimeMapping(fileName));
        }

        public ActionResult GetAll()
        {
            var persons = _personRepository.GetAllNames();

            var txtBuilder = new StringBuilder();
            for (var n = 0; n < persons.Count; n++)
            {
                txtBuilder.AppendLine(persons.ElementAt(n));
            }

            var txtContent = txtBuilder.ToString();
            var txtStream = new MemoryStream(Encoding.UTF8.GetBytes(txtContent));
            return File(txtStream, "text/plain", "Persons.txt");
        }

    }
}