using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly IImageFileService _imagefileService;

        public GalleryController(IImageFileService imagefileService)
        {
            _imagefileService = imagefileService;
        }

        public ActionResult Index()
        {
            var files = _imagefileService.GetList();
            return View(files);
        }
    }
}