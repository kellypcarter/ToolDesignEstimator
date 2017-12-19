using System.Web;
using System.Web.Mvc;

namespace Boeing_Tool_Design
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
