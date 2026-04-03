using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LiverTransplantAR.Data;

namespace LiverTransplantAR.UI
{
    public class DashboardUI : MonoBehaviour
    {
        public SimulationState Data;

        [Header("HP Bar")]
        public Image HealthBarFill;
        public TextMeshProUGUI HealthPercentageText;

        [Header("Biochemical Markers")]
        public TextMeshProUGUI ASTText;
        public TextMeshProUGUI ALTText;
        public TextMeshProUGUI BilirubinText;

        [Header("Regeneration Status")]
        public TextMeshProUGUI GrowthText;
        public Slider GrowthSlider;

        [Header("Warning & Feedback")]
        public GameObject WarningPanel;
        public TextMeshProUGUI WarningText;

        void Update()
        {
            if (Data == null) return;

            UpdateHealthUI();
            UpdateBiometricUI();
            UpdateRegenerationUI();
            CheckWarnings();
        }

        private void UpdateHealthUI()
        {
            float fillAmount = Data.HealthPoints / 100f;
            HealthBarFill.fillAmount = fillAmount;
            
            // Red overlay for low health
            HealthBarFill.color = Color.Lerp(Color.red, Color.green, fillAmount);
            HealthPercentageText.text = $"{Mathf.CeilToInt(Data.HealthPoints)}%";
        }

        private void UpdateBiometricUI()
        {
            ASTText.text = $"AST: {Data.AST:F1} U/L";
            ALTText.text = $"ALT: {Data.ALT:F1} U/L";
            BilirubinText.text = $"Bili: {Data.Bilirubin:F2} mg/dL";

            // Highlight critical values
            ASTText.color = Data.AST > 100 ? Color.red : Color.white;
            ALTText.color = Data.ALT > 100 ? Color.red : Color.white;
            BilirubinText.color = Data.Bilirubin > 2.0f ? Color.red : Color.white;
        }

        private void UpdateRegenerationUI()
        {
            float percentage = Data.GrowthPercentage * 100f;
            GrowthText.text = $"Regeneration: {percentage:F1}%";
            GrowthSlider.value = Data.GrowthPercentage;
        }

        private void CheckWarnings()
        {
            if (Data.ImmuneAttack)
            {
                WarningPanel.SetActive(true);
                WarningText.text = "Immune Response Triggered!";
            }
            else if (Data.HealthPoints < 30)
            {
                WarningPanel.SetActive(true);
                WarningText.text = "CRITICAL ORGAN FAILURE!";
            }
            else
            {
                WarningPanel.SetActive(false);
            }
        }
    }
}
