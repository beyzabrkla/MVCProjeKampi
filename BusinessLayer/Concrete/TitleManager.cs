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
    public class TitleManager : ITitleService
    {
        private readonly IUnitOfWork _uow;

        public TitleManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Title GetById(int id)
        {
            return _uow.Titles.Get(x => x.TitleId == id);
        }

        public List<Title> GetList()
        {
            return _uow.Titles.List();
        }

        public List<Title> GetListByWriter(int id)
        {
            return _uow.Titles.List(x => x.WriterId == id, x => x.Writer);
        }

        public List<Title> GetTitlesByCategoryId(int id)
        {
            return _uow.Titles.List(x => x.CategoryId == id);
        }

        public void TitleAdd(Title title)
        {
            _uow.Titles.Insert(title);
            _uow.Commit();
        }

        public void TitleDelete(Title title)
        {
            title.TitleStatus = false;
            _uow.Titles.Update(title);
            _uow.Commit();
        }

        public void TitleUpdate(Title title)
        {
            _uow.Titles.Update(title);
            _uow.Commit();
        }

        public List<Title> GetListWithWriterAndCategory()
        {
            return _uow.Titles.List(
                filter: null,
                includeProperties: "Writer,Category" // İlişkileri virgülle ayırarak yüklüyoruz
            );
        }
    }
}
