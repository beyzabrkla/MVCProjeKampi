using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class WriterManager : IWriterService
    {
        private readonly IUnitOfWork _uow; // IWriterDal yerine IUnitOfWork bağımlılığı

        public WriterManager(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public void WriterAdd(Writer writer)
        {
            _uow.Writers.Insert(writer); // UOW üzerinden Repository'yi çağırır
            _uow.Commit(); // Veritabanına kaydetme sorumluluğu
        }
        
        public List<Writer> GetList()
        {
            // UOW üzerinden Repository'deki List metodu çağrılır
            return _uow.Writers.List();
        }

        public List<Writer> GetTrashList()
        {
            // YALNIZCA WriterStatus=FALSE (Pasif/Silinmiş) olan yazarları çekiyoruz
            return _uow.Writers.List(x => x.WriterStatus == false); // <<< Düzeltme: IsTrash yerine WriterStatus
        }

        public void WriterDelete(Writer writer)
        {
            // Soft Delete (Yumuşak Silme) yapılır: Durumu pasif (false) olarak ayarla
            writer.WriterStatus = false;

            // Veritabanında güncelleme yapılmalıdır, silme değil!
            _uow.Writers.Update(writer); // <<< Düzeltme: Delete yerine Update
            _uow.Commit();
        }

        public void WriterUpdate(Writer writer)
        {
            _uow.Writers.Update(writer);
            _uow.Commit(); 
        }

        public Writer GetById(int id)
        {
            return _uow.Writers.Get(x => x.WriterId == id);
        }

        public Writer WriterLogin(string writerMail, string writerPassword)
        {
            // UOW içindeki Writers repository'sinin Get metodu ile filtreleme yapılıyor.
            var writer = _uow.Writers.Get(
                x => x.WriterMail == writerMail && x.WriterPassword == writerPassword
            );

           return writer;
        }

        public int GetWriterIdByMail(string mail)
        {
            var writer = _uow.Writers.Get(x => x.WriterMail == mail);
            if (writer != null)
            {
                return writer.WriterId;
            }
            else
            {
                return 0;
            }
        }
    }
}
