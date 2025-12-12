using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Utilities
{
    public static class HashingHelper
    {
        // 1. Düz metin şifreyi alıp hash (karma) değerini oluşturur.
        public static string CreatePasswordHash(string password)
        {
            // SHA256 algoritmasını kullan (Tekrarlayan işlemler için en uygunu bu değil ama basit bir örnektir)
            using (var sha256 = SHA256.Create())
            {
                // Şifreyi byte dizisine çevir
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                // Hash değerini hesapla
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Hash'i string olarak döndür (Veritabanına kaydetmek için)
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        // 2. Kullanıcının girdiği şifre ile veritabanındaki hash'lenmiş şifreyi karşılaştırır.
        public static bool VerifyPasswordHash(string password, string storedHash)
        {
            // Gelen şifreyi tekrar hashle
            string incomingHash = CreatePasswordHash(password);

            // Yeni hash ile veritabanındaki hash'i karşılaştır
            return incomingHash == storedHash;
        }
    }
}
