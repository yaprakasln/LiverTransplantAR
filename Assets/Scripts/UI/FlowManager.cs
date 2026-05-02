using UnityEngine;
using LiverTransplantAR.Data;
using TMPro;

namespace LiverTransplantAR.UI
{
    public class FlowManager : MonoBehaviour
    {
        public SimulationState Data;
        
        [Header("UI Panels")]
        public GameObject IntroPanel;
        public GameObject MenuPanel;
        public GameObject ScenarioPanel;
        
        [Header("Text Displays")]
        public TMP_Text HeaderText;
        public TMP_Text DescriptionText;

        void Start()
        {
            if (Data != null)
            {
                ShowIntro();
            }
        }

        public void ShowIntro()
        {
            if (Data == null) return;
            Data.CurrentMode = AppFlowState.Intro;
            UpdateUI();
            if (HeaderText != null) HeaderText.text = "Karaciğerimizi Tanıyalım";
            if (DescriptionText != null) DescriptionText.text = "Karaciğer, vücudun en büyük iç organıdır ve 500'den fazla fonksiyonu vardır. Şimdi nakil sonrası süreci birlikte inceleyelim.";
        }

        public void ShowMainMenu()
        {
            if (Data == null) return;
            Data.CurrentMode = AppFlowState.MainMenu;
            UpdateUI();
            if (HeaderText != null) HeaderText.text = "Senaryo Seçimi";
            if (DescriptionText != null) DescriptionText.text = "İncelemek istediğiniz nakil sonrası senaryoyu seçin.";
        }

        public void StartRecoveryScenario()
        {
            if (Data == null) return;
            Data.CurrentMode = AppFlowState.Scenario_Recovery;
            Data.ResetToDefault();
            UpdateUI();
            if (HeaderText != null) HeaderText.text = "Senaryo 1: Onarım Süreci";
            if (DescriptionText != null) DescriptionText.text = "Karaciğer, nakil sonrası kendini hızla tamamlamaya başlar. Özellikle ilk 4 hafta bu süreç çok dramatiktir.";
        }

        public void StartMedicationScenario()
        {
            if (Data == null) return;
            Data.CurrentMode = AppFlowState.Scenario_Medication;
            Data.ResetToDefault();
            UpdateUI();
            if (HeaderText != null) HeaderText.text = "Senaryo 2: İlaç Uyumu";
            if (DescriptionText != null) DescriptionText.text = "İlaçların düzenli kullanımı, vücudun yeni karaciğeri reddetmemesi için hayati önem taşır.";
        }

        public void StartLifestyleScenario()
        {
            if (Data == null) return;
            Data.CurrentMode = AppFlowState.Scenario_Lifestyle;
            Data.ResetToDefault();
            UpdateUI();
            if (HeaderText != null) HeaderText.text = "Senaryo 4: Yaşam Tarzı ve Sağlık";
            if (DescriptionText != null) DescriptionText.text = "Beslenme ve egzersiz alışkanlıklarının nakil sonrası karaciğer sağlığı üzerindeki etkilerini inceleyin.";
        }

        private void UpdateUI()
        {
            // GLASSMORPHISM: Ensure panels are semi-transparent and don't block the liver
            ApplyGlassEffect(IntroPanel);
            ApplyGlassEffect(MenuPanel);
            ApplyGlassEffect(ScenarioPanel);

            if (IntroPanel != null) IntroPanel.SetActive(Data.CurrentMode == AppFlowState.Intro);
            if (MenuPanel != null) MenuPanel.SetActive(Data.CurrentMode == AppFlowState.MainMenu);
            if (ScenarioPanel != null) ScenarioPanel.SetActive(Data.CurrentMode == AppFlowState.Scenario_Recovery || Data.CurrentMode == AppFlowState.Scenario_Medication || Data.CurrentMode == AppFlowState.Scenario_Lifestyle);
        }

        private void ApplyGlassEffect(GameObject panel)
        {
            if (panel == null) return;
            var image = panel.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                // Semi-transparent dark medical theme
                image.color = new Color(0.1f, 0.1f, 0.2f, 0.6f); 
            }
        }

        public void ToggleMedication(bool state)
        {
            if (Data == null) return;
            Data.IsAdherent = state;
            if (DescriptionText == null) return;

            if (!state)
                DescriptionText.text = "DİKKAT: İlaç alınmadığında iyileşme durur ve organ reddi süreci başlar!";
            else
                DescriptionText.text = "İlaç alımı düzenli. İyileşme süreci devam ediyor.";
        }

        public void UpdateLifestyleFeedback(string feedbackMessage)
        {
            if (DescriptionText != null)
            {
                DescriptionText.text = feedbackMessage;
            }
        }
    }
}
