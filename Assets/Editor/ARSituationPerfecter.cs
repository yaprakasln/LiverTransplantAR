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
                reg.VitalBox = hud.vitals;
            });
        }

        [MenuItem("Liver AR/2. CREATE MEDICATION SCENE")]
        public static void CreateMedicationScene()
        {
            SetupBaseScene("Assets/Scenes/Scenario2.unity", (canvas, liver) => {
                var med = liver.transform.parent.gameObject.AddComponent<MedicationManager>();
                med.Data = AssetDatabase.LoadAssetAtPath<SimulationState>("Assets/NewSimulationState.asset");
                
                var hud = SetupScenarioHUD(canvas, "MEDICATION IMPORTANCE", "İlacı Al", "SetAdherenceTrue", typeof(MedicationButtonHelper), "İlacı Atla", "SetAdherenceFalse");
                med.MainStatusText = hud.main;
                med.ClinicalBox = hud.c1;
                med.StatusBox = hud.c2;
                med.AdviceBox = hud.c3;
                med.VitalBox = hud.vitals;
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
                    // INCREASED SCALE (from 2.5 to 3.8) for better visibility
                    pivot.transform.localScale = Vector3.one * (3.8f / Mathf.Max(b.size.x, b.size.y, b.size.z));
                    
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
            // --- Premium Header ---
            GameObject titleObj = new GameObject("Title"); titleObj.transform.SetParent(canvas.transform, false);
            var titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "<size=120%><b>LIVER TRANSPLANT</b></size>\n<color=#00ccff><size=80%>DIGITAL AR EXPERIENCE</size></color>";
            titleTxt.alignment = TextAlignmentOptions.Center;
            titleTxt.lineSpacing = -20;
            var tRect = titleObj.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0, 0.75f); tRect.anchorMax = new Vector2(1, 0.95f);
            tRect.anchoredPosition = Vector2.zero; tRect.sizeDelta = Vector2.zero;

            // --- Scenario Grid/List ---
            string[] names = { "🌱 ONARIM SÜRECİ", "💊 İLAÇ ÖNEMİ", "⚠️ ORGAN REDDİ", "🏃 SAĞLIKLI YAŞAM" };
            Color[] colors = { 
                new Color(0.1f, 0.4f, 0.2f, 0.85f), // Greenish
                new Color(0.1f, 0.3f, 0.5f, 0.85f), // Bluish
                new Color(0.5f, 0.1f, 0.1f, 0.85f), // Reddish
                new Color(0.4f, 0.3f, 0.1f, 0.85f)  // Goldish
            };

            for (int i = 0; i < 4; i++) {
                // Button Panel with "Glass" look
                GameObject btnObj = CreateResponsivePanel(canvas.transform, "Btn_" + i, new Vector2(0.15f, 0.55f - i*0.13f), new Vector2(0.85f, 0.65f - i*0.13f), colors[i]);
                
                // Add a subtle border/outline effect
                var outline = btnObj.AddComponent<Outline>();
                outline.effectColor = new Color(1, 1, 1, 0.3f);
                outline.effectDistance = new Vector2(2, -2);

                var txt = CreateTextInPanel(btnObj, "Txt", 42);
                txt.text = "<b>" + names[i] + "</b>"; 
                txt.color = Color.white;
                txt.alignment = TextAlignmentOptions.Center;
                txt.enableWordWrapping = false;

                var b = btnObj.AddComponent<Button>();
                var loader = btnObj.AddComponent<MenuSceneLoader>();
                loader.targetScene = i == 3 ? "Scenario4_Final" : "Scenario" + (i+1);
                UnityEventTools.AddPersistentListener(b.onClick, loader.LoadTarget);

                // Add Hover/Click visual feedback
                var colorsBlock = b.colors;
                colorsBlock.highlightedColor = colors[i] * 1.2f;
                colorsBlock.pressedColor = Color.white;
                b.colors = colorsBlock;
            }

            // Footer
            GameObject footerObj = new GameObject("Footer"); footerObj.transform.SetParent(canvas.transform, false);
            var footerTxt = footerObj.AddComponent<TextMeshProUGUI>();
            footerTxt.text = "<alpha=#66>Eğitim Amaçlı AR Simülasyonu v2.0";
            footerTxt.fontSize = 24; footerTxt.alignment = TextAlignmentOptions.Center;
            var fRect = footerObj.GetComponent<RectTransform>();
            fRect.anchorMin = new Vector2(0, 0.02f); fRect.anchorMax = new Vector2(1, 0.08f);
            fRect.sizeDelta = Vector2.zero; fRect.anchoredPosition = Vector2.zero;
        }

        public struct HUDResult { public TMP_Text main; public TMP_Text c1; public TMP_Text c2; public TMP_Text c3; public TMP_Text vitals; }

        private static HUDResult SetupScenarioHUD(GameObject canvas, string title, string b1Name, string b1Method, System.Type helperType, string b2Name = "", string b2Method = "")
        {
            HUDResult res = new HUDResult();

            // HUD Title (Offset to the right to avoid MENU button)
            GameObject titleObj = new GameObject("HUDTitle"); titleObj.transform.SetParent(canvas.transform, false);
            var titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "<b>" + title + "</b>"; titleTxt.fontSize = 50; titleTxt.alignment = TextAlignmentOptions.Center;
            var tRect = titleObj.GetComponent<RectTransform>();
            // True center (0 to 1) but ensuring text doesn't overlap the menu button at the far left
            tRect.anchorMin = new Vector2(0.2f, 0.9f); tRect.anchorMax = new Vector2(0.8f, 0.98f);
            tRect.sizeDelta = Vector2.zero; tRect.anchoredPosition = Vector2.zero;

            // --- Holographic Panels Setup ---
            Color glassBlue = new Color(0, 0.15f, 0.3f, 0.7f);
            Color neonCyan = new Color(0, 0.8f, 1f, 0.6f);

            // Main Narrative Panel
            GameObject narrPanel = CreateResponsivePanel(canvas.transform, "MainNarrative", new Vector2(0.1f, 0.76f), new Vector2(0.9f, 0.88f), glassBlue);
            AddHUDDecoration(narrPanel, neonCyan);
            res.main = CreateTextInPanel(narrPanel, "MainText", 36);

            // Callout 1 (Left)
            GameObject c1Panel = CreateResponsivePanel(canvas.transform, "Box_Cellular", new Vector2(0.02f, 0.45f), new Vector2(0.32f, 0.65f), glassBlue);
            AddHUDDecoration(c1Panel, neonCyan);
            AddSectionTitle(c1Panel, "HÜCRESEL ANALİZ");
            res.c1 = CreateTextInPanel(c1Panel, "Text", 26);

            // Callout 2 (Right)
            GameObject c2Panel = CreateResponsivePanel(canvas.transform, "Box_Volume", new Vector2(0.68f, 0.45f), new Vector2(0.98f, 0.65f), glassBlue);
            AddHUDDecoration(c2Panel, neonCyan);
            AddSectionTitle(c2Panel, "HACİMSEL TAKİP");
            res.c2 = CreateTextInPanel(c2Panel, "Text", 26);

            // Callout 3 (Bottom)
            GameObject c3Panel = CreateResponsivePanel(canvas.transform, "Box_Functional", new Vector2(0.1f, 0.18f), new Vector2(0.9f, 0.32f), glassBlue);
            AddHUDDecoration(c3Panel, neonCyan);
            AddSectionTitle(c3Panel, "FONKSİYONEL ANALİZ & TIBBİ NOTLAR");
            res.c3 = CreateTextInPanel(c3Panel, "Text", 28);

            // Vital Signs (Top Right)
            GameObject vitalsPanel = CreateResponsivePanel(canvas.transform, "Box_Vitals", new Vector2(0.72f, 0.68f), new Vector2(0.98f, 0.85f), new Color(0, 0.05f, 0.1f, 0.85f));
            AddHUDDecoration(vitalsPanel, new Color(1, 0.7f, 0, 0.6f));
            AddSectionTitle(vitalsPanel, "VİTAL & LABORATUVAR");
            res.vitals = CreateTextInPanel(vitalsPanel, "Text", 24);
            res.vitals.alignment = TextAlignmentOptions.TopLeft;

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

        private static void AddHUDDecoration(GameObject panel, Color glowColor)
        {
            // 1. Glow Border
            var outline = panel.AddComponent<Outline>();
            outline.effectColor = glowColor;
            outline.effectDistance = new Vector2(2, -2);
            
            // 2. Corner Brackets (The "Fancy" part)
            string[] anchors = { "TL", "TR", "BL", "BR" };
            Vector2[] min = { new Vector2(0,0.9f), new Vector2(0.9f,0.9f), new Vector2(0,0), new Vector2(0.9f,0) };
            Vector2[] max = { new Vector2(0.1f,1), new Vector2(1,1), new Vector2(0.1f,0.1f), new Vector2(1,0.1f) };

            foreach(var a in anchors) {
                var bracket = CreateResponsivePanel(panel.transform, "Bracket_"+a, new Vector2(0,0), new Vector2(1,1), new Color(0,0,0,0));
                var bRect = bracket.GetComponent<RectTransform>();
                // Adjusting brackets to corners
                if(a=="TL") { bRect.anchorMin=new Vector2(0,0.85f); bRect.anchorMax=new Vector2(0.05f,1); }
                if(a=="TR") { bRect.anchorMin=new Vector2(0.95f,0.85f); bRect.anchorMax=new Vector2(1,1); }
                if(a=="BL") { bRect.anchorMin=new Vector2(0,0); bRect.anchorMax=new Vector2(0.05f,0.15f); }
                if(a=="BR") { bRect.anchorMin=new Vector2(0.95f,0); bRect.anchorMax=new Vector2(1,0.15f); }
                
                var img = bracket.GetComponent<Image>();
                img.color = glowColor;
            }

            // 3. Subtle "Scanning" text
            GameObject scanObj = new GameObject("ScanLabel");
            scanObj.transform.SetParent(panel.transform, false);
            var scanTxt = scanObj.AddComponent<TextMeshProUGUI>();
            scanTxt.text = "<alpha=#44>LIVE DATA FEED // " + panel.name.ToUpper();
            scanTxt.fontSize = 14; scanTxt.alignment = TextAlignmentOptions.BottomRight;
            var sRect = scanObj.GetComponent<RectTransform>();
            sRect.anchorMin = new Vector2(0.5f, 0); sRect.anchorMax = new Vector2(0.98f, 0.15f);
            sRect.sizeDelta = Vector2.zero; sRect.anchoredPosition = Vector2.zero;
        }

        private static void AddGlowBorder(GameObject panel, Color glowColor)
        {
            var outline = panel.AddComponent<Outline>();
            outline.effectColor = glowColor;
            outline.effectDistance = new Vector2(3, -3);
            
            // Add a second shadow for extra glow
            var shadow = panel.AddComponent<Shadow>();
            shadow.effectColor = new Color(glowColor.r, glowColor.g, glowColor.b, 0.2f);
            shadow.effectDistance = new Vector2(-4, 4);
        }

        private static void AddSectionTitle(GameObject panel, string title)
        {
            GameObject titleObj = new GameObject("SectionTitle");
            titleObj.transform.SetParent(panel.transform, false);
            var txt = titleObj.AddComponent<TextMeshProUGUI>();
            txt.text = "<size=70%>" + title + "</size>";
            txt.color = new Color(0, 1, 1, 0.8f);
            txt.alignment = TextAlignmentOptions.TopLeft;
            var rect = txt.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, 0.75f); rect.anchorMax = new Vector2(0.95f, 0.95f);
            rect.sizeDelta = Vector2.zero; rect.anchoredPosition = Vector2.zero;
        }

        private static TMP_Text CreateTextInPanel(GameObject panel, string name, int fontSize)
        {
            var txtObj = new GameObject(name).AddComponent<TextMeshProUGUI>();
            txtObj.transform.SetParent(panel.transform, false);
            txtObj.fontSize = fontSize; txtObj.alignment = TextAlignmentOptions.Center;
            txtObj.text = "...";
            var rect = txtObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = new Vector2(1, 0.85f); // Leave room for title
            rect.sizeDelta = new Vector2(-20, -20);
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
