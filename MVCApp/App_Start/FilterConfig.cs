using System.Web;
using System.Web.Mvc;
using MVCApp.FilterAttributes;

namespace MVCApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleExceptionAttribute());
        }
    }
}
