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
    [Authorize(Roles ="B")]
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            var categoryValues = _categoryService.GetList();
            return View(categoryValues);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(Category p)
        {
            CategoryValidator categoryValidator = new CategoryValidator(); //categoryvalidator dan nesne ürettik
            ValidationResult results = categoryValidator.Validate(p); //p parametresini validate metodu ile doğruluyoruz
            if (results.IsValid)
            {
                _categoryService.CategoryAdd(p);// cm(category manager) nesnesi ile category ekleme işlemi yapıyoruz
                return RedirectToAction("Index");

            }
            else
            {
                foreach (var x in results.Errors)
                {
                    ModelState.AddModelError(x.PropertyName, x.ErrorMessage); //hata mesajlarını modelstate e ekliyoruz
                }
            }
            return View();
        }

        public ActionResult DeleteCategory(int id)
        {
            var categoryValue= _categoryService.GetById(id);
            _categoryService.CategoryDelete(categoryValue);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            var categoryValue = _categoryService.GetById(id);
            return View(categoryValue);
        }

        [HttpPost]
        public ActionResult EditCategory(Category p)
        {
            _categoryService.CategoryUpdate(p);
           return RedirectToAction("Index");
        }
    }
}