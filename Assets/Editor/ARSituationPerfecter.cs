using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using LiverTransplantAR.Data;
using LiverTransplantAR.UI;
using LiverTransplantAR.Scenarios;
using UnityEditor.Events;

namespace LiverTransplantAR.EditorTools
{
    public class ARSituationPerfecter : EditorWindow
    {
        [MenuItem("Liver AR/0. CREATE MAIN MENU")]
        public static void CreateMainMenu()
        {
            SetupBaseScene("Assets/Scenes/MainLauncher.unity", (canvas, liver) => {
                SetupMenuUI(canvas);
            });
        }

        [MenuItem("Liver AR/1. CREATE RECOVERY SCENE")]
        public static void CreateRecoveryScene()
        {
            SetupBaseScene("Assets/Scenes/Scenario1.unity", (canvas, liver) => {
                // Add Recovery Manager
                var reg = liver.transform.parent.gameObject.AddComponent<LiverRegeneration>();
                reg.Data = AssetDatabase.LoadAssetAtPath<SimulationState>("Assets/NewSimulationState.asset");
                reg.LiverRenderer = liver.GetComponentInChildren<Renderer>();
                
                var hud = SetupScenarioHUD(canvas, "RECOVERY PHASE", "Sonraki Hafta", "NextWeek", typeof(RecoveryButtonHelper));
                reg.MainDescriptionText = hud.main;
                reg.CellularBox = hud.c1;
                reg.VolumeBox = hud.c2;
                reg.FunctionalBox = hud.c3;
            });
        }

        [MenuItem("Liver AR/2. CREATE MEDICATION SCENE")]
        public static void CreateMedicationScene()
        {
            SetupBaseScene("Assets/Scenes/Scenario2.unity", (canvas, liver) => {
                var med = liver.transform.parent.gameObject.AddComponent<MedicationManager>();
                med.Data = AssetDatabase.LoadAssetAtPath<SimulationState>("Assets/NewSimulationState.asset");
                
                var hud = SetupScenarioHUD(canvas, "MEDICATION ADHERENCE", "İlacı Al", "SetAdherenceTrue", typeof(MedicationButtonHelper), "İlacı Atla", "SetAdherenceFalse");
                med.MainStatusText = hud.main;
                med.ClinicalBox = hud.c1;
                med.StatusBox = hud.c2;
                med.AdviceBox = hud.c3;
            });
        }

