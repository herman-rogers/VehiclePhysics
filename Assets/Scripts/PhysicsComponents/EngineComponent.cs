using UnityEngine;
using System.Collections;

public class EngineComponent : MonoBehaviour
{
    private float vehicleThrottle;
    private float horsePower;
    private float steer;
    private float motor;
    private float brake;
    private float minimumTurn = 5;
    private float maximumTurn = 7;
    private GearsComponent gearComponent;
    private WheelComponent wheelComponent;
    private VehiclePhysics vehiclePhysics;
    private static double engineSpeed;
    private const float horsePowerMultiplier = 50;

    public void Start ( )
    {
        vehiclePhysics = GetComponent<VehiclePhysics>( );
        gearComponent = vehiclePhysics.gearsComponent;
        wheelComponent = vehiclePhysics.wheelComponent;
    }

    public void StartEngine ( )
    { 
    
    }

    public void EngineUpdate ( )
    {
        UpdateInput( );
    }

    public void EngineFixedUpdate ( Vector3 vehicleVelocity )
    {
        HorsePower( vehicleVelocity );
        ApplyThrottle( vehicleVelocity );
        ApplySteering( vehicleVelocity );
    }

    //TODO: Move this into an Input Component Script
    private void UpdateInput ( )
    {
        steer = Input.GetAxis( "Horizontal" );
        vehicleThrottle = Input.GetAxis( "Acceleration" ) * 30;
        brake = Input.GetAxis( "Brake" );
    }

    public float SteeringSpeed ( )
    {
        return steer * maximumTurn;
    }

    public float GetVehicleThrottle ( )
    {
        return vehicleThrottle;
    }

    public static double GetEngineSpeed ( )
    {
        return engineSpeed;
    }

    private void HorsePower ( Vector3 velocity )
    {
        float vehicleGear = gearComponent.currentGear;
        if ( vehicleThrottle == 0 )
        {
            horsePower -= Time.deltaTime * horsePowerMultiplier;
        }
        else if ( SameSign( velocity.z, vehicleThrottle ) )
        {
            horsePower += Time.deltaTime 
                          * horsePowerMultiplier 
                          * gearComponent.GetNormalizedPower( horsePower );
        }
        else
        {
            horsePower += Time.deltaTime * horsePowerMultiplier;
        }
        if ( vehicleGear == 0 )
        {
            horsePower = Mathf.Clamp( horsePower, 0, gearComponent.GetGearValue( 0 ) );
        }
        else
        {
            horsePower = Mathf.Clamp( horsePower,
                                      gearComponent.GetGearValue( vehicleGear - 1 ),
                                      gearComponent.GetGearValue( vehicleGear ) );
        }
    }

    private void ApplyThrottle ( Vector3 velocity )
    {
        float throttleForce = 0.0f;
        float brakeForce = 0.0f;
        if ( WheelComponent.wheelsGrounded == WheelsOnGround.REAR_WHEELS_IN_AIR )
        {
            return;
        }
        if ( vehicleThrottle > 0.0f )
        {
            throttleForce = Mathf.Sign( vehicleThrottle ) * horsePower * rigidbody.mass;
        }
        if ( brake > 0.0f )
        {
            brakeForce = -brake * ( gearComponent.GetGearValue( 0 ) * rigidbody.mass );
        }
        rigidbody.AddForce( transform.forward * Time.deltaTime * ( throttleForce + brakeForce ) );
        engineSpeed = rigidbody.velocity.magnitude;
    }

    private void ApplySteering ( Vector3 velocity )
    {
        double turnRadius = 3.0 / Mathf.Sin( ( 90 - ( steer * 30 ) ) * Mathf.Deg2Rad );
        float minMaxTurn = ( float )SpeedTurnRatio( );
        float turnSpeed = Mathf.Clamp( velocity.z / ( float )turnRadius, -minMaxTurn / 10, minMaxTurn / 10 );
        transform.RotateAround( transform.position + transform.right * ( float )turnRadius * steer,
                                transform.up,
                                turnSpeed * Mathf.Rad2Deg * Time.deltaTime * steer );
        //Add Handbrake here
        //Vector3 frontRightWheel = wheelComponent.vehicleWheels[ 0 ].visualWheel.parent.transform.localPosition;
        //Vector3 frontLeftWheel = wheelComponent.vehicleWheels[ 1 ].visualWheel.parent.transform.localPosition;
        //transform.RotateAround( transform.TransformPoint( ( frontRightWheel + frontLeftWheel ) * 0.5 ),
        //                                                    transform.up,
        //                                                    rigidbody.velocity.magnitude *
        //                                                    Mathf.Clamp01( 1 - rigidbody.velocity.magnitude / VehiclePhysics.speedCap )
        //                                                     );
    }

    private double SpeedTurnRatio ( )
    {
        //Debug.Log( " speed index: " + ( ( maximumTurn - minimumTurn ) / engineSpeed )
        //           + "\nEngine Speed: " + engineSpeed );
        //return ( ( maximumTurn - minimumTurn ) / engineSpeed );
        if ( VehiclePhysics.speedCap < ( 160 / 2 ) )
        {
            return minimumTurn;
        }
        double speedIndex = 1 - ( engineSpeed / ( 160 / 2 ) );
        return minimumTurn + speedIndex * ( maximumTurn - minimumTurn );
    }

    private bool SameSign ( float firstValue, float secondValue )
    {
        return firstValue >= 0 ^ secondValue < 0;
    }
}