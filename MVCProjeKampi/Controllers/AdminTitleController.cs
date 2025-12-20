using BusinessLayer.Abstract;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using EntityLayer.Concrete;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class AdminTitleController : Controller
    {
        private readonly ITitleService _titleService;
        private readonly ICategoryService _categoryService;
        private readonly IWriterService _writerService;

        public AdminTitleController(ITitleService titleService, ICategoryService categoryService, IWriterService writerService)
        {
            _titleService = titleService;
            _categoryService = categoryService;
            _writerService = writerService;
        }

        public ActionResult TitlesByCategory(int id, int page = 1)
        {
            var titles = _titleService.GetTitlesByCategoryId(id);

            var category = _categoryService.GetById(id);
            if (category != null)
            {
                ViewBag.CategoryName = category.CategoryName;
                ViewBag.CategoryId = id;
            }

            var orderedTitles = titles.OrderByDescending(x => x.TitleDate).ToList();

            return View(orderedTitles.ToPagedList(page, 10));
        }

    }
}