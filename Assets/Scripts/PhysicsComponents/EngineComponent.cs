using UnityEngine;
using System.Collections;

public class EngineComponent : MonoBehaviour {

    //private void HorsePower ( Vector3 velocity )
    //{
    //    float vehicleGear = gearComponent.currentGear;
    //    if ( vehicleThrottle == 0 )
    //    {
    //        horsePower -= Time.deltaTime * 200;
    //    }
    //    else if ( SameSign( velocity.z, vehicleThrottle ) )
    //    {
    //        horsePower += Time.deltaTime * 200 * gearComponent.GetNormalizedPower( horsePower );
    //    }
    //    else
    //    {
    //        horsePower += Time.deltaTime * 300;
    //    }
    //    if ( vehicleGear == 0 )
    //    {
    //        horsePower = Mathf.Clamp( horsePower, 0, gearComponent.GetGearValue( 0 ) );
    //    }
    //    else
    //    {
    //        horsePower = Mathf.Clamp( horsePower,
    //                                  gearComponent.GetGearValue( vehicleGear - 1 ),
    //                                  gearComponent.GetGearValue( vehicleGear ) );
    //    }
    //}

    //private void ApplyThrottle ( Vector3 velocity )
    //{
    //    float throttleForce = 0.0f;
    //    float brakeForce = 0.0f;
    //    if ( vehicleThrottle > 0.0f )
    //    {
    //        throttleForce = Mathf.Sign( vehicleThrottle ) * horsePower * rigidbody.mass;
    //    }
    //    if ( brake > 0.0f )
    //    {
    //        brakeForce = -brake * ( gearComponent.GetGearValue( 0 ) * rigidbody.mass );
    //    }
    //    rigidbody.AddForce( transform.forward * Time.deltaTime * ( throttleForce + brakeForce ) );
    //}

    //private void ApplySteering ( Vector3 velocity )
    //{
    //    double turnRadius = 3.0 / Mathf.Sin( ( 90 - ( steer * 30 ) ) * Mathf.Deg2Rad );
    //    float minMaxTurn = SpeedTurnRatio( );
    //    float turnSpeed = Mathf.Clamp( velocity.z / ( float )turnRadius, -minMaxTurn / 10, minMaxTurn / 10 );
    //    transform.RotateAround( transform.position + transform.right * ( float )turnRadius * steer,
    //                            transform.up,
    //                            turnSpeed * Mathf.Rad2Deg * Time.deltaTime * steer );
    //    //Add Handbrake here
    //}
}
