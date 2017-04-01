using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
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

        [HttpPost]
        public ActionResult Create(PersonCreateViewModel person)
        {
            var age = CommonHelper.GetAge(person.BirthDate);
            var path = "";
            if (person.Photo.ContentLength > 0)
            {
                var fileName = Path.GetFileName(person.Photo.FileName);
                if (fileName != null)
                {
                    path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                    person.Photo.SaveAs(path);
                }
            }
            var newPerson = new Person { Name = person.Name, BirthDate = person.BirthDate, Age = age, PhotoUrl = path};
            _personRepository.Add(newPerson);
            return RedirectToAction("Index");
        }

        [HttpGet]
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