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
    public class AboutController : Controller
    {
        private readonly IAboutService _aboutService;


        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public ActionResult Index()
        {
            var aboutvalues = _aboutService.GetList();    
            return View(aboutvalues);
        }

        [HttpGet]
        public ActionResult AboutAdd()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult AboutAdd(About p)
        {
            _aboutService.AboutAdd(p);
            return RedirectToAction("Index");
        }

        public PartialViewResult AboutPartial()
        {
            return PartialView();
        }
    }
}