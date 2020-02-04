using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapMovingController : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 2f;
    private float _cameraHeight;

    private Vector3 _targetPosition;

    private Camera _camera;


    // Start is called before the first frame update
    void Start()
    {
        _targetPosition = new Vector3();
        _camera = GetComponent<Camera>();
        _cameraHeight = transform.position.y;
        _targetPosition = transform.position;
    }

    private void SetTargetPosition(Vector3 target)
    {
        _targetPosition = new Vector3(target.x, _cameraHeight, target.z);
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _moveSpeed);


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) 
                {

                    if (hit.collider.CompareTag("Plane"))
                    {
                                            Debug.Log("aaaa");
                                            SetTargetPosition(hit.point);
                    }
                    
                }
        }



        if (Input.touchCount == 1)
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.touches[0].position);
        
            if (Physics.Raycast(ray, out hit)) 
                {
                    if (hit.collider.CompareTag("Plane"))
                    SetTargetPosition(hit.point);
                }
        }
    }
}
