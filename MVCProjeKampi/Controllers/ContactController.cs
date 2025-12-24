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

        public ContactController(IContactService contactService, IMessageService messageService)
        {
            _contactService = contactService;
            _messageService = messageService;
        }

        public ActionResult Index(string search)
        {
            // 1. Tüm iletişim kayıtlarını getir
            var contactvalues = _contactService.GetList().Where(x => x.IsTrash == false).AsQueryable();

            // 2. Arama terimi var mı kontrol et ve filtrele
            if (!string.IsNullOrEmpty(search))
            {
                // Arama yap: İsim, Konu veya Mesaj içeriğinde arama yap
                contactvalues = contactvalues.Where(x =>
                    x.UserName.Contains(search) ||
                    x.Subject.Contains(search) ||
                    x.Message.Contains(search)
                );
            }

            contactvalues = contactvalues.OrderByDescending(x => x.ContactDate); 

            // 3. View'a göndermeden önce Listeye çevir (veya sadece Listeye çevirerek LINQ metodunu çağırabilirsiniz)
            var filteredList = contactvalues.ToList();

            // Arama kutusunda değeri tutmak için (View'daki input'un value'sunda kullanılır)
            ViewBag.CurrentSearch = search;

            return View(filteredList);
        }

        public ActionResult GetContactDetails(int id)
        {
            // 1. Mesajı veritabanından getir
            var contactvalues = _contactService.GetById(id);

            // Kontrol: Mesaj var mı ve durumu okunmamış (false) ise
            if (contactvalues != null && contactvalues.ContactStatus == false)
            {
                // Durumu Okunmuş (TRUE) olarak değiştir
                contactvalues.ContactStatus = true;

                // Veritabanına kaydet
                _contactService.ContactUpdate(contactvalues);
            }

            // Navigasyon mantığı 
            var allContacts = _contactService.GetList().OrderByDescending(c => c.ContactDate).ToList();

            int currentIndex = allContacts.FindIndex(x => x.ContactId == id);

            // Önceki ve Sonraki mesajların ID'lerini hesapla
            int? previousId = null;
            if (currentIndex > 0)
            {
                previousId = allContacts[currentIndex - 1].ContactId;
            }

            int? nextId = null;
            if (currentIndex != -1 && currentIndex < allContacts.Count - 1)
            {
                // Mevcut mesajdan bir sonraki mesajın ID'si
                nextId = allContacts[currentIndex + 1].ContactId;
            }

            ViewBag.PreviousContactId = previousId;
            ViewBag.NextContactId = nextId;

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

        [HttpPost]
        public ActionResult AddContact(EntityLayer.Concrete.Contact contact)
        {
            // ... (Validasyon kodları) ...
            ContactValidator cv = new ContactValidator();
            var results = cv.Validate(contact);

            if (results.IsValid)
            {
                // 1. Contact tablosuna kayıt için varsayılan değerleri ayarla
                contact.ContactDate = DateTime.Now;
                contact.IsTrash = false;

                contact.ContactStatus = false;

                try
                {
                    _contactService.ContactAdd(contact);


                    string[] adminMails = new string[]
                    {
                "admin@gmail.com",
                "admin2@gmail.com",
                "admin3@gmail.com"
                    };

                    string originalSubject = contact.Subject;
                    string safeSubject = originalSubject.Length > 80 ? originalSubject.Substring(0, 80) : originalSubject;
                    string finalSubject = "[İletişim Formu] " + safeSubject;

                    // Her bir admin için DÖNGÜ BAŞLAT
                    foreach (var receiverMail in adminMails)
                    {
                        EntityLayer.Concrete.Message message = new EntityLayer.Concrete.Message
                        {
                            SenderMail = contact.UserMail,
                            ReceiverMail = receiverMail,

                            Subject = finalSubject,
                            MessageContent = contact.Message,
                            MessageDate = DateTime.Now,

                            IsDraft = false,

                            // KRİTİK DÜZELTME: Okunmamış (IsRead = false) olarak ayarlanmalıdır.
                            IsRead = false,

                            SenderTrash = false,
                            ReceiverTrash = false,

                            // İletişim Formu'ndan geldiğini belirtir.
                            SourceFolder = "Contact"
                        };

                        // Mesajı veritabanına ekle
                        _messageService.MessageAdd(message);
                    }

                    return Json(new { success = true, message = "Teşekkürler! Mesajınız başarıyla gönderildi." });
                }
                catch (Exception ex)
                {
                    // ... (Hata yakalama kodu) ...
                    string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                    // Loglama yapılması ŞİDDETLE önerilir!
                    // Örneğin: LoglamaServisi.Logla(errorMessage);

                    return Json(new { success = false, message = "Hata oluştu! Lütfen admin mail listesini veya veritabanı kısıtlamalarını kontrol edin.", detail = errorMessage });
                }
            }
            else
            {
                // ... (Validasyon başarısız olursa) ...
                string errorMessages = string.Join(" ", results.Errors.Select(x => x.ErrorMessage));
                return Json(new { success = false, message = "Gönderim başarısız! Lütfen formdaki tüm alanları kontrol edin.", detail = errorMessages });
            }
        }

        public ActionResult UnreadMessages()
        {
            // Business katmanında tanımlanan GetUnreadContacts() metodunu doğrudan kullanıyoruz.
            // Bu metot, ContactManager'da (x => x.ContactStatus == false && x.IsTrash == false) filtresini uygular.
            var unreadContacts = _contactService.GetUnreadContacts().OrderByDescending(x => x.ContactDate).ToList();

            ViewBag.Title = "Okunmamış İletişim Mesajları";
            // Index View'ını kullanmak için View adını belirtebiliriz.
            return View("Index", unreadContacts);
        }

        // 1. İLETİŞİM ÇÖP KUTUSU (Contact verilerini listeler)
        public ActionResult ContactTrashBox()
        {
            var contactTrashValues = _contactService.GetList().Where(x => x.IsTrash == true).OrderByDescending(x=>x.ContactDate).ToList();
            ViewBag.Title = "İletişim Çöp Kutusu";
            return View(contactTrashValues);
        }

        // 2. TEKİL TAŞIMA VE DÜZELTİLMİŞ YÖNLENDİRME (Çift tanımlı metot kaldırıldı)
        public ActionResult ContactMoveToTrash(int id)
        {
            _contactService.ContactMoveToTrash(id);

            return RedirectToAction("TrashBox", "Message");
        }

        [HttpPost]
        public ActionResult ContactBatchMoveToTrash(int[] selectedContacts)
        {
            if (selectedContacts != null && selectedContacts.Length > 0)
            {
                foreach (var id in selectedContacts)
                {
                    // DÜZELTME: Artık manuel güncelleme yerine, tekil taşıma metodunu çağırıyoruz.
                    // Bu, ContactManager'ın taşıma mantığını (IsTrash=true, TrashDate=DateTime.Now vb.) merkezi olarak yönetmesini sağlar.
                    _contactService.ContactMoveToTrash(id);
                }
            }
            // Gelen Kutusu'ndan sonra kullanıcıyı Merkezi Çöp Kutusuna yönlendir.
            return RedirectToAction("TrashBox", "Message");
        }

        [HttpPost]
        public ActionResult ContactBatchRestore(int[] selectedContactIds)
        {
            if (selectedContactIds != null && selectedContactIds.Length > 0)
            {
                foreach (var id in selectedContactIds) // <-- İç döngüde kullanılan değişken adı da düzeltilmeli
                {
                    var contactValue = _contactService.GetById(id);

                    if (contactValue != null)
                    {
                        contactValue.IsTrash = false; // Çöp Kutusu'ndan çıkar
                        _contactService.ContactUpdate(contactValue);
                    }
                }
            }
            return RedirectToAction("TrashBox", "Message");
        }

        // 3. GERİ YÜKLEME (Contact verisini restore eder)
        public ActionResult ContactRestore(int id)
        {
            // Eğer Business Layer'da tek satırlık bir metot yoksa
            var contactValue = _contactService.GetById(id);

            if (contactValue != null)
            {
                contactValue.IsTrash = false; // Çöp Kutusu'ndan çıkar
                _contactService.ContactUpdate(contactValue);
            }

            // DÜZELTME: Geri yüklemeden sonra kullanıcıyı İletişim Çöp Kutusu listesine yönlendir.
            return RedirectToAction("TrashBox", "Message");
        }

        // 4. KALICI SİLME (Contact verisini veritabanından tamamen siler)
        public ActionResult ContactHardDelete(int id)
        {
            var contactValue = _contactService.GetById(id);

            if (contactValue != null)
            {
                _contactService.ContactDelete(contactValue);

                // DÜZELTME: Silme işleminden sonra kullanıcıyı İletişim Çöp Kutusu'na yönlendiriyoruz.
                return RedirectToAction("TrashBox","Message");
            }
            return RedirectToAction("TrashBox", "Message");
        }

        [HttpPost]
        public ActionResult ContactBatchHardDelete(int[] selectedContactIds)
        {
            // selectedContactIds parametresi View'dan gelen seçili Contact ID'lerini içerir.
            if (selectedContactIds != null && selectedContactIds.Length > 0)
            {
                foreach (var id in selectedContactIds) // <-- İç döngüde kullanılan değişken adı da düzeltilmeli
                {
                    var contact = _contactService.GetById(id);
                    if (contact != null)
                    {
                        // Kalıcı silme işlemi
                        _contactService.ContactDelete(contact);
                    }
                }
            }
            return RedirectToAction("TrashBox", "Message");
        }
    }
}