using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LiverTransplantAR.Scenarios
{
    public class LifestyleManager : MonoBehaviour
    {
        public LiverTransplantAR.Data.SimulationState Data;
        public LiverTransplantAR.UI.FlowManager FlowUI;

        [Header("Text References")]
        public TMPro.TMP_Text InfoText;
        public TMPro.TMP_Text StatusLabel;

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
            UpdateStatus("SİSTEM HAZIR. Seçim yapın.");
        }

        void Update()
        {
            // Smoothly change Liver Color (Fixing the _Color error)
            if (LiverRenderer != null)
            {
                Color currentColor = Color.white;
                if (LiverRenderer.material.HasProperty("_BaseColor")) currentColor = LiverRenderer.material.GetColor("_BaseColor");
                else if (LiverRenderer.material.HasProperty("_Color")) currentColor = LiverRenderer.material.color;

                Color nextColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * 1.5f);
                
                if (LiverRenderer.material.HasProperty("_BaseColor")) LiverRenderer.material.SetColor("_BaseColor", nextColor);
                else LiverRenderer.material.color = nextColor;
            }
        }

        public void SetHighProteinDiet()
        {
            targetColor = HealthyColor;
            UpdateStatus("<color=green>PROTEİN DİYETİ:</color> Karaciğer dokusu onarımı destekleniyor.");
        }

        public void SetHighFatDiet()
        {
            targetColor = FattyColor;
            UpdateStatus("<color=red>YAĞLI DİYET:</color> Karaciğerde yağlanma (Steatoz) riski artıyor!");
        }

        public void ToggleExerciseTrue()
        {
            targetColor = Color.Lerp(targetColor, Color.white, 0.2f); // Make it look "brighter/healthier"
            UpdateStatus("<color=cyan>EGZERSİZ AKTİF:</color> Kan akışı ve metabolizma hızı arttı.");
        }

        public void ToggleExerciseFalse()
        {
            UpdateStatus("<color=yellow>HAREKETSİZLİK:</color> Metabolik riskler izleniyor.");
        }

        private void UpdateStatus(string msg)
        {
            if (StatusLabel != null) StatusLabel.text = msg;
        }
    }
}
