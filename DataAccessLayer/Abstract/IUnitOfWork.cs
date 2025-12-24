using DataAccessLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IUnitOfWork : IDisposable // Tüm DAL arayüzleri (Repositoryler) buraya ReadOnly Property olarak eklenir.
                                               // Bu sayede Manager'lar DAL'daki tüm tablolara tek bir UOW üzerinden erişebilir.
    {
        // Repository Erişim Property'leri (Tüm Repository'lere erişim noktası)
        // Manager artık IWriterDal değil, IUnitOfWork üzerinden Writers'a erişir.
        IAdminDal Admins { get; }
        ICategoryDal Categories { get; }
        IContactDal Contacts { get; }
        IContentDal Contents { get; }
        IImageFileDal ImageFiles { get; }
        IMessageDal Messages { get; }
        ITitleDal Titles { get; }
        IWriterDal Writers { get; }

        void Commit(); // Context.SaveChanges() metodunu çağıracak 
    }
}
