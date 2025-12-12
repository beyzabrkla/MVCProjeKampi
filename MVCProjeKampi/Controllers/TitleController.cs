using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
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


        public ActionResult Index()
        {
            var titleValues = _titleService.GetList();
            return View(titleValues);
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
            List<SelectListItem> valueCategory = (from x in _categoryService.GetList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.CategoryName,   // Kullanıcıya gözükecek değer (DisplayMember)
                                                      Value = x.CategoryId.ToString() // Arkada tutulacak değer (ValueMember)
                                                  }).ToList();
            ViewBag.vlc = valueCategory;
            var TitleValue = _titleService.GetById(id);   
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