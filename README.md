# 🫁 LiverTransplantAR: Karaciğer Nakli Sonrası Yaşam Simülasyonu

**LiverTransplantAR**, karaciğer nakli operasyonu sonrasında hastaları bekleyen süreçleri, karşılaşılabilecek komplikasyonları ve sağlıklı bir yaşam için gereken adımları **Arttırılmış Gerçeklik (AR)** teknolojisi ile görselleştiren interaktif bir eğitim platformudur.

---

## 📄 Öğrenci Sınav Bilgileri

* **Öğrenci No / Adı Soyadı:** 
  * `[235541301]` - **Yaprak ASLAN** (Scrum Master)
  * `[215541097]` - **Asiye KAYMAK** (Geliştirici)
  * `[235541139]` - **Ebrar Sena MANGAN** (Geliştirici)
  * `[205541001]` - **Yusuf DOĞAN** (Geliştirici)

### 🎯 Proje Konusu (Tek Cümleyle)
Bu proje, karaciğer nakli operasyonu sonrasındaki kritik süreçleri (rejenerasyon, ilaç uyumu, organ reddi ve yaşam tarzı) arttırılmış gerçeklik (AR) ortamında mobil cihazlar aracılığıyla görselleştirerek tıp eğitimi ve hasta bilgilendirmesi süreçlerini iyileştirmeyi amaçlamaktadır.

---

## 👥 Ekip Üyeleri, Sorumluluklar ve Katkı Payları

| Ekip Üyesi | Rolü & Projedeki Sorumluluğu (Tek Cümleyle) | Önem (Katkı) Yüzdesi |
| :--- | :--- | :---: |
| **Yaprak ASLAN** | Scrum Master, AR Entegrasyonu ve Senaryo 1 (Onarım Süreci) | %25 |
| **Yusuf DOĞAN** | Senaryo 2 (İlaç Uyumu) | %25 |
| **Ebrar Sena MANGAN** | Senaryo 3 (Uyumsuzluk ve Red) | %25 |
| **Asiye KAYMAK** | Senaryo 4 (Yaşam Tarzı ve Sağlık) | %25 |

---

## 🚀 Proje Senaryoları

Uygulama, kullanıcının seçimlerine göre şekillenen 4 ana senaryo üzerinden ilerler:

### 1. 🌱 Onarım Süreci (Recovery Phase)
Karaciğerin mucizevi kendini yenileme (rejenerasyon) yeteneği görselleştirilir.
- **Odak:** Nakil edilen karaciğer parçasının, operasyon sonrası ilk haftalarda nasıl hızla büyüyüp fonksiyonlarını tamamladığı 3B model üzerinde gösterilir.
- **Görsel:** Karaciğerin fiziksel olarak büyümesi ve damarlanma (vascularization) süreci.

### 2. 💊 İlaç Uyumu (Medication Adherence)
İlaç kullanımının hayati önemi simüle edilir.
- **İyi Senaryo:** İlaçların düzenli alınması durumunda karaciğerin sağlıklı büyümesi ve doku bütünlüğünün korunması.
- **Kötü Senaryo:** İlaçların aksatılması durumunda karaciğerin kendini onaramaması ve doku bozulmalarının başlaması.

### 3. ⚠️ Uyumsuzluk ve Red (Incompatibility/Rejection)
Vücudun yeni organı reddetme süreci (rejeksiyon) ve bu sırada meydana gelen fizyolojik değişiklikler.
- **İçerik:** Organ reddi sırasında karaciğerde oluşan sararma (icterus), damar tıkanıklıkları ve hastanın yaşayabileceği semptomlar.
- **Görsel:** Sağlıksız doku rengi değişimleri ve vasküler komplikasyon modellemeleri.

### 4. 🍏 Yaşam Tarzı ve Sağlık (Lifestyle)
Nakil sonrası hayat kalitesini artırmak için yapılması gerekenler.
- **Odak:** Kişiye özel beslenme planları ve egzersiz rutinleri.
- **İçerik:** Operasyon sonrası fiziksel aktivitenin karaciğer sağlığı üzerindeki olumlu etkileri ve beslenme önerileri.

---

## 🛠️ Teknik Özellikler
- **AR Teknolojisi:** AR Foundation (ARCore/ARKit) ile gerçek dünya entegrasyonu.
- **Görselleştirme:** URP (Universal Render Pipeline) tabanlı özel tıbbi shader'lar.
- **Etkileşim:** Seçim tabanlı senaryo yönetimi (FlowManager).
- **Platform:** Android & iOS (Mobil AR). Geliştirilen uygulama hem Android hem de iOS fiziksel mobil cihazlarında test edilmiş ve doğrudan çalıştığı doğrulanmıştır.

---

## ⚙️ Projede Değişiklik Yapma ve Geliştirici Kılavuzu (Başlangıç Rehberi)

Bu projeyi ilk kez açacak ve üzerinde değişiklik yapacak bir geliştiricinin izlemesi gereken adımlar:

### 1. Ön Gereksinimler
* **Unity Sürümü:** Proje **Unity 2022.3 LTS** sürümü kullanılarak geliştirilmiştir. Unity Hub üzerinden bu sürümü yükleyin.
* **Mobil Platform Desteği:** Build alabilmek için Unity kurulumunda **Android Build Support** veya **iOS Build Support** modüllerinin işaretli olduğundan emin olun.

### 2. Projenin Açılması
1. Bu GitHub reposunu bilgisayarınıza klonlayın: `git clone https://github.com/yaprakasln/LiverTransplantAR.git`
2. Unity Hub'ı açın, **Add** butonuna tıklayarak projenin klasörünü seçin ve projeyi açın.
3. Proje açıldığında gerekli paketler (AR Foundation, Universal RP) otomatik olarak yüklenecektir.

### 3. Sahne Yapısı ve Değişiklik Yapma
* **Giriş Sahnesi:** Ana menü ve başlangıç sahneleri `Assets/Scenes/MainLauncher.unity` dosyası altındadır. Uygulama buradan başlatılmalıdır.
* **Senaryolar:** Senaryoların sahneleri `Scenario1.unity`, `Scenario2.unity`, `Scenario3.unity` ve `Scenario4_Final.unity` adıyla `Assets/Scenes/` klasöründe yer alır.
* **Kodlarda Değişiklik Yapma:** AR yerleştirme mantığı `Assets/Scripts/AR/ARPlacementManager.cs` ve `FixedARPlacement.cs` dosyalarındadır. Karaciğer modellerini düzenlemek veya ölçeklendirmek için `Assets/Editor/ARSituationPerfecter.cs` editör scriptini inceleyebilir ve Unity üst menüsündeki "Liver AR" araçlarını kullanabilirsiniz.

### 4. Mobil Cihaza Derleme (Build) Alma
* **Android (APK) için:**
  1. Unity menüsünden `File > Build Settings` penceresini açın.
  2. Platform listesinden **Android**'i seçin ve **Switch Platform** butonuna tıklayın.
  3. Cihazınızı USB ile bilgisayara bağlayıp (Geliştirici Seçenekleri açık şekilde) **Build And Run** butonuna basarak doğrudan cihaza yükleyebilir veya **Build** diyerek `.apk` çıktısı alabilirsiniz.
* **iOS için:**
  1. Platform listesinden **iOS**'u seçip **Switch Platform** yapın.
  2. **Build** butonuna basarak bir Xcode projesi oluşturun.
  3. Oluşan projeyi Mac bilgisayarınızda Xcode ile açarak cihazınıza yükleyin.
