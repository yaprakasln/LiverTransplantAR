using UnityEngine;

namespace LiverTransplantAR.AR
{
    public class FixedARPlacement : MonoBehaviour
    {
        public GameObject ContentToPlace;
        public float DistanceInFront = 1.0f;
        public float VerticalOffset = -0.3f;

        void Start()
        {
            if (ContentToPlace != null)
            {
                ContentToPlace.SetActive(true);
                
                // Position it exactly in front of the starting camera view
                // Uses the configurable DistanceInFront and VerticalOffset
                Vector3 spawnPos = new Vector3(0, VerticalOffset, DistanceInFront); 
                ContentToPlace.transform.position = spawnPos;
                ContentToPlace.transform.rotation = Quaternion.Euler(0, 180, 0);

                // Ensure it's on a visible layer
                ContentToPlace.layer = 0; // Default
                foreach(Transform child in ContentToPlace.transform) child.gameObject.layer = 0;
            }
        }
    }
}
