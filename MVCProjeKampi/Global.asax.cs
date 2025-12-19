using Autofac;
using Autofac.Integration.Mvc;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.DependencyResolvers;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Unity;
using Unity.Mvc5;

namespace MVCProjeKampi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Autofac Entegrasyonu Başlar
            var builder = new ContainerBuilder();

            builder.RegisterType<BusinessLayer.ValidationRules.WriterValidator>().InstancePerRequest();

            // Controller'ları kaydedin
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Modülü (BusinessLayer'daki eşleştirmelerimizi) kaydedin
            builder.RegisterModule(new AutofacBusinessModule());

            // Container'ı oluşturun
            var container = builder.Build();

            // MVC'ye, Controller'ları bu Container üzerinden oluşturmasını söyleyin
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Kullanıcı zaten kimlik doğrulamasından geçmişse (giriş yapmışsa) işlem yapma.
            if (Context.User != null && Context.User.Identity.IsAuthenticated)
            {
                return;
            }

            // URL'yi küçük harfe çevirerek büyük/küçük harf duyarsız kontrol için hazırlarız.
            string requestedUrlLower = Context.Request.RawUrl.ToLowerInvariant();
            string writerPanel = "writerpanel";
            string writerLogin = "/login/writerlogin";


            // 1. WriterPanel içeren bir URL'ye erişim girişimi
            // "WriterPanel" kelimesini içeriyor mu? (ToLowerInvariant ile büyük/küçük harf duyarsız kontrol)
            if (requestedUrlLower.Contains(writerPanel))
            {
                // Anonim kullanıcıyı direkt Writer Girişi'ne yönlendir.
                string writerLoginUrl = "/Login/WriterLogin";

                // 2. Yönlendirme zaten Login sayfasına değilse, yönlendirmeyi zorla
                if (!requestedUrlLower.Contains(writerLogin))
                {
                    // Forms Auth'un yönlendirmesini engellemek için Response'u sonlandır
                    Response.Clear();
                    Response.Redirect(writerLoginUrl);
                    Response.End(); // Uygulama akışını burada bitir ve yönlendir
                }
            }
        }
    }
}