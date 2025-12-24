using BusinessLayer.Abstract;
using BusinessLayer.ValidationRules;
using EntityLayer.Concrete;
using FluentValidation.Results;
using Ganss.Xss;
using MVCProjeKampi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [WriterAuthorize]
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

        [HttpGet]
        public ActionResult Inbox(string search = null) // search parametresi eklendi
        {
            string userMail = GetUserMail(); // BaseWriterPanelController'dan geliyor

            // 1. Gelen mesajları çekme (ReceiverMail olanlar)
            // IMessageService'de GetListInbox metodu veya benzer bir filtreleme metodu olduğunu varsayıyoruz.
            List<Message> messageList = _messageService.GetListInbox(userMail);

            // =======================================================
            // ARAMA FİLTRESİ ENTEGRASYONU
            // =======================================================
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();

                // Mesajları Gönderen Mail, Konu veya İçerik üzerinden filtreleme
                messageList = messageList
                    .Where(m => (m.SenderMail != null && m.SenderMail.ToLower().Contains(searchLower)) ||
                                (m.Subject != null && m.Subject.ToLower().Contains(searchLower)) ||
                                (m.MessageContent != null && m.MessageContent.ToLower().Contains(searchLower)))
                    .ToList();
            }

            // Filtrelemeden sonra tarihe göre sıralama (genellikle en yeni üste)
            messageList = messageList.OrderByDescending(x => x.MessageDate).ToList();

            // Arama kelimesini input alanında tutmak için View'a gönderiyoruz
            ViewBag.CurrentSearch = search;

            return View(messageList);
        }

        [HttpPost]
        public ActionResult Inbox(List<int> selectedMessages)
        {
            // Bu metot toplu işlem içindir (InboxBulkMoveToTrash'e yönlendirir)
            return RedirectToAction("InboxBulkMoveToTrash", new { messageIds = selectedMessages });
        }

        [HttpGet]
        public ActionResult SendBox(string search = null) // search parametresi eklendi
        {
            string userMail = GetUserMail(); // BaseWriterPanelController'dan geliyor

            // 1. Gönderilen mesajları çekme (SenderMail olanlar)
            // IMessageService'de GetListSendBox metodu veya benzer bir filtreleme metodu olduğunu varsayıyoruz.
            List<Message> messageList = _messageService.GetListSendbox(userMail);

            // =======================================================
            // ARAMA FİLTRESİ ENTEGRASYONU
            // =======================================================
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();

                // Mesajları Alıcı Mail, Konu veya İçerik üzerinden filtreleme
                messageList = messageList
                    .Where(m => (m.ReceiverMail != null && m.ReceiverMail.ToLower().Contains(searchLower)) ||
                                (m.Subject != null && m.Subject.ToLower().Contains(searchLower)) ||
                                (m.MessageContent != null && m.MessageContent.ToLower().Contains(searchLower)))
                    .ToList();
            }

            // Filtrelemeden sonra tarihe göre sıralama (genellikle en yeni üste)
            messageList = messageList.OrderByDescending(x => x.MessageDate).ToList();

            // Arama kelimesini input alanında tutmak için View'a gönderiyoruz
            ViewBag.CurrentSearch = search;

            return View(messageList);
        }

        [ChildActionOnly]
        public PartialViewResult MessageListMenu()
        {
            string userMail = (string)Session["WriterMail"];

            if (string.IsNullOrEmpty(userMail))
            {
                userMail = "bosmail@mail.com";
            }

            // Aksi halde buraya da filtre eklemek gerekir. Şu anki Business katmanınızın filtreden arındırılmış
            // listeleri getirdiğini varsayarak kodun geri kalanını ona göre güncelleyelim.
            ViewBag.InboxCount = _messageService.GetListInbox(userMail).Where(x => x.ReceiverTrash == false).Count();
            ViewBag.SendBoxCount = _messageService.GetListSendbox(userMail).Where(x => x.SenderTrash == false).Count();
            ViewBag.UnreadCount = _messageService.GetUnreadMessageCountByReceiver(userMail);
            ViewBag.DraftCount = _messageService.GetDraftMessageCountBySender(userMail);

            // Düzeltme: Çöp kutusundaki toplam mesaj sayısını SenderTrash VEYA ReceiverTrash true olanlar üzerinden hesapla.
            ViewBag.TrashCount = _messageService.GetTrashMessageCountByMail(userMail);

            return PartialView();
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
            if (values != null && values.IsRead == false)
            {
                values.IsRead = true;
                _messageService.MessageUpdate(values);
            }
            return View(values);
        }

        [HttpGet]
        public ActionResult NewMessage(int? id)
        {
            if (id.HasValue)
            {
                var draft = _messageService.GetById(id.Value);
                return View(draft);
            }
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewMessage(Message p, string action)
        {
            string userMail = (string)Session["WriterMail"];

            if (string.IsNullOrEmpty(userMail))
            {
                return RedirectToAction("Index", "Login");
            }

            p.SenderMail = userMail;
            p.MessageDate = DateTime.Now;
            var sanitizer = new HtmlSanitizer();
            p.MessageContent = sanitizer.Sanitize(p.MessageContent);

            if (action == "send")
            {
                ValidationResult results = messagevalidator.Validate(p);

                if (results.IsValid)
                {
                    p.IsDraft = false;
                    p.IsRead = false;

                    // Gönderilen mesajın çöp kutusu bayraklarını sıfırla
                    p.SenderTrash = false;
                    p.ReceiverTrash = false;

                    if (p.MessageId > 0)
                    {
                        // Mevcut taslağı gönderirken güncelle
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
                p.IsRead = true;

                // Taslaklar çöp kutusunda değil, bu yüzden bayrakları sıfırlıyoruz.
                p.SenderTrash = false;
                p.ReceiverTrash = false;

                if (p.MessageId > 0)
                {
                    _messageService.MessageUpdate(p);
                }
                else
                {
                    _messageService.MessageAdd(p);
                }
            }
            return RedirectToAction("Drafts");
        }

        [HttpGet] 
        public ActionResult Drafts(string search = null)
        {
            string userMail = GetUserMail();

            if (string.IsNullOrEmpty(userMail))
            {
                return RedirectToAction("Login", "WriterPanelLogin"); // Veya uygun hata sayfasına yönlendirme
            }

            // Arayüzden gelen, taslakları getiren metodu kullanıyoruz:
            // (IMessageService'de GetDraftMessagesBySender metodu olmalı)
            List<Message> messageList = _messageService.GetDraftMessagesBySender(userMail);

            // =======================================================
            // ARAMA FİLTRESİ ENTEGRASYONU
            // =======================================================
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();

                // Taslakları Konu, Alıcı Mail veya Mesaj İçeriği üzerinden filtreleme
                messageList = messageList
                    .Where(m => (m.Subject != null && m.Subject.ToLower().Contains(searchLower)) ||
                                (m.ReceiverMail != null && m.ReceiverMail.ToLower().Contains(searchLower)) ||
                                (m.MessageContent != null && m.MessageContent.ToLower().Contains(searchLower)))
                    .ToList();
            }

            // Filtrelemeden sonra sıralama (en son tarihe göre)
            messageList = messageList.OrderByDescending(x => x.MessageDate).ToList();

            // Arama kelimesini input alanında tutmak için View'a gönderiyoruz
            ViewBag.CurrentSearch = search;

            return View(messageList);
        }

        [HttpPost]
        public ActionResult Drafts(List<int> selectedMessages)
        {
            // Bu metot toplu işlem içindir.
            return RedirectToAction("DraftsBulkMoveToTrash", new { messageIds = selectedMessages });
        }

        [HttpGet]
        public ActionResult TrashBox(string search = null)
        {
            string userMail = GetUserMail();

            // ===============================================
            // 1. Mesajları Çekme (Sadece yazarın kendi mesajları)
            // ===============================================
            List<Message> messageList = _messageService.GetListTrash(userMail);

            // 2. Contact Mesajlarını Çekme kısmı tamamen KALDIRILDI.
            // --------------------------------------------------------------------------------

            // ===============================================
            // 3. Arama Filtresi Uygulama (Sadece Message için)
            // ===============================================
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();

                // a) Message Listesi Filtresi
                messageList = messageList
                    .Where(m => (m.ReceiverMail != null && m.ReceiverMail.ToLower().Contains(searchLower)) ||
                                (m.SenderMail != null && m.SenderMail.ToLower().Contains(searchLower)) ||
                                (m.Subject != null && m.Subject.ToLower().Contains(searchLower)) ||
                                (m.MessageContent != null && m.MessageContent.ToLower().Contains(searchLower)))
                    .ToList();

                // b) Contact Listesi Filtresi kısmı tamamen KALDIRILDI.
            }

            // ===============================================
            // 4. View'a Gönderme
            // ===============================================
            // Contact listesini ViewBag ile gönderme KALDIRILDI.
            // ViewBag.ContactTrash = contactList; // Bu satırı silin.

            ViewBag.CurrentSearch = search;

            return View(messageList);
        }

        public ActionResult MessageMoveToTrash(int id)
        {
            var message = _messageService.GetById(id);
            string userMail = (string)Session["WriterMail"];
            string redirectAction = "TrashBox";

            if (message != null)
            {
                _messageService.MessageMoveToTrash(id, userMail);

                if (message.SenderMail == userMail)
                {
                    // Mesaj taslak ise veya giden ise
                    redirectAction = (message.IsDraft == true) ? "Drafts" : "SendBox";
                }
                else if (message.ReceiverMail == userMail)
                {
                    // Mesaj gelen kutusu ise
                    redirectAction = "Inbox";
                }
            }
            return RedirectToAction(redirectAction);
        }

        // Toplu Taşıma Metodu (Giden/Gelen Kutusu'ndan tek bir yerde toplanabilir)
        [HttpPost]
        public ActionResult MessageBatchMoveToTrash(List<int> selectedMessages)
        {
            // Yazar/Yönetici paneline göre bu Session adını kullanıyoruz
            string userMail = (string)Session["WriterMail"];

            if (selectedMessages != null && selectedMessages.Count > 0)
            {
                foreach (var id in selectedMessages)
                {
                    var message = _messageService.GetById(id);
                    if (message != null)
                    {
                        // Mesajın kime ait olduğuna bakarak doğru bayrağı işaretle
                        if (message.SenderMail == userMail)
                        {
                            // Gönderen (Yazar/Yönetici) siliyor (Giden Kutusu veya Taslak)
                            message.SenderTrash = true;

                            // KRİTİK DÜZELTME: Kaynak klasörü belirleme
                            if (message.IsDraft)
                            {
                                message.IsDraft = false;
                                // Kaynak: Taslaklar
                                message.SourceFolder = "Taslak";
                            }
                            else
                            {
                                // Kaynak: Giden Kutusu
                                message.SourceFolder = "Giden";
                            }
                        }
                        else if (message.ReceiverMail == userMail)
                        {
                            // Alıcı (Yazar/Yönetici) siliyor (Gelen Kutusu)
                            message.ReceiverTrash = true;
                            // KRİTİK DÜZELTME: Kaynak klasörü belirleme
                            // Kaynak: Gelen Kutusu
                            message.SourceFolder = "Gelen";
                        }
                        // Eğer mesaj ne gönderene ne de alıcıya aitse (ki bu olmamalı), 
                        // SourceFolder null kalacak ve View'da 'Bilinmiyor' olarak görünecektir.

                        _messageService.MessageUpdate(message);
                    }
                }
            }
            return RedirectToAction("TrashBox");
        }

        //çöp kutusu sayfasındaki tekli geri yükle metodu için
        public ActionResult MessageRestore(int id)
        {
            // 1. Oturumdaki Kullanıcı Mailini Çekme (Bu, WriterMail olmalı)
            // Eğer mail oturumda yoksa, bu kısmı kendi projenize göre (örn: FormsAuthentication, Identity) ayarlayın.
            string userMail = (string)Session["WriterMail"];

            if (string.IsNullOrEmpty(userMail))
            {
                // Kullanıcı oturumu bulunamazsa giriş sayfasına yönlendirme (opsiyonel hata kontrolü)
                return RedirectToAction("WriterLogin", "Login");
            }

            // 2. Business Layer'daki metodu doğru parametrelerle çağırma
            _messageService.MessageRestore(id, userMail);

            // İşlem bittikten sonra genellikle Çöp Kutusu listesine yönlendirilir
            return RedirectToAction("TrashBox");
        }

        //çöp kutusu sayfasındaki tekli kalıcı sil metodu için
        public ActionResult MessageHardDelete(int id)
        {
            var messageValue = _messageService.GetById(id);

            if (messageValue != null)
            {
                _messageService.MessageDelete(messageValue);
                return RedirectToAction("TrashBox");
            }
            return RedirectToAction("TrashBox");
        }

        [HttpPost]
        public ActionResult TrashHardDeleteItems(int[] selectedMessageIds) // selectedContactIds KALDIRILDI
        {
            // A) Mesaj Kalıcı Silme
            if (selectedMessageIds != null && selectedMessageIds.Length > 0)
            {
                foreach (var id in selectedMessageIds)
                {
                    var message = _messageService.GetById(id);
                    if (message != null)
                    {
                        // Business katmanındaki kalıcı silme metodunu çağırıyoruz.
                        _messageService.MessageDelete(message);
                    }
                }
            }

            // B) Contact Kalıcı Silme kısmı tamamen KALDIRILDI.

            // İşlem bittikten sonra çöp kutusuna geri dön
            return RedirectToAction("TrashBox");
        }

        [HttpPost]
        public ActionResult TrashRestoreMessages(int[] selectedMessageIds) // selectedContactIds KALDIRILDI
        {
            string userMail = (string)Session["WriterMail"];

            if (selectedMessageIds != null && selectedMessageIds.Length > 0)
            {
                foreach (var id in selectedMessageIds)
                {
                    var message = _messageService.GetById(id);
                    if (message != null)
                    {
                        // Hangi bayrak True ise onu False yap
                        if (message.SenderMail == userMail)
                        {
                            message.SenderTrash = false;
                        }
                        else if (message.ReceiverMail == userMail)
                        {
                            message.ReceiverTrash = false;
                        }

                        _messageService.MessageUpdate(message);
                    }
                }
            }

            // Contact geri yükleme mantığı tamamen KALDIRILDI.

            return RedirectToAction("TrashBox");
        }
        [HttpPost]
        public ActionResult InboxBulkMoveToTrash(int[] messageIds)
        {
            // Gelen kutusundan silme işlemi için MessageBatchMoveToTrash'i kullanıyoruz.
            return MessageBatchMoveToTrash(messageIds.ToList());
        }

        [HttpPost]
        public ActionResult SendBoxBulkMoveToTrash(int[] messageIds)
        {
            // Giden kutusundan silme işlemi için MessageBatchMoveToTrash'i kullanıyoruz.
            return MessageBatchMoveToTrash(messageIds.ToList());
        }

        [HttpPost]
        public ActionResult DraftsBulkMoveToTrash(List<int> messageIds)
        {
            string userMail = (string)Session["WriterMail"];

            if (messageIds != null && messageIds.Any())
            {
                foreach (var id in messageIds)
                {
                    var message = _messageService.GetById(id);

                    // KRİTİK KONTROL: Mesajın mevcut kullanıcıya ait bir taslak olduğundan emin ol
                    if (message != null && message.IsDraft == true && message.SenderMail == userMail)
                    {
                        message.SenderTrash = true;
                        message.SourceFolder = "Taslak";
                        _messageService.MessageUpdate(message);
                    }
                }
            }
            return RedirectToAction("TrashBox");
        }
    }
}