using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using MVCApp.FilterAttributes;
using MVCApp.Helper;
using MVCApp.Models;
using MVCApp.Repository;
using MVCApp.ViewModels;
using Newtonsoft.Json;

namespace MVCApp.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonRepository _personRepository;
        private readonly RecognitionRepository _recognitionRepository;
        private readonly BadgeRepository _badgeRepository;

        public PersonController()
        {
            _personRepository = new PersonRepository();
            _recognitionRepository = new RecognitionRepository();
            _badgeRepository = new BadgeRepository();
        }

        public ActionResult Index()
        {
            var persons = _personRepository.GetAll();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Person, PersonViewModel>();
                cfg.CreateMap<Recognition, BadgeViewModel>()
                    .ForMember(c => c.Id, r => r.MapFrom(rcg => rcg.Badge.Id))
                    .ForMember(c => c.Title, r => r.MapFrom(rcg => rcg.Badge.Title))
                    .ForMember(c => c.Description, r => r.MapFrom(rcg => rcg.Badge.Description))
                    .ForMember(c => c.ImageUrl, r => r.MapFrom(rcg => rcg.Badge.ImageUrl));
            });

            var mapper = config.CreateMapper();
            var personViewModels = mapper.Map<IEnumerable<Person>, IEnumerable<PersonViewModel>>(persons);

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
                BirthDate = person.BirthDate
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

            var path = GetPhotoPath(newPerson.Id);
            var photoPath = path + "/" + fileName;
            Directory.CreateDirectory(path);
            person.Photo.SaveAs(photoPath);

            newPersonViewModel = Mapper.Map<Person, PersonViewModel>(newPerson);
            return PartialView("Partial/_PersonTableRow", newPersonViewModel);
        }


        public ActionResult Edit(int id)
        {
            var person = _personRepository.Get(id);
            if (person == null)
            {
                return HttpNotFound();
            }

            Mapper.Initialize(cfg => cfg.CreateMap<Person, PersonEditViewModel>());
            var personViewModel = Mapper.Map<Person, PersonEditViewModel>(person);
            return PartialView("Partial/_EditPersonForm", personViewModel);
        }

        [HttpPost]
        [ValidateModelState]
        public ActionResult Edit(PersonEditViewModel editPerson)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<PersonEditViewModel, Person>());
            var person = Mapper.Map<PersonEditViewModel, Person>(editPerson);
            person.PhotoUrl = _personRepository.Get(person.Id).PhotoUrl;
            if (editPerson.DeletePhoto)
            {
                person.PhotoUrl = null;
            }
            else if(editPerson.Photo != null)
            {
                var fileName = Path.GetFileName(editPerson.Photo.FileName);
                person.PhotoUrl = fileName;

                var path = GetPhotoPath(editPerson.Id);
                var photoPath = path + "/" + fileName;
                Directory.CreateDirectory(path);
                editPerson.Photo.SaveAs(photoPath);
            }

            if (_personRepository.Update(person))
            {
                return Json(new {success = true});
            }
            return Json(new {error = true});
        }


        public ActionResult GetPhoto(int personId, string fileName)
        {
            if (fileName == null)
            {
                return File("~/App_Data/Uploads/default.png", "image/png");
            }
            var photoPath = GetPhotoPath(personId) + "/" + fileName;
            return File(photoPath, MimeMapping.GetMimeMapping(fileName));
        }

        public ActionResult GetAll()
        {
            var personDetails = _personRepository.GetAll().ToList();

            var txtBuilder = new StringBuilder();

            foreach (var p in personDetails)
            {
                txtBuilder.Append(p.Name);
                var recognitions = _recognitionRepository.GetByPersonId(p.Id).ToList();
                if (recognitions.Count != 0)
                {
                    txtBuilder.AppendLine(" : " + String.Join(",", recognitions.Select(x=>x.Badge.Title).ToList()));
                }
                else
                {
                    txtBuilder.AppendLine(": No badges for this person");
                }
            }

            var txtContent = txtBuilder.ToString();
            var txtStream = new MemoryStream(Encoding.UTF8.GetBytes(txtContent));
            return File(txtStream, "text/plain", "Persons.txt");
        }


        public ActionResult AddRecognition(int personId, string personName)
        {

            Mapper.Initialize(cfg => cfg.CreateMap<Badge, BadgeViewModel>());
            var badges = _badgeRepository.GetAll();
            var selectBadges = new SelectBadgesViewModel()
            {
                PersonId = personId,
                PersonName = personName,
                Badges = Mapper.Map<List<Badge>, List<BadgeViewModel>>(badges)
            };

            return PartialView("Partial/_AddNewBadge", selectBadges);
        }

        [HttpPost]
        public ActionResult AddRecognition(AddNewBadgeViewModel recognition)
        {
            if (_recognitionRepository.Exists(recognition.PersonId, recognition.BadgeId))
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Badges", new [] {"Person already has that badge"}}
                };
                var errorMessage = new JavaScriptSerializer().Serialize(errors);
                return Json(new {error = true, message = errorMessage});
            }
            if (_recognitionRepository.Add(recognition.PersonId, recognition.BadgeId))
                return Json(new {success = true});
            return new HttpStatusCodeResult(500);
        }

        private string GetPhotoPath(int personId)
        {
            return Server.MapPath(ConfigurationManager.AppSettings["PersonPhotoUploadPath"]) + personId;
        }

    }
}