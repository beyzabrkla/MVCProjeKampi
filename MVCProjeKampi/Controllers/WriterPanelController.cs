using BusinessLayer.Abstract;
using BusinessLayer.ValidationRules;
using EntityLayer.Concrete;
using FluentValidation.Results;
using MVCProjeKampi.Attributes;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [WriterAuthorize]
    public class WriterPanelController : BaseWriterPanelController
    {
        private readonly ITitleService _titleService;
        private readonly IWriterService _writerService;
        private readonly ICategoryService _categoryService;
        private readonly IContentService _contentService;
        WriterValidator writerValidator = new WriterValidator();


        public WriterPanelController(ITitleService titleService, ICategoryService categoryService, IWriterService writerService, WriterValidator writerValidator, IContentService contentService)
        {
            _titleService = titleService;
            _categoryService = categoryService;
            _writerService = writerService;
            _contentService = contentService;
            this.writerValidator = writerValidator;
        }

        [HttpGet]
        public ActionResult WriterProfile()
        {
            string mail = GetUserMail();
            int writerId = _writerService.GetWriterIdByMail(mail);
            var writervalue = _writerService.GetById(writerId);
            return View(writervalue);
        }

        [HttpPost]
        public ActionResult WriterProfile(Writer p)
        {
            ValidationResult results = writerValidator.Validate(p);
            if (results.IsValid)
            {
                _writerService.WriterUpdate(p);
                return RedirectToAction("AllTitles","WriterPanel");
            }
            else
            {
                foreach (var x in results.Errors)
                {
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                }
            }
            return View();
        }

        public ActionResult MyTitle()
        {
            //Session'dan yazarın mail adresini al
            string p = GetUserMail();

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
            p.TitleDate = DateTime.Now;
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
                                                      Text = x.CategoryName,
                                                      Value = x.CategoryId.ToString()
                                                  }).ToList();
            ViewBag.vlc = valueCategory;

            List<SelectListItem> statusValues = new List<SelectListItem>()
        {
            new SelectListItem { Text = "Aktif", Value = "True" },
            new SelectListItem { Text = "Pasif", Value = "False" }
        };

            var TitleValue = _titleService.GetById(id);

            if (TitleValue.TitleStatus == true)
            {
                statusValues.Where(x => x.Value == "True").First().Selected = true;
            }
            else 
            {
                statusValues.Where(x => x.Value == "False").First().Selected = true;
            }

            ViewBag.vls = statusValues;

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
        public ActionResult WriterContents(int id)
        {
            // 1. O anki başlığın ID'si
            ViewBag.TitleId = id;

            // 2. Servis metodu ile filtrelenmiş içerikleri çek
            // Bu metodunuzun Writer ve Title nesnelerini EAGER LOADING ile getirdiğinden emin olun (Include).
            var contentValues = _contentService.GetListByTitleId(id);

            // 3. View'a gönder
            return View(contentValues);
        }
    }
}