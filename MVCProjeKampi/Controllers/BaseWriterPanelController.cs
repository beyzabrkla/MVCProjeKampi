using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCProjeKampi.Controllers
{
    public class BaseWriterPanelController : Controller
    {
        // Tüm Action metotları çalışmadan önce bu metot çalışır
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Eğer kullanıcı giriş yapmışsa (Forms Auth çerezi var), rolünü kontrol et.
            if (User.Identity.IsAuthenticated)
            {
                // BURASI KRİTİK: Sadece Writer rolü bu sayfaya erişebilir.
                // Eğer kişi AdminUserName session'ına sahipse (Admin olarak giriş yapmışsa), engelle!
                if (Session["AdminUserName"] != null)
                {
                    // Admin, Writer paneline erişmeye çalışıyor. Ana Sayfaya veya Admin Paneline at.
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            { "Controller", "Home" },
                            { "Action", "Index" }
                        });
                }
            }
        }
    }
}