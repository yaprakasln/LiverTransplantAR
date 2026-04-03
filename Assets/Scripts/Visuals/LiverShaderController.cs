using UnityEngine;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Visuals
{
    /// <summary>
    /// This script acts as a bridge between SimulationData and the URP Shader Graph.
    /// It drives shader properties like Icterus, Steatosis, and Vascularization.
    /// </summary>
    public class LiverShaderController : MonoBehaviour
    {
        public SimulationState Data;
        public Renderer LiverRenderer;

        [Header("Shader Property Names")]
        public string IcterusProperty = "_IcterusAmount";
        public string SteatosisProperty = "_SteatosisAmount";
        public string VascularProperty = "_VascularIntensity";
        public string EdemaProperty = "_EdemaStrength";

        private MaterialPropertyBlock _propBlock;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
        }

        void Update()
        {
            if (Data == null || LiverRenderer == null) return;

            LiverRenderer.GetPropertyBlock(_propBlock);

            // 1. Icterus (Yellowing) - Corresponds to Bilirubin elevations
            float icterus = Mathf.InverseLerp(1.2f, 8.0f, Data.Bilirubin);
            _propBlock.SetFloat(IcterusProperty, icterus);

            // 2. Steatosis (Fatty liver) - Corresponds to diet
            float steatosis = Data.IsFattyDiet ? 1.0f : 0.0f;
            float currentSteatosis = _propBlock.GetFloat(SteatosisProperty);
            _propBlock.SetFloat(SteatosisProperty, Mathf.Lerp(currentSteatosis, steatosis, Time.deltaTime));

            // 3. Vascularization (Vein visibility) - Corresponds to growth + exercise
            float vascular = (Data.GrowthPercentage - 0.4f) * Data.ExerciseMultiplier;
            _propBlock.SetFloat(VascularProperty, Mathf.Clamp01(vascular));

            // 4. Edema (Swelling vertex offset) - Corresponds to Immune Attack
            float edema = Data.ImmuneAttack ? 1.0f : 0.0f;
            float currentEdema = _propBlock.GetFloat(EdemaProperty);
            _propBlock.SetFloat(EdemaProperty, Mathf.Lerp(currentEdema, edema, Time.deltaTime * 3f));

            LiverRenderer.SetPropertyBlock(_propBlock);
        }
    }
}
