# Arsivim - Kişisel Belge Arşiv Sistemi

## 📋 Proje Tanımı

Arsivim, kişisel veya kurumsal belgeleri güvenli, merkezi bir sistemde saklamak, yönetmek, görüntülemek ve yedeklemek için geliştirilmiş bir .NET MAUI uygulamasıdır. Sistem, tek kullanıcı odaklı olup gizliliğe öncelik verir ve yerel depolama kullanır.

## 🚀 Özellikler

### ✅ Tamamlanan Özellikler
- Modüler proje yapısı (Core, Data, Services, UI katmanları)
- Entity Framework Core ile SQLite veritabanı
- Repository pattern implementasyonu
- MVVM pattern ile View-ViewModel ayrımı
- Dependency Injection yapılandırması
- Temel veri modelleri (Belge, Kişi, Etiket, Favori, vb.)
- Ana sayfa tasarımı ve ViewModel'i

### 🔄 Geliştirme Aşamasında
- Belge ekleme/düzenleme/silme işlemleri
- Kişi yönetimi
- Etiket sistemi
- Arama ve filtreleme
- Tema yönetimi
- Yedekleme sistemi

### 📅 Planlanan Özellikler
- OCR desteği (Tesseract.NET)
- PDF görüntüleme
- QR kod paylaşımı
- LAN senkronizasyonu
- Güvenlik ve şifreleme
- Mobil platform optimizasyonları

## 🏗️ Proje Yapısı

```
Arsivim/
├── 📁 Arsivim.Core/           # Temel modeller, interface'ler ve enum'lar
│   ├── Models/                # Veri modelleri
│   ├── Interfaces/            # Servis arayüzleri
│   └── Enums/                 # Sabit değerler
├── 📁 Arsivim.Data/           # Veritabanı katmanı
│   ├── Context/               # Entity Framework DbContext
│   └── Repositories/          # Repository pattern implementasyonu
├── 📁 Arsivim.Services/       # İş mantığı servisleri
│   └── Core/                  # Temel servisler
├── 📁 Arsivim.Shared/         # Paylaşılan yardımcı sınıflar
│   ├── Helpers/               # Yardımcı fonksiyonlar
│   └── Constants/             # Sabit değerler
├── 📁 Arsivim/ (MAUI)         # Ana uygulama
│   ├── ViewModels/            # MVVM pattern ViewModels
│   └── Views/                 # XAML sayfaları
```

## 🛠️ Teknolojiler

- **.NET 8.0** - Temel framework
- **.NET MAUI** - Çapraz platform UI
- **Entity Framework Core** - ORM
- **SQLite** - Veritabanı
- **MVVM Pattern** - UI pattern
- **Repository Pattern** - Veri erişim pattern
- **Dependency Injection** - IoC container

## 🔧 Kurulum

### Gereksinimler
- Visual Studio 2022 (17.8+)
- .NET 8.0 SDK
- MAUI workload

### Adımlar
1. Projeyi klonlayın
2. Visual Studio'da solution'ı açın
3. NuGet paketlerini restore edin
4. Build edip çalıştırın

```bash
git clone [repository-url]
cd Arsivim
dotnet restore
dotnet build
```

## 📊 Veritabanı Modeli

### Ana Tablolar
- **Kisiler** - Kişi bilgileri
- **Belgeler** - Belge meta verileri ve içeriği
- **Etiketler** - Kategorizasyon için etiketler
- **KisiBelge** - Kişi-Belge ilişkisi (Many-to-Many)
- **BelgeEtiket** - Belge-Etiket ilişkisi (Many-to-Many)
- **BelgelerVersiyonlar** - Belge versiyon takibi
- **Notlar** - Belge notları
- **Favoriler** - Favori belgeler
- **Gecmis** - İşlem geçmişi
- **Ayarlar** - Uygulama ayarları
- **Kullanici** - Kullanıcı bilgileri

## 🎯 Hedef Platformlar

- ✅ **Windows** - Ana hedef platform
- 🔄 **Android** - Mobil erişim
- 📅 **iOS** - Gelecek sürümler için
- 📅 **macOS** - Gelecek sürümler için

## 📈 Geliştirme Durumu

| Modül | Durum | Tamamlanma |
|-------|-------|------------|
| Core Models | ✅ | %100 |
| Data Layer | ✅ | %100 |
| Basic Services | ✅ | %80 |
| UI Framework | ✅ | %60 |
| Business Logic | 🔄 | %40 |
| Advanced Features | 📅 | %0 |

## 🚦 Sonraki Adımlar

1. **Belge İşlemleri** - CRUD operasyonları
2. **Dosya Yükleme** - Drag & drop desteği
3. **Arama Sistemi** - Gelişmiş filtreleme
4. **UI/UX İyileştirmeleri** - Modern tasarım
5. **Test Coverage** - Unit ve integration testler
6. **Performance** - Optimizasyon

## 📝 Lisans

Bu proje MIT lisansı altında geliştirilmektedir.

## 👥 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun
3. Commit edin
4. Push edin
5. Pull Request açın

---

**Arsivim** - Belgelerinizi düzenli tutmanın en kolay yolu! 📁✨