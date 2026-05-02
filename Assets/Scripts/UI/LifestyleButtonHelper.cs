using UnityEngine;
using UnityEngine.UI;
using LiverTransplantAR.Scenarios;

namespace LiverTransplantAR.UI
{
    public class LifestyleButtonHelper : MonoBehaviour
    {
        public string methodName;
        private Button _btn;

        void Start()
        {
            _btn = GetComponent<Button>();
            if (_btn == null) return;

            _btn.onClick.AddListener(() => {
                Debug.Log($"<color=orange>UI: Button {methodName} clicked!</color>");
                LifestyleManager lm = GameObject.FindObjectOfType<LifestyleManager>();
                if (lm != null) {
                    lm.Invoke(methodName, 0);
                } else {
                    Debug.LogError("UI Error: LifestyleManager not found in scene!");
                }
            });
        }
    }
}
