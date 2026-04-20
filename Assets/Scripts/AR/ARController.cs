using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace LiverTransplantAR.AR
{
    public class ARController : MonoBehaviour
    {
        [Header("AR References")]
        public ARSession ARSession;
        public GameObject XROrigin;
        public GameObject LiverModel;

        [Header("Settings")]
        public float DistanceInFront = 1.5f; // Initial distance from camera (meters)
        public float HoverHeight = 0.0f;     // Height adjustment

        private void Start()
        {
            if (LiverModel != null)
            {
                // Position the liver in front of the camera's initial position
                Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * DistanceInFront;
                spawnPos.y += HoverHeight;
                LiverModel.transform.position = spawnPos;
                
                // Make it look at the user initially
                LiverModel.transform.LookAt(Camera.main.transform);
                LiverModel.transform.Rotate(0, 180, 0); // Correct orientation
            }
        }
    }
}
