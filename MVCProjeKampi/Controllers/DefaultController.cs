using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using PagedList;
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
            // 1. Oturum Kontrolü:
            // Eğer kullanıcı oturum açmamışsa (WriterMail session'ı boşsa)
            if (Session["WriterMail"] == null)
            {
                // Kullanıcıyı Yazar Giriş sayfasına yönlendir
                return RedirectToAction("WriterLogin", "Login");
            }

            // 2. Eğer oturum açıksa, normal işleme devam et:

            ViewBag.CurrentTitleId = id;

            // NOT: Yazar girişi yapıldıktan sonra tüm başlıkları değil, 
            // sadece ilgili ID'ye ait başlıkları listelemek daha mantıklı olacaktır.
            // Ancak sizin mevcut kodunuz tüm listeyi getirdiği için, şimdilik bu kısmı koruyorum:
            var titleList = _titleService.GetList();

            return View(titleList);
        }

        // Düzeltme: id parametresini int? olarak değiştiriyoruz.
        public PartialViewResult ContentPartial(int? id)
        {
            // 1. ID boş (null) ise, ilk aktif başlığın ID'sini varsayılan olarak al.
            if (id == null)
            {
                var firstTitle = _titleService.GetList().FirstOrDefault(x => x.TitleStatus == true);
                if (firstTitle != null)
                {
                    id = firstTitle.TitleId;
                }
                else
                {
                    // Hiç başlık yoksa boş liste gönder
                    return PartialView(new List<Content>());
                }
            }

            // 2. İçerikleri çek (id artık null değildir)
            var contents = _contentService.GetListByTitleId(id.Value); // id.Value ile int değerini alıyoruz

            // 3. View'a gönder
            return PartialView(contents);
        }
    }
}