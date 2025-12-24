using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class Context : DbContext //db context sınıfına ait özellikler context sınıfına miras olarak verildi
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact>Contacts { get; set; }
        public DbSet<Content>Contents { get; set; }
        public DbSet<Title>Titles { get; set; }
        public DbSet<Writer>Writers { get; set; }
        public DbSet<Message>Messages { get; set; }
        public DbSet<ImageFile>ImageFiles{ get; set; }
        public DbSet<Admin>Admins{ get; set; }
        //context sınıfı sql e tablo olarak yazacak bunları 
        //Örnek: Writer sınfından Writers sayfası


    }
}
