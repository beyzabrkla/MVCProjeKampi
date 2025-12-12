using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class AdminManager : IAdminService
    {
        private readonly IUnitOfWork _uow; // <-- Değişiklik: IUnitOfWork kullanıyoruz

        public AdminManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // --- GÜVENLİK İÇİN YARDIMCI METOT ---
        // Basit SHA256 Hashleme (Güvenlik notu geçerlidir!)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public void AdminAdd(Admin admin)
        {
            // KAYIT İŞLEMİNDEN ÖNCE ŞİFREYİ HASHLE
            admin.AdminPassword = HashPassword(admin.AdminPassword);

            _uow.Admins.Insert(admin); // <-- UOW üzerinden AdminDal'a erişim
            _uow.Commit();             // <-- Değişiklikleri kaydetme
        }

        public void AdminDelete(Admin admin)
        {
            // _adminDal.Delete(admin); yerine
            _uow.Admins.Delete(admin); // <-- DÜZELTME
            _uow.Commit(); // Eğer Delete işlemi için de Commit gerekiyorsa ekleyin.
        }

        public Admin AdminLogin(string username, string password)
        {
            // UOW üzerinden AdminDal'ı kullanarak Admin kaydını çek
            var admin = _uow.Admins.Get(x => x.AdminUserName == username);

            if (admin != null)
            {
                // 2. Girilen şifreyi Hashle
                string hashedPassword = HashPassword(password);

                // 3. Hashlenmiş şifreleri karşılaştır
                if (admin.AdminPassword == hashedPassword)
                {
                    return admin; // Giriş başarılı
                }
            }

            return null; // Giriş başarısız
        }

        public void AdminUpdate(Admin admin)
        {
            _uow.Admins.Update(admin); // <-- DÜZELTME
            _uow.Commit(); // Kaydetme işlemi için Commit ekleyin
        }

        public Admin GetByID(int id)
        {
            return _uow.Admins.Get(x => x.AdminId == id);
        }

        public List<Admin> GetList()
        {
            return _uow.Admins.List();
        }
    }
}
