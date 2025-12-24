using EntityLayer.Concrete;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface IContactService
    {
        List<Contact> GetUnreadContacts();
        List<Contact> GetList();
        void ContactAdd(Contact contact);
        Contact GetById(int id);
        void ContactDelete(Contact contact);
        void ContactUpdate(Contact contact);
        void ContactMoveToTrash(int id);
        void ContactRestore(int id);
        int GetContactCountNonTrash();
        List<Contact> GetListTrash();
    }
}