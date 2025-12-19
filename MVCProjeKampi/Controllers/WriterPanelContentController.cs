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
    public class WriterPanelContentController : BaseWriterPanelController
    {
        private readonly IContentService _contentService;
        private readonly IWriterService _writerService;
        private readonly ITitleService _titleService;
        public WriterPanelContentController(IContentService contentService, IWriterService writerService, ITitleService titleService)
        {
            _contentService = contentService;
            _writerService = writerService;
            _titleService = titleService;
        }
        public ActionResult MyContent()
        {
            string writerMail = (string)Session["WriterMail"];
            int writerId = _writerService.GetWriterIdByMail(writerMail);
            var contentvalues = _contentService.GetListByWriter(writerId); // contentvalues tanımlandı            // Sıralanmış listeyi View'a gönder
            var orderedContentValues = contentvalues
                .OrderByDescending(x => x.ContentDate)
                .ToList();
            return View(orderedContentValues);
        }

        [HttpGet]
        public ActionResult AddContent(int? id)
        {
            if (id == null)
            {
                // Eğer TitleId gelmezse hata sayfasına veya başlık listesine yönlendirilebilir.
                // Şimdilik sadece ViewBag'e TitleId'yi atayalım.
                ViewBag.TitleId = 0;
            }
            else
            {
                ViewBag.TitleId = id; // Yakalanan TitleId'yi View'a taşıyoruz
            }

            // NOT: Bu Action'da artık Title listesini çekmeye gerek yok, çünkü TitleId URL'den geliyor.
            // Önceki yanitlamiş olduğumuz SelectList oluşturma kodunu buradan kaldırabilirsiniz.

            return View();
        }

        [HttpPost]
        public ActionResult AddContent(Content p)
        {
            p.ContentDate = DateTime.Now.Date;
            string mail = (string)Session["WriterMail"];
            if (!string.IsNullOrEmpty(mail))
            {
                int writerId = _writerService.GetWriterIdByMail(mail);
                p.WriterId = writerId;
                p.ContentStatus = true;
                _contentService.ContentAdd(p);

                return RedirectToAction("MyContent");
            }
            else
            {
                return RedirectToAction("Login", "WriterPanelLogin");
            }
        }
    }
}