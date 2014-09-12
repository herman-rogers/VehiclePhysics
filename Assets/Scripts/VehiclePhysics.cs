using UnityEngine;
using System.Collections;

public class VehiclePhysics : MonoBehaviour
{
    public WheelCollider frontRightWheel;
    public WheelCollider frontLeftWheel;
    public WheelCollider rearRightWheel;
    public WheelCollider rearLeftWheel;
    private float steer;
    private float motor;
    private float brake;

     void Update ( )
    {
        steer = Mathf.Clamp( Input.GetAxis( "Horizontal" ), -1, 1 );
        motor = Mathf.Clamp( Input.GetAxis( "Acceleration" ), 0, 1 );
        brake = -1 * Mathf.Clamp(  Input.GetAxis( "Brake" ), -1, 0 );

        rearRightWheel.motorTorque = motor * 30;
        rearLeftWheel.motorTorque = motor * 30;

        rearRightWheel.brakeTorque = brake * 30;
        rearLeftWheel.brakeTorque = brake * 30;

        frontRightWheel.steerAngle = steer * 30;
        frontLeftWheel.steerAngle = steer * 30;
    }
}