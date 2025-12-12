using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Concrete.Repositories;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void CategoryAdd(Category category)
        {
            _uow.Categories.Insert(category);
            _uow.Commit();
        }

        public void CategoryDelete(Category category)
        {
            _uow.Categories.Delete(category);
            _uow.Commit();
        }

        public void CategoryUpdate(Category category)
        {
            _uow.Categories.Update(category);
            _uow.Commit();
        }

        public Category GetById(int id)
        {
            return _uow.Categories.Get(x => x.CategoryId == id);//categoryden gelen id ile getbyid metodundaki id yi eşitle
        }

        public List<Category> GetList()
        {
            return _uow.Categories.List();
        }

    }
}
