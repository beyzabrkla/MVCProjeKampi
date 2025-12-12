using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;

        private EFAboutDal _aboutDal; 
        private EFAdminDal _adminDal;
        private EFCategoryDal _categoryDal;
        private EFContactDal _contactDal;
        private EFContentDal _contentDal;
        private EFImageFileDal _imageFileDal;
        private EFMessageDal _messageDal;   
        private EFTitleDal _titleDal;
        private EFWriterDal _writerDal;

        public UnitOfWork(Context context)
        {
            _context = context;
        }


        // --- Repository Erişim Property'leri ---
        public IAboutDal Abouts => _aboutDal ??(_aboutDal= new EFAboutDal(_context));
        public IAdminDal Admins => _adminDal ?? (_adminDal = new EFAdminDal(_context));
        public ICategoryDal Categories => _categoryDal ?? (_categoryDal = new EFCategoryDal(_context));
        public IContactDal Contacts => _contactDal ?? (_contactDal = new EFContactDal(_context));
        public IContentDal Contents => _contentDal ?? (_contentDal = new EFContentDal(_context));
        public IImageFileDal ImageFiles => _imageFileDal ?? (_imageFileDal = new EFImageFileDal(_context));
        public IMessageDal Messages => _messageDal ?? (_messageDal = new EFMessageDal(_context));
        public ITitleDal Titles => _titleDal ?? (_titleDal = new EFTitleDal(_context));
        public IWriterDal Writers => _writerDal ?? (_writerDal = new EFWriterDal(_context));
        // ... Diğer Repository'ler buraya eklenir.

        public void Commit()// Bu metot çağrılmadan hiçbir şey veritabanına kalıcı olarak kaydedilmez.
        {
            _context.SaveChanges();// Context'in SaveChanges'ini çağırır.

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
