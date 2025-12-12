using Antlr.Runtime.Misc;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using FluentValidation.Results;
using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageservice;
        private readonly IContactService _contactService; // Yeni bağımlılık eklendi
        MessageValidator messagevalidator = new MessageValidator();

        public MessageController(IMessageService messageservice, IContactService contactService, MessageValidator messagevalidator)
        {
            _messageservice = messageservice;
            _contactService = contactService;
            this.messagevalidator = messagevalidator;
        }

        public ActionResult Inbox(string status)
        {
            string userMail = (string)Session["AdminMail"];
            List<Message> messageList;

            if (string.IsNullOrEmpty(userMail))
            {
                // Eğer mail adresi çekilemezse, boş bir liste döndür veya yetki hatası ver.
                return RedirectToAction("Login", "Admin"); // Örn: Giriş sayfasına yönlendir
            }

            //status değişkeni metodun parametresi olarak tanımlandı.
            if (status == "unread")
            {
                // Okunmamışları filtrele
                messageList = _messageservice.GetListUnreadInbox(userMail);
            }
            else
            {
                //Tüm gelen mesajları getir (default)
                messageList = _messageservice.GetListInbox(userMail);
            }
            return View(messageList);
        }

        public ActionResult SendBox()
        {
            string userMail = (string)Session["AdminMail"]; // Session'dan mail çekilmeli
            var messagelist = _messageservice.GetListSendbox(userMail);
            return View(messagelist);
        }

        public ActionResult GetInboxMessageDetails(int id)
        {
            var values = _messageservice.GetById(id);
            if (values != null && values.IsRead == false)
            {
                values.IsRead = true;
                _messageservice.MessageUpdate(values);
            }
            return View(values);
        }

        public ActionResult GetSendboxMessageDetails(int id)
        {
            var values = _messageservice.GetById(id);
            if (values != null && values.IsRead == false) // Mesaj okunmadıysa
            {
                values.IsRead = true; // Okundu olarak işaretle
                _messageservice.MessageUpdate(values); // Veritabanına kaydet
            }
            return View(values);
        }

        [HttpGet]
        public ActionResult NewMessage(int? id)
        {
            if (id.HasValue)
            {
                var draft = _messageservice.GetById(id.Value);
                return View(draft); // Taslak içeriğini formda gösterecek
            }
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewMessage(Message p, string action)
        {
            p.SenderMail = "admin@gmail.com";
            p.MessageDate = DateTime.Now;
            var sanitizer = new HtmlSanitizer(); //mesaj içeriğindeki zararlı yazılımlar için 
            p.MessageContent = sanitizer.Sanitize(p.MessageContent);

            if (action == "send")
            {
                // GÖNDERME İŞLEMİ: Fluent Validation kontrolü yapılır.
                ValidationResult results = messagevalidator.Validate(p);

                if (results.IsValid)
                {
                    p.IsDraft = false;
                    _messageservice.MessageAdd(p);
                    return RedirectToAction("SendBox");
                }
                else
                {
                    foreach (var x in results.Errors)
                    {
                        ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                    }
                    return View(p);
                }
            }
            else if (action == "save")
            {
                p.IsDraft = true;
                _messageservice.MessageAdd(p);
                return RedirectToAction("Drafts");
            }
            return View(p);
        }

        public ActionResult Drafts()
        {
            var draftMessages = _messageservice.GetDraftMessages();
            return View(draftMessages);
        }

        [ChildActionOnly]
        public ActionResult MailboxMenu()
        {
            string userMail = (string)Session["AdminMail"];

            // 1. İletişim Sayısı (Kullanıcıdan bağımsızdır, her zaman gösterilir)
            ViewBag.ContactCount = _contactService.GetList().Count;

            if (string.IsNullOrEmpty(userMail))
            {
                // Oturum yoksa mesaj sayaçlarını 0 yap
                ViewBag.InboxTotalCount = 0;
                ViewBag.SendBoxCount = 0;
                ViewBag.InboxUnreadCount = 0;
                return PartialView();
            }

            // 2. Mesaj Sayaçları (Sadece oturumdaki mail adresine göre)
            ViewBag.InboxTotalCount = _messageservice.GetListInbox(userMail).Count;
            ViewBag.SendBoxCount = _messageservice.GetListSendbox(userMail).Count;
            ViewBag.InboxUnreadCount = _messageservice.GetUnreadMessageCountByReceiver(userMail);

            return PartialView();
        }
    }
}