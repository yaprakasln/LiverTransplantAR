using UnityEngine;
using UnityEngine.UI;
using LiverTransplantAR.Scenarios;

namespace LiverTransplantAR.UI
{
    /// <summary>
    /// UI buton yardımcısı — RejectionManager üzerindeki metotları çağırır.
    /// Kullanım: methodName = "AdvanceStage" veya "ResetRejection"
    /// </summary>
    public class RejectionButtonHelper : MonoBehaviour
    {
        public string methodName;
        private Button _btn;

        void Start()
        {
            _btn = GetComponent<Button>();
            if (_btn == null) return;

            _btn.onClick.AddListener(() => {
                Debug.Log($"<color=red>UI: Rejection Button {methodName} clicked!</color>");
                RejectionManager rm = GameObject.FindObjectOfType<RejectionManager>();
                if (rm != null) {
                    rm.Invoke(methodName, 0);
                } else {
                    Debug.LogError("UI Error: RejectionManager not found in scene!");
                }
            });
        }
    }
}
