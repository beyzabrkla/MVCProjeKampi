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

### 🔑 Admin Girişi
<img width="1919" height="911" alt="1" src="https://github.com/user-attachments/assets/58966ca7-6690-4422-aef0-2f454fa03287" />
Kullanıcı Mail ve Şifre ile sisteme erişim. |

### 🛂 Rol Yönetimi

* **Özel Yetki Attribute'ları:** `WriterAuthorizeAttribute` gibi özel sınıflar, sadece ilgili rollere sahip kullanıcıların belirli Controller/Action metotlarına erişmesini sağlar.
* **Rol Sağlayıcı (Role Provider):** `AdminRoleProvider.cs` sınıfı, sistemin tanımlanmış rolleri (Admin, Writer vb.) kontrol etmesine olanak tanır.

### 🖥️ Admin Paneli: Kategori Yönetimi (CRUD)
<img width="1919" height="910" alt="2" src="https://github.com/user-attachments/assets/023fa9b0-2db4-4375-a79c-732b1a68f244" />
Admin girişinden sonra erişilen ana panellerden biri olan Kategori Yönetim Sayfası, Admin rolüne sahip kullanıcının sistemdeki tüm blog kategorilerini görmesini ve yönetmesini sağlar. Bu, projenin temel CRUD (Oluşturma, Okuma, Güncelleme, Silme) işlevlerini somutlaştırır.

### 3. 🎬 Genel Başlık (İçerik) Yönetimi
<img width="1919" height="910" alt="4" src="https://github.com/user-attachments/assets/0f923a29-9d49-4086-9092-1ceb5585814c" />
Kategoriden bağımsız olarak sistemdeki tüm içerik başlıklarının listelendiği ana sayfa.

### 3. 🎬 İçerik (Başlık) Yönetimi
<img width="1919" height="914" alt="3" src="https://github.com/user-attachments/assets/15958f96-4859-4a59-ab49-288b382a5c60" />
<img width="1919" height="916" alt="5" src="https://github.com/user-attachments/assets/40c72095-aa66-4973-b8b3-60f09ecc63f3" />
Admin, Kategori Yönetimi sayfasından bir kategori (Örn: Dizi) için **Başlıklar** butonuna tıkladığında, o kategoriye ait tüm başlıklar listelenir ve kimin tarafından oluşturulduğu takip edilir.
**Yazılar** butonuna tıklandığında, başlığın detay sayfası açılır. Burada, başlığa ait tüm alt metinler veya ilgili yazar katkıları listelenir.
Seçilen başlığa (**Breaking Bad**) ait tüm katkılar (içerik parçaları veya yorumlar) listelenir. Her katkının hangi yazar tarafından yapıldığı ve metni açıkça belirtilir. |



