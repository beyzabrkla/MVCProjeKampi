using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Content //içerik
    {
        [Key]
        public int ContentId { get; set; }

        [StringLength(1000)]
        public string ContentValue { get; set; }

        public DateTime ContentDate { get; set; }

        public bool ContentStatus { get; set; }

        public int TitleId { get; set; }//foreing key title tablosuna bağlanıcak ve ıd değerini çekicek
        public virtual Title Title { get; set; }//Title sınıfından çağırılan içerik bilgisi
                                                // aynı içerik 2 başlıkla kullanılamaz o yüzden virtual yaptık
   
        public int? WriterId { get; set; } //foreing key writer tablosuna bağlanıcak ve ıd değerini çekicek
        public virtual Writer Writer { get; set; } //bir içerik bir yazara ait olucak
                                                   //Writer sınıfından çağırılan nesne

    }

}
