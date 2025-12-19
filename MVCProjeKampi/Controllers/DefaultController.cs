using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    public class DefaultController : Controller
    {
        private readonly ITitleService _titleService;
        private readonly IContentService _contentService;
        public DefaultController(ITitleService titleService, IContentService contentService)
        {
            _titleService = titleService;
            _contentService = contentService;
        }

        public ActionResult Titles(int? id)
        {
            // 1. Sağ içerik için ID'yi ayarla:
            // Eğer URL'de bir ID varsa (id.HasValue), onu kullan.
            // Eğer URL'de ID yoksa (null), null olarak View'a gönder.
            ViewBag.CurrentTitleId = id;

            // 2. Sol Menü için tüm başlıkları çek.
            var titleList = _titleService.GetList();

            // 3. Sol menüdeki başlıklar için Modeli gönder
            return View(titleList);
        }

        public PartialViewResult ContentPartial(int? id)
        {
            List<Content> contentValues;

            if (id.HasValue)
            {
                // 1. Durum: ID geldi (Sol menüden tıklandı) -> Sadece o başlığın içeriklerini getir.
                // Writer ve Title nesnelerini yüklemeyi unutmayın!
                contentValues = _contentService.GetListByTitleId(id.Value);
            }
            else
            {
                // 2. Durum: ID gelmedi (Ana sayfaya girildi) -> TÜM içerikleri getir.
                // ContentManager'da tüm içeriği getiren bir metodunuz olmalı (örneğin GetList).
                contentValues = _contentService.GetList();
            }

            // Content listesini View'a gönder
            return PartialView(contentValues);
        }
    }
}