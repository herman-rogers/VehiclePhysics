using UnityEngine;
using System.Collections;
public class VehiclePhysics : MonoBehaviour
{
    public WheelCollider frontRightWheel;
    public WheelCollider frontLeftWheel;
    public WheelCollider rearRightWheel;
    public WheelCollider rearLeftWheel;
    private GearsComponent gearsComponent;
    private EngineComponent engineComponent;
    private float steer;
    private float motor;
    private float brake;
    public const int numberOfGears = 6;
    public const float speedCap = 180;

    void Start( )
    {
        gearsComponent = new GearsComponent( 6, 180 );
        engineComponent = GetComponent< EngineComponent >( );
    }
    void Update ( )
    {
        Vector3 vehicleVelocity = transform.InverseTransformDirection( rigidbody.velocity );
        gearsComponent.UpdateGears( vehicleVelocity );
        engineComponent.EngineUpdate( );
        //rearRightWheel.motorTorque = motor * 30;
        //rearLeftWheel.motorTorque = motor * 30;
        //rearRightWheel.brakeTorque = brake * 30;
        //rearLeftWheel.brakeTorque = brake * 30;
        //frontRightWheel.steerAngle = steer * 30;
        //frontLeftWheel.steerAngle = steer * 30;
    }

    void FixedUpdate ( )
    {
        Vector3 vehicleVelocity = transform.InverseTransformDirection( rigidbody.velocity );
        engineComponent.EngineFixedUpdate( vehicleVelocity, gearsComponent );
    }
}