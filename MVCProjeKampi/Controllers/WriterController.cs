using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]

    public class WriterController : Controller
    {
        private readonly IWriterService _writerService;
        WriterValidator writerValidator = new WriterValidator();

        public WriterController(IWriterService writerService, WriterValidator writerValidator)
        {
            _writerService = writerService;
            this.writerValidator = writerValidator;
        }

        public ActionResult Index()
        {
            var writerValues = _writerService.GetList();
            return View(writerValues);
        }

        [HttpGet]
        public ActionResult AddWriter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddWriter(Writer p)
        {
            ValidationResult results = writerValidator.Validate(p);
            if(results.IsValid)
            {
                _writerService.WriterAdd(p);
                return RedirectToAction("Index");
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

        [HttpGet]
        public ActionResult EditWriter(int id)
        {
            var writerValue = _writerService.GetById(id);
            return View(writerValue);
        }

        [HttpPost]
        public ActionResult EditWriter(Writer p)
        {
            ValidationResult results = writerValidator.Validate(p);
            if (results.IsValid)
            {
                _writerService.WriterUpdate(p);
                return RedirectToAction("Index");
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

        [HttpGet]
        public ActionResult DeleteWriter(int id)
        {
            // Writer'ı ID ile Business katmanından çekiyoruz.
            var writerValue = _writerService.GetById(id);

            if (writerValue != null)
            {
                // Yazarı silme (durumunu pasifize etme) metodunu çağırıyoruz.
                // Eğer Manager'da sadece ID ile çalışan metot varsa, onu çağırın:
                // _writerService.WriterSetStatusToFalse(id); 

                // Eğer Manager'da entity ile çalışan metot varsa, onu çağırın:
                _writerService.WriterDelete(writerValue);

                // İşlem tamamlandıktan sonra kullanıcıyı yazar listesine yönlendiriyoruz.
                return RedirectToAction("Index");
            }

            // Yazar bulunamazsa yine listeye dön.
            return RedirectToAction("Index");
        }
    }
}