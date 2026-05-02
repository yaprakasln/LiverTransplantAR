using UnityEngine;
using UnityEngine.UI;
using LiverTransplantAR.Scenarios;

namespace LiverTransplantAR.UI
{
    public class RecoveryButtonHelper : MonoBehaviour
    {
        public string methodName;
        private Button _btn;

        void Start()
        {
            _btn = GetComponent<Button>();
            if (_btn == null) return;

            _btn.onClick.AddListener(() => {
                Debug.Log($"<color=green>UI: Recovery Button {methodName} clicked!</color>");
                LiverRegeneration lr = GameObject.FindObjectOfType<LiverRegeneration>();
                if (lr != null) {
                    lr.Invoke(methodName, 0);
                } else {
                    Debug.LogError("UI Error: LiverRegeneration not found in scene!");
                }
            });
        }
    }
}
