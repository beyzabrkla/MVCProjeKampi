using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class WriterPanelController : BaseWriterPanelController
    {
        private readonly ITitleService _titleService;
        private readonly IWriterService _writerService;
        private readonly ICategoryService _categoryService;

        public WriterPanelController(ITitleService titleService, ICategoryService categoryService, IWriterService writerService)
        {
            _titleService = titleService;
            _categoryService = categoryService;
            _writerService = writerService;
        }

        public ActionResult WriterProfile()
        {
            return View();
        }

        public ActionResult MyTitle()
        {
            //Session'dan yazarın mail adresini al
            string p = (string)Session["WriterMail"];

            // Yazar ID'sini bulmak için IWriterService'i kullan.
            int writerId = _writerService.GetWriterIdByMail(p);

            if (writerId <= 0)
            {
                return RedirectToAction("WriterLogin", "Login");
            }

            var titleList = _titleService.GetListByWriter(writerId);
            // 3. Veriyi View'a gönder
            return View(titleList);
        }

        [HttpGet]
        public ActionResult NewTitle()
        {
            List<SelectListItem> valueCategory = (from x in _categoryService.GetList()
                                                  select new SelectListItem
                                                  {
                                                      Text = x.CategoryName,   // Kullanıcıya gözükecek değer (DisplayMember)
                                                      Value = x.CategoryId.ToString() // Arkada tutulacak değer (ValueMember)
                                                  }).ToList();
            ViewBag.vlc = valueCategory;
            return View();
        }

        [HttpPost]
        public ActionResult NewTitle(Title p)
        {
            p.TitleDate = DateTime.Today; // Veya DateTime.Now;
            p.WriterId = 4;
            p.TitleStatus = true;
            _titleService.TitleAdd(p);
            return RedirectToAction("MyTitle");
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
            return RedirectToAction("MyTitle");
        }

        public ActionResult DeleteTitle(int id)
        {
            var titleValue = _titleService.GetById(id); //title managerdan id ye göre değeri bul
            titleValue.TitleStatus = false;
            _titleService.TitleDelete(titleValue); //title managerdan gelen değeri sil ve ındexe yönlendir
            return RedirectToAction("MyTitle");
        }

        public PartialViewResult MessageListMenu()
        {
            return PartialView();
        }

        public ActionResult AllTitles(int page=1)
        {
            var titleValue = _titleService.GetList().ToPagedList(page,5);
            return View(titleValue);
        }
    }
}