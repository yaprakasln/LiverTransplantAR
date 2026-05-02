using UnityEngine;
using UnityEngine.UI;
using LiverTransplantAR.Scenarios;

namespace LiverTransplantAR.UI
{
    public class MedicationButtonHelper : MonoBehaviour
    {
        public string methodName;
        private Button _btn;

        void Start()
        {
            _btn = GetComponent<Button>();
            if (_btn == null) return;

            _btn.onClick.AddListener(() => {
                Debug.Log($"<color=cyan>UI: Medication Button {methodName} clicked!</color>");
                MedicationManager mm = GameObject.FindObjectOfType<MedicationManager>();
                if (mm != null) {
                    mm.Invoke(methodName, 0);
                } else {
                    Debug.LogError("UI Error: MedicationManager not found in scene!");
                }
            });
        }
    }
}
