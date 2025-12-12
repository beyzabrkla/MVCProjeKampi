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

        public List<Message> GetListInbox(string receiverMail)
        {
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail);
        }

        public List<Message> GetListSendbox(string senderMail)
        {
            return _uow.Messages.List(x => x.SenderMail == senderMail);
        }

        public List<Message> GetListUnreadInbox(string receiverMail)
        {
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail && x.IsRead == false);
        }

        //gelen kutusundaki okunmaamış mesajları sayan metot
        public int GetUnreadMessageCountByReceiver(string receiverMail)
        {
            return _uow.Messages.List(x => x.ReceiverMail == receiverMail && x.IsRead == false).Count;
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

        public void MessageUpdate(Message message)
        {
            _uow.Messages.Update(message);
            _uow.Commit();
        }

    }
}
