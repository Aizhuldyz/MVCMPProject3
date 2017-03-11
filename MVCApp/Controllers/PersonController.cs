using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using AutoMapper;
using MVCApp.Helper;
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
            if (_personRepository.Delete(id))
            {
                return Json(new {success="true"});
            }

            return Json(new {error = "true"});
        }

        [HttpPost]
        public ActionResult Add(string name, string birthdate)
        {
            var birthDate = DateTime.ParseExact(birthdate.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var age = CommonHelper.GetAge(birthDate);
            var person = new Person {Name = name, BirthDate = birthDate, Age = age};            
            _personRepository.Add(person);

            if (person.Id == 0) return Json(new {error = "true"});
            var rowHtml = CommonHelper.GetRowHtml(person);
            return Json(new {rowHtml});
        }

        
    }
}