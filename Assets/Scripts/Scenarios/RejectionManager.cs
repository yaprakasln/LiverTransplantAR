using UnityEngine;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Scenarios
{
    /// <summary>
    /// Senaryo 3: Uyumsuzluk ve Red (Incompatibility / Rejection)
    /// Vücudun nakil edilen karaciğeri reddetme sürecini 3 evrede simüle eder.
    /// Evre 1: Erken Uyarı (T-Hücre Aktivasyonu)
    /// Evre 2: Akut Hücresel Red (Lenfosit İnfiltrasyonu)
    /// Evre 3: Kronik Red (Fibrozis ve Organ Kaybı)
    /// </summary>
    public class RejectionManager : MonoBehaviour
    {
        public SimulationState Data;

        [Header("UI Feedback")]
        public TMPro.TMP_Text MainStatusText;
        public TMPro.TMP_Text SymptomsBox;
        public TMPro.TMP_Text ClinicalBox;
        public TMPro.TMP_Text PrognosisBox;

        [Header("VFX")]
        public ParticleSystem OcclusionParticles;

        private MaterialPropertyBlock _propBlock;
        private Renderer[] _renderers;

        // Smooth interpolation targets
        private float _targetIcterus = 0f;
        private float _targetNecrosis = 0f;
        private float _targetDarkening = 0f;
        private float _targetVascularOcclusion = 0f;
        private float _targetFibrosis = 0f;

        // Current smooth values
        private float _currentIcterus = 0f;
        private float _currentNecrosis = 0f;
        private float _currentDarkening = 0f;
        private float _currentVascularOcclusion = 0f;
        private float _currentFibrosis = 0f;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (Data == null)
            {
                Debug.LogError("SimulationData not assigned to RejectionManager!");
                return;
            }

            _renderers = GetComponentsInChildren<Renderer>();
            Data.CurrentMode = AppFlowState.Scenario_Rejection;
            Data.RejectionStage = 0;
            Data.IsRejecting = false;
            UpdateStageTargets();
            UpdateNarrative();
        }

        void Update()
        {
            if (Data == null) return;

            UpdateRejectionProgress();
            UpdateShaderEffects();
            UpdateParticles();
        }

        /// <summary>
        /// Aktif red sürecinde biyokimyasal değerleri sürekli kötüleştirir.
        /// </summary>
        private void UpdateRejectionProgress()
        {
            if (!Data.IsRejecting) return;

            // Gradually increase rejection progress within current stage
            float progressSpeed = 0.05f; // Slower for dramatic effect
            Data.RejectionProgress = Mathf.MoveTowards(Data.RejectionProgress, 1.0f, Time.deltaTime * progressSpeed);

            // Stage-dependent biochemical deterioration
            switch (Data.RejectionStage)
            {
                case 1: // Erken Uyarı
                    Data.AST = Mathf.MoveTowards(Data.AST, 120f, Time.deltaTime * 4.0f);
                    Data.ALT = Mathf.MoveTowards(Data.ALT, 100f, Time.deltaTime * 3.5f);
                    Data.Bilirubin = Mathf.MoveTowards(Data.Bilirubin, 3.5f, Time.deltaTime * 0.15f);
                    Data.HealthPoints = Mathf.MoveTowards(Data.HealthPoints, 70f, Time.deltaTime * 1.5f);
                    Data.VascularOcclusion = Mathf.MoveTowards(Data.VascularOcclusion, 0.25f, Time.deltaTime * 0.08f);
                    break;

                case 2: // Akut Hücresel Red
                    Data.AST = Mathf.MoveTowards(Data.AST, 350f, Time.deltaTime * 8.0f);
                    Data.ALT = Mathf.MoveTowards(Data.ALT, 300f, Time.deltaTime * 7.0f);
                    Data.Bilirubin = Mathf.MoveTowards(Data.Bilirubin, 8.0f, Time.deltaTime * 0.3f);
                    Data.HealthPoints = Mathf.MoveTowards(Data.HealthPoints, 35f, Time.deltaTime * 3.0f);
                    Data.VascularOcclusion = Mathf.MoveTowards(Data.VascularOcclusion, 0.65f, Time.deltaTime * 0.12f);
                    Data.ImmuneAttack = true;
                    break;

                case 3: // Kronik Red
                    Data.AST = Mathf.MoveTowards(Data.AST, 500f, Time.deltaTime * 10.0f);
                    Data.ALT = Mathf.MoveTowards(Data.ALT, 450f, Time.deltaTime * 9.0f);
                    Data.Bilirubin = Mathf.MoveTowards(Data.Bilirubin, 15.0f, Time.deltaTime * 0.5f);
                    Data.HealthPoints = Mathf.MoveTowards(Data.HealthPoints, 5f, Time.deltaTime * 4.0f);
                    Data.VascularOcclusion = Mathf.MoveTowards(Data.VascularOcclusion, 1.0f, Time.deltaTime * 0.15f);
                    Data.FibrosisFactor = Mathf.MoveTowards(Data.FibrosisFactor, 1.0f, Time.deltaTime * 0.1f);
                    Data.ImmuneAttack = true;
                    break;
            }

            Data.HealthPoints = Mathf.Clamp(Data.HealthPoints, 0, 100);
        }

        /// <summary>
        /// Evre değişiminde shader hedef değerlerini günceller.
        /// </summary>
        private void UpdateStageTargets()
        {
            switch (Data.RejectionStage)
            {
                case 0: // Normal — Sağlıklı
                    _targetIcterus = 0f;
                    _targetNecrosis = 0f;
                    _targetDarkening = 0f;
                    _targetVascularOcclusion = 0f;
                    _targetFibrosis = 0f;
                    break;

                case 1: // Evre 1 — Erken Uyarı
                    _targetIcterus = 0.35f;
                    _targetNecrosis = 0f;
                    _targetDarkening = 0.1f;
                    _targetVascularOcclusion = 0.25f;
                    _targetFibrosis = 0f;
                    break;

                case 2: // Evre 2 — Akut Red
                    _targetIcterus = 0.7f;
                    _targetNecrosis = 0.3f;
                    _targetDarkening = 0.35f;
                    _targetVascularOcclusion = 0.65f;
                    _targetFibrosis = 0.15f;
                    break;

                case 3: // Evre 3 — Kronik Red
                    _targetIcterus = 1.0f;
                    _targetNecrosis = 0.85f;
                    _targetDarkening = 0.7f;
                    _targetVascularOcclusion = 1.0f;
                    _targetFibrosis = 1.0f;
                    break;
            }
        }

        /// <summary>
        /// Shader property'lerini smooth interpolation ile günceller.
        /// </summary>
        private void UpdateShaderEffects()
        {
            if (_renderers == null || _renderers.Length == 0) return;

            float lerpSpeed = 0.8f;
            _currentIcterus = Mathf.Lerp(_currentIcterus, _targetIcterus, Time.deltaTime * lerpSpeed);
            _currentNecrosis = Mathf.Lerp(_currentNecrosis, _targetNecrosis, Time.deltaTime * lerpSpeed);
            _currentDarkening = Mathf.Lerp(_currentDarkening, _targetDarkening, Time.deltaTime * lerpSpeed);
            _currentVascularOcclusion = Mathf.Lerp(_currentVascularOcclusion, _targetVascularOcclusion, Time.deltaTime * lerpSpeed);
            _currentFibrosis = Mathf.Lerp(_currentFibrosis, _targetFibrosis, Time.deltaTime * lerpSpeed);

            foreach (var r in _renderers)
            {
                r.GetPropertyBlock(_propBlock);

                // Icterus (Sararma) — sarı-yeşil renk geçişi
                _propBlock.SetFloat("_IcterusIntensity", _currentIcterus);
                // Necrosis (Çürüme) — mor-siyah doku ölümü
                _propBlock.SetFloat("_NecrosisAmount", _currentNecrosis);
                // Darkening (Kararma) — genel sağlık düşüşü
                _propBlock.SetFloat("_DarkeningIntensity", _currentDarkening);
                // Vascular Occlusion (Damar Tıkanıklığı)
                _propBlock.SetFloat("_VascularOcclusion", _currentVascularOcclusion);
                // Fibrosis (Skar Dokusu)
                _propBlock.SetFloat("_FibrosisAmount", _currentFibrosis);

                r.SetPropertyBlock(_propBlock);
            }
        }

        /// <summary>
        /// Damar tıkanıklığı partikül efektini kontrol eder.
        /// </summary>
        private void UpdateParticles()
        {
            if (OcclusionParticles == null) return;

            var emission = OcclusionParticles.emission;
            // Stage 2+ → particles visible, intensity scales with occlusion
            emission.rateOverTime = Data.RejectionStage >= 2 ? Data.VascularOcclusion * 20f : 0f;
        }

        /// <summary>
        /// UI metinlerini mevcut evreye göre günceller.
        /// Her evre detaylı Türkçe tıbbi bilgi içerir.
        /// </summary>
        private void UpdateNarrative()
        {
            switch (Data.RejectionStage)
            {
                case 0: // Normal
                    SetTexts(
                        "<b>DURUM: <color=green>STABIL — RED BELİRTİSİ YOK</color></b>",
                        "<b>Semptomlar:</b>\nHerhangi bir semptom bulunmamaktadır.\nOrgan fonksiyonları normal sınırlarda.",
                        $"<b>Klinik Değerler</b>\nAST: {(int)Data.AST} U/L (Normal)\nALT: {(int)Data.ALT} U/L (Normal)\nBilirubin: {Data.Bilirubin:F1} mg/dL (Normal)",
                        "<b>Prognoz:</b> Organ toleransı sağlanmış durumda. İmmünosüpresif tedavi devam etmeli."
                    );
                    break;

                case 1: // Evre 1 — Erken Uyarı
                    SetTexts(
                        "<b>DURUM: <color=yellow>EVRE 1 — ERKEN UYARI</color></b>",
                        "<b>Semptomlar:</b>\n• Hafif ateş (37.5-38°C)\n• Karın bölgesinde belirsiz ağrı\n• Hafif halsizlik ve yorgunluk\n• İdrarda hafif koyulaşma",
                        $"<b>Klinik Değerler</b>\nAST: <color=yellow>{(int)Data.AST} U/L (Yükseliyor)</color>\nALT: <color=yellow>{(int)Data.ALT} U/L (Yükseliyor)</color>\nBilirubin: <color=yellow>{Data.Bilirubin:F1} mg/dL (Yüksek)</color>",
                        "<b>Prognoz:</b> T-Lenfositler aktive oldu. Bağışıklık sistemi yabancı dokuyu tanımaya başladı. Erken müdahale ile geri döndürülebilir."
                    );
                    break;

                case 2: // Evre 2 — Akut Hücresel Red
                    SetTexts(
                        "<b>DURUM: <color=#FF6600>EVRE 2 — AKUT HÜCRESEL RED</color></b>",
                        "<b>Semptomlar:</b>\n• Yüksek ateş (38.5-39.5°C)\n• Şiddetli karın ağrısı (sağ üst kadran)\n• Belirgin sarılık (icterus) — göz akı ve ciltte\n• Koyu renkli idrar (çay rengi)\n• Açık renkli / kil rengi dışkı\n• İştahsızlık ve bulantı",
                        $"<b>Klinik Değerler</b>\nAST: <color=red>{(int)Data.AST} U/L (KRİTİK)</color>\nALT: <color=red>{(int)Data.ALT} U/L (KRİTİK)</color>\nBilirubin: <color=red>{Data.Bilirubin:F1} mg/dL (ÇOK YÜKSEK)</color>\nDamar Tıkanıklığı: <color=red>%{(int)(Data.VascularOcclusion * 100)}</color>",
                        "<b>Prognoz:</b> Lenfosit infiltrasyonu ilerledi. Safra kanallarında hasar ve damar endotelinde yıkım başladı. Yüksek doz pulse steroid tedavisi gerekli!"
                    );
                    break;

                case 3: // Evre 3 — Kronik Red
                    SetTexts(
                        "<b>DURUM: <color=red>EVRE 3 — KRONİK RED (GERİ DÖNÜŞÜMSÜZ)</color></b>",
                        "<b>Semptomlar:</b>\n• Kalıcı ve derin sarılık\n• Kaşıntı (pruritus)\n• Asit birikimi (karında şişlik)\n• Hepatik ensefalopati riski\n• Şiddetli halsizlik\n• Kilo kaybı ve kas erimesi\n• Pıhtılaşma bozuklukları",
                        $"<b>Klinik Değerler</b>\nAST: <color=red>{(int)Data.AST} U/L (ORGAN YETMEZLİĞİ)</color>\nALT: <color=red>{(int)Data.ALT} U/L (ORGAN YETMEZLİĞİ)</color>\nBilirubin: <color=red>{Data.Bilirubin:F1} mg/dL (ALARM)</color>\nDamar Tıkanıklığı: <color=red>%{(int)(Data.VascularOcclusion * 100)}</color>\nFibrozis: <color=red>%{(int)(Data.FibrosisFactor * 100)}</color>",
                        "<b>Prognoz:</b> Duktopenik rejeksiyon ve obliteratif arteriopati. Safra kanalları geri dönüşümsüz olarak yok edildi. Fibrotik skar dokusu oluşuyor. Re-transplantasyon değerlendirilmeli!"
                    );
                    break;
            }
        }

        private void SetTexts(string status, string symptoms, string clinical, string prognosis)
        {
            if (MainStatusText != null) MainStatusText.text = status;
            if (SymptomsBox != null) SymptomsBox.text = symptoms;
            if (ClinicalBox != null) ClinicalBox.text = clinical;
            if (PrognosisBox != null) PrognosisBox.text = prognosis;
        }

        // ─────────── PUBLIC METHODS (UI Buttons) ───────────

        /// <summary>
        /// Rejeksiyon evresini bir sonraki aşamaya ilerletir. UI butonundan çağrılır.
        /// </summary>
        public void AdvanceStage()
        {
            if (Data.RejectionStage < 3)
            {
                Data.RejectionStage++;
                Data.IsRejecting = true;
                Data.RejectionProgress = 0f;
                UpdateStageTargets();
                UpdateNarrative();
                Debug.Log($"<color=red>REJECTION:</color> Advanced to Stage {Data.RejectionStage}");
            }
            else
            {
                Debug.Log("<color=red>REJECTION:</color> Already at final stage (Chronic).");
            }
        }

        /// <summary>
        /// Simülasyonu sıfırlar (normal duruma döner). UI butonundan çağrılır.
        /// </summary>
        public void ResetRejection()
        {
            Data.RejectionStage = 0;
            Data.RejectionProgress = 0f;
            Data.VascularOcclusion = 0f;
            Data.FibrosisFactor = 0f;
            Data.IsRejecting = false;
            Data.ImmuneAttack = false;
            Data.HealthPoints = 100f;
            Data.AST = 25f;
            Data.ALT = 30f;
            Data.Bilirubin = 0.8f;
            UpdateStageTargets();
            UpdateNarrative();
            Debug.Log("<color=green>REJECTION:</color> Reset to healthy state.");
        }
    }
}
