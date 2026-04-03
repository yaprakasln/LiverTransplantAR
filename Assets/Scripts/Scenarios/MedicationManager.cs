using UnityEngine;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.Scenarios
{
    public class MedicationManager : MonoBehaviour
    {
        public SimulationState Data;
        public SkinnedMeshRenderer LiverRenderer;
        
        private MaterialPropertyBlock _propBlock;

        void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            if (Data == null) Debug.LogError("SimulationData not assigned to MedicationManager!");
        }

        void Update()
        {
            UpdateMedicationState();
            UpdateShaderEffects();
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
                Data.HealthPoints = Mathf.MoveTowards(Data.HealthPoints, 100f, Time.deltaTime * 0.5f);
                
                // Gradually normalize biochemical markers
                Data.AST = Mathf.MoveTowards(Data.AST, 25f, Time.deltaTime * 1.0f);
                Data.ALT = Mathf.MoveTowards(Data.ALT, 30f, Time.deltaTime * 1.0f);
                Data.Bilirubin = Mathf.MoveTowards(Data.Bilirubin, 0.8f, Time.deltaTime * 0.05f);
            }

            Data.HealthPoints = Mathf.Clamp(Data.HealthPoints, 0, 100);
        }

        private void UpdateShaderEffects()
        {
            if (LiverRenderer == null) return;

            LiverRenderer.GetPropertyBlock(_propBlock);
            
            // Icterus (yellowing) increases with Bilirubin or ImmuneAttack
            float icterusAmount = Data.ImmuneAttack ? Mathf.InverseLerp(0.8f, 5.0f, Data.Bilirubin) : 0f;
            _propBlock.SetFloat("_IcterusIntensity", icterusAmount);
            
            // Darkening/Edema visual feedback
            float healthEffect = 1.0f - (Data.HealthPoints / 100f);
            _propBlock.SetFloat("_DarkeningIntensity", healthEffect);

            LiverRenderer.SetPropertyBlock(_propBlock);
        }
        
        // Method to be called by UI button
        public void ToggleAdherence()
        {
            Data.IsAdherent = !Data.IsAdherent;
            Debug.Log($"Medication Adherence: {Data.IsAdherent}");
        }
    }
}
