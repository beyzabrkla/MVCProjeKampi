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
            var contactvalues = _contactService.GetList();
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

            // İletişim Sayısı her zaman gösterilir.
            ViewBag.ContactCount = _contactService.GetList().Count;

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
            ViewBag.UnreadCount = _messageService.GetUnreadMessageCountByReceiver(userMail);

            return PartialView();
        }
    }
}