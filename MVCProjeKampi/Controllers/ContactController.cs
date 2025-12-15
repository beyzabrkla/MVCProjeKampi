using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IMessageService _messageService;

        ContactValidator cv = new ContactValidator();

        public ContactController(IContactService contactService, IMessageService messageService, ContactValidator cv)
        {
            _contactService = contactService;
            _messageService = messageService;
            this.cv = cv;
        }

        public ActionResult Index()
        {
            var contactvalues = _contactService.GetList().Where(x => x.IsTrash == false).ToList();
            return View(contactvalues);
        }
        
        public ActionResult GetContactDetails(int id)
        {
            var contactvalues =_contactService.GetById(id);
            return View(contactvalues);
        }

        public PartialViewResult ContactPartial()
        {
            string userMail = (string)Session["AdminMail"];

            // Oturum yoksa, mesaj sayaçları 0 olmalıdır.
            if (string.IsNullOrEmpty(userMail))
            {
                ViewBag.InboxCount = 0;
                ViewBag.SendBoxCount = 0;
                ViewBag.UnreadCount = 0;
                return PartialView();
            }

            // Mesaj sayaçları oturumdaki kullanıcıya göre filtrele
            ViewBag.InboxCount = _messageService.GetListInbox(userMail).Count;
            ViewBag.SendBoxCount = _messageService.GetListSendbox(userMail).Count;
            ViewBag.ContactCount = _contactService.GetContactCountNonTrash();
            ViewBag.UnreadCount = _messageService.GetUnreadMessageCountByReceiver(userMail);
            ViewBag.TrashCount = _messageService.GetTrashMessageCountByMail(userMail);
            ViewBag.DraftCount = _messageService.GetDraftMessageCountBySender(userMail); 

            return PartialView();
        }

        public ActionResult ContactMoveToTrash(int id)
        {
            _contactService.ContactMoveToTrash(id);

            return RedirectToAction("TrashBox", "Message");
        }

        public ActionResult ContactRestore(int id)
        {
            // ContactManager'daki yeni metodu çağırıyoruz.
            _contactService.ContactRestore(id);

            // Geri yüklemeden sonra kullanıcıyı Çöp Kutusu listesine yönlendir
            return RedirectToAction("TrashBox", "Message");
        }

        public ActionResult ContactHardDelete(int id)
        {
            // Öncelikle mesajı ID ile Business katmanından çekiyoruz.
            var contactValue = _contactService.GetById(id);

            if (contactValue != null)
            {
                // Mesajı silme metodunu çağırıyoruz.
                _contactService.ContactDelete(contactValue);

                // Silme işleminden sonra kullanıcıyı tekrar çöp kutusuna yönlendiriyoruz.
                return RedirectToAction("TrashBox");
            }

            // Mesaj bulunamazsa hata sayfasına veya ana sayfaya yönlendirilebilir.
            return RedirectToAction("TrashBox");
        }
    }
}