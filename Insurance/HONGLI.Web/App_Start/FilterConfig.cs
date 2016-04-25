using System.Web;
using System.Web.Mvc;

using HONGLI.Web.Controllers;

namespace HONGLI.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new ExceptionFilter());
        }
    }
}
