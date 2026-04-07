# Gereksinim Analizi (SRS)

## 🏗️ 1. Sistem Mimarisi ve Katman Analizi
Projenin ölçeklenebilir olması için **Clean Architecture** prensipleri Unity ortamına uyarlanmıştır:

* **Data Layer:** Blender'dan gelen `.fbx` modelleri, komplikasyon metadata'ları (JSON/Scriptable Objects) ve anatomik metin içerikleri.
* **Domain Layer (Business Logic):** Komplikasyon durum yönetimi için **State Machine** yapısı.
    * *Örn:* `NormalState`, `ThrombosisState`, `BiliaryStrictureState`.
* **Presentation Layer:** **AR Foundation** üzerinden kamera feed'i ile 3D render'ın birleştirilmesi ve UI (Canvas) yönetimi.

---

## ⚙️ 2. Fonksiyonel Gereksinimler

### 2.1. AR Core Pipeline & Spatial Mapping
* **FR-1 (Plane Detection & Anchoring):** Sistem, yatay düzlemleri (Horizontal Planes) tespit etmeli ve `ARAnchorManager` kullanarak karaciğer modelini dünya koordinat sisteminde ($x, y, z$) sabit tutmalıdır.
* **FR-2 (Raycasting):** Kullanıcı dokunuşları `ARRaycastManager` ile 3D uzaya projekte edilmeli ve "Hotspot" (damar girişleri vb.) noktalarıyla çakışma (**Collision**) kontrolü yapılmalıdır.

### 2.2. Dinamik Komplikasyon Simülasyonu
* **FR-3 (Shader Manipulation):** Komplikasyonlar, mevcut mesh üzerindeki **Shader** parametreleri değiştirilerek gösterilmelidir (Renk değişimi veya **Normal Map** manipülasyonu ile morfolojik bozulma).
* **FR-4 (State Synchronization):** Bir komplikasyon seçildiğinde, ilgili damarın `FlowAnimation` hızı dinamik olarak güncellenmelidir.

---

## ⚡ 3. Fonksiyonel Olmayan (Teknik) Gereksinimler

* **NFR-1 (Mesh Optimization):** Mobil GPU (Samsung S24 FE) performansı için ana modelin **Poly Count** değeri **50k vertex**'i geçmemeli; **LOD (Level of Detail)** grupları tanımlanmalıdır.
* **NFR-2 (Latency):** Motion-to-Photon latansı **20ms**'nin altında tutulmalıdır (Drifting engelleme).
* **NFR-3 (Memory Management):** Bellek yönetimi için **Addressables** sistemi kullanılmalı, RAM üzerinde gereksiz asset yükü tutulmamalıdır.

---

## 🔄 4. Teknik Veri Akışı (Sequence)

Bir komplikasyonun tetiklenme süreci şu adımları izler:

1.  **Input:** UI'dan `OnComplicationSelected(Type)` event'i fırlatılır.
2.  **Controller:** `LiverController`, seçilen komplikasyonun `ScriptableObject` verisini çeker.
3.  **Mesh Modifier:** İlgili alt-mesh (**Sub-mesh**) tespit edilir (Örn: `HepaticArtery_Mesh`).
4.  **Visual Feedback:** Shader üzerinden `Lerp` fonksiyonu ile patolojik doku rengine geçiş yapılır.
5.  **Information Overlay:** World-Space Canvas üzerinde teknik bilgi kartı instantiate edilir.

---

## ⚠️ 5. Risk ve Çözüm Analizi (Engineering Trade-offs)

| Risk | Teknik Karşılığı | Mühendislik Çözümü |
| :--- | :--- | :--- |
| **Occlusion Sorunu** | Gerçek elin modelin önüne geçmesi. | AR Foundation **Occlusion Manager** (Depth API) entegrasyonu. |
| **Scale Accuracy** | Modelin gerçek boyutlarda olmaması. | 1 birim = 1 metre kuralına sadık kalarak anatomik gerçek boyut kısıtlaması. |
| **Tracking Loss** | Kameranın yüzeyi kaybetmesi. | **SLAM** algoritmaları ve yüksek kontrastlı yüzey uyarı sistemi. |

---

## ✅ Proje Teslimi İçin Kritik Metrikler
- [ ] **Birim Testleri (Unit Tests):** Komplikasyon durum geçişlerinin logic kontrolü.
- [ ] **Integration Tests:** Farklı ışık koşullarında AR objesinin stabilite yüzdesi.
