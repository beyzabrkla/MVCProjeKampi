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
    public class WriterManager : IWriterService
    {
        private readonly IUnitOfWork _uow; // IWriterDal yerine IUnitOfWork bağımlılığı

        public WriterManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // WriterAdd metodu artık Commit() çağırıyor
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

        public void WriterDelete(Writer writer)
        {
            _uow.Writers.Delete(writer);
            _uow.Commit(); // UOW üzerinden kaydedilir
        }

        public void WriterUpdate(Writer writer)
        {
            _uow.Writers.Update(writer);
            _uow.Commit(); // UOW üzerinden kaydedilir
        }

        // List ve GetById metotları Commit() çağırmaz
        public Writer GetById(int id)
        {
            return _uow.Writers.Get(x => x.WriterId == id);
        }
    }
}
