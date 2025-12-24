# 🛡️ MVC PROJE KAMPI: Gelişmiş Yetki, Rol Yönetim, Yazar Blog Sistemi

## ✨ Proje Özeti
Bu uygulama, **ASP.NET MVC** mimarisi üzerinde inşa edilmiş, Katmanlı Mimari (N-Tier) prensiplerini uygulayan kapsamlı bir blog sistemidir. Temel odak noktaları; **Code First** yaklaşımıyla veritabanı yönetimi, **Dependency Injection (DI)** ile bağımlılıkların yönetilmesi ve özelleştirilmiş **Rol/Yetki Kontrolü**dür.

## 🏗️ Mimarinin Detayları ve Katmanlar
Proje, temiz kod ve sürdürülebilirlik için dört ana katmandan oluşmaktadır. 

### 1️⃣ Presentation Layer (MVCProjeKampi)
Bu katman, Controller'lar, View'ler ve özelleştirilmiş yetkilendirme filtrelerini barındırır.

* **Controllers:** `LoginController`, `AdminCategoryController`, `WriterPanelContentController` gibi temel modülleri yönetir.
* **Yetkilendirme:** `WriterAuthorizeAttribute` ve `RoleBasedRedirectAttribute` gibi özel `Attribute`'lar kullanılarak Role-Based Access Control (RBAC) uygulanmıştır.
* **Görünümler (Views):** Ayrı klasörlenmiş yapısı ile yönetim paneli ve yazar paneli görünüm setlerini içerir.

### 2️⃣ Business Layer (İş Mantığı Katmanı)
Tüm iş kuralları ve validasyonlar bu katmanda yer alır. **Autofac** kullanılarak **Dependency Injection (DI)** ile bağımlılıklar yönetilmiştir.

* **Soyutlama (Abstract):** Tüm servis arabirimleri (`IAdminService`, `IContentService`, vb.) burada tanımlanmıştır.
* **Somut (Concrete):** Servis yöneticileri (`AdminManager`, `ContentManager`) arabirimleri uygular.
* **Validasyonlar:** Veri girişi kuralları (örneğin, `CategoryValidator`, `WriterValidator`) için **Fluent Validation** kullanılmıştır.
* **Güvenlik:** Şifreleme işlemleri için `HashingHelper` bulunur.

### 3️⃣ Data Access Layer (Veri Erişim Katmanı)
Veri tabanıyla ilgili tüm işlemlerin yönetildiği katmandır.

* **Entity Framework:** Veri tabanı ile iletişim ORM aracı olarak kullanılmıştır.
* **Repository Pattern:** Veri erişim işlemlerinin soyutlanması için `IRepository` ve somut `GenericRepository` kullanılmıştır.
* **Unit of Work (UOW):** `IUnitOfWork` yapısı ile birden fazla Repository işleminin tek bir transaction altında toplanması sağlanmıştır.
* **Code First & Migration:** Veritabanı yapısı **Code First** ile oluşturulmuş ve değişiklikler **Migrations** ile yönetilmiştir.

### 4️⃣ Entity Layer (Varlık Katmanı)
Veritabanındaki tablolara karşılık gelen POCO (Plain Old CLR Object) sınıflarını içerir (`Admin`, `Category`, `Content` vb.). 

## 🔐 Yetkilendirme ve Güvenlik Mekanizmaları
Proje, hem **Forms Authentication** hem de özelleştirilmiş Role Provider'lar kullanarak yüksek güvenlik ve esnek yetkilendirme sağlar.

### 1. 🔑 Admin Girişi
Kullanıcı Mail ve Şifre ile sisteme erişim. 
<img width="1919" height="911" alt="1" src="https://github.com/user-attachments/assets/58966ca7-6690-4422-aef0-2f454fa03287" />

### 🛂 Rol Yönetimi

* **Özel Yetki Attribute'ları:** `WriterAuthorizeAttribute` gibi özel sınıflar, sadece ilgili rollere sahip kullanıcıların belirli Controller/Action metotlarına erişmesini sağlar.
* **Rol Sağlayıcı (Role Provider):** `AdminRoleProvider.cs` sınıfı, sistemin tanımlanmış rolleri (Admin, Writer vb.) kontrol etmesine olanak tanır.

