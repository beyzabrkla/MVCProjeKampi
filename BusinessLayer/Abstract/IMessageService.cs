using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IMessageService
    {
        List<Message> GetList();
        List<Message> GetListInbox(string receiverMail);
        List<Message> GetListSendbox(string senderMail);
        List<Message> GetListTrash(string userMail);
        void MessageAdd(Message message);   
        Message GetById (int id);
        void MessageDelete(Message message);
        void MessageUpdate(Message message);
        List<Message> GetDraftMessages();
        List<Message> GetDraftMessagesBySender(string senderMail);
        List<Message> GetListUnreadInbox(string receiverMail);
        List<Message> GetUnreadMessages(string receiverMail);
        int GetUnreadMessageCountByReceiver(string receiverMail);
        void MessageMoveToTrash(int id, string userMail);
        void MessageRestore(int id, string userMail);
        int GetTrashMessageCountByMail(string userMail);
        int GetDraftMessageCountBySender(string userMail);
    }
}
