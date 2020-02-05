using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARRaycastManager))]
public class MakeAppearOnPlane : MonoBehaviour
{
    [field: Tooltip("A transform which should be made to appear to be at the touch point.")]
    [field: SerializeField]
    public Transform Content { get; set; }

    [field: Tooltip("The rotation the content should appear to have.")] [field: SerializeField]
    public Quaternion rotation;

    public Quaternion Rotation
    {
        get => rotation;
        set
        {
            rotation = value;
            if (_sessionOrigin != null)
                _sessionOrigin.MakeContentAppearAt(Content, Content.transform.position, rotation);
        }
    }

    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private ARSessionOrigin _sessionOrigin;
    private ARRaycastManager _raycastManager;
    private ARPlaneManager _planeManager;
    private bool _objectSpawned;

    void Awake()
    {
        _sessionOrigin = GetComponent<ARSessionOrigin>();
        _raycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
        
        Content.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount == 0 || Content == null || _objectSpawned)
            return;

        var touch = Input.GetTouch(0);

        if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = _hits[0].pose;
            //_sessionOrigin.MakeContentAppearAt(Content, hitPose.position, Rotation);
            float minY = float.MaxValue;
            foreach (var plane in _planeManager.trackables)
            {
                if (plane.transform.position.y < minY)
                    minY = plane.transform.position.y;
            }
            
            _sessionOrigin.MakeContentAppearAt(Content, new Vector3(_sessionOrigin.camera.transform.position.x, minY, _sessionOrigin.camera.transform.position.z), Rotation);
            
            GetComponent<ARPlaneManager>().enabled = false;
            GetComponent<ARPointCloudManager>().enabled = false;
            foreach (Transform trackable in GetComponent<ARSessionOrigin>().trackablesParent)
            {
                Destroy(trackable.gameObject);
            }
            
            Content.gameObject.SetActive(true);
            _objectSpawned = true;
        }
    }
}
