using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using LiverTransplantAR.UI;
using LiverTransplantAR.Scenarios;

namespace LiverTransplantAR.EditorTools
{
    public class Scenario4Binder : EditorWindow
    {
        [MenuItem("Liver AR/Create and Setup Scenario 4")]
        public static void CreateAndBindScenario4()
        {
            FlowManager flowManager = GameObject.FindObjectOfType<FlowManager>();
            LifestyleManager lifestyleManager = GameObject.FindObjectOfType<LifestyleManager>();

            if (flowManager == null)
            {
                Debug.LogError("Setup Error: FlowManager not found!");
                return;
            }

            // --- 1. LIVER MODEL FIX ---
            if (lifestyleManager == null)
            {
                GameObject lifestyleObj = new GameObject("LifestyleManager");
                lifestyleManager = lifestyleObj.AddComponent<LifestyleManager>();
                lifestyleManager.Data = flowManager.Data;
            }

            // --- 2. CREATE PREMIUM LIFESTYLE UI PANEL ---
            GameObject scenarioPanel = flowManager.ScenarioPanel;
            if (scenarioPanel != null)
            {
                Transform lifestylePanel = scenarioPanel.transform.Find("LifestyleUIPanel");
                if (lifestylePanel != null) GameObject.DestroyImmediate(lifestylePanel.gameObject);

                GameObject panelObj = new GameObject("LifestyleUIPanel");
                panelObj.transform.SetParent(scenarioPanel.transform, false);
                
                // Add Glassmorphism Background
                var bgImage = panelObj.AddComponent<UnityEngine.UI.Image>();
                bgImage.color = new Color(0.1f, 0.1f, 0.2f, 0.8f); // Deep medical blue, semi-transparent
                
                var rect = panelObj.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(900, 250);
                rect.anchoredPosition = new Vector2(0, -300);

                // Add 3 Premium Buttons
                CreatePremiumButton(panelObj.transform, "ProteinBtn", "Yüksek Protein", lifestyleManager, "SetHighProteinDiet", new Vector3(-300, 0, 0));
                CreatePremiumButton(panelObj.transform, "FattyBtn", "Yağlı Diyet", lifestyleManager, "SetHighFatDiet", new Vector3(0, 0, 0));
                CreatePremiumButton(panelObj.transform, "ExerciseBtn", "Egzersiz Modu", lifestyleManager, "ToggleExerciseTrue", new Vector3(300, 0, 0));

                Debug.Log("<color=green>Success:</color> Premium Lifestyle Panel created with glassmorphism style.");
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        private static void CreatePremiumButton(Transform parent, string name, string label, MonoBehaviour target, string methodName, Vector3 pos)
        {
            GameObject btnBase = GameObject.Find("OnarimButonu");
            if (btnBase == null) return;

            GameObject newBtn = GameObject.Instantiate(btnBase, parent);
            newBtn.name = name;
            newBtn.transform.localPosition = pos;
            
            var rect = newBtn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(250, 180);

            var btnComp = newBtn.GetComponent<Button>();
            btnComp.onClick = new Button.ButtonClickedEvent();
            
            // Fix: Use direct method binding
            UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(btnComp.onClick, (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), target, methodName));
            
            var text = newBtn.GetComponentInChildren<TMPro.TMP_Text>();
            if (text != null)
            {
                text.text = label;
                text.fontSize = 24;
                text.color = Color.white;
            }

            // Visual Polish: Add a subtle border or glow if possible, or just color
            var img = newBtn.GetComponent<UnityEngine.UI.Image>();
            if (img != null) img.color = new Color(0.2f, 0.4f, 0.8f, 0.5f);
        }
    }
}
