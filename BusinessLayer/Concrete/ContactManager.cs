using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
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
                _uow.Contacts.Update(contact);
                _uow.Commit();
            }
        }

        public void ContactRestore(int id)
        {
            var contact = _uow.Contacts.Get(x => x.ContactId == id);
            if (contact != null)
            {
                contact.IsTrash = false; // IsTrash'i false yaparak geri yüklüyoruz.
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

        public List<Contact> GetList()
        {
            return _uow.Contacts.List(x=>x.IsTrash==false);
        }

        public int GetContactCountNonTrash()
        {
            return _uow.Contacts.List(x => x.IsTrash == false).Count;
        }
    }
}
