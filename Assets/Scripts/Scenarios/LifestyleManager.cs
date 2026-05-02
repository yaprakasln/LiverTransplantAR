using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LiverTransplantAR.Scenarios
{
    public class LifestyleManager : MonoBehaviour
    {
        public LiverTransplantAR.Data.SimulationState Data;
        public LiverTransplantAR.UI.FlowManager FlowUI;

        [Header("HUD References")]
        public Image ASTGauge;
        public Image ALTGauge;
        public Image BiliGauge;

        [Header("Visual References")]
        public Renderer LiverRenderer;
        public Color HealthyColor = new Color(0.6f, 0.1f, 0.1f);
        public Color FattyColor = new Color(0.85f, 0.75f, 0.35f);

        private float targetAST = 0.4f;
        private float targetALT = 0.3f;
        private float targetBili = 0.2f;
        private Color targetColor;

        void Start()
        {
            targetColor = HealthyColor;
            if (LiverRenderer == null) {
                var visuals = GameObject.FindObjectOfType<ARVisualsController>();
                if (visuals != null) LiverRenderer = visuals.GetComponentInChildren<Renderer>();
            }
        }

        void Update()
        {
            // Smoothly update the gauges (HUD)
            if (ASTGauge != null) ASTGauge.fillAmount = Mathf.Lerp(ASTGauge.fillAmount, targetAST, Time.deltaTime * 2f);
            if (ALTGauge != null) ALTGauge.fillAmount = Mathf.Lerp(ALTGauge.fillAmount, targetALT, Time.deltaTime * 2f);
            if (BiliGauge != null) BiliGauge.fillAmount = Mathf.Lerp(BiliGauge.fillAmount, targetBili, Time.deltaTime * 2f);

            // Smoothly change Liver Color
            if (LiverRenderer != null)
            {
                Color currentColor = LiverRenderer.material.color;
                LiverRenderer.material.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * 1.5f);
            }
        }

        public void SetHighProteinDiet()
        {
            Debug.Log("Lifestyle: High Protein Diet selected.");
            targetAST = 0.25f; 
            targetALT = 0.2f;
            targetBili = 0.15f;
            targetColor = HealthyColor;
        }

        public void SetHighFatDiet()
        {
            Debug.Log("Lifestyle: High Fat Diet selected.");
            targetAST = 0.85f; 
            targetALT = 0.9f;
            targetBili = 0.75f;
            targetColor = FattyColor;
        }

        public void ToggleExerciseTrue()
        {
            Debug.Log("Lifestyle: Exercise ON.");
            targetAST = Mathf.Max(0.1f, targetAST - 0.2f);
            targetALT = Mathf.Max(0.1f, targetALT - 0.2f);
            targetBili = Mathf.Max(0.1f, targetBili - 0.15f);
        }

        public void ToggleExerciseFalse()
        {
            Debug.Log("Lifestyle: Exercise OFF.");
            if (targetColor == FattyColor)
            {
                targetAST = Mathf.Min(1.0f, targetAST + 0.1f);
                targetALT = Mathf.Min(1.0f, targetALT + 0.1f);
            }
        }
    }
}
