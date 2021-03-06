﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.OData;
using AutoMapper;
using MVCApp.Models;
using MVCApp.Repository;
using Badge = MVCApp.Models.Badge;

namespace MVCApp.API.Controllers
{
    [RoutePrefix("api")]
    public class BadgeController : ApiController
    {
        private readonly BadgeRepository _badgeRepository;

        public BadgeController()
        {
            _badgeRepository = new BadgeRepository();
            _badgeRepository.SetAppContext(new ApplicationDbContext());
        }

        [Route("awards")]
        [EnableQuery]
        public IQueryable<Badge> Get()
        {
            var awards = _badgeRepository.GetAll();
            return new EnumerableQuery<Badge>(awards);
        }

        [Route("award/{id}")]
        public IHttpActionResult Get(int id)
        {
            var award = _badgeRepository.Get(id);
            if (award == null)
                return NotFound();
            return Ok(award);
        }

        [Route("awards/{firstLetter:length(1)}")]
        [EnableQuery]
        public IQueryable<Badge> GetByFirstLetter(string firstLetter)
        {
            var awards = _badgeRepository.FindAll(award => award.Title.StartsWith(firstLetter));
            return new EnumerableQuery<Badge>(awards);
        }

        [Route("awards/{name}")]
        [EnableQuery]
        public IQueryable<Badge> GetByName(string name)
        {
            var awards = _badgeRepository.FindAll(award => award.Title.Contains(name));
            return new EnumerableQuery<Badge>(awards);
        }

        [Route("award/{name:regex(^[a-zA-Z]+_[a-zA-z]+$)}")]
        public IHttpActionResult GetByFullName(string name)
        {
            var fullTitle = name.Replace("_", " ");
            var award = _badgeRepository.FindSingle(p => p.Title.Equals(fullTitle));
            return Ok(award);
        }

        [Route("award")]
        public HttpResponseMessage Post()
        {
            var formCollection = HttpContext.Current.Request.Form;
            HttpResponseMessage response;
            if (formCollection["Title"] == null
                || !HttpContext.Current.Request.Files.AllKeys.Any() ||
                HttpContext.Current.Request.Files["Image"] == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                var httpPostedFile = HttpContext.Current.Request.Files["Image"];
                var fileName = Path.GetFileName(httpPostedFile.FileName);
                var award = new Badge
                {
                    Title = formCollection["Title"],
                    Description = formCollection["Description"],
                    ImageUrl = fileName
                };
                _badgeRepository.Add(award);
                if (award.Id == 0)
                {
                    response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, "error saving badge to db");
                }
                else
                {
                    var path =
                        HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BadgeImageUploadPath"] +
                                                           award.Id);
                    var imagePath = $"{path}/{fileName}";
                    Directory.CreateDirectory(path);
                    httpPostedFile.SaveAs(imagePath);
                    response = Request.CreateResponse(HttpStatusCode.Created, award);
                    var uri = Url.Link("DefaultApi", new {Id = award.Id});
                    response.Headers.Location = new Uri(uri);
                }                
            }
            return response;
        }

        [Route("award/{id:decimal}")]
        public IHttpActionResult Put(int id, Badge award)
        {
            award.Id = id;
            if (!_badgeRepository.Update(award))
                return NotFound();
            return Ok();
        }

        [Route("award/{id:decimal}")]
        public IHttpActionResult Delete(int id)
        {
            if (!_badgeRepository.Delete(id))
                return NotFound();
            return Ok();
        }

    }
}