### 2. 🖥️ Admin Paneli: Kategori Yönetimi (CRUD)
Admin girişinden sonra erişilen ana panellerden biri olan Kategori Yönetim Sayfası, Admin rolüne sahip kullanıcının sistemdeki tüm blog kategorilerini görmesini ve yönetmesini sağlar. Bu, projenin temel CRUD (Oluşturma, Okuma, Güncelleme, Silme) işlevlerini somutlaştırır.
<img width="1919" height="910" alt="2" src="https://github.com/user-attachments/assets/023fa9b0-2db4-4375-a79c-732b1a68f244" />

### 3. 🎬 Genel Başlık (İçerik) Yönetimi
Kategoriden bağımsız olarak sistemdeki tüm içerik başlıklarının listelendiği ana sayfa.
<img width="1919" height="910" alt="4" src="https://github.com/user-attachments/assets/0f923a29-9d49-4086-9092-1ceb5585814c" />

### 4. 🔗 Kategori Bazında İçerik Filtreleme
Admin, Kategori Yönetimi sayfasından bir kategori (Örn: Dizi) için **Başlıklar** butonuna tıkladığında, o kategoriye ait tüm başlıklar listelenir ve kimin tarafından oluşturulduğu takip edilir.
**Yazılar** butonuna tıklandığında, başlığın detay sayfası açılır. Burada, başlığa ait tüm alt metinler veya ilgili yazar katkıları listelenir.
Seçilen başlığa (**Breaking Bad**) ait tüm katkılar (içerik parçaları veya yorumlar) listelenir. Her katkının hangi yazar tarafından yapıldığı ve metni açıkça belirtilir. |
<img width="1919" height="914" alt="3" src="https://github.com/user-attachments/assets/15958f96-4859-4a59-ab49-288b382a5c60" />
<img width="1919" height="916" alt="5" src="https://github.com/user-attachments/assets/40c72095-aa66-4973-b8b3-60f09ecc63f3" />

### 5. 📊 Gelişmiş Raporlama ve Dışa Aktarma (Başlık Listesi Raporu)
Admin Paneli, sadece CRUD işlemleriyle sınırlı kalmaz; aynı zamanda sistemdeki verilerin **raporlanması** ve farklı formatlarda dışa aktarılması (export) yeteneğini sunar. **Raporlar** bölümü, özellikle büyük veri setlerinin yönetilmesi ve analiz edilmesi için önemlidir.
<img width="1919" height="889" alt="6" src="https://github.com/user-attachments/assets/000d15bf-3cd9-4e1c-bf66-2cfb5423b6e5" />

### 6. 👥 Yazar Yönetimi (Profil ve Katkı Takibi)
**Yazarlar Sayfası**, Admin'in sistemdeki tüm yazar profillerini toplu olarak görüntülemesini, profillerini düzenlemesini, yeni yazar eklenmesini ve yazarların içeriklerini takip etmesini sağlar. Bu, insan kaynakları (HR) ve içerik denetimi açısından kritik bir modüldür.
<img width="1919" height="915" alt="7" src="https://github.com/user-attachments/assets/bd25e587-dd43-4c92-be82-ed9348766dca" />

### 7. 📈 İçerik Dağılım Grafiği (Analitik Görselleştirme)
Admin Paneli, yalnızca ham veri yönetimi değil, aynı zamanda sistemdeki içeriğin dağılımını görsel olarak analiz etme yeteneği sunar. Bu grafik modülü, hangi başlığın veya kategorinin sisteme ne kadar katkı sağladığını gösterir.
<img width="1919" height="916" alt="8" src="https://github.com/user-attachments/assets/badcf2d1-afb1-42d1-bea0-44ffe4f04303" />


### 8. 📧 İletişim ve Mesajlaşma Modülü
Admin Paneli, **İletişim & Mesajlar** modülü aracılığıyla hem dışarıdan gelen (web sitesi iletişim formu) mesajları yönetir hem de sistem içi mesajlaşma yeteneği sunar. Bu modül, tam bir e-posta istemcisi işlevselliği taşır.

