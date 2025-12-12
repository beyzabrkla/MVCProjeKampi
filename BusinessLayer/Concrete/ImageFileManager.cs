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
    public class ImageFileManager :IImageFileService
    {
        private readonly IUnitOfWork _uow;

        public ImageFileManager (IUnitOfWork uow)
        {
            _uow = uow;
        }

        public List<ImageFile> GetList()
        {
            return _uow.ImageFiles.List();
        }
    }
}
