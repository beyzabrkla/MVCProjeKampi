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
            // Contact sayısı
            ViewBag.ContactCount = _contactService.GetList().Count;

            // Message sayıları
            ViewBag.InboxCount = _messageService.GetListInbox().Count; 
            ViewBag.SendBoxCount = _messageService.GetListSendbox().Count; 

            return PartialView();
        }
    }
}