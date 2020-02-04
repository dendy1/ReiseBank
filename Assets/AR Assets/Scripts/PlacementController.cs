using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementController : MonoBehaviour
{
    #region PUBLIC FIELDS
    
    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject PlacedPrefab
    {
        get { return placedPrefab; }
        set { placedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject SpawnedPrefab { get; set; }
    
    /// <summary>
    /// Invoked whenever an object is placed in on a plane.
    /// </summary>
    public static event Action OnPlacedObject;

    #endregion

    #region PRIVATE FIELDS

    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    [SerializeField] private GameObject placedPrefab;
    
    private ARRaycastManager _raycastManager;
    private static List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();
    
    #endregion
    
    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif

        touchPosition = default;
        return false;
    }

    private void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (_raycastManager.Raycast(touchPosition, _raycastHits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = _raycastHits[0].pose;

            if (SpawnedPrefab == null && PlacedPrefab != null)
            {
                SpawnedPrefab = Instantiate(PlacedPrefab, hitPose.position, hitPose.rotation);

                GetComponent<ARPlaneManager>().enabled = false;
                GetComponent<ARPointCloudManager>().enabled = false;

                foreach (Transform trackable in GetComponent<ARSessionOrigin>().trackablesParent)
                {
                    Destroy(trackable.gameObject);
                }
            }
            else
            {
                //spawnedObject.transform.position = hitPose.position;
            }
        }
    }
}