        private static void SetupBaseScene(string scenePath, System.Action<GameObject, GameObject> onReady)
        {
            if (EditorApplication.isPlaying) EditorApplication.isPlaying = false;

            // Build Sync
            var buildScenes = EditorBuildSettings.scenes.ToList();
            if (File.Exists(scenePath) && !buildScenes.Any(s => s.path == scenePath)) {
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                EditorBuildSettings.scenes = buildScenes.ToArray();
            }

            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
            
            // Camera
            Camera mainCam = Camera.main;
            if (mainCam != null) {
                mainCam.transform.position = new Vector3(0, 0, -7f);
                mainCam.backgroundColor = Color.black; mainCam.clearFlags = CameraClearFlags.SolidColor;
            }

            // 2b. Background (Removed per user request)

            // Liver
            string liverPath = "Assets/human-liver-and-gallbladder/source/Liver project - Copy/liver exported for sketchfab - now with the fucking base colours included.fbx";
            GameObject liverPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(liverPath);
            GameObject liverInstance = null;
            if (liverPrefab != null) {
                liverInstance = (GameObject)PrefabUtility.InstantiatePrefab(liverPrefab);
                GameObject pivot = new GameObject("Liver_Pivot");
                pivot.transform.position = new Vector3(0, 0.4f, 0); liverInstance.transform.SetParent(pivot.transform);
                Renderer[] rs = liverInstance.GetComponentsInChildren<Renderer>();
                if (rs.Length > 0) {
                    Bounds b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
                    liverInstance.transform.localPosition = -b.center;
                    pivot.transform.localScale = Vector3.one * (2.5f / Mathf.Max(b.size.x, b.size.y, b.size.z));
                    
                    Material mat = new Material(Shader.Find("Custom/LiverOrganicShader"));
                    if (mat.shader == null) mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    mat.color = new Color(0.7f, 0.2f, 0.2f);
                    foreach (var r in rs) r.material = mat;

                    // RESTORE ROTATION
                    ARVisualsController vc = pivot.AddComponent<ARVisualsController>();
                    vc.RotationTarget = pivot.transform; vc.LiverRenderer = rs[0];
                    vc.EnableAutoRotation = true; vc.RotationSpeed = 30f;
                }
            }

            // UI
            GameObject canvasObj = new GameObject("Canvas");
            Canvas c = canvasObj.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            
            // NEW INPUT SYSTEM COMPATIBILITY
            System.Type inputType = null;
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies()) {
                inputType = a.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule");
                if (inputType != null) break;
            }
            if (inputType != null) esObj.AddComponent(inputType);
            else esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            onReady?.Invoke(canvasObj, liverInstance);

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"<color=green>Scene Saved: {scenePath}</color>");
        }

        private static void SetupMenuUI(GameObject canvas)
        {
            // Simple Title
            GameObject titleObj = new GameObject("Title"); titleObj.transform.SetParent(canvas.transform, false);
            var titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "<b>LIVER TRANSPLANT</b>\n<color=#005577>AR EXPERIENCE</color>";
            titleTxt.alignment = TextAlignmentOptions.Center; titleTxt.fontSize = 80;
            var tRect = titleObj.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0, 0.8f); tRect.anchorMax = new Vector2(1, 0.95f);
            tRect.anchoredPosition = Vector2.zero; tRect.sizeDelta = Vector2.zero;

            string[] names = { "RECOVERY", "MEDICATION", "REJECTION", "LIFESTYLE" };
            for (int i = 0; i < 4; i++) {
                GameObject btnObj = CreateResponsivePanel(canvas.transform, "Btn_" + i, new Vector2(0.1f, 0.4f - i*0.1f), new Vector2(0.9f, 0.48f - i*0.1f), new Color(0, 0.6f, 0.7f));
                var txt = new GameObject("Txt").AddComponent<TextMeshProUGUI>();
                txt.transform.SetParent(btnObj.transform, false);
                txt.text = names[i]; txt.alignment = TextAlignmentOptions.Center;
                var b = btnObj.AddComponent<Button>();
                var loader = btnObj.AddComponent<MenuSceneLoader>();
                loader.targetScene = i == 3 ? "Scenario4_Final" : "Scenario" + (i+1);
                UnityEventTools.AddPersistentListener(b.onClick, loader.LoadTarget);
            }
        }

        public struct HUDResult { public TMP_Text main; public TMP_Text c1; public TMP_Text c2; public TMP_Text c3; }

        private static HUDResult SetupScenarioHUD(GameObject canvas, string title, string b1Name, string b1Method, System.Type helperType, string b2Name = "", string b2Method = "")
        {
            HUDResult res = new HUDResult();

            // HUD Title (Offset to the right to avoid MENU button)
            GameObject titleObj = new GameObject("HUDTitle"); titleObj.transform.SetParent(canvas.transform, false);
            var titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "<b>" + title + "</b>"; titleTxt.fontSize = 50; titleTxt.alignment = TextAlignmentOptions.Center;
            var tRect = titleObj.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0.22f, 0.9f); tRect.anchorMax = new Vector2(0.98f, 0.98f);
            tRect.sizeDelta = Vector2.zero; tRect.anchoredPosition = Vector2.zero;

            // Main Narrative Panel (Top)
            GameObject narrPanel = CreateResponsivePanel(canvas.transform, "MainNarrative", new Vector2(0.15f, 0.78f), new Vector2(0.85f, 0.88f), new Color(0, 0.2f, 0.4f, 0.8f));
            res.main = CreateTextInPanel(narrPanel, "MainText", 38);

            // Callout 1 (Left - Cellular)
            GameObject c1Panel = CreateResponsivePanel(canvas.transform, "Box_Cellular", new Vector2(0.05f, 0.5f), new Vector2(0.3f, 0.65f), new Color(0, 0, 0, 0.7f));
            res.c1 = CreateTextInPanel(c1Panel, "Text", 28);

            // Callout 2 (Right - Volume)
            GameObject c2Panel = CreateResponsivePanel(canvas.transform, "Box_Volume", new Vector2(0.7f, 0.5f), new Vector2(0.95f, 0.65f), new Color(0, 0, 0, 0.7f));
            res.c2 = CreateTextInPanel(c2Panel, "Text", 28);

            // Callout 3 (Bottom - Functional)
            GameObject c3Panel = CreateResponsivePanel(canvas.transform, "Box_Functional", new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.32f), new Color(0, 0, 0, 0.7f));
            res.c3 = CreateTextInPanel(c3Panel, "Text", 30);

            // Interaction Buttons
            bool hasTwoButtons = !string.IsNullOrEmpty(b2Name);
            
            // Interaction Button 1
            Vector2 b1Min = hasTwoButtons ? new Vector2(0.05f, 0.05f) : new Vector2(0.3f, 0.05f);
            Vector2 b1Max = hasTwoButtons ? new Vector2(0.48f, 0.15f) : new Vector2(0.7f, 0.15f);
            
            GameObject btn1 = CreateResponsivePanel(canvas.transform, "Btn1", b1Min, b1Max, new Color(0, 0.5f, 1f));
            var t1 = CreateTextInPanel(btn1, "Label", 40); t1.text = "<b>" + b1Name + "</b>";
            t1.alignment = TextAlignmentOptions.Center;
            t1.enableWordWrapping = false;
            btn1.AddComponent<Button>();
            var h1 = btn1.AddComponent(helperType);
            h1.GetType().GetField("methodName").SetValue(h1, b1Method);

            // Interaction Button 2
            if (hasTwoButtons) {
                GameObject btn2 = CreateResponsivePanel(canvas.transform, "Btn2", new Vector2(0.52f, 0.05f), new Vector2(0.95f, 0.15f), new Color(0.8f, 0.1f, 0.1f));
                var t2 = CreateTextInPanel(btn2, "Label", 45); t2.text = "<b>" + b2Name + "</b>";
                btn2.AddComponent<Button>();
                var h2 = btn2.AddComponent(helperType);
                h2.GetType().GetField("methodName").SetValue(h2, b2Method);
            }
            
            // Home Button
            GameObject homeBtn = CreateResponsivePanel(canvas.transform, "HomeBtn", new Vector2(0.02f, 0.93f), new Vector2(0.2f, 0.98f), new Color(0.2f, 0.2f, 0.2f));
            var homeTxt = new GameObject("X").AddComponent<TextMeshProUGUI>();
            homeTxt.transform.SetParent(homeBtn.transform, false); homeTxt.text = "<- MENU";
            homeTxt.fontSize = 30; homeTxt.alignment = TextAlignmentOptions.Center;
            var bHome = homeBtn.AddComponent<Button>();
            var loader = homeBtn.AddComponent<MenuSceneLoader>();
            loader.targetScene = "MainLauncher";
            UnityEventTools.AddPersistentListener(bHome.onClick, loader.LoadTarget);

            return res;
        }

        private static TMP_Text CreateTextInPanel(GameObject panel, string name, int fontSize)
        {
            var txtObj = new GameObject(name).AddComponent<TextMeshProUGUI>();
            txtObj.transform.SetParent(panel.transform, false);
            txtObj.fontSize = fontSize; txtObj.alignment = TextAlignmentOptions.Center;
            txtObj.text = "...";
            var rect = txtObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.sizeDelta = new Vector2(-15, -15);
            return txtObj;
        }

        private static GameObject CreateResponsivePanel(Transform parent, string name, Vector2 min, Vector2 max, Color accent) {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max;
            rect.anchoredPosition = Vector2.zero; rect.sizeDelta = Vector2.zero;
            var img = panel.AddComponent<Image>(); img.color = accent;
            return panel;
        }
    }
}
