using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ARTapToPlace : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject stadiumPrefab;

    public GameObject spawnedStadium;

    private ARRaycastManager raycastManager;
    private ARAnchorManager anchorManager;
    private ARPlaneManager planeManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Vector3 dragOffset;
    private float initialPinchDistance;
    private Vector3 baseSpawnScale;
    private Vector3 scaleAtPinchStart;

    private ARAnchor currentAnchor;

    void Awake()
    {
        Application.targetFrameRate = 30;
        raycastManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        var primaryTouch = Touchscreen.current.primaryTouch;
        bool isPrimaryPressed = primaryTouch.press.isPressed;
        bool isSecondTouchActive = Touchscreen.current.touches.Count > 1 && Touchscreen.current.touches[1].press.isPressed;

        if (isPrimaryPressed && !isSecondTouchActive)
        {
            Vector2 touchPosition = primaryTouch.position.ReadValue();

            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                ARRaycastHit hit = hits[0];
                Pose hitPose = hit.pose;

                if (spawnedStadium == null)
                {
                    if (primaryTouch.press.wasPressedThisFrame)
                    {
                        Quaternion finalRotation = hitPose.rotation * stadiumPrefab.transform.rotation;
                        spawnedStadium = Instantiate(stadiumPrefab, hitPose.position, finalRotation);
                        baseSpawnScale = spawnedStadium.transform.localScale;
                        dragOffset = Vector3.zero;

                        AttachAnchorAtPose(hitPose, hit.trackableId);
                        
                        // Lock detection to this specific plane and hide the rest
                        IsolateActivePlane(hit.trackableId);
                    }
                }
            }
        }
    }

    private void DetachAnchor()
    {
        if (currentAnchor != null)
        {
            spawnedStadium.transform.SetParent(null, true);
            Destroy(currentAnchor.gameObject);
            currentAnchor = null;
        }
    }

    private void AttachAnchorAtPose(Pose pose, TrackableId planeId)
    {
        DetachAnchor();

        ARPlane plane = planeManager.GetPlane(planeId);
        if (plane != null && anchorManager != null)
        {
            currentAnchor = anchorManager.AttachAnchor(plane, pose);
        }

        if (currentAnchor == null)
        {
            GameObject anchorGO = new GameObject("AR_Stadium_Anchor");
            anchorGO.transform.SetPositionAndRotation(pose.position, pose.rotation);
            currentAnchor = anchorGO.AddComponent<ARAnchor>();
        }

        if (currentAnchor != null)
        {
            spawnedStadium.transform.SetParent(currentAnchor.transform, true);
        }
    }

    private void IsolateActivePlane(TrackableId activePlaneId)
    {
        // Stop scanning for new planes to save performance and freeze the environment
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;

        // Hide the visual grids of all other planes
        foreach (var plane in planeManager.trackables)
        {
            if (plane.trackableId != activePlaneId)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
}