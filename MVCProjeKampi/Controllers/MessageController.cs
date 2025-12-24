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
        // Bağımlılık Enjeksiyonu (Constructor Injection)
        private readonly IMessageService _messageservice;
        private readonly IContactService _contactService;
        private readonly MessageValidator messagevalidator;

        // Constructor
        public MessageController(IMessageService messageservice, IContactService contactService, MessageValidator messagevalidator)
        {
            _messageservice = messageservice;
            _contactService = contactService;
            this.messagevalidator = messagevalidator;
        }

        // Genel Oturum Mailini Çekme Metodu (Admin'i Writer'a tercih eder)
        private string GetUserMail()
        {
            string userMail = (string)Session["AdminMail"];
            if (string.IsNullOrEmpty(userMail))
            {
                userMail = (string)Session["WriterMail"];
            }
            return userMail;
        }

        // Gelen Kutusu
        public ActionResult Inbox(string status, string search)
        {
            string userMail = GetUserMail();
            List<Message> messageList = new List<Message>();

            if (string.IsNullOrEmpty(userMail))
            {
                ViewBag.DebugMail = "HATA: Oturum Maili Bulunamadı!";
                return View(messageList);
            }

            ViewBag.DebugMail = "Oturum Maili: " + userMail;

            // 1. Temel filtreleme (Okunmamış ve Çöp Kutusu kontrolü)
            if (status == "unread")
            {
                messageList = _messageservice.GetListUnreadInbox(userMail)
                                             .Where(x => x.ReceiverTrash == false).ToList();
            }
            else
            {
                messageList = _messageservice.GetListInbox(userMail)
                                             .Where(x => x.ReceiverTrash == false).ToList();
            }

            // 2. Arama filtreleme
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                messageList = messageList
                    .Where(m => m.Subject.ToLower().Contains(searchLower) ||
                                m.SenderMail.ToLower().Contains(searchLower))
                    .ToList();
            }

            // 3. SIRALAMA EKLENDİ: En yeniden en eskiye
            messageList = messageList.OrderByDescending(x => x.MessageDate).ToList(); // <-- EKLENDİ

            ViewBag.CurrentSearch = search;
            ViewBag.DebugCount = messageList.Count;

            return View(messageList);
        }

        public ActionResult CombinedUnreadInbox()
        {
            // 1. Admin Mesajlarından (Message Entity) Okunmamışları Al
            string userMail = (string)Session["AdminMail"];
            // Not: MessageService'inizde Okunmamış (IsDraft=false, MessageStatus=false, ReceiverMail=userMail) filtrelemesi yapan bir metot olmalı.
            var unreadMessages = _messageservice.GetListUnreadInbox(userMail)
                                     .Where(x => x.ReceiverTrash == false).ToList();

            // 2. İletişim Mesajlarından (Contact Entity) Okunmamışları Al
            // ContactService'inizde sadece ContactStatus=false ve IsTrash=false olanları getiren metodu çağırın.
            var unreadContacts = _contactService.GetUnreadContacts();

            // 3. İki listeyi ViewModel'e dönüştür ve birleştir
            var combinedList = new List<CombinedUnreadMessage>();

            // Message (Admin) Mesajlarını Ekle
            foreach (var msg in unreadMessages)
            {
                combinedList.Add(new CombinedUnreadMessage
                {
                    Id = msg.MessageId,
                    Sender = msg.SenderMail,
                    Subject = msg.Subject,
                    ShortContent = msg.MessageContent.Length > 50 ? msg.MessageContent.Substring(0, 50) + "..." : msg.MessageContent,
                    Date = msg.MessageDate,
                    IsRead = msg.IsRead,
                    Type = "Message",
                    DetailLink = Url.Action("GetMessageDetails", "Message", new { id = msg.MessageId })
                });
            }

            // Contact (Web Sitesi) Mesajlarını Ekle
            foreach (var contact in unreadContacts)
            {
                combinedList.Add(new CombinedUnreadMessage
                {
                    Id = contact.ContactId,
                    Sender = contact.UserName,
                    Subject = contact.Subject,
                    ShortContent = contact.Message.Length > 50 ? contact.Message.Substring(0, 50) + "..." : contact.Message,
                    Date = (DateTime)contact.ContactDate,
                    IsRead = contact.ContactStatus,
                    Type = "Contact",
                    DetailLink = Url.Action("GetContactDetails", "Contact", new { id = contact.ContactId })
                });
            }

            // 4. Tarihe göre sırala ve View'a gönder
            var finalModel = combinedList.OrderByDescending(x => x.Date).ToList();

            return View("CombinedUnreadIndex", finalModel); // Yeni bir View'a gönderiyoruz
        }

        // Giden Kutusu
        public ActionResult SendBox(string search)
        {
            string userMail = GetUserMail();
            List<Message> messageList = new List<Message>();

            if (string.IsNullOrEmpty(userMail))
            {
                return View(messageList);
            }

            // 1. Temel filtreleme (Gönderilen ve Çöp Kutusu kontrolü)
            messageList = _messageservice.GetListSendbox(userMail)
                                         .Where(x => x.SenderTrash == false)
                                         .ToList();

            // 2. Arama filtreleme
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                messageList = messageList
                    .Where(m => m.Subject.ToLower().Contains(searchLower) ||
                                m.ReceiverMail.ToLower().Contains(searchLower))
                    .ToList();
            }

            // 3. SIRALAMA EKLENDİ: En yeniden en eskiye
            messageList = messageList.OrderByDescending(x => x.MessageDate).ToList(); // <-- EKLENDİ

            ViewBag.CurrentSearch = search;
            return View(messageList);
        }

        // Gelen Kutusundaki Mesaj Detayı
        public ActionResult GetInboxMessageDetails(int id)
        {
            var currentMessage = _messageservice.GetById(id);

            // Okundu bilgisini güncelle
            if (currentMessage != null && currentMessage.IsRead == false && currentMessage.IsDraft == false)
            {
                currentMessage.IsRead = true;
                _messageservice.MessageUpdate(currentMessage);
            }

            var allInboxMessages = _messageservice.GetListInbox(GetUserMail())
                                                 .Where(x => x.ReceiverTrash == false)
                                                 .OrderByDescending(m => m.MessageDate)
                                                 .ToList();

            int currentIndex = allInboxMessages.FindIndex(m => m.MessageId == id);

            ViewBag.PreviousMessageId = (currentIndex > 0) ? (int?)allInboxMessages[currentIndex - 1].MessageId : null;
            ViewBag.NextMessageId = (currentIndex != -1 && currentIndex < allInboxMessages.Count - 1)
                                    ? (int?)allInboxMessages[currentIndex + 1].MessageId
                                    : null;

            return View(currentMessage);
        }

        // Giden Kutusundaki Mesaj Detayı
        public ActionResult GetSendboxMessageDetails(int id)
        {
            var currentMessage = _messageservice.GetById(id);

            var allSendboxMessages = _messageservice.GetListSendbox(GetUserMail())
                                                    .Where(x => x.SenderTrash == false)
                                                    .OrderByDescending(m => m.MessageDate)
                                                    .ToList();

            int currentIndex = allSendboxMessages.FindIndex(m => m.MessageId == id);

            ViewBag.PreviousMessageId = (currentIndex > 0) ? (int?)allSendboxMessages[currentIndex - 1].MessageId : null;
            ViewBag.NextMessageId = (currentIndex != -1 && currentIndex < allSendboxMessages.Count - 1)
                                    ? (int?)allSendboxMessages[currentIndex + 1].MessageId
                                    : null;

            return View(currentMessage);
        }

        // Yeni Mesaj Yazma (GET)
        [HttpGet]
        public ActionResult NewMessage(int? id)
        {
            if (id.HasValue)
            {
                var draft = _messageservice.GetById(id.Value);
                return View(draft);
            }
            return View();
        }

        // Yeni Mesaj/Taslak Kaydetme (POST)
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewMessage(Message p, string action)
        {
            string userMail = GetUserMail();

            if (string.IsNullOrEmpty(userMail))
            {
                return RedirectToAction("Index", "Login");
            }

            p.SenderMail = userMail;
            p.MessageDate = DateTime.Now;
            var sanitizer = new HtmlSanitizer();
            p.MessageContent = sanitizer.Sanitize(p.MessageContent);

            p.SenderTrash = false;
            p.ReceiverTrash = false;

            if (action == "send")
            {
                ValidationResult results = messagevalidator.Validate(p);

                if (results.IsValid)
                {
                    p.IsDraft = false;
                    p.IsRead = false;

                    if (p.MessageId > 0)
                    {
                        _messageservice.MessageUpdate(p);
                    }
                    else
                    {
                        _messageservice.MessageAdd(p);
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
                p.IsRead = true; // Taslaklar okundu kabul edilir

                if (p.MessageId > 0)
                {
                    _messageservice.MessageUpdate(p);
                }
                else
                {
                    _messageservice.MessageAdd(p);
                }
            }
            return RedirectToAction("Drafts");
        }

        // Taslaklar
        public ActionResult Drafts(string search)
        {
            string userMail = GetUserMail();
            List<Message> messageList = new List<Message>();

            if (string.IsNullOrEmpty(userMail))
            {
                return View(messageList);
            }

            // 1. Temel filtreleme (Taslaklar ve Çöp Kutusu kontrolü)
            messageList = _messageservice.GetDraftMessagesBySender(userMail)
                                         .Where(x => x.SenderTrash == false)
                                         .ToList();

            // 2. Arama filtreleme
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                messageList = messageList
                    .Where(m => (m.Subject != null && m.Subject.ToLower().Contains(searchLower)) ||
                                (m.ReceiverMail != null && m.ReceiverMail.ToLower().Contains(searchLower)))
                    .ToList();
            }

            // 3. SIRALAMA EKLENDİ: En yeniden en eskiye
            messageList = messageList.OrderByDescending(x => x.MessageDate).ToList(); // <-- EKLENDİ

            ViewBag.CurrentSearch = search;
            return View(messageList);
        }

        // ... (MailboxMenu, MessageMoveToTrash, MoveDraftsToTrash, MessageBatchMoveToTrash, MessageRestore, MessageHardDelete metotları mantık olarak sorunsuz, değişiklik yapılmadı) ...
        [ChildActionOnly]
        public ActionResult MailboxMenu()
        {
            string userMail = GetUserMail();

            if (string.IsNullOrEmpty(userMail))
            {
                ViewBag.InboxTotalCount = 0;
                ViewBag.SendBoxCount = 0;
                ViewBag.InboxUnreadCount = 0;
                ViewBag.DraftCount = 0;
                ViewBag.TrashCount = 0;
                return PartialView();
            }

            ViewBag.InboxTotalCount = _messageservice.GetListInbox(userMail).Count(x => x.ReceiverTrash == false);
            ViewBag.SendBoxCount = _messageservice.GetListSendbox(userMail).Count(x => x.SenderTrash == false);
            ViewBag.InboxUnreadCount = _messageservice.GetUnreadMessageCountByReceiver(userMail);
            ViewBag.DraftCount = _messageservice.GetDraftMessagesBySender(userMail).Count(x => x.SenderTrash == false);

            ViewBag.TrashCount = _messageservice.GetList()
                                                 .Count(x => (x.SenderMail == userMail && x.SenderTrash == true) ||
                                                             (x.ReceiverMail == userMail && x.ReceiverTrash == true));

            return PartialView();
        }

        // Gelen Giden kutusundan silinen tekli mesajlar için
        public ActionResult MessageMoveToTrash(int id)
        {
            string userMail = GetUserMail();
            var message = _messageservice.GetById(id);

            if (message != null)
            {
                if (message.SenderMail == userMail)
                {
                    message.SenderTrash = true;
                    if (message.IsDraft == true)
                    {
                        message.IsDraft = false;
                        message.SourceFolder = "Taslak";
                    }
                    else
                    {
                        message.SourceFolder = "Giden";
                    }
                }
                else if (message.ReceiverMail == userMail)
                {
                    message.ReceiverTrash = true;
                    message.SourceFolder = "Gelen";
                }

                _messageservice.MessageUpdate(message);
            }
            return RedirectToAction("TrashBox");
        }

        // SADECE Taslaklar sayfasından toplu taşıma için
        [HttpPost]
        public ActionResult MoveDraftsToTrash(List<int> selectedDraftIds)
        {
            string userMail = GetUserMail();

            if (selectedDraftIds != null && selectedDraftIds.Count > 0)
            {
                foreach (var id in selectedDraftIds)
                {
                    var message = _messageservice.GetById(id);

                    if (message != null && message.SenderMail == userMail)
                    {
                        message.SenderTrash = true;
                        message.IsDraft = false;
                        message.SourceFolder = "Taslak";

                        _messageservice.MessageUpdate(message);
                    }
                }
            }

            return RedirectToAction("TrashBox");
        }

        [HttpPost]
        public ActionResult MessageBatchMoveToTrash(int[] selectedMessages)
        {
            string userMail = GetUserMail();
            if (selectedMessages != null && selectedMessages.Length > 0) // .Count yerine .Length kullanıldı
            {
                foreach (var id in selectedMessages)
                {
                    var message = _messageservice.GetById(id);
                    if (message != null)
                    {
                        if (message.SenderMail == userMail)
                        {
                            message.SenderTrash = true;
                            // Eğer taslak ise, taslak bayrağını kapatıp SourceFolder'ı belirtir.
                            if (message.IsDraft)
                            {
                                message.IsDraft = false;
                                message.SourceFolder = "Taslak";
                            }
                            else
                            {
                                // Giden kutusundan taşınanlar
                                message.SourceFolder = "Giden";
                            }
                        }
                        else if (message.ReceiverMail == userMail)
                        {
                            message.ReceiverTrash = true;
                            // Gelen kutusundan taşınanlar
                            message.SourceFolder = "Gelen";
                        }
                        _messageservice.MessageUpdate(message);
                    }
                }
            }
            return RedirectToAction("TrashBox");
        }

        // çöp kutusu sayfasındaki tekli geri yükle metodu için
        public ActionResult MessageRestore(int id)
        {
            var message = _messageservice.GetById(id);
            string userMail = GetUserMail();

            if (message != null)
            {
                message.SenderTrash = false;
                message.ReceiverTrash = false;

                if (message.SourceFolder == "Taslak")
                {
                    message.IsDraft = true;
                }

                message.SourceFolder = null;
                _messageservice.MessageUpdate(message);

                // YÖNLENDİRME: Mesajın geldiği yere göre yönlendir
                if (message.IsDraft == true) // Taslak olarak kaydedilmişse (Trash'dan geri döndü)
                {
                    return RedirectToAction("Drafts");
                }
                else if (message.SenderMail == userMail) // Giden kutusu mesajı
                {
                    return RedirectToAction("SendBox");
                }
                else if (message.ReceiverMail == userMail) // Gelen kutusu mesajı
                {
                    return RedirectToAction("Inbox");
                }
            }

            return RedirectToAction("TrashBox"); // Varsayılan: Çöp kutusuna geri dön
        }

        // çöp kutusu sayfasındaki tekli kalıcı sil metodu için
        public ActionResult MessageHardDelete(int id)
        {
            var messageValue = _messageservice.GetById(id);
            if (messageValue != null)
            {
                _messageservice.MessageDelete(messageValue);
            }
            return RedirectToAction("TrashBox");
        }

        /// Çöp kutusu sayfasını yükler. Admin ise Contact mesajlarını da listeler.
        [HttpGet]
        public ActionResult TrashBox(string search = null)
        {
            string userMail = GetUserMail();
            // Oturum Admin mi? (Gelen/Giden Kutusu'nun AdminMail üzerinden mi açıldığına bakar)
            bool isAdmin = (string)Session["AdminMail"] == userMail && !string.IsNullOrEmpty(userMail);

            // 1. Kullanıcı Mesajlarını (Message) Çekme (Temel Liste)
            var messageTrashList = _messageservice.GetList()
                .Where(x => (x.SenderMail == userMail && x.SenderTrash == true) ||
                            (x.ReceiverMail == userMail && x.ReceiverTrash == true))
                .ToList();

            // 2. İletişim Formlarını (Contact) Çekme (Sadece Admin yetkisinde çekilmeli)
            List<Contact> contactTrashList = new List<Contact>();
            if (isAdmin) // Yazar bu listeyi görmez.
            {
                contactTrashList = _contactService.GetListTrash().ToList();
            }

            // 3. Arama Filtresi Uygulama
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();

                // A. SİSTEM MESAJLARI (MESSAGE) FİLTRESİ
                messageTrashList = messageTrashList
                    .Where(m => (m.Subject != null && m.Subject.ToLower().Contains(searchLower)) ||
                                 (m.SenderMail != null && m.SenderMail.ToLower().Contains(searchLower)) ||
                                 (m.ReceiverMail != null && m.ReceiverMail.ToLower().Contains(searchLower)))
                    .ToList();

                // B. İLETİŞİM MESAJLARI (CONTACT) FİLTRESİ (Sadece Admin ise çalışır)
                if (isAdmin)
                {
                    contactTrashList = contactTrashList
                        .Where(c => (c.Subject != null && c.Subject.ToLower().Contains(searchLower)) ||
                                     (c.UserName != null && c.UserName.ToLower().Contains(searchLower)) ||
                                     (c.UserMail != null && c.UserMail.ToLower().Contains(searchLower)))
                        .ToList();
                }
            }

            // 4. Listeleri tarihe göre sıralama
            messageTrashList = messageTrashList.OrderByDescending(x => x.MessageDate).ToList();
            contactTrashList = contactTrashList.OrderByDescending(x => x.ContactDate).ToList();

            // 5. View'a Geri Gönderme
            ViewBag.ContactTrash = contactTrashList; // Admin ise dolu, yazar ise boş gider.
            ViewBag.IsAdmin = isAdmin; // View'da Contact tablosunu göstermek için bayrak.
            ViewBag.CurrentSearch = search;

            return View(messageTrashList);
        }

        // çöp kutusundaki toplu geri yükle metodu (SADECE MESSAGE)
        // Contact geri yükleme mantığı buradan TAMAMEN ÇIKARILMIŞTIR.
        [HttpPost]
        public ActionResult TrashRestoreMessages(int[] selectedMessageIds)
        {
            // Contact geri yükleme mantığı tamamen ContactController'a aittir veya silinmiştir.

            if (selectedMessageIds != null && selectedMessageIds.Length > 0)
            {
                foreach (var id in selectedMessageIds)
                {
                    var message = _messageservice.GetById(id);
                    if (message != null)
                    {
                        message.SenderTrash = false;
                        message.ReceiverTrash = false;

                        if (message.SourceFolder == "Taslak")
                        {
                            message.IsDraft = true;
                        }
                        message.SourceFolder = null; // Klasör bilgisini temizle

                        _messageservice.MessageUpdate(message);
                    }
                }
            }

            return RedirectToAction("TrashBox");
        }

        // çöp kutusundaki toplu kalıcı sil metodu
        // Contact silme işlemi SADECE Admin ise gerçekleşir.
        [HttpPost]
        public ActionResult TrashHardDeleteMessages(List<int> selectedMessageIds, List<int> selectedContactIds)
        {
            // Yazar/Admin fark etmeksizin, sadece kendi Message'larını silme işlemi.
            if (selectedMessageIds != null && selectedMessageIds.Any())
            {
                foreach (var id in selectedMessageIds)
                {
                    var messageValue = _messageservice.GetById(id);
                    if (messageValue != null)
                    {
                        // Message (Kullanıcı Mesajları) Kalıcı Silme
                        _messageservice.MessageDelete(messageValue);
                    }
                }
            }

            // Contact silme işlemi artık ContactController/ContactBatchHardDelete'den yapılmalıdır.

            return RedirectToAction("TrashBox");
        }
    }
}