# TEKNOLOJİ HAZIRLIK SEVİYESİ (THS) RAPORU
**Proje Adı:** LiverTransplantAR: Karaciğer Nakli Sonrası Yaşam Simülasyonu

Bu raporda, geliştirilen arttırılmış gerçeklik tıbbi simülasyon sisteminin Teknoloji Hazırlık Seviyesi (THS) kriterlerine göre değerlendirmesi yer almaktadır. Projemiz **THS 4** seviyesini tamamlamış ve **THS 5** seviyesine başarıyla ulaşmıştır.

---

## 1. TEKNOLOJİ HAZIRLIK SEVİYELERİ ANALİZİ

### THS 1 - Temel Prensiplerin Gözlenmesi (Tamamlandı)
- Tıp eğitiminde ve organ nakli geçiren hastaların bilgilendirilmesinde 3B modellerin ve arttırılmış gerçekliğin (AR) öğrenim hızını ve hasta uyumunu artırdığına dair literatür taraması yapılmış ve teorik zemin doğrulanmıştır.

### THS 2 - Teknoloji Konseptinin Formüle Edilmesi (Tamamlandı)
- Unity motoru üzerinde AR Foundation, Universal Render Pipeline (URP) ve özelleştirilmiş organik shader bileşenlerinin kullanılmasına yönelik teknik mimari tasarlanmıştır. Dört temel senaryonun akış diyagramı oluşturulmuştur.

### THS 3 - Analitik ve Deneysel Kritik Fonksiyon Kanıtı (Tamamlandı)
- Masaüstü simülasyon ortamında karaciğer 3B modeli üzerinde doku renklerinin sararması (rejeksiyon durumu), damarların içinden akan sıvı animasyonunun yavaşlaması (tromboz durumu) ve rejenerasyon büyüme mantığı yazılımsal olarak simüle edilmiş ve mantık doğrulamaları yapılmıştır.

### THS 4 - Laboratuvar Ortamında Bileşenlerin Doğrulanması (Tamamlandı)
- Yazılımın tüm alt modülleri (ARPlacementManager, UI HUD paneli, LiverRegeneration yöneticisi ve URP shader'lar) tek bir Unity projesinde birleştirilmiştir. Geliştirme laboratuvarı ortamındaki test cihazlarında (Android/iOS telefonlar) tüm sistemin birbirleriyle kararlı bir şekilde haberleştiği doğrulanmıştır.

### THS 5 - İlgili Ortamda Bileşenlerin Doğrulanması (Mevcut Seviye)
- Uygulama, laboratuvar dışındaki ilgili gerçek ortamlarda (farklı ışık koşullarına sahip oda ve masa üstü yüzeylerinde) test edilmiştir. 
- Gerçek ortam testlerinde SLAM (Eşzamanlı Konumlandırma ve Haritalama) algoritmalarının düzlem tespiti kararlılığı ölçülmüştür. Karaciğer modelinin zemin üzerinde titreme yapmadan sabit kalma oranı %98 olarak tespit edilmiştir. Proje, hedef kullanıcı grubuna sunulabilecek düzeyde kararlı bir mobil prototip aşamasındadır.

---

## 2. GELECEK HEDEFLER (THS 6 ve Sonrası)
- **THS 6 (Pilot Uygulama):** Tıp fakültesi öğrencileriyle ders ortamında pilot testlerin yapılması ve kullanıcı geribildirimlerinin toplanması.
- **THS 7 (Klinik Doğrulama):** Nakil olmuş gerçek hastalarla poliklinik şartlarında bilgilendirme aracı olarak test edilmesi.
