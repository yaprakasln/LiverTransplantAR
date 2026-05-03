using UnityEngine;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Scenarios
{
    public class MedicationManager : MonoBehaviour
    {
        public SimulationState Data;
        [Header("UI Feedback")]
        public TMPro.TMP_Text MainStatusText;
        public TMPro.TMP_Text ClinicalBox;
        public TMPro.TMP_Text StatusBox;
        public TMPro.TMP_Text AdviceBox;
        public TMPro.TMP_Text VitalBox;

        private MaterialPropertyBlock _propBlock;
        private Renderer[] _renderers;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (Data == null) Debug.LogError("SimulationData not assigned to MedicationManager!");
            
            _renderers = GetComponentsInChildren<Renderer>();
            Data.CurrentMode = AppFlowState.Scenario_Medication;
            Data.IsAdherent = true; // Start healthy
        }

        void Update()
        {
            UpdateMedicationState();
            UpdateShaderEffects();
            UpdateNarrative();
        }

        private void UpdateMedicationState()
        {
            // Logic: Non-adherence leads to immune attack
            if (!Data.IsAdherent)
            {
                Data.ImmuneAttack = true;
                Data.HealthPoints -= Time.deltaTime * 2.0f; // Rapid health drop
                
                // Increase biochemical markers (AST/ALT/Bilirubin)
                Data.AST += Time.deltaTime * 5.0f;
                Data.ALT += Time.deltaTime * 4.0f;
                Data.Bilirubin += Time.deltaTime * 0.1f;
            }
            else
            {
                Data.ImmuneAttack = false;
                // FAST HEALING: Recovery is quicker when medicine is taken
                Data.HealthPoints = Mathf.MoveTowards(Data.HealthPoints, 100f, Time.deltaTime * 15.0f);
                
                // Gradually normalize biochemical markers
                Data.AST = Mathf.MoveTowards(Data.AST, 25f, Time.deltaTime * 8.0f);
                Data.ALT = Mathf.MoveTowards(Data.ALT, 30f, Time.deltaTime * 8.0f);
                Data.Bilirubin = Mathf.MoveTowards(Data.Bilirubin, 0.8f, Time.deltaTime * 0.5f);
            }

            Data.HealthPoints = Mathf.Clamp(Data.HealthPoints, 0, 100);
        }

        private void UpdateNarrative()
        {
            if (Data.IsAdherent)
            {
                if (MainStatusText != null) MainStatusText.text = "<b>DURUM: <color=green>OPTIMAL İMMÜNOTOLERANS</color></b>";
                if (ClinicalBox != null) ClinicalBox.text = $"<b>Klinik Analiz</b>\nAST: {(int)Data.AST} (Normal)\nALT: {(int)Data.ALT} (Normal)\nBilirubin: {Data.Bilirubin:F1} (Stabil)";
                if (StatusBox != null) StatusBox.text = $"<b>Hücresel Durum</b>\nHepatosit yapısı korunuyor.\nLenfosit Aktivitesi: Baskılanmış";
                if (AdviceBox != null) AdviceBox.text = "<b>Tıbbi Rapor:</b> İmmünosupresyon (Takrolimus) seviyesi ideal. Graft reddi riski minimum.";
            }
            else
            {
                if (MainStatusText != null) MainStatusText.text = "<b>DURUM: <color=red>AKUT HÜCRESEL REJEKSİYON</color></b>";
                if (ClinicalBox != null) ClinicalBox.text = $"<b>Klinik Analiz</b>\nAST: <color=red>{(int)Data.AST} (Yüksek)</color>\nALT: <color=red>{(int)Data.ALT} (Yüksek)</color>\nBilirubin: <color=red>{Data.Bilirubin:F1}</color>";
                
                string status = "<b>Hücresel Durum</b>\n<color=red>T-Hücresi İnfiltrasyonu</color>";
                string advice = "<b>KRİTİK UYARI:</b> İlaç uyumsuzluğu nedeniyle bağışıklık sistemi graft dokusuna saldırıyor.";
                
                if (Data.HealthPoints < 50f) {
                    status = "<b>Hücresel Durum</b>\n<color=red>İSKEMİK NEKROZ</color>";
                    advice = "<color=red>GERİ DÖNÜLEMEZ HASAR:</color> Organ dokusu ölüyor. Acil re-transplantasyon gerekebilir!";
                }
                
                if (StatusBox != null) StatusBox.text = status;
                if (AdviceBox != null) AdviceBox.text = advice;
                
                if (VitalBox != null) {
                    string v = "<b>DİNAMİK VİTAL</b>\n";
                    v += $"Albumin: {Mathf.Lerp(2.5f, 4.0f, Data.HealthPoints/100f):F1}\n";
                    v += $"INR: {Mathf.Lerp(2.2f, 1.1f, Data.HealthPoints/100f):F1}\n";
                    v += $"Glikoz: {(int)Mathf.Lerp(60, 100, Data.HealthPoints/100f)} mg/dL";
                    VitalBox.text = v;
                    VitalBox.color = Data.HealthPoints < 50 ? Color.red : Color.white;
                }
            }
        }

        private void UpdateShaderEffects()
        {
            if (_renderers == null || _renderers.Length == 0) return;

            // Necrosis (Rotting) - Starts at 90 health, becomes 100% at 0 health
            float necrosisAmount = Mathf.InverseLerp(90f, 0f, Data.HealthPoints);
            float healthEffect = 1.0f - (Data.HealthPoints / 100f);

            foreach (var r in _renderers)
            {
                r.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat("_IcterusIntensity", Mathf.InverseLerp(0.8f, 3.0f, Data.Bilirubin));
                _propBlock.SetFloat("_DarkeningIntensity", healthEffect * 0.5f);
                _propBlock.SetFloat("_NecrosisAmount", necrosisAmount);
                r.SetPropertyBlock(_propBlock);
            }
        }
        
        // Method to be called by UI button
        public void ToggleAdherence()
        {
            Data.IsAdherent = !Data.IsAdherent;
            Debug.Log($"Medication Adherence: {Data.IsAdherent}");
        }

        public void SetAdherenceTrue()
        {
            Data.IsAdherent = true;
            Debug.Log("Medication Adherence: TRUE");
        }

        public void SetAdherenceFalse()
        {
            Data.IsAdherent = false;
            Debug.Log("Medication Adherence: FALSE");
        }
    }
}
