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

        public LoginController(IAdminService adminService)
        {
            _adminService = adminService;
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

            if (admin == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
                return View();
            }

            FormsAuthentication.SetAuthCookie(admin.AdminUserName, false);
            Session["AdminUserName"] = admin.AdminUserName;
            return RedirectToAction("Index", "AdminCategory");
        }

        // Oturumu kapatma metodu
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut(); // Forms Authentication oturumunu sonlandırır
            // Session.Abandon(); // Session kullanılıyorsa Session'ı da sonlandırır.
            return RedirectToAction("Index");
        }
    }
}