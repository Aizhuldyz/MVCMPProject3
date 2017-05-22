using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using MVCApp.Models;
using MVCApp.Repository;

namespace MVCApp.API.Controllers
{
    [RoutePrefix("api")]
    public class PersonController : ApiController
    {
        private readonly PersonRepository _personRepository;
        private readonly RecognitionRepository _recognitionRepository;
        private readonly BadgeRepository _badgeRepository;

        public PersonController()
        {
            var dbContext = new ApplicationDbContext();
            _personRepository = new PersonRepository(dbContext);
            _recognitionRepository = new RecognitionRepository(dbContext);
            _badgeRepository = new BadgeRepository();
            _badgeRepository.SetAppContext(new ApplicationDbContext());
        }


        [Route("user/{id}")]
        public IHttpActionResult Get(int id)
        {
            var person = _personRepository.Get(id);
            if (person == null)
                return NotFound();
            return Ok(person);
        }

        [Route("users/{firstLetter:length(1)}")]
        [EnableQuery]
        public IQueryable<Person> GetByFirstLetter(string firstLetter)
        {
            var users = _personRepository.FindAll(person => person.Name.StartsWith(firstLetter));
            return new EnumerableQuery<Person>(users);
        }

        [Route("users/{name}")]
        [EnableQuery]
        public IQueryable<Person> GetByName(string name)
        {
            var users = _personRepository.FindAll(person => person.Name.StartsWith(name) || person.Name.EndsWith(name));
            return new EnumerableQuery<Person>(users);
        }

        [Route("user/{name:regex(^[a-zA-Z]+_[a-zA-z]+$)}")]
        public IHttpActionResult GetByFullName(string name)
        {
            var fullName = name.Replace("_", " ");
            var person = _personRepository.FindByFullName(p => p.Name.Equals(fullName));
            return Ok(person);
        }

        [Route("user")]
        public HttpResponseMessage Post(Person person)
        {
            _personRepository.Add(person);
            HttpResponseMessage response;
            if (person.Id == 0)
            {
                response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, person);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.Created, person);
                var uri = Url.Link("DefaultApi", new { Id = person.Id });
                response.Headers.Location = new Uri(uri);
            }
            return response;
        }

        [Route("user/{id:decimal}")]
        public HttpResponseMessage Put()
        {
            var formCollection = HttpContext.Current.Request.Form;
            HttpResponseMessage response;
            
            if (formCollection["Name"] == null|| formCollection["BirthDate"] == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                var person = new Person
                {
                    Id = int.Parse(formCollection["Id"]),
                    Name = formCollection["Name"],
                    BirthDate = DateTime.Parse(formCollection["BirthDate"])
                };
                person.PhotoUrl = _personRepository.Get(person.Id).PhotoUrl;

                if (bool.Parse(formCollection["DeletePhoto"]))
                {
                    person.PhotoUrl = null;
                }
                else if (HttpContext.Current.Request.Files["Photo"] != null)
                {
                    var httpPostedFile = HttpContext.Current.Request.Files["Photo"];
                    var fileName = Path.GetFileName(httpPostedFile.FileName);
                    person.PhotoUrl = fileName;
                    var path =
                        HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["PersonPhotoUploadPath"] +
                                                           person.Id);
                    var imagePath = $"{path}/{fileName}";
                    Directory.CreateDirectory(path);
                    httpPostedFile.SaveAs(imagePath);
                }
                response = Request.CreateResponse(_personRepository.Update(person) ? HttpStatusCode.Created 
                    : HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [Route("user/{id:decimal}")]
        public IHttpActionResult Delete(int id)
        {
            if (!_personRepository.Delete(id))
                return NotFound();
            return Ok();
        }

        [Route("user/{userId:decimal}/award/{awardId:decimal}")]
        public IHttpActionResult PostAward(int userId, int awardId)
        {
            if (_personRepository.Get(userId) == null || _badgeRepository.Get(awardId) == null)
            {
                return NotFound();
            }
            if (_recognitionRepository.GetByPersonId(userId).Any(p => p.BadgeId == awardId))
            {
                return BadRequest("User already has that badge");
            }
            if (_recognitionRepository.Add(userId, awardId))
            {
                return Ok();
            }
            return InternalServerError();
        }
        
    }
}