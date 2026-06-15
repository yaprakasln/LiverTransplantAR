# RAMS TASARIM İLKELERİ RAPORU
**Proje Adı:** LiverTransplantAR: Karaciğer Nakli Sonrası Yaşam Simülasyonu

Bu belge, uygulamanın yazılım mühendisliği disiplinindeki RAMS (Güvenilirlik, Kullanılabilirlik, Bakım Yapılabilirlik, Güvenlik) tasarım kriterlerine nasıl hizmet ettiğini açıklamaktadır.

## 1. GÜVENİLİRLİK (Reliability)
Uygulamanın kararlı çalışması ve çökmelerin önlenmesi için aşağıdaki önlemler alınmıştır:
- **Deterministik State Machine Mimari:** Kullanıcının etkileşimde bulunduğu 4 ana senaryo (Recovery, Medication, Rejection, Lifestyle) arasındaki durum geçişleri, hata olasılığını sıfıra indiren bir State Machine mimarisi ile yönetilmektedir.
- **Çapa (Anchoring) Kararlılığı:** `ARAnchorManager` kullanılarak 3B karaciğer modeli dünya koordinatlarına sıkıca bağlanmıştır. Bu sayede kameranın hareketlerinden kaynaklanan kaymalar (drifting) engellenmiş, kararlı bir AR takibi sağlanmıştır.
- **Hata Toleransı ve İstisna Yönetimi:** Kaynakların yüklenmesi veya model dosyalarının okunması esnasında oluşabilecek çalışma zamanı (Runtime) hataları try-catch bloklarıyla yakalanarak uygulamanın aniden kapanması önlenmiştir.

## 2. KULLANILABİLİRLİK / ERİŞİLEBİLİRLİK (Availability)
Sistemin her an çalışmaya hazır ve erişilebilir olması şu yollarla sağlanmıştır:
- **Tamamen Çevrimdışı (Offline) Yapı:** Uygulama, sunucu veya internet bağlantısına ihtiyaç duymadan cihaz üzerinde yerel olarak çalışmaktadır. Bu durum hastanelerde, laboratuvarlarda veya internetin çekmediği kritik alanlarda dahi yüksek erişilebilirlik sunar.
- **Akıllı Bellek Yönetimi (Addressables):** Unity Addressable Asset System kullanılarak, kullanılmayan 3D modeller ve ağır grafik dosyaları RAM üzerinde gereksiz yer kaplamayacak şekilde dinamik olarak yüklenip boşaltılmaktadır. Bu sayede uygulamanın cihaz belleğinin dolmasından dolayı kapanması engellenmiştir.

## 3. BAKIM YAPILABILİRLİK (Maintainability)
Yazılımın gelecekte kolayca güncellenebilmesi ve hataların hızlı giderilmesi için tasarımsal olarak şu kurallara uyulmuştur:
- **Clean Architecture Prensipleri:** Kod yapısı; Veri (fbx modelleri, ScriptableObject dosyaları), İş Mantığı (durum yönetim scriptleri) ve Sunum (UI, Canvas, ARRenderer) olarak birbirinden tamamen ayrılmıştır.
- **Unity Editör Araçları Entegrasyonu:** Geliştirilen `ARSituationPerfecter.cs` özel editör aracı sayesinde, teknik bilgisi olmayan bir tasarımcı bile kod satırına dokunmadan karaciğer ölçeklerini ayarlayabilir ve sahneleri yeniden derleyebilir.
- **Modüler Senaryo Ekleme:** Yeni bir tıbbi komplikasyon veya organ simülasyonu eklenmek istendiğinde, mevcut kodu değiştirmek yerine sadece yeni bir ScriptableObject tanımlamak yeterli olmaktadır.

## 4. GÜVENLİK (Safety)
Kullanıcı güvenliği ve veri gizliliği üst seviyede tutulmuştur:
- **Kişisel Verilerin Korunması (KVKK/HIPAA):** Uygulama hiçbir hasta verisi, kişisel bilgi veya tıbbi geçmiş kaydı toplamamaktadır. Bulut depolama kullanılmadığı için veri sızıntısı veya çalınması riski sıfırdır.
- **Fiziksel Kullanıcı Güvenliği:** AR modülü ilk açıldığında, kullanıcının gerçek çevresindeki fiziksel nesnelere (masa kenarları, engeller vb.) çarpmaması için ekran üzerinde belirgin bir "Güvenli Alan ve Çevre Uyarısı" gösterilmektedir.
- **Tıbbi Bilgilendirme Sınırları:** Yanlış tedavi yorumlarını önlemek amacıyla simülasyon içerisindeki her bilgi kartının altında "Bu uygulama sadece eğitim amaçlıdır, tıbbi tavsiye yerine geçmez" ibaresi yer almaktadır.
