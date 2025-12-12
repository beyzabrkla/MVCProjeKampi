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
    }
}
