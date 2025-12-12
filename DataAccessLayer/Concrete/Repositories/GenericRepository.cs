using DataAccessLayer.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class //t hangi tabloyu temsil ediyorsa o tabloyu alır ve repositorydeki metotları kullanır
    {

        private readonly Context _c; //vertiabanındaki sınıfı alıyoruz

        DbSet<T> _object;//Dbset entity frameworkteki tablo karşılığı hangi tablo olduğunu bilmiyoruz çünkü T generic yapıda 

        public GenericRepository(Context c)
        {
            _c = c; // Context'in atanması
            _object = _c.Set<T>();
        }

        public void Delete(T p)
        {
            var deletedEntity = _c.Entry(p); //silinecek entity değerini parametreden gelen değere eşitledik
            deletedEntity.State = EntityState.Deleted; //silme durumunu entitystate komutuna atadık 
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            return  _object.SingleOrDefault(filter); //filterden gelen değer döndürüldü
        }

        public void Insert(T p)
        {
            var addEntity= _c.Entry(p);//ekleyeceğimiz değeri addEntity değişkenine atadık
            addEntity.State = EntityState.Added;//ekleme durumunu entitystate komutunun durumuna atadık
        }

        public List<T> List()
        {
            return _object.ToList(); //tüm kayıtlar listelenir
        }

        public List<T> List(Expression<Func<T, bool>> filter) //filtrelemeden gelen değere göre listeleme yapılacak
        {
            return _object.Where(filter).ToList(); 
        }

        public void Update(T p)
        {
            var updatedEntity = _c.Entry(p); //güncellenecek entity değerini paramtreden gelen değere atadık
            updatedEntity.State = EntityState.Modified; //güncelleme durumunu entitystate komutuna modified(değiştirildi) olarak atadık
        }
    }
}
