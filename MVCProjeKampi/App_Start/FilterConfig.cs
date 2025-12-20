using System.Web;
using System.Web.Mvc;
using MVCProjeKampi.Filters;

namespace MVCProjeKampi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

        }
    }
}
