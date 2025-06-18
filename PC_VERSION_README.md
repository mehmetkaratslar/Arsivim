# Arsivim PC Versiyonu

## 📋 Genel Bakış

Arsivim, profesyonel belge arşivleme ve yönetim sistemidir. Bu PC versiyonu, Windows 10/11 sistemlerde çalışmak üzere optimize edilmiştir ve masa üstü kullanıcılar için gelişmiş özellikler sunar.

## 🖥️ Sistem Gereksinimleri

- **İşletim Sistemi:** Windows 10 version 19041 (May 2020 Update) veya üzeri
- **Framework:** .NET 8.0 Runtime
- **Bellek:** Minimum 4 GB RAM (8 GB önerilir)
- **Disk Alanı:** 500 MB boş alan (belgeler için ek alan gerekir)
- **Ekran Çözünürlüğü:** Minimum 1024x768 (1920x1080 önerilir)

## 🚀 Kurulum

### Seçenek 1: Build Script ile Otomatik Kurulum
1. Proje klasöründe `build-and-run-windows.bat` dosyasını çalıştırın
2. Script otomatik olarak gerekli bağımlılıkları kontrol edecek ve uygulamayı derleyecektir

### Seçenek 2: Manuel Kurulum
1. **.NET 8.0 SDK**'yı yükleyin: https://dotnet.microsoft.com/download/dotnet/8.0
2. **MAUI Workload**'ı yükleyin:
   ```bash
   dotnet workload install maui
   ```
3. Projeyi derleyin:
   ```bash
   dotnet build -c Release -f net8.0-windows10.0.19041.0
   ```
4. Uygulamayı çalıştırın:
   ```bash
   dotnet run -c Release -f net8.0-windows10.0.19041.0
   ```

## 🎯 PC Özel Özellikler

### 🖱️ Gelişmiş Fare Desteği
- **Sağ Tık Menüsü:** Belge listesinde her belge için kapsamlı context menu
- **Drag & Drop:** Dosyaları doğrudan pencereye sürükleyip bırakın
- **Çift Tık:** Belgeler üzerinde çift tıkla hızlı açma

### ⌨️ Klavye Kısayolları
- `Ctrl + N` - Yeni Belge Ekle
- `Ctrl + F` - Belge Arama
- `Ctrl + Shift + S` - Ayarlar
- `F1` - Yardım ve Klavye Kısayolları
- `Ctrl + R` - Listeyi Yenile

### 🪟 Pencere Yönetimi
- **Minimum Boyut:** 800x600 piksel
- **Varsayılan Boyut:** 1200x800 piksel
- **Merkeze Hizalama:** Uygulama başlangıçta ekranın ortasında açılır
- **Özel Başlık Çubuğu:** Modern ve şık tasarım

### 📁 Dosya Sistemi Entegrasyonu
- **Klasörde Göster:** Belgeyi Windows Explorer'da göster
- **Dosya Yolu Kopyala:** Dosya yolunu panoya kopyala
- **Dosya Uzantısı Desteği:** PDF, DOCX, XLSX, PPTX, JPG, PNG, TIFF formatları

## 🔧 Kullanım Kılavuzu

### Belge Ekleme
1. **Drag & Drop:** Dosyaları pencereye sürükleyin
2. **Dosya Seçici:** "Yeni Belge" butonuna tıklayın
3. **Kamera:** Fotoğraf çekerek belge ekleyin

### Belge Arama
- **Metin Arama:** Belge adı, açıklama ve OCR metinlerinde arama
- **Etiket Arama:** Etiket adlarında arama
- **Kişi Arama:** Belgeyle ilişkili kişilerde arama
- **Filtreler:** Belge türü ve favori durumuna göre filtreleme

