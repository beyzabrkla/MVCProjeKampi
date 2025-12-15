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
            // Yalnızca SenderMail'i eşleşen ve IsDraft=false olanları getir.
            return _uow.Messages.List(x => x.SenderMail == senderMail && x.IsDraft == true && x.IsTrash==false);
        }

        public List<Message> GetList()
        {
            // En muhtemel senaryo: Repository'deki List metodu parametresiz çağrılabilir olmalıdır.
            return _uow.Messages.List();
        }

        public List<Message> GetListInbox(string receiverMail)
        {
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail && x.IsTrash ==false);
        }

        public List<Message> GetListSendbox(string senderMail)
        {
            return _uow.Messages.List(x => x.SenderMail == senderMail && x.IsTrash == false);
        }

        public List<Message> GetListUnreadInbox(string receiverMail)
        {
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail && x.IsRead == false && x.IsTrash==false);
        }

        //gelen kutusundaki okunmaamış mesajları sayan metot
        public int GetUnreadMessageCountByReceiver(string receiverMail)
        {
            var unreadList = _uow.Messages.List(x => x.ReceiverMail == receiverMail
                                                        && x.IsRead == false
                                                        && x.IsTrash == false)
                                                        .ToList();
            return unreadList.Count;
        }

        public void MessageAdd(Message message)
        {
            _uow.Messages.Insert(message); // UOW üzerinden Repository'yi çağırır
            _uow.Commit(); //Kaydetme sorumluluğu UOW'da
        }

        public void MessageDelete(Message message)
        {
            _uow.Messages.Delete(message);
            _uow.Commit();
        }

        public void MessageMoveToTrash(int id)
        {
            var message = _uow.Messages.Get(x=>x.MessageId==id);
            if (message != null)
            {
                message.IsTrash = true; // Mesajı çöp kutusuna taşı
                message.TrashDate = DateTime.Now;
                _uow.Messages.Update(message);
                _uow.Commit();
            }
        }

        public void MessageRestore(int id)
        {
            var message = _uow.Messages.Get(x => x.MessageId == id);
            if (message != null)
            {
                message.IsTrash = false; // IsTrash'i false yaparak geri yüklüyoruz.
                message.TrashDate = null;
                _uow.Messages.Update(message);
                _uow.Commit();
            }
        }

        public void MessageUpdate(Message message)
        {
            _uow.Messages.Update(message);
            _uow.Commit();
        }

        public int GetTrashMessageCountByMail(string userMail)
        {
            // Hem alıcı hem de gönderici olarak size ait olan ve IsTrash=true olan tüm mesajları sayar.
            return _uow.Messages.List(x => x.IsTrash == true &&
                                           (x.SenderMail == userMail || x.ReceiverMail == userMail)).Count;
        }

        public int GetDraftMessageCountBySender(string senderMail)
        {
            // Sadece göndericiye ait olan, taslak olan (IsDraft=true) ve
            // çöp kutusunda olmayan (IsTrash=false) mesajları sayar.
            return _uow.Messages.List(x => x.SenderMail == senderMail &&
                                           x.IsDraft == true &&
                                           x.IsTrash == false)
                                .Count();
        }
    }
}
