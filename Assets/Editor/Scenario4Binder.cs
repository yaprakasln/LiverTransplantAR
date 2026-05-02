using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using LiverTransplantAR.Scenarios;

namespace LiverTransplantAR.EditorTools
{
    public class Scenario4Binder : EditorWindow
    {
        [MenuItem("Liver AR/PREMIUM SETUP: Scenario 4 Hologram HUD")]
        public static void SetupScenario4Hologram()
        {
            if (EditorApplication.isPlaying) EditorApplication.isPlaying = false;

            // 1. CLEANUP
            string[] toDestroy = { "AR_Hologram_HUD", "AR_UI_Canvas", "Scenario_Text", "LIVER AR FINAL" };
            foreach (string s in toDestroy) {
                GameObject target = GameObject.Find(s);
                if (target != null) GameObject.DestroyImmediate(target);
            }

            // 2. LIVER & CAMERA
            GameObject liver = GameObject.Find("AR_Liver_Pivot") ?? GameObject.Find("Menu_Liver_Pivot") ?? GameObject.FindObjectOfType<ARVisualsController>()?.gameObject;
            if (liver != null) {
                liver.transform.position = new Vector3(0, 0.40f, 0); 
                liver.transform.localScale = Vector3.one * 1.6f; 
            }

            Camera mainCam = Camera.main;
            if (mainCam != null) {
                mainCam.transform.position = new Vector3(0, 0.45f, -1.35f); 
                mainCam.transform.LookAt(new Vector3(0, 0.40f, 0));
            }

            LifestyleManager lm = GameObject.FindObjectOfType<LifestyleManager>();
            if (lm == null) {
                lm = new GameObject("LifestyleManager").AddComponent<LifestyleManager>();
                lm.Data = AssetDatabase.LoadAssetAtPath<LiverTransplantAR.Data.SimulationState>("Assets/Data/SimulationData.asset");
            }

            // 3. HUD ROOT & EVENT SYSTEM (CRITICAL)
            if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null) {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            GameObject hudObj = new GameObject("AR_Hologram_HUD");
            Canvas canvas = hudObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // Ensure it's on top
            var scaler = hudObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            hudObj.AddComponent<GraphicRaycaster>();

            // 4. TOP PANEL (Enzymes)
            GameObject topPanel = new GameObject("TopPanel");
            topPanel.transform.SetParent(hudObj.transform, false);
            var tPRect = topPanel.AddComponent<RectTransform>();
            tPRect.anchorMin = new Vector2(0, 0.80f); tPRect.anchorMax = new Vector2(1, 1); 
            tPRect.anchoredPosition = Vector2.zero; tPRect.sizeDelta = Vector2.zero;
            topPanel.AddComponent<Image>().color = new Color(0, 0.05f, 0.12f, 0.6f);

            lm.ASTGauge = CreateAnchoredGauge(topPanel.transform, "AST ENZYME", 0.66f, 1.0f);
            lm.ALTGauge = CreateAnchoredGauge(topPanel.transform, "ALT ENZYME", 0.33f, 0.66f);
            lm.BiliGauge = CreateAnchoredGauge(topPanel.transform, "BILIRUBIN", 0.0f, 0.33f);

            // 5. BOTTOM PANEL (Buttons)
            GameObject bottomPanel = new GameObject("BottomPanel");
            bottomPanel.transform.SetParent(hudObj.transform, false);
            var bPRect = bottomPanel.AddComponent<RectTransform>();
            bPRect.anchorMin = new Vector2(0, 0); bPRect.anchorMax = new Vector2(1, 0.22f); 
            bPRect.anchoredPosition = Vector2.zero; bPRect.sizeDelta = Vector2.zero;
            bottomPanel.AddComponent<Image>().color = new Color(0, 0.05f, 0.12f, 0.7f);

            CreateSmartButton(bottomPanel.transform, "PROTEIN DIET", lm, "SetHighProteinDiet", new Vector2(0.05f, 0.55f), new Vector2(0.48f, 0.95f), new Color(0, 0.7f, 0.4f, 1));
            CreateSmartButton(bottomPanel.transform, "FATTY DIET", lm, "SetHighFatDiet", new Vector2(0.52f, 0.55f), new Vector2(0.95f, 0.95f), new Color(0.8f, 0.15f, 0.1f, 1));
            CreateSmartButton(bottomPanel.transform, "EXERCISE ON", lm, "ToggleExerciseTrue", new Vector2(0.05f, 0.05f), new Vector2(0.48f, 0.45f), new Color(0, 0.5f, 0.9f, 1));
            CreateSmartButton(bottomPanel.transform, "EXERCISE OFF", lm, "ToggleExerciseFalse", new Vector2(0.52f, 0.05f), new Vector2(0.95f, 0.45f), new Color(0.4f, 0.4f, 0.4f, 1));

            Debug.Log("<color=green>SCENARIO 4: SMART HUD READY!</color>");
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        private static Image CreateAnchoredGauge(Transform parent, string label, float minV, float maxV)
        {
            GameObject container = new GameObject(label);
            container.transform.SetParent(parent, false);
            var rect = container.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, minV); rect.anchorMax = new Vector2(1, maxV);
            rect.anchoredPosition = Vector2.zero; rect.sizeDelta = Vector2.zero;

            var txt = new GameObject("Txt").AddComponent<TextMeshProUGUI>();
            txt.transform.SetParent(container.transform, false);
            txt.text = $"<b>{label}</b>"; txt.fontSize = 28; txt.color = Color.cyan;
            txt.raycastTarget = false;
            var tRect = txt.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0.05f, 0.2f); tRect.anchorMax = new Vector2(0.35f, 0.8f);
            tRect.anchoredPosition = Vector2.zero; tRect.sizeDelta = Vector2.zero;

            GameObject bg = new GameObject("Bg");
            bg.transform.SetParent(container.transform, false);
            bg.AddComponent<Image>().color = new Color(1, 1, 1, 0.15f);
            var bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.40f, 0.35f); bgRect.anchorMax = new Vector2(0.95f, 0.65f);
            bgRect.anchoredPosition = Vector2.zero; bgRect.sizeDelta = Vector2.zero;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(bg.transform, false);
            var fillImg = fill.AddComponent<Image>();
            fillImg.type = Image.Type.Filled; fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.color = new Color(0, 1, 1, 1f);
            fillImg.raycastTarget = false;
            var fRect = fill.GetComponent<RectTransform>();
            fRect.anchorMin = Vector2.zero; fRect.anchorMax = Vector2.one; fRect.sizeDelta = Vector2.zero;

            return fillImg;
        }

        private static void CreateSmartButton(Transform parent, string label, LifestyleManager target, string method, Vector2 min, Vector2 max, Color c)
        {
            GameObject btnObj = new GameObject(label);
            btnObj.transform.SetParent(parent, false);
            var rect = btnObj.AddComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max;
            rect.anchoredPosition = Vector2.zero; rect.sizeDelta = Vector2.zero;

            var img = btnObj.AddComponent<Image>();
            img.color = c;
            img.raycastTarget = true;

            var txt = new GameObject("Label").AddComponent<TextMeshProUGUI>();
            txt.transform.SetParent(btnObj.transform, false);
            txt.text = $"<b>{label}</b>"; txt.fontSize = 26; txt.alignment = TextAlignmentOptions.Center;
            txt.raycastTarget = false;
            var tRect = txt.GetComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one; tRect.sizeDelta = Vector2.zero;

            btnObj.AddComponent<Button>();
            var helper = btnObj.AddComponent<LiverTransplantAR.UI.LifestyleButtonHelper>();
            helper.methodName = method;
        }
    }
}
