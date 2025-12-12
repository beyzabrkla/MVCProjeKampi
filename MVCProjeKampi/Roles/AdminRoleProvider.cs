using BusinessLayer.Concrete;         // AdminManager için
using DataAccessLayer.Abstract;       // Interface'ler için (Opsiyonel, duruma göre)
using DataAccessLayer.Concrete;        // Context, UnitOfWork, GenericRepository için
using DataAccessLayer.Concrete.Repositories;
using EntityLayer.Concrete;           // Admin entity'si için
using System;
using System.Linq;
using System.Web.Security;


namespace MVCProjeKampi.Roles
{
    public class AdminRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            //Context'i Oluşturma
            Context context = new Context();

            //UnitOfWork'ü Oluşturma
            UnitOfWork uow = new UnitOfWork(context);

            // GenericRepository'yi Admin Entity'si için Oluşturma
            GenericRepository<Admin> adminRepository = new GenericRepository<Admin>(context);

            //AdminManager'ı Oluşturma
            // CS1503 hatasını çözer.
            AdminManager adminManager = new AdminManager(uow);

            // Manager Üzerinden Rol Bilgisini Çekme (Mimariye Uygun Okuma İşlemi)
            // Manager sınıfınızda kullanıcı adına göre Admin döndüren bir metot (örneğin TGetByUserName) olmalıdır.
            var admin = adminManager.TGetByUserName(username);

            // Context ve UnitOfWork'i manuel olarak yarattığımız için onları Dispose etmemize gerek yoktur, 
            // garbage collector temizleyecektir. Commit() burada gerekmez çünkü sadece okuma yapıyoruz.

            if (admin != null)
            {
                return new string[] { admin.AdminRole };
            }

            // Kullanıcı bulunamazsa veya rolü yoksa boş dizi döndür
            return new string[] { };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}