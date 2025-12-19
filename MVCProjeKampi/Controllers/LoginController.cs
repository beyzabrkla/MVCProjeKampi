using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCProjeKampi.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // DAL yerine BLL'e bağımlı oluyoruz.
        // DI (Dependency Injection) yapısını taklit ederek Manager'ı oluşturuyoruz:
        private readonly IAdminService _adminService;
        private readonly IWriterService _writerService;

        public LoginController(IAdminService adminService, IWriterService writerService)
        {
            _adminService = adminService;
            _writerService = writerService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Admin p)
        {
            if (string.IsNullOrWhiteSpace(p.AdminUserName) || string.IsNullOrWhiteSpace(p.AdminPassword))
            {
                ViewBag.ErrorMessage = "Kullanıcı adı ve şifre zorunludur.";
                return View();
            }

            var admin = _adminService.AdminLogin(p.AdminUserName, p.AdminPassword);

            if (admin != null)
            {
                FormsAuthentication.SetAuthCookie(admin.AdminUserName, false);

                Session["AdminUserName"] = admin.AdminUserName;

                // KRİTİK EKLENTİ: SenderMail'in NULL geçmesini engeller
                Session["AdminMail"] = admin.AdminUserName; // <-- ARTIK DOĞRU MAİL ADRESİNİ ÇEKECEK
                return RedirectToAction("Index", "AdminCategory");
            }

            ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        // Oturumu kapatma metodu
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut(); // Forms Authentication oturumunu sonlandırır
            Session.Abandon(); // Session kullanılıyorsa Session'ı da sonlandırır.
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public ActionResult WriterLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult WriterLogin(Writer p)
        {
            var writer = _writerService.WriterLogin(p.WriterMail, p.WriterPassword);

            if (writer != null)
            {
                FormsAuthentication.SetAuthCookie(writer.WriterMail, false);

                // Session'a ATAMA işlemini yapın. (Bu satır önemli!)
                // Bu, yazarın mail adresini (string) oturumda tutar.
                Session["WriterMail"] = writer.WriterMail;
                Session["WriterImage"] = writer.WriterImage; // <-- Bu değer doğru URL olmalı
                // Hata veren okuma satırını (string mail = (string)Session["WriterMail"];) KALDIRIN. 
                // Zaten yönlendirme yapacaksınız, burada okumaya ihtiyacınız yok.

                return RedirectToAction("MyContent", "WriterPanelContent");
            }

            ViewBag.ErrorMessage = "Mail Adresi veya şifre hatalı.";
            return View();
        }

        // Oturumu kapatma metodu
        public ActionResult WriterLogOut()
        {
            FormsAuthentication.SignOut(); // Forms Authentication oturumunu sonlandırır
            Session.Abandon(); // Session kullanılıyorsa Session'ı da sonlandırır.
            return RedirectToAction("WriterLogin", "Login");
        }
    }
}