using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class MessageManager : IMessageService
    {
        private readonly IUnitOfWork _uow;

        public MessageManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Message GetById(int id)
        {
            return _uow.Messages.Get(x => x.MessageId == id);
        }

        public List<Message> GetDraftMessages()
        {
            return _uow.Messages.List(x => x.IsDraft == true);
        }

        public List<Message> GetDraftMessagesBySender(string senderMail)
        {
            // Gönderene ait olan, taslak olan ve çöp kutusunda olmayanları getirir.
            return _uow.Messages.List(x => x.SenderMail == senderMail && x.IsDraft == true && x.SenderTrash == false);
        }

        public List<Message> GetList()
        {
            return _uow.Messages.List();
        }

        public List<Message> GetListInbox(string receiverMail)
        {
            // Gelen kutusu: Alıcıya ait, çöp kutusuna atılmamış ve taslak olmayanlar.
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail && x.ReceiverTrash == false && x.IsDraft == false);
        }

        public List<Message> GetListSendbox(string senderMail)
        {
            // Giden kutusu: Gönderene ait, çöp kutusuna atılmamış ve taslak olmayanlar.
            return _uow.Messages.List(x => x.SenderMail == senderMail && x.SenderTrash == false && x.IsDraft == false);
        }

        public List<Message> GetListUnreadInbox(string receiverMail)
        {
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail && x.IsRead == false && x.ReceiverTrash == false && x.IsDraft == false);
        }

        public List<Message> GetUnreadMessages(string receiverMail)
        {
            // Bu metot, mevcut GetListUnreadInbox metodunu çağırarak arayüz gerekliliğini sağlar.
            return GetListUnreadInbox(receiverMail);
        }

        public List<Message> GetListTrash(string userMail)
        {
            return _uow.Messages.List(x =>
                (x.SenderMail == userMail && x.SenderTrash == true) ||
                (x.ReceiverMail == userMail && x.ReceiverTrash == true)
            ).ToList();
        }

        public int GetUnreadMessageCountByReceiver(string receiverMail)
        {
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail
                                                 && x.IsRead == false
                                                 && x.ReceiverTrash == false
                                                 && x.IsDraft == false)
                                     .Count;
        }

        public void MessageAdd(Message message)
        {
            _uow.Messages.Insert(message);
            _uow.Commit();
        }

        public void MessageDelete(Message message)
        {
            _uow.Messages.Delete(message);
            _uow.Commit();
        }

        public void MessageMoveToTrash(int id, string userMail)
        {
            // Mesajı ID ile çekme
            var message = _uow.Messages.Get(x => x.MessageId == id);

            // 1. GÖNDEREN (Yazar) SİLİYORSA
            if (message.SenderMail == userMail)
            {
                message.SenderTrash = true;

                // KRİTİK KONTROL: SourceFolder'ı ayarlama (Taslak kontrolü öncelikli)
                if (message.IsDraft == true)
                {
                    // 1a) Mesaj taslak ise (ve gönderen siliyorsa)
                    message.SourceFolder = "Taslak";
                }
                else
                {
                    // 1b) Mesaj taslak değilse (Giden Kutusundan silinmiştir)
                    message.SourceFolder = "Giden";
                }
            }
            // 2. ALICI SİLİYORSA
            else if (message.ReceiverMail == userMail)
            {
                message.ReceiverTrash = true;
                // Gelen mesaj ise
                message.SourceFolder = "Gelen";
            }
            // NOT: IsDraft bayrağına bilerek dokunulmaz.

            _uow.Messages.Update(message);
            _uow.Commit();
        }

        public void MessageRestore(int id, string userMail)
        {
            var message = _uow.Messages.Get(x => x.MessageId == id);

            // Yazar tarafından silinmişse (Gönderen)
            if (message.SenderMail == userMail && message.SenderTrash == true)
            {
                message.SenderTrash = false;
            }
            // Alıcı tarafından silinmişse
            else if (message.ReceiverMail == userMail && message.ReceiverTrash == true)
            {
                message.ReceiverTrash = false;
            }

            // **ÖNEMLİ DÜZELTME:** Geri yüklendiği için SourceFolder bilgisini sıfırla
            message.SourceFolder = null;

            // NOT: IsDraft bayrağına dokunulmadığı sürece, SenderTrash=false olduktan sonra 
            // IsDraft=true olan mesajlar otomatikman Taslaklar listesinde görünür.

            _uow.Messages.Update(message);
            _uow.Commit();
        }

        public void MessageUpdate(Message message)
        {
            _uow.Messages.Update(message);
            _uow.Commit();
        }

        public int GetTrashMessageCountByMail(string userMail)
        {
            return _uow.Messages.List(x =>
                (x.SenderMail == userMail && x.SenderTrash == true) ||
                (x.ReceiverMail == userMail && x.ReceiverTrash == true)
            ).Count;
        }

        public int GetDraftMessageCountBySender(string senderMail)
        {
            return _uow.Messages.List(x => x.SenderMail == senderMail &&
                                           x.IsDraft == true &&
                                           x.SenderTrash == false)
                                .Count();
        }
    }
}