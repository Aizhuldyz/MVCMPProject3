using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using MVCApp.FilterAttributes;
using MVCApp.Models;
using MVCApp.Repository;
using MVCApp.ViewModels;

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

        [LogAction]
        public ActionResult Index()
        {
            var persons = _personRepository.GetAll();
            var personViewModels = Mapper.Map<IEnumerable<Person>, IEnumerable<PersonViewModel>>(persons);
            return View(personViewModels);
        }

        [HttpPost]
        [Route("user/{id:decimal}/delete")]        
        [LogAction]
        [HandleException]
        public ActionResult Delete(int id)
        {
            if (_personRepository.Delete(id))
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }


        [Route("create-user")]
        [LogAction]
        public ActionResult Create()
        {
            return View("Partial/_CreatePersonForm");
        }

        [HttpPost]
        [ValidateModelState]
        [LogAction]
        [HandleException]
        public ActionResult Create(PersonCreateViewModel person)
        {
            var newPerson = new Person
            {
                Name = person.Name,
                BirthDate = person.BirthDate
            };
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

        [LogAction]
        public ActionResult EditById(int id)
        {
            var person = _personRepository.Get(id);
            if (person == null)
            {
                return HttpNotFound();
            }

            var personViewModel = Mapper.Map<Person, PersonEditViewModel>(person);
            return View("Partial/_EditPersonForm", personViewModel);
        }

        public ActionResult Edit(int id)
        {
            var person = _personRepository.Get(id);
            if (person == null)
            {
                return HttpNotFound();
            }

            var personViewModel = Mapper.Map<Person, PersonEditViewModel>(person);
            return PartialView("Partial/_EditPersonForm", personViewModel);
        }

        [HttpPost]
        [ValidateModelState]
        [LogAction]
        public ActionResult Edit(PersonEditViewModel editPerson)
        {
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

        [LogAction]
        public ActionResult GetPhoto(int personId, string fileName)
        {
            if (fileName == null)
            {
                return File("~/App_Data/Uploads/default.png", "image/png");
            }
            var photoPath = GetPhotoPath(personId) + "/" + fileName;
            return File(photoPath, MimeMapping.GetMimeMapping(fileName));
        }

        [LogAction]
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

        [LogAction]
        public ActionResult AddRecognition(int personId, string personName)
        {
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
        [LogAction]
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

        [Route("users")]
        [LogAction]
        public ActionResult FindAll()
        {
            var persons = _personRepository.GetAll().ToList();
            var personViewModels = Mapper.Map<List<Person>, List<PersonViewModel>>(persons);
            return View("Partial/_Personlist", personViewModels);
        }

        [LogAction]
        public ActionResult FindByName(string name)
        {
            var persons = _personRepository.FindAll(person => person.Name.Contains(name));
            var personViewModels = Mapper.Map<List<Person>, List<PersonViewModel>>(persons);            
            return View("Partial/_Personlist", personViewModels);
        }


        [LogAction]
        public ActionResult FindByFullName(string name)
        {
            var fullName = name.Replace("_", " ");
            var person = _personRepository.FindByFullName(p => p.Name.Equals(fullName));
            var personViewModel = Mapper.Map<Person, PersonViewModel>(person);
            return View("Partial/_PersonDetails", personViewModel);
        }

        [Route("user/{id:decimal}")]
        [LogAction]
        public ActionResult FindById(int id)
        {
            var person = _personRepository.Get(id);
            var personViewModel = Mapper.Map<Person, PersonViewModel>(person);
            return View("Partial/_PersonDetails", personViewModel);
        }

        private string GetPhotoPath(int personId)
        {
            return Server.MapPath(ConfigurationManager.AppSettings["PersonPhotoUploadPath"]) + personId;
        }


    }
}