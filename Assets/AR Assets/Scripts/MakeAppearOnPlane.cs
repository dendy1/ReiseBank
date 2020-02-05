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

    [field: Tooltip("The rotation the content should appear to have.")]
    [field: SerializeField]
    public Quaternion Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            if (_sessionOrigin != null)
                _sessionOrigin.MakeContentAppearAt(Content, Content.transform.position, _rotation);
        }
    }

    private Quaternion _rotation;
    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    private ARSessionOrigin _sessionOrigin;
    private ARRaycastManager _raycastManager;
    private bool _objectSpawned;

    void Awake()
    {
        _sessionOrigin = GetComponent<ARSessionOrigin>();
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0 || Content == null || _objectSpawned)
            return;

        var touch = Input.GetTouch(0);

        if (_raycastManager.Raycast(touch.position, _hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = _hits[0].pose;
            _sessionOrigin.MakeContentAppearAt(Content, hitPose.position, Rotation);
            
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
