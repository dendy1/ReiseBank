using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
//using UnityEngine.Rendering.PostProcessing;

public class GyroCameraController : MonoBehaviour
{

    [SerializeField] private Text _rotationText;

    private Vector3 _currentRotation;

    private float yRotation;
    private float xRotation;
    private float zRotation;

    [SerializeField] private float _speed;

    [SerializeField] private List<Transform> _waypoints;

    private Queue<Vector3> _waypointPositions;

    private Vector3 _targetPosition;


    public void SwitchWaypoint()
    {
        _targetPosition = _waypointPositions.Dequeue();
        _waypointPositions.Enqueue(_targetPosition);
    }

    // Start is called before the first frame update
    void Awake()
    {
        _rotations = new Queue<Vector3>();
        _targetRotation = new Vector3();
        _waypointPositions = new Queue<Vector3>();
        foreach (var item in _waypoints)
        {
            _waypointPositions.Enqueue(item.position);
        }
        _targetPosition = transform.position;
        yRotation = 0;
        xRotation = 0;
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        Input.compensateSensors = true;

        _currentRotation = Input.gyro.attitude.eulerAngles;

        Application.targetFrameRate = 60;

        Input.gyro.updateInterval = 0.016f;
        StartCoroutine(UpdateRotations());
            }

    void GyroModifyCamera()
    {
        transform.rotation = GyroToUnity(Input.gyro.attitude);
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }


    private Vector3 _accel;
    public float filterSpeed = 1;



    float xValue;
    float xValueMinMax = 5.0f;

    float yValue;
    float yValueMinMax = 5.0f;

    float cameraSpeed = 1.0f;// Greater the lower speed
    Vector3 accelometerSmoothValue;

    void cameraRotationAccelerometer()
    {
        /*
        //Set X Min Max
        if (xValue < -xValueMinMax)
            xValue = -xValueMinMax;

        if (xValue > xValueMinMax)
            xValue = xValueMinMax;

        //Set Y Min Max
        if (yValue < -yValueMinMax)
            yValue = -yValueMinMax;

        if (yValue > yValueMinMax)
            yValue = yValueMinMax;

        */
        accelometerSmoothValue = lowpass();

        xValue += accelometerSmoothValue.x;
        yValue += accelometerSmoothValue.y;

        transform.rotation = new Quaternion(yValue, xValue, 0, cameraSpeed);
    }

    public float AccelerometerUpdateInterval = 1.0f / 100.0f;
    public float LowPassKernelWidthInSeconds = 0.001f;
    public Vector3 lowPassValue = Vector3.zero;
    Vector3 lowpass()
    {
        float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;//tweakable
        lowPassValue = Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
        return lowPassValue;
    }


    private Vector3 CalculateCurrentMedieval(List<Vector3> rotations)
    {
        var summary = Vector3.zero;
        foreach (var item in rotations)
        {
            summary += item;
        }
        return summary / rotations.Count;
    }

    private Queue<Vector3> _rotations;

    private Vector3 _targetRotation;

    IEnumerator UpdateRotations()
    {
        yield return new WaitForSeconds(1 / 60f);
        _targetRotation = CalculateCurrentMedieval(_rotations.ToList<Vector3>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // _rotations.Enqueue(Input.gyro.attitude.eulerAngles);


        // Camera.main.transform.rotation = Quaternion.Euler(Vector3.Lerp(Camera.main.transform.rotation.eulerAngles, _targetRotation, Time.deltaTime));

        /*

        _accel = Vector3.Lerp(_accel, Input.acceleration, filterSpeed * Time.deltaTime);
        yRotation += -Input.gyro.rotationRateUnbiased.y * 1.1f;
        Vector3 tempAccel = _accel;
        tempAccel.Scale(new Vector3(-1, 1, 1));
       Camera.main.transform.rotation = Quaternion.FromToRotation(Vector3.down, tempAccel);
        Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, yRotation + 180f, Camera.main.transform.eulerAngles.z);
        */

        //Camera.main.transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, Input.gyro.rotationRateUnbiased.z);

        //cameraRotationAccelerometer();

        /*
        Vector3 tempAccel = _accel;
        tempAccel.Scale(new Vector3(-1, 1, 1));
        Camera.main.transform.rotation = Quaternion.Euler(tempAccel);

        */

        //Camera.main.transform.rotation = Quaternion.Euler(new Vector3(-_accel.x, _accel.z, _accel.y) * 180f / Mathf.PI);

        yRotation += -Input.gyro.rotationRateUnbiased.y * 1.1f;

        xRotation += -Input.gyro.rotationRateUnbiased.x * 1.1f;

        zRotation += Input.gyro.rotationRateUnbiased.z;
        

        transform.eulerAngles = new Vector3(xRotation, yRotation + 180f, 0);
        


        /*
        
        var orientation = Quaternion.Euler(_accel);


        Quaternion rot = Quaternion.Inverse(new Quaternion(orientation.x, orientation.y, -orientation.z, orientation.w));
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f)) * rot;
        */



        //GyroModifyCamera();


        Debug.Log(Input.gyro.attitude.eulerAngles);

        //transform.rotation = Quaternion.Euler(Input.gyro.attitude.eulerAngles.x, -Input.gyro.attitude.eulerAngles.z, Input.gyro.attitude.eulerAngles.y);

        //transform.rotation = Input.gyro.attitude * new Quaternion(0, 0, 1, 0);
        //transform.eulerAngles = new Vector3(-Input.gyro.attitude.ToEulerAngles().x, -Input.gyro.attitude.ToEulerAngles().y, Input.gyro.attitude.ToEulerAngles().z);

        //transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _speed);

    }
}
