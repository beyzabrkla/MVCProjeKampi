using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCProjeKampi.Attributes
{
    public class WriterAuthorizeAttribute : AuthorizeAttribute
    {
        // Kullanıcı yetkilendirilmediğinde (oturum açmadığında) çağrılır
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Yönlendirme URL'sini WriterLogin olarak ayarla
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                { "controller", "Login" },
                { "action", "WriterLogin" },
                // Kullanıcının gitmek istediği URL'yi (ReturnUrl) koru
                { "ReturnUrl", filterContext.HttpContext.Request.Url.AbsolutePath }
                });
        }
    }
}