### Context Menu (Sağ Tık) Seçenekleri
- 🔍 **Aç** - Belgeyi görüntüle
- ⬇️ **İndir** - Belgeyi kaydet
- 📤 **Paylaş** - Belgeyi paylaş
- ✏️ **Düzenle** - Belge bilgilerini düzenle
- 📋 **Yolu Kopyala** - Dosya yolunu kopyala
- 📁 **Klasörde Göster** - Windows Explorer'da aç
- ❤️ **Favoriye Ekle/Çıkar** - Favori durumunu değiştir
- 🗑️ **Sil** - Belgeyi sil

## 🏷️ Etiketleme Sistemi
- **Otomatik Renk:** Her etiket otomatik renk alır
- **Çoklu Etiket:** Bir belgeye birden fazla etiket eklenebilir
- **Arama Desteği:** Etiketlere göre arama yapılabilir
- **Global Yönetim:** Tüm etiketler merkezi olarak yönetilir

## 👥 Kişi Yönetimi
- **Belge-Kişi Bağlantısı:** Belgeleri kişilerle ilişkilendirme
- **Kişi Arama:** Kişilere göre belge filtreleme
- **Detaylı Bilgi:** Ad, soyad, telefon, e-posta desteği

## 📊 İstatistikler
- **Toplam Belge Sayısı:** Sistemdeki toplam belge sayısı
- **Dosya Boyutu İstatistikleri:** Belge boyutları analizi
- **En Çok Görüntülenen:** Popüler belgeler
- **Son Eklenenler:** Yeni belgeler listesi

## 🔒 Güvenlik
- **Yerel Depolama:** Veriler sadece bilgisayarınızda saklanır
- **SQLite Veritabanı:** Güvenli ve hızlı veri depolama
- **Dosya Hash'leme:** Belge bütünlüğü kontrolü
- **Veri Şifreleme:** Gelecek versiyonlarda eklenecek

## 🐛 Sorun Giderme

### Uygulama Açılmıyor
1. .NET 8.0 Runtime yüklü olduğundan emin olun
2. Windows güncellemelerini kontrol edin
3. Uygulamayı yönetici olarak çalıştırmayı deneyin

### Dosya Sürükle-Bırak Çalışmıyor
1. Uygulamanın yönetici yetkisiyle çalıştığından emin olun
2. Dosya formatının desteklendiğini kontrol edin
3. Dosya boyutunun makul olduğundan emin olun

### Performans Problemleri
1. Bilgisayarınızda yeterli RAM olduğundan emin olun
2. Çok büyük dosyalarla çalışırken sabırlı olun
3. Uygulamayı yeniden başlatın

## 📝 Sürüm Notları

### v1.0.0 (İlk PC Sürümü)
- ✅ Drag & Drop desteği
- ✅ Klavye kısayolları
- ✅ Context menü (sağ tık)
- ✅ Windows Explorer entegrasyonu
- ✅ Gelişmiş pencere yönetimi
- ✅ Dosya yolu kopyalama
- ✅ OCR desteği (temel)
- ✅ Çoklu format desteği

## 🔮 Gelecek Özellikler
- 🔄 Otomatik yedekleme
- 🌐 Bulut desteği
- 🔍 Gelişmiş OCR
- 📱 Mobil senkronizasyon
- 🎨 Tema seçenekleri
- 📈 Detaylı raporlama

## 📞 Destek ve İletişim

Sorularınız ve önerileriniz için:
- 📧 E-posta: [destek@arsivim.com]
- 🐛 Bug Report: GitHub Issues
- 💡 Özellik İsteği: GitHub Discussions
- 📖 Dokümantasyon: Wiki sayfası

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için LICENSE dosyasına bakınız.

---

**Arsivim PC Versiyonu** - Belgelerinizi profesyonel şekilde yönetin! 🚀 

# Debug modunda build
dotnet build -c Debug -f net8.0-windows10.0.19041.0

# Uygulamayı çalıştırma
dotnet run --launch-profile Arsivim --framework net8.0-windows10.0.19041.0

# Ya da batch dosyası ile
.\build-and-run-windows.bat 