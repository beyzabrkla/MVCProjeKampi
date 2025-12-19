using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class ContentManager : IContentService
    {
        private readonly IUnitOfWork _uow;

        public ContentManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void ContentAdd(Content content)
        {
            _uow.Contents.Insert(content); // UOW üzerinden Repository'yi çağırır
            _uow.Commit(); //Kaydetme sorumluluğu UOW'da
        }

        public void ContentDelete(Content content)
        {
            _uow.Contents.Delete(content);
            _uow.Commit();
        }

        public void ContentUpdate(Content content)
        {
            _uow.Contents.Update(content);
            _uow.Commit();
        }

        public Content GetById(int id)
        {
            return _uow.Contents.Get(x => x.ContentId == id);
        }

        public List<Content> GetList()
        {
            // Bütün içerikleri getirirken (filtresiz), Title VE Writer nesnelerini de yükle.
            return _uow.Contents.List(
                filter: null, // Filtre yok
                includeProperties: "Title,Writer" // İki ilişkili nesneyi de yükle
                ).OrderByDescending(x => x.ContentDate).ToList();
        }

        public List<Content> GetListByTitleId(int id) //idye göre verileri listeleme
        {
            return _uow.Contents.List(
                filter: x => x.TitleId == id,
                includeProperties: "Title,Writer"
                ).OrderByDescending(x => x.ContentDate).ToList();
        }

        public List<Content> GetListByWriter(int id)
        {
            return _uow.Contents.List(
                    filter: X => X.WriterId == id,
                    includeProperties: "Title" // İlişkili Başlık (Title) verilerini yükle
                );
        }
    }
}
