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
        [MenuItem("Liver AR/CREATE MAIN MENU")]
        public static void CreateMainMenu()
        {
            if (EditorApplication.isPlaying) EditorApplication.isPlaying = false;

            // 1. ALL SCENES BUILD SYNC
            var buildScenes = EditorBuildSettings.scenes.ToList();
            string[] scenePaths = { 
                "Assets/Scenes/MainLauncher.unity", 
                "Assets/Scenes/Scenario1.unity",
                "Assets/Scenes/Scenario2.unity",
                "Assets/Scenes/Scenario3.unity",
                "Assets/Scenes/Scenario4_Final.unity" 
            };
            
            bool changed = false;
            foreach (var p in scenePaths) {
                if (File.Exists(p) && !buildScenes.Any(s => s.path == p)) {
                    buildScenes.Add(new EditorBuildSettingsScene(p, true));
                    changed = true;
                }
            }
            if (changed) EditorBuildSettings.scenes = buildScenes.ToArray();

            AssetDatabase.Refresh();
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);
            
            float screenW = Screen.width; float screenH = Screen.height;
            bool isPortrait = screenH > screenW;

            // 2. Camera
            Camera mainCam = Camera.main;
            if (mainCam != null) {
                mainCam.transform.position = new Vector3(0, 0, isPortrait ? -7.5f : -6.5f);
                mainCam.transform.LookAt(Vector3.zero);
                mainCam.backgroundColor = Color.black; mainCam.clearFlags = CameraClearFlags.SolidColor;
            }

            // 2b. Background
            GameObject bgPlate = GameObject.CreatePrimitive(PrimitiveType.Quad);
            bgPlate.name = "Background_Plate";
            bgPlate.transform.position = new Vector3(0, 0, 10); 
            bgPlate.transform.localScale = new Vector3(40, 80, 1); 
            GameObject.DestroyImmediate(bgPlate.GetComponent<MeshCollider>());
            Material bgMat = new Material(Shader.Find("Unlit/Texture"));
            string imgPath = "Assets/MedicalBackground.png";
            if (File.Exists(imgPath)) {
                byte[] d = File.ReadAllBytes(imgPath); Texture2D t = new Texture2D(2, 2); t.LoadImage(d);
                bgMat.mainTexture = t;
            }
            bgPlate.GetComponent<Renderer>().material = bgMat;

            // 3. Liver (Responsive & Centered)
            string liverPath = "Assets/human-liver-and-gallbladder/source/Liver project - Copy/liver exported for sketchfab - now with the fucking base colours included.fbx";
            GameObject liverPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(liverPath);
            if (liverPrefab != null) {
                GameObject liver = (GameObject)PrefabUtility.InstantiatePrefab(liverPrefab);
                GameObject pivot = new GameObject("Menu_Liver_Pivot");
                pivot.transform.position = new Vector3(0, 0.4f, 0); liver.transform.SetParent(pivot.transform);
                Renderer[] rs = liver.GetComponentsInChildren<Renderer>();
                if (rs.Length > 0) {
                    Bounds b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
                    liver.transform.localPosition = -b.center;
                    float s = (isPortrait ? 2.1f : 2.8f) / Mathf.Max(b.size.x, b.size.y, b.size.z);
                    pivot.transform.localScale = Vector3.one * s;
                    Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    mat.color = new Color(0.95f, 0.1f, 0.1f);
                    mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor", new Color(0.4f, 0.05f, 0.05f));
                    foreach (var r in rs) r.material = mat;
                    ARVisualsController vc = pivot.AddComponent<ARVisualsController>();
                    vc.RotationTarget = pivot.transform; vc.LiverRenderer = rs[0];
                    vc.EnableAutoRotation = true; vc.RotationSpeed = 30f;
                }
            }

            // 4. UI Root
            GameObject canvasObj = new GameObject("MainMenu_Canvas");
            Canvas c = canvasObj.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            System.Type inputType = null;
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies()) {
                inputType = a.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule");
                if (inputType != null) break;
            }
            if (inputType != null) esObj.AddComponent(inputType);
            else esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // 5. Header (Responsive Navy)
            GameObject titleObj = new GameObject("Title"); titleObj.transform.SetParent(canvasObj.transform, false);
            var titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "<b>LIVER TRANSPLANT</b>\n<color=#005577>AR EXPERIENCE</color>";
            titleTxt.alignment = TextAlignmentOptions.Center; titleTxt.fontSize = isPortrait ? 75 : 95;
            titleTxt.color = new Color(0.0f, 0.15f, 0.45f, 1f); titleTxt.outlineWidth = 0.12f; titleTxt.outlineColor = Color.white;
            var tRect = titleObj.GetComponent<RectTransform>();
            tRect.anchorMin = new Vector2(0, 0.85f); tRect.anchorMax = new Vector2(1, 0.95f);
            tRect.anchoredPosition = Vector2.zero; tRect.sizeDelta = Vector2.zero;

            // 6. Buttons (Grid Bottom)
            string[] trNames = { "ANATOMİ VE YAPI", "NAKİL SÜRECİ", "İYİLEŞME SÜRECİ", "YAŞAM VE SAĞLIK" };
            string[] enNames = { "Anatomy & Structure", "Surgery", "Recovery", "Lifestyle" };
            Color teal = new Color(0.0f, 0.75f, 0.85f, 1f);

            for (int i = 0; i < 4; i++) {
                int row = i / 2; int col = i % 2;
                float xMin = (col == 0) ? 0.05f : 0.52f; float xMax = (col == 0) ? 0.48f : 0.95f;
                float yMin = (row == 0) ? 0.16f : 0.04f; float yMax = (row == 0) ? 0.26f : 0.14f;

                GameObject panel = CreateResponsivePanel(canvasObj.transform, "Btn_" + i, new Vector2(xMin, yMin), new Vector2(xMax, yMax), teal);
                var label = new GameObject("Label").AddComponent<TextMeshProUGUI>();
                label.transform.SetParent(panel.transform, false);
                label.text = $"<b>{trNames[i]}</b>\n<size=20><color=#003366>{enNames[i]}</color></size>";
                label.alignment = TextAlignmentOptions.Center; label.fontSize = 42;
                label.color = new Color(0.0f, 0.15f, 0.45f, 1f);
                var lRect = label.GetComponent<RectTransform>();
                lRect.anchorMin = Vector2.zero; lRect.anchorMax = Vector2.one; lRect.sizeDelta = Vector2.zero;

                Button btn = panel.AddComponent<Button>();
                btn.transition = Selectable.Transition.ColorTint;
                var colors = btn.colors;
                colors.normalColor = Color.white; colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                btn.colors = colors;

                var loader = panel.AddComponent<MenuSceneLoader>();
                int sIdx = i + 1;
                loader.targetScene = (sIdx == 4) ? "Scenario4_Final" : "Scenario" + sIdx;
                UnityEventTools.AddPersistentListener(btn.onClick, loader.LoadTarget);
            }

            string scenePath = "Assets/Scenes/MainLauncher.unity";
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("<color=green>COMPLETE MULTI-SCENE DASHBOARD READY!</color>");
        }

        private static GameObject CreateResponsivePanel(Transform parent, string name, Vector2 min, Vector2 max, Color accent) {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max;
            rect.anchoredPosition = Vector2.zero; rect.sizeDelta = Vector2.zero;
            var img = panel.AddComponent<Image>(); img.color = accent; img.raycastTarget = true;
            GameObject border = new GameObject("Border");
            border.transform.SetParent(panel.transform, false);
            var bRect = border.AddComponent<RectTransform>();
            bRect.anchorMin = Vector2.zero; bRect.anchorMax = Vector2.one;
            bRect.sizeDelta = new Vector2(-6, -6);
            var bImg = border.AddComponent<Image>(); bImg.color = new Color(1, 1, 1, 0.2f);
            return panel;
        }
    }
}
