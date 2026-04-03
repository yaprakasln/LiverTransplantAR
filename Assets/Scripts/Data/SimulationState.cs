using UnityEngine;

namespace LiverTransplantAR.Data
{
    [CreateAssetMenu(fileName = "NewSimulationState", menuName = "LiverAR/SimulationState")]
    public class SimulationState : ScriptableObject
    {
        [Header("Regeneration Status")]
        [Range(0.4f, 1.0f)]
        public float GrowthPercentage = 0.4f;
        public float BaseRegenerationRate = 0.01f; // Speed of growth per tick

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
            GrowthPercentage = 0.4f;
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
