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
    public class AboutManager : IAboutService
    {
        private readonly IUnitOfWork _uow; // IWriterDal yerine IUnitOfWork bağımlılığı

        public AboutManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void AboutAdd(About about)
        {
            _uow.Abouts.Insert(about);
            _uow.Commit();
        }

        public void AboutDelete(About about)
        {
            _uow.Abouts.Delete(about);
            _uow.Commit();
        }

        public void AboutUpdate(About about)
        {
            _uow.Abouts.Update(about);
            _uow.Commit();
        }

        public About GetById(int id)
        {
            return _uow.Abouts.Get(x => x.AboutId == id);
        }

        public List<About> GetList()
        {
            return _uow.Abouts.List();
        }
    }
}
