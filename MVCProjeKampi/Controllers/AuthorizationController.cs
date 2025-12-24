using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IAdminService _adminService;

        public AuthorizationController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public ActionResult Index()
        {
            // 3. Servis üzerinden verileri çekin
            var adminvalues = _adminService.GetList();

            // 4. Veriyi View'a gönderin
            return View(adminvalues);
        }

        [HttpGet]
        public ActionResult AddAdmin()
        {
            // Yöneticinin seçebileceği rol seçeneklerini tanımlıyoruz.
            List<SelectListItem> roleValues = new List<SelectListItem>()
            {
                // Value: Veritabanına kaydedilecek değer ('a', 'b', vb.)
                // Text: Kullanıcının Dropdown List'te göreceği açıklama
                new SelectListItem { Text = "Ana Yönetici (a)", Value = "a" },
                new SelectListItem { Text = "Normal Yönetici (b)", Value = "b" }
                // İhtiyaca göre başka roller ekleyebilirsiniz.
            };

            // Seçenekleri View tarafına taşımak için ViewBag kullanıyoruz.
            ViewBag.vlr = roleValues;

            return View();

        }

        [HttpPost]
        public ActionResult AddAdmin(Admin p)
        {
            _adminService.AdminAdd(p);
            return RedirectToAction("Index", "Authorization"); // Admin listesi sayfasına yönlendir
        }

        [HttpGet]
        public ActionResult EditAdmin(int id)
        {
            // 1. Yetki (Role) Seçenekleri
            // Tablonuzdaki 'a' ve 'b' yetkilerine göre oluşturulmuştur.
            List<SelectListItem> roleValues = new List<SelectListItem>()
            {
                // Örnek isimlendirme
                new SelectListItem { Text = "Normal Yönetici (b)", Value = "b" },
                new SelectListItem { Text = "Ana Yönetici (a)", Value = "a" }
            };

            // 2. İlgili Yöneticinin Verisini Getirme
            var adminValue = _adminService.GetByID(id);

            // 3. Mevcut Yetkiye Göre Role Dropdown'unu Seçili Yapma
            // adminValue nesnesi üzerindeki yetki alanının adının 'AdminRole' olduğunu varsayıyoruz.
            roleValues.Where(x => x.Value == adminValue.AdminRole).First().Selected = true;

            // 4. View'a veriyi taşıma
            ViewBag.vlr = roleValues;   // Yetki/Rol dropdown'u için

            return View(adminValue);
        }

        [HttpPost]
        public ActionResult EditAdmin(Admin p) // 'Category p' yerine 'Admin p' olmalı
        {
            // Gelen güncel Admin nesnesini (p) veritabanına kaydet
            _adminService.AdminUpdate(p);

            // İşlem bitince Admin listesi sayfasına geri dön
            return RedirectToAction("Index");
        }
    }
}