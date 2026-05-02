using UnityEngine;
using UnityEngine.UI;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Scenarios
{
    public class LifestyleManager : MonoBehaviour
    {
        public SimulationState Data;
        public Renderer LiverRenderer;
        
        [Header("UI & AR Visuals")]
        public LiverTransplantAR.UI.FlowManager FlowUI;
        public ARVisualsController Visuals;
        
        [Header("Biometric Gauges")]
        public Image ASTGauge;
        public Image ALTGauge;
        public Image BiliGauge;

        private MaterialPropertyBlock _propBlock;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (Data == null) Debug.LogError("SimulationData not assigned to LifestyleManager!");
            if (FlowUI == null) FlowUI = GameObject.FindObjectOfType<LiverTransplantAR.UI.FlowManager>();
        }

        void Update()
        {
            ProcessLifestyleEffects();
            SimulateBiometrics();
        }

        private void ProcessLifestyleEffects()
        {
            float currentRate = Data.BaseRegenerationRate * Data.NutritionMultiplier * Data.ExerciseMultiplier;
            
            if (Data.IsAdherent && Data.GrowthPercentage < 1.0f)
            {
                Data.GrowthPercentage += currentRate * Time.deltaTime;
            }
            
            Data.GrowthPercentage = Mathf.Clamp(Data.GrowthPercentage, 0.4f, 1.0f);
        }

        private void SimulateBiometrics()
        {
            // Perfect logic: Lifestyle impacts biochemical markers over time
            float targetAST = Data.IsFattyDiet ? 85f : 22f;
            float targetALT = Data.IsFattyDiet ? 90f : 28f;
            float targetBili = Data.IsFattyDiet ? 1.8f : 0.7f;

            // Exercise improves bilirubin clearance
            if (Data.ExerciseMultiplier > 1.0f) targetBili -= 0.2f;

            Data.AST = Mathf.Lerp(Data.AST, targetAST, Time.deltaTime * 0.5f);
            Data.ALT = Mathf.Lerp(Data.ALT, targetALT, Time.deltaTime * 0.5f);
            Data.Bilirubin = Mathf.Lerp(Data.Bilirubin, targetBili, Time.deltaTime * 0.5f);

            // Update UI Gauges (Normalized 0-1)
            if (ASTGauge != null) ASTGauge.fillAmount = Mathf.Clamp01(Data.AST / 100f);
            if (ALTGauge != null) ALTGauge.fillAmount = Mathf.Clamp01(Data.ALT / 100f);
            if (BiliGauge != null) BiliGauge.fillAmount = Mathf.Clamp01(Data.Bilirubin / 2.0f);
        }

        // Logic moved to ARVisualsController to prevent property block conflicts


        public void SetHighProteinDiet()
        {
            Data.NutritionMultiplier = 1.5f;
            Data.IsFattyDiet = false;
            string msg = "Beslenme: Yüksek proteinli diyet seçildi. Hücre yenilenmesi hızlanıyor, AST/ALT seviyeleri normale dönüyor.";
            Debug.Log("<color=green>SUCCESS:</color> High Protein Diet method called.");
            if (FlowUI != null) FlowUI.UpdateLifestyleFeedback(msg);
            if (Visuals != null) Visuals.TriggerPulse(new Color(0.2f, 1.0f, 0.5f, 1.0f)); // Healing Green
            Debug.Log(msg);
        }

        public void SetHighFatDiet()
        {
            Data.NutritionMultiplier = 0.7f;
            Data.IsFattyDiet = true;
            string msg = "UYARI: Yağlı beslenme karaciğerde yağ birikimine (steatoz) ve enzim değerlerinde (AST/ALT) artışa neden olur!";
            Debug.Log("<color=red>WARNING:</color> High Fat Diet method called.");
            if (FlowUI != null) FlowUI.UpdateLifestyleFeedback(msg);
            if (Visuals != null) Visuals.TriggerPulse(new Color(1.0f, 0.5f, 0.0f, 1.0f)); // Warning Orange
            Debug.Log(msg);
        }

        public void ToggleExercise(bool isActive)
        {
            Data.ExerciseMultiplier = isActive ? 1.3f : 1.0f;
            string msg = isActive ? 
                "Egzersiz: Kan akışı arttı. Toksin atılımı hızlanıyor ve Bilirubin seviyesi düşüyor." : 
                "Hareketsiz Yaşam: Kan dolaşımı yavaşladı, iyileşme hızı baz seviyeye düştü.";
            Debug.Log($"<color=cyan>INFO:</color> Toggle Exercise called. Active: {isActive}");
            if (FlowUI != null) FlowUI.UpdateLifestyleFeedback(msg);
            if (Visuals != null && isActive) Visuals.TriggerPulse(new Color(0.0f, 0.8f, 1.0f, 1.0f)); // Vascular Blue
            Debug.Log(msg);
        }

        // Wrapper for UI buttons
        public void ToggleExerciseTrue() => ToggleExercise(true);
        public void ToggleExerciseFalse() => ToggleExercise(false);
    }
}