#### 9.📝 Yeni Mesaj Oluşturma
Admin, sistemdeki diğer kullanıcılara veya harici mail adreslerine yeni mesaj gönderebilir veya taslaklara kaydedebilir.
<img width="1919" height="919" alt="9" src="https://github.com/user-attachments/assets/4988694d-1cca-484e-aeca-9300579e97b7" />
<img width="1919" height="912" alt="10" src="https://github.com/user-attachments/assets/52a64314-403e-47f5-919e-28b4386c4317" />


#### 10.📥 Mesaj Yönetimi Arayüzü
Sol menüde farklı klasörler halinde gelen ve giden mesajlar organize edilir.
| **İletişim** | Web sitesinin tanıtım kısmındaki doldurulan iletişim formundan gelen tüm mesajlar. |
| **Gelen Mesajlar** | Sistemdeki diğer kullanıcılardan gelen mesajlar. |
| **Gönderilen Mesajlar** | Admin'in gönderdiği mesajlar. |
| **Okunmamış Mesajlar** | Yeni gelen ve henüz görüntülenmemiş mesajlar (Kırmızı bildirim etiketi ile gösterilir). |
| **Taslaklar** | Kaydedilmiş ancak henüz gönderilmemiş mesajlar. |

#### 11. 🗑️ Çöp Kutusu ve Toplu İşlemler
Silinen tüm mesajlar geçici olarak Çöp Kutusu'nda saklanır ve buradan geri yüklenebilir veya kalıcı olarak silinebilir.
<img width="1919" height="913" alt="12" src="https://github.com/user-attachments/assets/447f5a71-dee0-47ac-815b-839322a76b43" />

### 12. 🛂 Gelişmiş Yetkilendirme ve Rol Yönetimi
**Yetkilendirmeler Sayfası**, Admin'e, alt Admin kullanıcılarının (Admin rolüne sahip diğer hesaplar) yetki seviyelerini **dinamik olarak** yönetme olanağı tanır. Bu, çok kullanıcılı yönetim panellerinde yetki ayrımı yapmak için kritik bir özelliktir.
<img width="1919" height="910" alt="13" src="https://github.com/user-attachments/assets/1e2ebc93-d323-4797-a400-3646513f8ae8" />

### 13. 🖼️ Sözlük Galeri ve Medya Yönetimi
örsel dosyaların yönetildiği ve **Ekko Lightbox** gibi kütüphanelerle görüntüleme yeteneği sunan Galeri modülü. |
<img width="1919" height="919" alt="14" src="https://github.com/user-attachments/assets/96f5a298-2a22-4cbd-b457-7ff1f50749e8" />

### 14. 🛑 Hata Yönetimi (Özelleştirilmiş Hata Sayfası)
Kullanıcı dostu bir deneyim için standart hata ekranları yerine özelleştirilmiş **404 - Sayfa Bulunamadı** ekranı. |
<img width="1915" height="992" alt="15" src="https://github.com/user-attachments/assets/9d694b06-b66d-404a-ad66-b367e7190096" />

### 15. 📝 Yazar Girişi (Writer Login) 
Yazar rolüne sahip kullanıcılar için özel olarak tasarlanmış giriş arayüzüdür. Bu giriş başarılı olduğunda, yazarın sadece kendi içeriklerini görebileceği ve yönetebileceği **Yazar Paneline** erişimi sağlanır.
<img width="1919" height="917" alt="16" src="https://github.com/user-attachments/assets/2624e821-2e68-46dc-bbea-6f44624d5d83" />

### 16. 👤 Yazar Profil Düzenleme
Yazar, panel menüsündeki **Profilim** sekmesinden kendi kişisel ve mesleki bilgilerini güncelleyebilir.
<img width="1919" height="909" alt="17" src="https://github.com/user-attachments/assets/34613c8a-7029-45d6-ab48-e24e4e108c0e" />

### 17. 📄 Yazarın Başlıkları (İçerik Yönetimi)
**Başlıklarım** menüsü, yazarın sisteme eklediği içerikleri filtreleyerek sadece kendisine ait başlıkları gösterir. Bu, yazarın kendi katkıları üzerinde tam kontrol sahibi olmasını sağlar.
<img width="1919" height="902" alt="18" src="https://github.com/user-attachments/assets/b633e863-a54c-47f2-879c-59f7b7a6f067" />





