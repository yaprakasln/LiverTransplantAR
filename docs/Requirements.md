# YAZILIM GEREKSİNİMLERİ (SRS) RAPORU
**Proje Adı:** LiverTransplantAR: Karaciğer Nakli Sonrası Yaşam Simülasyonu

Bu rapor, uygulamanın mimarisini, işlevsel (functional) ve işlevsel olmayan (non-functional) teknik gereksinimlerini detaylandırmaktadır.

---

## 1. SİSTEM MİMARİSİ VE KATMAN ANALİZİ
Projenin ölçeklenebilir ve sürdürülebilir olması için Clean Architecture (Temiz Mimari) prensipleri Unity ortamına uyarlanmıştır:
- **Data Layer (Veri Katmanı):** Blender ortamında hazırlanan ve `.fbx` olarak dışa aktarılan 3B karaciğer mesh yapıları, komplikasyon metadataları (JSON/Scriptable Objects) ve anatomik açıklama metinleri bu katmandadır.
- **Domain Layer (İş Mantığı Katmanı):** Komplikasyon durum yönetimi için tasarlanmış deterministik **State Machine** yapısını barındırır (NormalState, RejectionState, RecoveryState, LifestyleState).
- **Presentation Layer (Sunum Katmanı):** `AR Foundation` üzerinden cihaz kamerasından gelen görüntünün 3B render ile birleştirilmesi, World-Space UI (Holografik HUD) ve kullanıcı etkileşim arayüzlerini içerir.

---

## 2. FONKSİYONEL GEREKSİNİMLER (Functional Requirements)

### 2.1. AR Core Pipeline & Spatial Mapping
- **FR-1 (Düzlem Tespiti):** Sistem, arka kamerayı kullanarak yatay yüzeyleri (masa, zemin vb.) en geç 3 saniye içinde tespit etmeli ve ekranda görsel bir hedef halkası (Placement Indicator) göstermelidir.
- **FR-2 (Çapa Yerleştirme):** Kullanıcı ekrana dokunduğunda 3B karaciğer modeli hedef halkasının bulunduğu noktaya sabitlenmeli ve kameranın hareketinden bağımsız olarak dünya koordinat sisteminde sabit kalmalıdır (`ARAnchorManager`).
- **FR-3 (Raycasting):** Kullanıcı dokunuşları `ARRaycastManager` ile 3D uzaya projekte edilerek karaciğer üzerindeki anatomik bölgelerle (Hotspot) çakışma (Collision) kontrolü yapmalıdır.

### 2.2. Dinamik Komplikasyon Simülasyonu
- **FR-4 (Shader Manipülasyonu):** Komplikasyonlar seçildiğinde (örn. Organ Reddi), karaciğer dokusunun rengi shader parametreleri üzerinden yumuşak bir geçişle (Lerp) sağlıksız sarı/kahverengi tonlarına bürünmelidir.
- **FR-5 (Animasyon Senaryoları):** Karaciğer damarlarındaki sıvı akışı (Flow Animation), tromboz veya tıkanıklık senaryolarında akış hızını dinamik olarak yavaşlatmalı veya durdurmalıdır.

---

## 3. İŞLEVSEL OLMAYAN GEREKSİNİMLER (Non-Functional Requirements)
- **NFR-1 (Performans ve Poligon Sınırı):** Mobil GPU'ların aşırı ısınmasını önlemek amacıyla ana karaciğer mesh modelinin toplam poligon sayısı **50.000 vertex** değerini geçmemeli ve LOD (Level of Detail) grupları kullanılmalıdır.
- **NFR-2 (Gecikme Süresi):** Kullanıcının baş hareketleri ile sanal objenin hareketi arasındaki gecikme (Motion-to-Photon) **20ms**'nin altında tutulmalı, kayma hissi engellenmelidir.
- **NFR-3 (Bellek Tüketimi):** Uygulama çalışma zamanında mobil cihazın RAM belleğinde **300 MB**'tan fazla yer kaplamamalıdır. Bu amaçla grafikler dinamik olarak Addressables ile yüklenip boşaltılmalıdır.
- **NFR-4 (Pil Optimizasyonu):** AR işlemleri yüksek işlemci gücü gerektirdiğinden, batarya tüketimini optimize etmek adına hedef ekran yenileme hızı (Target Frame Rate) **60 FPS** değerine kilitlenmelidir.
