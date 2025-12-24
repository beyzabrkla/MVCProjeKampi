using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class CombinedUnreadMessage
    {
        // Ortak Alanlar
        public int Id { get; set; }
        public string Sender { get; set; } // Contact için UserName, Message için SenderMail
        public string Subject { get; set; }
        public string ShortContent { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; } // Status
        public string Type { get; set; } // "Message" veya "Contact"
        public string DetailLink { get; set; } // Detay sayfasına yönlendirme linki
    }
}
