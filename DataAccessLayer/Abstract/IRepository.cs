using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IRepository<T> //sql den hangi veriyi gönderiyorsan o türü alıcak,  tüm tablolar için ayrı ayrı interface yazmamak için generic yapı kullandık 
    {
        void Insert(T p); //ekleme işlemi
        T Get(Expression<Func<T, bool>> filter, Expression<Func<T, object>> include = null);
        void Update(T p); //güncelleme işlemi
        void Delete(T p); //silme işlemi
        List<T> List(Expression<Func<T,bool>>filter, Expression<Func<T, object>> include = null, string includeProperties = null); //şartlı listeleme işlemi
        List<T> List();
    }
}
