# KULLANICI SENARYOLARI (USER SCENARIO) RAPORU
**Proje Adı:** LiverTransplantAR: Karaciğer Nakli Sonrası Yaşam Simülasyonu

Bu belgede, bir tıp öğrencisinin veya hasta yakınının uygulama içerisindeki adım adım deneyim süreci (Kullanıcı Senaryoları) ve klinik önemleri açıklanmaktadır.

---

## Senaryo 1: 🌱 Onarım Süreci (Recovery Phase)
- **Klinik Önem:** Karaciğerin operasyon sonrasında kendini yenileme (rejenerasyon) hızını ve damarlanma sürecini anlamak.
- **Kullanıcı Akışı:**
  1. Kullanıcı uygulamayı başlatır ve kamerayı düz bir masa yüzeyine doğrultur.
  2. Yatay düzlem algılandığında ekranda yeşil bir hedef halkası (Placement Indicator) belirir.
  3. Kullanıcı ekrana dokunarak 3B karaciğer modelini masanın üzerine yerleştirir.
  4. Ekranda yer alan "Başlat" butonuna tıklandığında, karaciğer parçasının gün gün nasıl büyüyerek normal hacmine ulaştığı ve üzerinde yeni damarların (vascularization) oluştuğu 3B animasyonla gösterilir.
- **Beklenen Çıktı:** Kullanıcı, nakledilen küçük bir karaciğer parçasının haftalar içinde nasıl tam fonksiyonel bir organa dönüştüğünü somut olarak kavrar.

---

## Senaryo 2: 💊 İlaç Uyumu (Medication Adherence)
- **Klinik Önem:** Bağışıklık baskılayıcı (immunosuppressive) ilaçların düzenli kullanımının organ sağlığı üzerindeki etkisini kavramak.
- **Kullanıcı Akışı:**
  1. Kullanıcı ekran üzerindeki "İlaç Uyumu Simülasyonu" butonuna basar.
  2. Sistem kullanıcıya iki seçenek sunar: **Düzenli İlaç Kullanımı** ve **İlaç Aksatma**.
  3. **Düzenli Kullanım Seçilirse:** Karaciğer modeli pembe ve sağlıklı rengini korur, damar akış animasyonları kararlı şekilde devam eder.
  4. **İlaç Aksatma Seçilirse:** Karaciğer modelinin dokusunda morfolojik bozulmalar (hücresel deformasyon) başlar ve damarlardaki kan akış hızı yavaşlar.
- **Beklenen Çıktı:** Kullanıcı, ilaçların saatinde alınmamasının organ düzeyinde oluşturduğu mikroskobik hasarları makroskopik olarak gözlemler.

---

## Senaryo 3: ⚠️ Uyumsuzluk ve Red (Incompatibility/Rejection)
- **Klinik Önem:** Akut hücresel reddin (rejection) fizyolojik belirtilerini ve damar tıkanıklığı (tromboz) komplikasyonlarını görselleştirmek.
- **Kullanıcı Akışı:**
  1. Kullanıcı ana menüden "Red & Komplikasyon" modunu aktif hale getirir.
  2. Karaciğer modelinin dokusu, URP shader parametrelerinin değişmesiyle (Lerp) aşamalı olarak sararır (sarılık - icterus klinik tablosu).
  3. Portal ven ve hepatik arter damarlarının animasyonunda tıkanıklık (thrombosis) bölgeleri koyu kırmızı/mor renklerle belirir ve sıvı akışı tamamen durur.
  4. Holografik HUD panelinde acil müdahale edilmesi gereken semptomlar (yüksek ateş, karın ağrısı) listelenir.
- **Beklenen Çıktı:** Tıp öğrencisi, organ reddi sırasındaki doku deformasyonlarını ve vasküler komplikasyonların seyrini öğrenir.

---

## Senaryo 4: 🍏 Yaşam Tarzı ve Sağlık (Lifestyle)
- **Klinik Önem:** Nakil sonrasında hastanın yaşam kalitesini artıracak doğru beslenme ve fiziksel aktivite kurallarını öğrenmesi.
- **Kullanıcı Akışı:**
  1. Kullanıcı "Sağlıklı Yaşam Rehberi" modunu başlatır.
  2. Ekranda karaciğerin yanında interaktif bir beslenme tepsisi belirir. Sağlıklı gıdalar seçildiğinde karaciğer modeli üzerinde pozitif ışıma efekti oluşur.
  3. Sistem, haftalık yapılması gereken hafif tempolu yürüyüş egzersizlerinin karaciğer yağlanmasını nasıl önlediğini gösteren bilgi kartları sunar.
- **Beklenen Çıktı:** Kullanıcı, nakil sonrasındaki yeni hayatında uyması gereken diyet ve fiziksel aktivite şablonunu öğrenir.
