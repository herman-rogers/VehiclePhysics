using UnityEngine;
using System.Collections;

public class EngineComponent : MonoBehaviour
{
    private float vehicleThrottle;
    private float horsePower;
    private float steer;
    private float motor;
    private float brake;
    private GearsComponent gearComponent;

    public void EngineUpdate ( )
    {
        UpdateInput( );
    }

    public void EngineFixedUpdate ( Vector3 vehicleVelocity, GearsComponent gears )
    {
        HorsePower( vehicleVelocity, gears );
    }

    private void UpdateInput ( )
    {
        steer = Mathf.Clamp( Input.GetAxis( "Horizontal" ), -1, 1 );
        motor = Mathf.Clamp( Input.GetAxis( "Acceleration" ), 0, 1 );
        brake = -1 * Mathf.Clamp( Input.GetAxis( "Brake" ), -1, 0 );
    }

    private void HorsePower ( Vector3 velocity, GearsComponent gears )
    {
        float vehicleGear = gearComponent.currentGear;
        if ( vehicleThrottle == 0 )
        {
            horsePower -= Time.deltaTime * 200;
        }
        else if ( SameSign( velocity.z, vehicleThrottle ) )
        {
            horsePower += Time.deltaTime * 200 * gearComponent.GetNormalizedPower( horsePower );
        }
        else
        {
            horsePower += Time.deltaTime * 300;
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
        if ( vehicleThrottle > 0.0f )
        {
            throttleForce = Mathf.Sign( vehicleThrottle ) * horsePower * rigidbody.mass;
        }
        if ( brake > 0.0f )
        {
            brakeForce = -brake * ( gearComponent.GetGearValue( 0 ) * rigidbody.mass );
        }
        rigidbody.AddForce( transform.forward * Time.deltaTime * ( throttleForce + brakeForce ) );
    }

    private void ApplySteering ( Vector3 velocity )
    {
        double turnRadius = 3.0 / Mathf.Sin( ( 90 - ( steer * 30 ) ) * Mathf.Deg2Rad );
        float minMaxTurn = SpeedTurnRatio( );
        float turnSpeed = Mathf.Clamp( velocity.z / ( float )turnRadius, -minMaxTurn / 10, minMaxTurn / 10 );
        transform.RotateAround( transform.position + transform.right * ( float )turnRadius * steer,
                                transform.up,
                                turnSpeed * Mathf.Rad2Deg * Time.deltaTime * steer );
        //Add Handbrake here
    }

    private float SpeedTurnRatio ( )
    {
        float speed = rigidbody.velocity.magnitude;
        if ( speed > VehiclePhysics.speedCap )
        {
            return 10;
        }
        float speedIndex = 1 - ( speed / ( VehiclePhysics.speedCap / 2 ) );
        //10 is the minimum turn and 15 is the maximum turn
        return 10 + speedIndex * ( 15 - 10 );
    }

    private bool SameSign ( float firstValue, float secondValue )
    {
        return firstValue >= 0 ^ secondValue < 0;
    }
}
