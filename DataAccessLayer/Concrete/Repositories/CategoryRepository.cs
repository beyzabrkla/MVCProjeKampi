using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.Repositories
{
    public class CategoryRepository : ICategoryDal //ICategoryDal arayüzünü implemente ettik
    {
        private readonly Context _c;
        DbSet<Category> _object;//Category sınıfından _object adında DbSet türünde nesne türettik


        // Context, constructor'da dışarıdan alınıyor (DI) ve DbSet başlatılıyor.
        public CategoryRepository(Context c)
        {
            _c = c;
            _object = _c.Set<Category>();
        }

        public List<Category> List()
        {
            return _object.ToList(); //Tüm kategorileri listelemek için ToList() metodunu kullandık
        }
         
        public void Insert(Category p)
        {
            _object.Add(p); //Yeni kategori eklemek için Add() metodunu kullandık
        }

        public void Update(Category p)
        {
            var updatedEntity = _c.Entry(p);
            updatedEntity.State = EntityState.Modified;
        }

        public void Delete(Category p)
        {
            _object.Remove(p); //Kategori silmek için Remove() metodunu kullandık
        }

        public List<Category> List(Expression<Func<Category, bool>> filter, Expression<Func<Category, object>> include = null, string includeProperties = null) //şartlı listeleme için gereken metot
        {
            IQueryable<Category> query = _object.Where(filter);

            if (include != null)
            {
                query = query.Include(include);
            }

            return query.ToList();
        }

        public Category Get(Expression<Func<Category, bool>> filter, Expression<Func<Category, object>> include = null)
        {
            IQueryable<Category> query = _object.Where(filter);
            if (include != null) query = query.Include(include);
            return query.SingleOrDefault();
        }
    }
}
