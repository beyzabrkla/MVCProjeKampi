using BusinessLayer.Abstract;
using BusinessLayer.ValidationRules;
using EntityLayer.Concrete;
using FluentValidation.Results;
using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class WriterPanelMessageController : BaseWriterPanelController
    {
        private readonly IMessageService _messageService;
        private readonly IContactService _contactService;
        MessageValidator messagevalidator = new MessageValidator();


        public WriterPanelMessageController(IMessageService messageService, IContactService contactService, MessageValidator messagevalidator)
        {
            _messageService = messageService;
            _contactService = contactService;
            this.messagevalidator = messagevalidator;
        }

        public ActionResult Inbox(string status)
        {
            string userMail = (string)Session["WriterMail"];
            List<Message> messageList;

            if (string.IsNullOrEmpty(userMail))
            {
                // Eğer mail adresi çekilemezse, boş bir liste döndür veya yetki hatası ver.
                return RedirectToAction("Index", "Login"); // Örn: Giriş sayfasına yönlendir
            }

            //status değişkeni metodun parametresi olarak tanımlandı.
            if (status == "unread")
            {
                // Okunmamışları filtrele
                messageList = _messageService.GetListUnreadInbox(userMail);
            }
            else
            {
                //Tüm gelen mesajları getir (default)
                messageList = _messageService.GetListInbox(userMail);
            }
            return View(messageList);
        }

        public PartialViewResult MessageListMenu()
        {
            return PartialView();
        }

        public ActionResult SendBox()
        {
            string userMail = (string)Session["WriterMail"]; // Session'dan mail çekilmeli
            var messagelist = _messageService.GetListSendbox(userMail);
            return View(messagelist);
        }


        public ActionResult GetInboxMessageDetails(int id)
        {
            var values = _messageService.GetById(id);
            if (values != null && values.IsRead == false)
            {
                values.IsRead = true;
                _messageService.MessageUpdate(values);
            }
            return View(values);
        }

        public ActionResult GetSendboxMessageDetails(int id)
        {
            var values = _messageService.GetById(id);
            if (values != null && values.IsRead == false) // Mesaj okunmadıysa
            {
                values.IsRead = true; // Okundu olarak işaretle
                _messageService.MessageUpdate(values); // Veritabanına kaydet
            }
            return View(values);
        }

        [HttpGet]
        public ActionResult NewMessage(int? id)
        {
            if (id.HasValue)
            {
                var draft = _messageService.GetById(id.Value);
                return View(draft); // Taslak içeriğini formda gösterecek
            }
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewMessage(Message p, string action)
        {
            // Session'dan oturum açan kullanıcının mail adresini çekin
            string userMail = (string)Session["WriterMail"];

            // KRİTİK KONTROL: Mail adresi boşsa veya NULL ise, kullanıcıyı Login'e yönlendir.
            if (string.IsNullOrEmpty(userMail))
            {
                // Kullanıcıyı Login sayfasına yönlendirerek tekrar oturum açmasını sağlayın
                return RedirectToAction("Index", "Login");
                // Veya mesajı kaydetme (db ye NULL geçmesini engellemek için)
            }

            p.SenderMail = userMail;
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
                    // Yeni gönderilen mesajın başlangıçta okunmamış (IsRead=false) olması gerekir.
                    p.IsRead = false;
                    if (p.MessageId > 0)
                    {
                        // Mevcut taslağı güncelle
                        _messageService.MessageUpdate(p);
                    }
                    else
                    {
                        // Yeni mesajı ekle
                        _messageService.MessageAdd(p);
                    }

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
                // TASLAK İŞLEMİ
                p.IsDraft = true;
                p.IsRead = true; // Taslaklar genelde okundu sayılır.
                if (p.MessageId > 0)
                {
                    // Mevcut taslağı güncelle
                    _messageService.MessageUpdate(p);
                }
                else
                {
                    // Yeni taslağı ekle
                    _messageService.MessageAdd(p);
                }
            }
            return RedirectToAction("Drafts");
        }

        public ActionResult Drafts()
        {
            string userMail = (string)Session["WriterMail"];
            if (string.IsNullOrEmpty(userMail))
            {
                return RedirectToAction("Index", "Login");
            }

            // Yalnızca giriş yapan kullanıcının taslaklarını getir
            var draftMessages = _messageService.GetDraftMessagesBySender(userMail);

            return View(draftMessages);
        }

        [ChildActionOnly]
        public ActionResult MailboxMenu()
        {
            string userMail = (string)Session["WriterMail"];

            if (string.IsNullOrEmpty(userMail))
            {
                // Oturum yoksa mesaj sayaçlarını 0 yap
                ViewBag.InboxTotalCount = 0;
                ViewBag.SendBoxCount = 0;
                ViewBag.InboxUnreadCount = 0;
                return PartialView();
            }

            // 2. Mesaj Sayaçları (Sadece oturumdaki mail adresine göre)
            ViewBag.InboxTotalCount = _messageService.GetListInbox(userMail).Count;
            ViewBag.SendBoxCount = _messageService.GetListSendbox(userMail).Count;
            ViewBag.InboxUnreadCount = _messageService.GetUnreadMessageCountByReceiver(userMail);

            return PartialView();
        }

        public ActionResult TrashBox()
        {
            string userMail = (string)Session["WriterMail"];

            // Oturum yoksa, login sayfasına yönlendir
            if (string.IsNullOrEmpty(userMail))
            {
                return RedirectToAction("Index", "Login");
            }

            // 1. SİLİNMİŞ CONTACT (İletişim Formu) MESAJLARI
            // Tüm Contact mesajlarından sadece IsTrash=true olanları getir.
            var contactTrash = _contactService.GetList()
                                              .Where(x => x.IsTrash == true)
                                              .ToList();

            // 2. SİLİNMİŞ MESSAGE (Kullanıcı Mesajları)
            // Business Layer'da MessageService içine bu methodlar eklenmelidir:

            // MesajController'da GetListInbox/GetListSendbox'ı filtreleyen bir method yoksa, 
            // buradaki filtreleme mantığını kullanmalısınız:

            var messageTrash = _messageService.GetList()
                                              .Where(x => x.IsTrash == true &&
                                                         (x.SenderMail == userMail || x.ReceiverMail == userMail))
                                              .ToList();

            // İki farklı model türünü tek View'a göndermek için ViewBag veya ViewModel kullanacağız.
            ViewBag.ContactTrash = contactTrash;

            // Ana Model olarak silinmiş Message listesini gönderelim.
            return View(messageTrash);
        }

        public ActionResult MessageMoveToTrash(int id)
        {
            // Business Layer'da MessageMoveToTrash methodunuzun olduğunu varsayıyoruz.
            _messageService.MessageMoveToTrash(id);

            // Yönlendirme: Aynı Controller'daki TrashBox'a git
            return RedirectToAction("TrashBox");
        }

        public ActionResult MessageRestore(int id)
        {
            _messageService.MessageRestore(id);
            return RedirectToAction("TrashBox");
        }

        public ActionResult MessageHardDelete(int id)
        {
            // Öncelikle mesajı ID ile Business katmanından çekiyoruz.
            var messageValue = _messageService.GetById(id);

            if (messageValue != null)
            {
                // Mesajı silme metodunu çağırıyoruz.
                _messageService.MessageDelete(messageValue);

                // Silme işleminden sonra kullanıcıyı tekrar çöp kutusuna yönlendiriyoruz.
                return RedirectToAction("TrashBox");
            }
            // Mesaj bulunamazsa hata sayfasına veya ana sayfaya yönlendirilebilir.
            return RedirectToAction("TrashBox");
        }
    }
}