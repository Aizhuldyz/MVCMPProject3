﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using AutoMapper;
using MVCApp.Models;
using MVCApp.Repository;

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
            _personRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Add(string name, string birthdate)
        {
            var birthDate = DateTime.ParseExact(birthdate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var age = DateTime.Now.Year - birthDate.Year;
            var person = new Person {Name = name, BirthDate = birthDate, Age = age};
            _personRepository.Add(person);
            return RedirectToAction("Index");
        }
    }
}