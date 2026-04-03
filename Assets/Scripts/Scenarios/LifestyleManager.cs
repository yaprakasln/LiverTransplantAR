using UnityEngine;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Scenarios
{
    public class LifestyleManager : MonoBehaviour
    {
        public SimulationState Data;
        public SkinnedMeshRenderer LiverRenderer;
        
        private MaterialPropertyBlock _propBlock;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (Data == null) Debug.LogError("SimulationData not assigned to LifestyleManager!");
        }

        void Update()
        {
            ProcessLifestyleEffects();
            UpdateSteatosisVisuals();
        }

        private void ProcessLifestyleEffects()
        {
            // Modifying growth rate based on lifestyle
            float currentRate = Data.BaseRegenerationRate * Data.NutritionMultiplier * Data.ExerciseMultiplier;
            
            // If the liver is healthy (IsAdherent) and growing, increment growth
            if (Data.IsAdherent && Data.GrowthPercentage < 1.0f)
            {
                Data.GrowthPercentage += currentRate * Time.deltaTime;
            }
            
            // Limit growth at 100%
            Data.GrowthPercentage = Mathf.Clamp(Data.GrowthPercentage, 0.4f, 1.0f);
        }

        private void UpdateSteatosisVisuals()
        {
            if (LiverRenderer == null) return;
            
            LiverRenderer.GetPropertyBlock(_propBlock);
            
            // Steatosis (Fatty liver) texture overlay based on fatty diet
            float steatosisTarget = Data.IsFattyDiet ? 1.0f : 0.0f;
            float currentSteatosis = _propBlock.GetFloat("_SteatosisAmount");
            
            // Smoothly lerp towards target
            float lerpedSteatosis = Mathf.Lerp(currentSteatosis, steatosisTarget, Time.deltaTime);
            _propBlock.SetFloat("_SteatosisAmount", lerpedSteatosis);
            
            LiverRenderer.SetPropertyBlock(_propBlock);
        }

        // Methods for UI interaction
        public void SetHighProteinDiet()
        {
            Data.NutritionMultiplier = 1.5f;
            Data.IsFattyDiet = false;
            Debug.Log("Lifestyle: High protein diet selected.");
        }

        public void SetHighFatDiet()
        {
            Data.NutritionMultiplier = 0.8f;
            Data.IsFattyDiet = true;
            Debug.Log("Lifestyle: High fat diet selected.");
        }

        public void ToggleExercise(bool isActive)
        {
            Data.ExerciseMultiplier = isActive ? 1.3f : 1.0f;
            Debug.Log($"Lifestyle: Exercise multiplier set to {Data.ExerciseMultiplier}");
        }
    }
}
