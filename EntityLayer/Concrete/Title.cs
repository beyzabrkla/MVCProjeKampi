using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Title
    {
        [Key]
        public int TitleId { get; set; }

        [StringLength(50)]
        public string TitleName { get; set; }

        public DateTime TitleDate { get; set; }

        public bool TitleStatus { get; set; }

        public  int CategoryId { get; set; } //foreing key categori tablosuna bağlanıcak
        public virtual Category Category { get; set; } //bir başlık bir kategoriye ait olucak
                                                        //Category sınıfından çağırılan nesne

        public  ICollection<Content> Contents{ get; set; } //Collection türündeki interface
                                                           // bir başlıkta birden çok içerik olabilir

        public int WriterId { get; set; } //yazarı başlıkla birleştiriyoruz
                                          //writer tablosuna bağlanıcak

        public virtual Writer Writer { get; set; } //bir başlık bir yazara ait olucak
                                                   //Writer sınıfından çağırılan nesne

    }
}
