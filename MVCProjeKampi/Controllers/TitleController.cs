using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class TitleController : Controller
    {
        private readonly ITitleService _titleService;
        private readonly ICategoryService _categoryService;
        private readonly IWriterService _writerService;


        public TitleController(ITitleService titleService, ICategoryService categoryService, IWriterService writerService)
        {
            _titleService = titleService;
            _categoryService = categoryService;
            _writerService = writerService;
        }


        public ActionResult Index(int page=1)
        {
            var titleValues = _titleService.GetList();

            var orderedTitles = titleValues.OrderByDescending(x => x.TitleDate).ToList();

            return View(orderedTitles.ToPagedList(page, 10));
        }

        [HttpGet]
        public ActionResult AddTitle()
        {
            List<SelectListItem> valueCategory = (from x in _categoryService.GetList() select new SelectListItem
                                                  {
                                                    Text = x.CategoryName,   // Kullanıcıya gözükecek değer (DisplayMember)
                                                    Value = x.CategoryId.ToString() // Arkada tutulacak değer (ValueMember)
                                                  }).ToList();

            List<SelectListItem> valuewriter = (from x in _writerService.GetList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.WriterName + " " + x.WriterSurname,   // Kullanıcıya gözükecek değer (DisplayMember)
                                                      Value = x.WriterId.ToString() // Arkada tutulacak değer (ValueMember)
                                                  }).ToList();

            ViewBag.vlc= valueCategory;
            ViewBag.vlw= valuewriter;
            return View(); 
        }

        [HttpPost]
        public ActionResult AddTitle(Title p)
        {
            p.TitleDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            _titleService.TitleAdd(p);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditTitle(int id)
        {
            // 1. Kategori Dropdown Listesi (Mevcut kodunuz)
            List<SelectListItem> valueCategory = (from x in _categoryService.GetList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.CategoryName,
                                                      Value = x.CategoryId.ToString()
                                                  }).ToList();
            ViewBag.vlc = valueCategory;


            // 🚀 YENİ EKLENEN BAŞLIK DURUMU İŞLEMLERİ 🚀

            // 2. Başlık durum seçeneklerini oluşturma
            List<SelectListItem> statusValues = new List<SelectListItem>()
        {
            new SelectListItem { Text = "Aktif", Value = "True" },
            new SelectListItem { Text = "Pasif", Value = "False" }
        };

            // 3. Mevcut başlık değerini çekme
            var TitleValue = _titleService.GetById(id);

            // 4. Mevcut duruma göre Dropdown'da seçili (Selected) yapma
            if (TitleValue.TitleStatus == true)
            {
                statusValues.Where(x => x.Value == "True").First().Selected = true;
            }
            else // False ise Pasif seçili gelir
            {
                statusValues.Where(x => x.Value == "False").First().Selected = true;
            }

            // 5. ViewBag ile Durum Listesini View'a taşıma
            ViewBag.vls = statusValues; // vls: Value List Status

            return View(TitleValue);
        }

        [HttpPost]
        public ActionResult EditTitle(Title p)
        {
            _titleService.TitleUpdate(p);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteTitle(int id)
        {
            var titleValue = _titleService.GetById(id); //title managerdan id ye göre değeri bul
            titleValue.TitleStatus = false;
            _titleService.TitleDelete(titleValue); //title managerdan gelen değeri sil ve ındexe yönlendir
            return RedirectToAction("Index");
        }
    }
}