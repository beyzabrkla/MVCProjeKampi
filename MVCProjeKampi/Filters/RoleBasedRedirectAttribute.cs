using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCProjeKampi.Filters
{
    //Authorize filtresinden kalıtım alıyoruz
    public class RoleBasedRedirectAttribute :AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // İstenen Controller adını al (Örn: "WriterPanelContent", "AdminCategory")
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
           
            // Forms Authentication'ın Web.config'deki loginUrl'e otomatik yönlendirmesini durdur.
            filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            
            // Writer Paneli Kontrolü:
            // Eğer Controller adı WriterPanel" ile başlıyorsa, Yazar girişine yönlendir.
            if (controllerName.StartsWith("WriterPanel", StringComparison.OrdinalIgnoreCase))
            {
                // Yazar Giriş sayfasına yönlendir: /Login/WriterLogin
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Login" },
                        { "Action", "WriterLogin" }
                    });
                return;
            }

            // Diğerleri (Admin Controller'lar dahil) için Admin Girişi'ne yönlendir
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "Controller", "Login" },
                    { "Action", "Index" } // ADMIN GİRİŞİ (Web.config'deki loginUrl)
                });
        }
    }
}