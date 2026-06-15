using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace LiverTransplantAR.AR
{
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(ARPlaneManager))]
    public class ARPlacementManager : MonoBehaviour
    {
        [Header("Placement Settings")]
        public GameObject PlacementIndicator;
        public GameObject ContentToPlace;
        
        private ARRaycastManager _raycastManager;
        private ARPlaneManager _planeManager;
        private Pose _placementPose;
        private bool _placementPoseIsValid = false;
        private bool _isContentPlaced = false;

        void Awake()
        {
            _raycastManager = GetComponent<ARRaycastManager>();
            _planeManager = GetComponent<ARPlaneManager>();
            
            if (ContentToPlace != null)
                ContentToPlace.SetActive(false);
        }

        void Update()
        {
            if (!_isContentPlaced)
            {
                UpdatePlacementPose();
                UpdatePlacementIndicator();

                if (_placementPoseIsValid && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0)))
                {
                    PlaceContent();
                }
            }
        }

        private void UpdatePlacementPose()
        {
            var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            var hits = new List<ARRaycastHit>();
            _raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

            _placementPoseIsValid = hits.Count > 0;
            if (_placementPoseIsValid)
            {
                _placementPose = hits[0].pose;

                var cameraForward = Camera.main.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }

        private void UpdatePlacementIndicator()
        {
            if (PlacementIndicator != null)
            {
                if (_placementPoseIsValid)
                {
                    PlacementIndicator.SetActive(true);
                    PlacementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
                }
                else
                {
                    PlacementIndicator.SetActive(false);
                }
            }
        }

        private void PlaceContent()
        {
            if (ContentToPlace == null) return;

            ContentToPlace.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
            ContentToPlace.SetActive(true);
            _isContentPlaced = true;

            // Hide indicator and stop showing planes for better immersion
            if (PlacementIndicator != null) PlacementIndicator.SetActive(false);
            
            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            _planeManager.enabled = false;
        }

        public void ResetPlacement()
        {
            _isContentPlaced = false;
            if (ContentToPlace != null) ContentToPlace.SetActive(false);
            _planeManager.enabled = true;
            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }
        }
    }
}
