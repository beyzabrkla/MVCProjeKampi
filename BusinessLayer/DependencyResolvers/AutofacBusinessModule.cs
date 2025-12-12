using Autofac; // Module sınıfı ve ContainerBuilder için gerekli
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.Abstract; // IUnitOfWork için gerekli
using DataAccessLayer.Concrete; // Context için (Opsiyonel ama temiz bir pratik)
using DataAccessLayer.EntityFramework;
using System.Reflection; // Tüm somut DAL sınıfları için gerekli

namespace BusinessLayer.DependencyResolvers
{
    // Module sınıfından kalıtım alıyoruz
    public class AutofacBusinessModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // ----------------------------------------------------------
            // A. BUSINESS LAYER (SERVICE <-> MANAGER) EŞLEŞTİRMELERİ
            // ----------------------------------------------------------

            // Tüm IService arayüzlerinin somut Manager sınıflarıyla eşleştirilmesi
            builder.RegisterType<CategoryManager>().As<ICategoryService>().InstancePerRequest();
            builder.RegisterType<TitleManager>().As<ITitleService>().InstancePerRequest();
            builder.RegisterType<AdminManager>().As<IAdminService>().InstancePerRequest();
            builder.RegisterType<MessageManager>().As<IMessageService>().InstancePerRequest();
            // ... (Elinizdeki diğer tüm Manager/Service eşleştirmeleri buraya eklenecektir)
            builder.RegisterType<ContactManager>().As<IContactService>().InstancePerRequest();
            builder.RegisterType<ContentManager>().As<IContentService>().InstancePerRequest();
            builder.RegisterType<WriterManager>().As<IWriterService>().InstancePerRequest();
            builder.RegisterType<AboutManager>().As<IAboutService>().InstancePerRequest();
            builder.RegisterType<ImageFileManager>().As<IImageFileService>().InstancePerRequest();

            // ----------------------------------------------------------
            // B. DATA ACCESS LAYER (UNIT OF WORK) EŞLEŞTİRMELERİ
            // ----------------------------------------------------------

            // UnitOfWork sınıfını IUnitOfWork arayüzü ile eşleştiriyoruz.
            // Bu, Manager'larımızın UOW'u almasını sağlar.
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();

            // Context sınıfının da her istekte yeni bir örneğini oluşturmasını sağlayabiliriz.
            // builder.RegisterType<Context>().InstancePerRequest();

            // 1. Context sınıfını Autofac'e tanıt (CRITICAL)
            builder.RegisterType<DataAccessLayer.Concrete.Context>().InstancePerRequest();

            // 2. UnitOfWork sınıfını IUnitOfWork arayüzü ile eşleştir.
            // Artık UOW'un beklediği Context'i Autofac yukarıdaki satırdan alıp verebilir.
            builder.RegisterType<DataAccessLayer.Concrete.UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();


            // ----------------------------------------------------------
            // C. VALIDATION RULES EŞLEŞTİRMELERİ (CRITICAL)
            // ----------------------------------------------------------

            // Autofac'e Controller'ların istediği Validator sınıflarını nasıl oluşturacağını söyleyin.
            builder.RegisterType<WriterValidator>().InstancePerRequest();
            builder.RegisterType<MessageValidator>().InstancePerRequest();
            builder.RegisterType<ContactValidator>().InstancePerRequest();
            builder.RegisterType<CategoryValidator>().InstancePerRequest();
        }
    }
}