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

        [Header("VFX Settings")]
        public ParticleSystem VascularizationParticles;
        private MaterialPropertyBlock _propBlock;

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

            // Set initial state
            Data.GrowthPercentage = Data.HasTransplant ? 0.4f : 0.3f;
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

            // Scenario 1 & 2 Logic: First 4 weeks are faster
            float weekMultiplier = Data.SimulationWeek <= 4 ? 2.0f : 1.0f;
            
            // Scenario 2: Only grow if medication is taken (IsAdherent)
            if (Data.IsAdherent && Data.GrowthPercentage < 1.0f)
            {
                float regenerationAmount = Data.BaseRegenerationRate * weekMultiplier * Time.deltaTime;
                Data.GrowthPercentage += regenerationAmount;
            }
            else if (!Data.IsAdherent && Data.CurrentMode == AppFlowState.Scenario_Medication)
            {
                // Bad Scenario: If not taking meds, health drops (optional logic)
                Data.HealthPoints -= Time.deltaTime * 2f;
                Data.ImmuneAttack = true;
            }

            // --- Vertex Growth (Shader-based) ---
            // We map 0.3-1.0 growth range to a small offset (0.0 to 0.05) to simulate expansion
            float offset = Mathf.InverseLerp(0.3f, 1.0f, Data.GrowthPercentage) * 0.05f;
            
            if (_propBlock == null) _propBlock = new MaterialPropertyBlock();
            
            LiverRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_GrowthOffset", offset);
            
            // Rejection tinting
            _propBlock.SetFloat("_IcterusIntensity", Data.ImmuneAttack ? 1.0f : 0f);
            
            LiverRenderer.SetPropertyBlock(_propBlock);

            // Scale logic for initial states
            // Instead of blendshapes, we can also use transform scale if needed, 
            // but vertex inflation is more "organic".
        }

        private void UpdateVascularization()
        {
            if (LiverRenderer == null) return;

            // shader property for vascularization visibility
            LiverRenderer.GetPropertyBlock(_propBlock);
            float vascularIntensity = Mathf.Clamp01(Data.GrowthPercentage) * Data.ExerciseMultiplier;
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
            Data.SimulationWeek++;
            Debug.Log($"Current Week: {Data.SimulationWeek}. Speed Multiplier: {(Data.SimulationWeek <= 4 ? "2x" : "1x")}");
        }
    }
}
