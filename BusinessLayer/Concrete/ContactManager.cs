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
    public class ContactManager : IContactService
    {
        private readonly IUnitOfWork _uow;

        public ContactManager(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public List<Contact> GetUnreadContacts()
        {
            // Web sitesinden gelen, çöp kutusunda olmayan ve okunmamış (false) mesajları getirir.
            return _uow.Contacts.List(x => x.ContactStatus == false && x.IsTrash == false);
        }

        public void ContactAdd(Contact contact)
        {
            _uow.Contacts.Insert(contact);
            _uow.Commit(); //Kaydetme sorumluluğu UOW'da
        }

        public void ContactDelete(Contact contact)
        {
            _uow.Contacts.Delete(contact);
            _uow.Commit(); //Kaydetme sorumluluğu UOW'da
        }

        public void ContactMoveToTrash(int id)
        {
            var contact = _uow.Contacts.Get(x => x.ContactId == id);
            if (contact != null)
            {
                contact.IsTrash = true;
                contact.TrashDate = DateTime.Now;
                _uow.Contacts.Update(contact);
                _uow.Commit();
            }
        }

        public void ContactRestore(int id)
        {
            var contact = _uow.Contacts.Get(x => x.ContactId == id);
            if (contact != null)
            {
                contact.IsTrash = false;
                contact.TrashDate = null;
                _uow.Contacts.Update(contact);
                _uow.Commit();
            }
        }

        public void ContactUpdate(Contact contact)
        {
            _uow.Contacts.Update(contact);
            _uow.Commit();
        }

        public Contact GetById(int id)
        {
            return _uow.Contacts.Get(x => x.ContactId == id);
        }

        // Çöp kutusunda olmayan mesajları listeler
        public List<Contact> GetList()
        {
            return _uow.Contacts.List(x=>x.IsTrash==false);
        }

        //İlk tanımı koruyoruz: Çöp kutusunda olmayanları sayar.
        public int GetContactCountNonTrash()
        {
            return _uow.Contacts.List(x => x.IsTrash == false).Count;
        }

        // Çöp kutusundaki mesajları listeler.
        public List<Contact> GetListTrash()
        {
            return _uow.Contacts.List(x => x.IsTrash == true).ToList();
        }

    }
}
