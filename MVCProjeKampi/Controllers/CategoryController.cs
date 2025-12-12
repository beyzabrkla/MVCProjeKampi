using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCategoryList()
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
          // cm.CategoryAddBL(p);
            CategoryValidator categoryValidator = new CategoryValidator(); //categoryvalidator dan nesne ürettik

            //validasyon sonuçları
            ValidationResult results = categoryValidator.Validate(p); //p parametresini validate metodu ile doğruluyoruz 
            if (results.IsValid)
            {
                _categoryService.CategoryAdd(p);// cm(category manager) nesnesi ile category ekleme işlemi yapıyoruz
                return RedirectToAction("GetCategoryList"); 
            }
            else
            {
                foreach (var item in results.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage); //hata mesajlarını modelstate e ekliyoruz
                }
            }
            return View();
        }
    }
}