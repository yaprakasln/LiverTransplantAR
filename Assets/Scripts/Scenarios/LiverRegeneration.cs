using UnityEngine;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Scenarios
{
    public class LiverRegeneration : MonoBehaviour
    {
        public SimulationState Data;
        public Renderer LiverRenderer;
        
        [Header("Blend Shape Indices")]
        public int GrowthShapeIndex = 0;
        public int SwellingShapeIndex = 1;

        [Header("UI Feedback")]
        public TMPro.TMP_Text MainDescriptionText;
        public TMPro.TMP_Text CellularBox;
        public TMPro.TMP_Text VolumeBox;
        public TMPro.TMP_Text FunctionalBox;

        [Header("VFX Settings")]
        public ParticleSystem VascularizationParticles;
        private MaterialPropertyBlock _propBlock;
        private Renderer[] _childRenderers;
        private Vector3[] _originalScales;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (Data == null) 
            {
                Debug.LogError("SimulationData not assigned to LiverRegeneration!");
                return;
            }

            // AUTO-FIND RENDERER: If the user dragged a parent object, find the renderer inside
            if (LiverRenderer == null)
            {
                LiverRenderer = GetComponentInChildren<Renderer>();
            }

            // FIND ALL CHILD SEGMENTS
            _childRenderers = GetComponentsInChildren<Renderer>();
            _originalScales = new Vector3[_childRenderers.Length];
            for (int i = 0; i < _childRenderers.Length; i++) {
                _originalScales[i] = _childRenderers[i].transform.localScale;
            }

            // Set initial state
            Data.CurrentMode = AppFlowState.Scenario_Recovery; // Ensure we are in the right mode
            Data.GrowthPercentage = Data.HasTransplant ? 0.4f : 0.3f;
            UpdateGrowth(); // Trigger initial text
        }

        void Update()
        {
            if (Data == null) return;
            
            UpdateGrowth();
            UpdateVascularization();
        }

        private void UpdateGrowth()
        {
            if (LiverRenderer == null) return;

            // --- Scenario Logic ---
            if (Data.CurrentMode == AppFlowState.Intro || Data.CurrentMode == AppFlowState.MainMenu)
            {
                // In Intro/Menu, we just stay at the initial state or don't grow
                return; 
            }

            string mainDesc = "";
            string cellular = "";
            string volume = "";
            string functional = "";
            float targetGrowth = 0.3f;

            if (Data.SimulationWeek == 1) {
                targetGrowth = 0.45f;
                mainDesc = "<b>HAFTA 1:</b> Rejenerasyon Başlangıcı";
                cellular = "Hücreler (hepatositler) 24 saat içinde hızla bölünmeye başlar.";
                volume = "Hacim: %45\n(Başlangıç)";
                functional = "Protein sentezi kısıtlı, dinlenme hayati önemde.";
            } else if (Data.SimulationWeek == 2) {
                targetGrowth = 0.65f;
                mainDesc = "<b>HAFTA 2:</b> Vasküler Genişleme";
                cellular = "Anjiyogenez (yeni damar oluşumu) en yüksek hızda.";
                volume = "Hacim: %65\n(Hızlı Artış)";
                functional = "Safra üretimi normale dönmeye başladı.";
            } else if (Data.SimulationWeek == 3) {
                targetGrowth = 0.85f;
                mainDesc = "<b>HAFTA 4:</b> Fonksiyonel Olgunlaşma";
                cellular = "Hücre bölünmesi yavaşlar, doku düzenlenmesi başlar.";
                volume = "Hacim: %85\n(Neredeyse Tam)";
                functional = "Kan süzme ve detoks kapasitesi %80'in üzerine çıktı.";
            } else {
                targetGrowth = 1.0f;
                mainDesc = "<b>HAFTA 8+:</b> Tam İyileşme";
                cellular = "Hücresel yapı stabil hale geldi.";
                volume = "Hacim: %100\n(Orijinal Boyut)";
                functional = "Karaciğer tüm fonksiyonlarını tam kapasite yerine getiriyor.";
            }

            if (MainDescriptionText != null) MainDescriptionText.text = mainDesc;
            if (CellularBox != null) CellularBox.text = cellular;
            if (VolumeBox != null) VolumeBox.text = volume;
            if (FunctionalBox != null) FunctionalBox.text = functional;

            // Smooth Lerp to target growth
            if (Data.IsAdherent)
            {
                Data.GrowthPercentage = Mathf.Lerp(Data.GrowthPercentage, targetGrowth, Time.deltaTime * 0.5f);
            }
            else if (Data.CurrentMode == AppFlowState.Scenario_Medication)
            {
                // Bad Scenario
                Data.HealthPoints -= Time.deltaTime * 2f;
                Data.ImmuneAttack = true;
                if (MainDescriptionText != null) MainDescriptionText.text = "<color=red>DİKKAT: İlaç aksatıldı!</color>";
            }

            // --- Segmented Growth (New Approach) ---
            float offset = Mathf.InverseLerp(0.3f, 1.0f, Data.GrowthPercentage) * 0.015f;
            float healthDarkening = Mathf.Lerp(0.4f, 0f, Mathf.InverseLerp(0.4f, 1.0f, Data.GrowthPercentage));

            // Instead of one big swelling, we show pieces one by one
            int segmentsToShow = Mathf.CeilToInt(Mathf.InverseLerp(0.3f, 1.0f, Data.GrowthPercentage) * _childRenderers.Length);
            
            for (int i = 0; i < _childRenderers.Length; i++)
            {
                float segmentTarget = (i < segmentsToShow) ? 1.0f : 0.01f; // 0.01 instead of 0 to avoid physics/bounds issues
                
                // Smoothly scale each segment into existence
                _childRenderers[i].transform.localScale = Vector3.Lerp(_childRenderers[i].transform.localScale, _originalScales[i] * segmentTarget, Time.deltaTime * 2f);
                
                // Also apply shader properties to each segment
                _childRenderers[i].GetPropertyBlock(_propBlock);
                _propBlock.SetFloat("_GrowthOffset", offset);
                _propBlock.SetFloat("_DarkeningIntensity", healthDarkening);
                _propBlock.SetFloat("_IcterusIntensity", Data.ImmuneAttack ? 1.0f : 0f);
                _childRenderers[i].SetPropertyBlock(_propBlock);
            }

            // Scale logic for initial states
            // Instead of blendshapes, we can also use transform scale if needed, 
            // but vertex inflation is more "organic".
        }

        private void UpdateVascularization()
        {
            if (LiverRenderer == null) return;

            // shader property for vascularization visibility
            LiverRenderer.GetPropertyBlock(_propBlock);
            // More growth = more visible healthy vascularization
            float vascularIntensity = Mathf.InverseLerp(0.3f, 1.0f, Data.GrowthPercentage) * 0.8f;
            _propBlock.SetFloat("_VascularIntensity", vascularIntensity);
            LiverRenderer.SetPropertyBlock(_propBlock);

            // Trigger particles if growing fast
            if (VascularizationParticles != null)
            {
                var emission = VascularizationParticles.emission;
                emission.rateOverTime = Data.GrowthPercentage > 0.5f ? 10f : 0f;
            }
        }
        
        // Helper to advance weeks (Call from UI button)
        public void NextWeek()
        {
            if (Data.SimulationWeek < 4) Data.SimulationWeek++;
            Debug.Log($"Current Week: {Data.SimulationWeek}");
        }
    }
}
