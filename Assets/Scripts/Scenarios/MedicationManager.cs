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
                if (MainStatusText != null) MainStatusText.text = "<b>DURUM: <color=green>OPTIMAL TOLERANS</color></b>";
                if (ClinicalBox != null) ClinicalBox.text = $"<b>Klinik Analiz</b>\nAST: {(int)Data.AST} (Kararlı)\nALT: {(int)Data.ALT} (Kararlı)\nBilirubin: {Data.Bilirubin:F1} (Normal)";
                if (StatusBox != null) StatusBox.text = $"<b>Doku Bütünlüğü</b>\nHücresel yapı korunuyor.\nİmmün Yanıt: Baskılanmış (İdeal)";
                if (AdviceBox != null) AdviceBox.text = "<b>Tıbbi Not:</b> İlaçlar (Takrolimus/Siklosporin) kandaki hedef seviyesinde. Bağışıklık sistemi organı kendi dokusu gibi kabul ediyor.";
            }
            else
            {
                if (MainStatusText != null) MainStatusText.text = "<b>DURUM: <color=red>AKUT HÜCRESEL RED</color></b>";
                if (ClinicalBox != null) ClinicalBox.text = $"<b>Klinik Analiz</b>\nAST: <color=red>{(int)Data.AST} (Yükseliyor)</color>\nALT: <color=red>{(int)Data.ALT} (Yükseliyor)</color>\nBilirubin: <color=red>{Data.Bilirubin:F1}</color>";
                
                string status = "<b>Doku Bütünlüğü</b>\n<color=red>Lenfosit İnfiltrasyonu</color>";
                string advice = "<b>DİKKAT:</b> İlaç dozu atlandığı için T-Hücreleri organa saldırmaya başladı.";
                
                if (Data.HealthPoints < 50f) {
                    status = "<b>Doku Bütünlüğü</b>\n<color=red>DOKU NEKROZU (ÇÜRÜME)</color>";
                    advice = "<color=red>KRİTİK HATA:</color> Hücre ölümleri başladı! Karaciğer dokusu işlevini yitiriyor ve çürüyor!";
                }
                
                if (StatusBox != null) StatusBox.text = status;
                if (AdviceBox != null) AdviceBox.text = advice;
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
