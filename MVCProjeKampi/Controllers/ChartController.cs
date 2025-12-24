using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
    
        private readonly IContentService _contentService;
        private readonly ITitleService _titleService;

        public ChartController(IContentService contentService, ITitleService titleService)
        {
            _contentService = contentService;
            _titleService = titleService;
        }

        public ActionResult VisualizeContentResult() 
        {
            // İçerik verilerini Başlıklara göre gruplayıp sayan sorgu
            var contentData = _contentService.GetList() // Tüm içerikleri alın
                .GroupBy(c => c.Title.TitleName) // İçeriklerdeki Başlık Adına göre gruplayın (Title navigasyon özelliğinin yüklenmiş olması gerekir - Include)
                .Select(g => new ContentCountByTitle // Yeni ViewModel'i kullanın
                {
                    TitleName = g.Key,
                    ContentCount = g.Count()
                })
                .OrderByDescending(x => x.ContentCount)
                .ToList();

            return Json(contentData, JsonRequestBehavior.AllowGet);
        }

        // Grafik HTML'ini gösterecek aksiyon
        public ActionResult TitleContentChart() 
        {
            return View();
        }
    }
}