using UnityEngine;

namespace LiverTransplantAR.Data
{
    public enum AppFlowState { Intro, MainMenu, Scenario_Recovery, Scenario_Medication }

    [CreateAssetMenu(fileName = "NewSimulationState", menuName = "LiverAR/SimulationState")]
    public class SimulationState : ScriptableObject
    {
        [Header("Flow Control")]
        public AppFlowState CurrentMode = AppFlowState.Intro;

        [Header("Regeneration Status")]
        [Range(0.3f, 1.0f)]
        public float GrowthPercentage = 0.3f;
        public float BaseRegenerationRate = 0.005f; // Speed of growth per tick
        public int SimulationWeek = 1;              // Current week in simulation
        public bool HasTransplant = false;          // True if transplant, False if resection (%30)

        [Header("Medication & Immunology")]
        public bool IsAdherent = true;
        public bool ImmuneAttack = false;
        [Range(0, 100)]
        public float HealthPoints = 100f;

        [Header("Clinical Dashboard")]
        public float AST = 25f;       // Normal: 10-40 U/L
        public float ALT = 30f;       // Normal: 7-56 U/L
        public float Bilirubin = 0.8f; // Normal: 0.1-1.2 mg/dL

        [Header("Lifestyle Multipliers")]
        public float NutritionMultiplier = 1.0f; // Protein focus
        public float ExerciseMultiplier = 1.0f;  // Vascular flow focus
        public bool IsFattyDiet = false;         // Steatosis trigger

        public void ResetToDefault()
        {
            GrowthPercentage = HasTransplant ? 0.4f : 0.3f; // Transplant starts with more
            SimulationWeek = 1;
            HasTransplant = false;
            IsAdherent = true;
            ImmuneAttack = false;
            HealthPoints = 100f;
            AST = 25f;
            ALT = 30f;
            Bilirubin = 0.8f;
            NutritionMultiplier = 1.0f;
            ExerciseMultiplier = 1.0f;
            IsFattyDiet = false;
        }
    }
}
