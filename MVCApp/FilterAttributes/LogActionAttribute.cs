using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.FilterAttributes
{
    public class LogActionAttribute : ActionFilterAttribute
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Log.Info($"Action {filterContext.ActionDescriptor.ActionName} of {filterContext.ActionDescriptor.ControllerDescriptor.ControllerName} started on {DateTime.Now:MM/dd/yyyy}");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log.Info($"Action {filterContext.ActionDescriptor.ActionName} of {filterContext.ActionDescriptor.ControllerDescriptor.ControllerName} finished on {DateTime.Now:MM/dd/yyyy}");
        }
    }
}