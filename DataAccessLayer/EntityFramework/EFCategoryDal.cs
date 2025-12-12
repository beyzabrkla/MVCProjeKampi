using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Concrete.Repositories;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EFCategoryDal : GenericRepository<Category>, ICategoryDal //Generic repository'den category sınıfını miras alır ve ICategory dal arayüzünü uygular
    {
        // Context alan bir constructor ekliyoruz
        // ve bu context'i 'base(c)' ile üst sınıfa (GenericRepository) gönderiyoruz.
        public EFCategoryDal(Context c) : base(c)
        {
        }
    }
}
