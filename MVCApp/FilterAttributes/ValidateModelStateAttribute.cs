using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MVCApp.FilterAttributes
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                var errors = filterContext.Controller.ViewData.ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                var json = new JavaScriptSerializer().Serialize(errors);

                // send 400 status code (Bad Request)
                filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, json);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}