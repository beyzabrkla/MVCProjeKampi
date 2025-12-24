using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EntityLayer.Concrete
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [StringLength(50)]
        public string SenderMail { get; set; }

        [StringLength(50)]
        public string ReceiverMail { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }

        [AllowHtml]
        public string MessageContent { get; set; }
        public DateTime MessageDate { get; set; }

        public bool IsDraft { get; set; }

        public bool IsRead { get; set; }

        // DÜZELTME: IsTrash yerine SenderTrash ve ReceiverTrash kullanılmalı

        // Mesajın gönderen tarafından (yani yazar tarafından) silinip çöp kutusuna atıldığını belirtir.
        public bool SenderTrash { get; set; }

        // Mesajın alıcı tarafından silinip çöp kutusuna atıldığını belirtir.
        public bool ReceiverTrash { get; set; }

        // Eğer bu alanları kullanmayacaksanız silin:
        // public bool IsTrash { get; set; } 
        // public DateTime? TrashDate { get; set; }

        // Opsiyonel: Çöp Kutusu'na atılma tarihini tek bir alanda tutabilirsiniz
        public DateTime? TrashDate { get; set; }

        public string SourceFolder { get; set; }
    }
}