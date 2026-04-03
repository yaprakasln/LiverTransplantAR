using UnityEngine;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Scenarios
{
    public class LiverRegeneration : MonoBehaviour
    {
        public SimulationState Data;
        public SkinnedMeshRenderer LiverRenderer;
        
        [Header("Blend Shape Indices")]
        public int GrowthShapeIndex = 0;
        public int SwellingShapeIndex = 1;

        [Header("VFX Settings")]
        public ParticleSystem VascularizationParticles;
        private MaterialPropertyBlock _propBlock;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (Data == null) Debug.LogError("SimulationData not assigned to LiverRegeneration!");
        }

        void Update()
        {
            UpdateGrowth();
            UpdateVascularization();
        }

        private void UpdateGrowth()
        {
            if (LiverRenderer == null) return;

            // Grow based on Data.GrowthPercentage (0.4 to 1.0)
            // BlendShape is 0-100, so we map 0.4-1.0 to 0-100 if the model is 100% at full size.
            // Assuming 0% BlendShape is 100% size, and 100% BlendShape is "Small" (40% size).
            // Or vice versa. Let's assume 0% blend is 40% size, 100% blend is 100% size.
            float weight = Mathf.InverseLerp(0.4f, 1.0f, Data.GrowthPercentage) * 100f;
            LiverRenderer.SetBlendShapeWeight(GrowthShapeIndex, weight);

            // If immune response is active, trigger swelling
            float targetSwelling = Data.ImmuneAttack ? 100f : 0f;
            float currentSwelling = LiverRenderer.GetBlendShapeWeight(SwellingShapeIndex);
            LiverRenderer.SetBlendShapeWeight(SwellingShapeIndex, Mathf.Lerp(currentSwelling, targetSwelling, Time.deltaTime * 2f));
        }

        private void UpdateVascularization()
        {
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
    }
}
