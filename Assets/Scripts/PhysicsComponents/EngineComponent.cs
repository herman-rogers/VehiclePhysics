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
    private VehiclePhysics vehiclePhysics;
    private static double engineSpeed;
    private const float horsePowerMultiplier = 50;

    //NEW PHYSICS
    private WheelComponent wheelComponent;
    private const float airDensity = 1.29f; //air density of earth
    private const float vehicleFrontalArea = 2.2f;
    private const float amountOfDragForVehicleShape = 0.30f; //the tunneltest for a corvette
    private const float airFrictionDrag = 0.5f;//constant value for the drag of air on a vehicle
    private const float pi = 3.14f;
    private float gearRatio = 2.66f;
    private float differentialRatio = 3.42f;

    public void Start ( )
    {
        vehiclePhysics = GetComponent<VehiclePhysics>( );
        gearComponent = vehiclePhysics.gearsComponent;
        wheelComponent = vehiclePhysics.wheelComponent;
    }

    public void EngineUpdate ( )
    {
        UpdateInput( );
    }

    public void EngineFixedUpdate ( Vector3 vehicleVelocity )
    {
        LongitudinalForces( );
        //HorsePower( vehicleVelocity );
        //ApplyThrottle( vehicleVelocity );
        ApplySteering( vehicleVelocity );
    }

    //TODO: Move this into an Input Component Script
    private void UpdateInput ( )
    {
        steer = Input.GetAxis( "Horizontal" );
        vehicleThrottle = Input.GetAxis( "Acceleration" );
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

    private void LongitudinalForces ( )
    {
        Vector3 velocity = rigidbody.velocity;
        float vehicleSpeed = Mathf.Sqrt( ( velocity.x * velocity.x ) + ( velocity.y * velocity.y ) );
        float acceleration = ( ( rigidbody.mass / 2 ) * 9.8f ) / rigidbody.mass;
        //Total Longitudinal Force Flong = Ftraction + Fdrag + Frr
        float engineRPM = CalculateEngineRPM( );
        float engineTorque = ( vehicleThrottle * acceleration ) * CalculateTorque( engineRPM );
        float horsePower = ( engineTorque * engineRPM ) / 5252;
        
        //transmission efficiency
        //Fdrive = u * Tengine *xg * xd * n / Rw
        //xg is the gear ratio and xd is the differential ratio, n is transmission effeciency
        //TODO: add in differential ratio and the trasmission efficiency

        float transmissionEfficiency = ( 0.7f / ( wheelComponent.vehicleWheels[ 0 ].wheelRadius ) );
        float torqueToRearWheels = engineTorque * gearRatio * differentialRatio * transmissionEfficiency;

        //Weight of Car and max amount of traction the rear wheels can provide

        //Vehicle Resistance
        //float coefficientDrag = amountOfDragForVehicleShape * vehicleFrontalArea 
        //                        * ( vehicleSpeed * vehicleSpeed );
        //float forwardsDrag = airFrictionDrag * coefficientDrag * acceleration;
        //float rollingResistance = 30 * coefficientDrag;

        float driveTraction = acceleration * vehicleThrottle * torqueToRearWheels;

        Debug.Log( "RPMs: " + engineRPM
                   + " \n Torque: " + engineTorque
                   + " :: HorsePower: " + horsePower );

        rigidbody.AddForce( transform.forward * Time.deltaTime * ( driveTraction  ) );
    
    }

    private float CalculateEngineRPM ( )
    {
        float wheelRotation = ( wheelComponent.vehicleWheels[ 2 ].mainWheelCollider.rpm
                                + wheelComponent.vehicleWheels[ 3 ].mainWheelCollider.rpm );
        //TODO: Add in more roboust gear ratio and differential ratio

        //TODO: convert to coordinate system
        float rpm = vehicleThrottle * ( ( wheelRotation * gearRatio * differentialRatio * 60 ) / ( 2 * pi ) );
        if( rpm < 1000 )
        {
            return 1000;
        }
        if( rpm > 6000 )
        {
            return 6000;
        }
        return rpm;
    }

    private float CalculateTorque ( float rpms )
    {
        //RPMs is in ft/lbs
        if ( rpms < 1000 )
        {
            return 300;
        }
        if( rpms > 1000 && rpms <= 2000 )
        {
            return 320;
        }
        if ( rpms > 2000 && rpms <= 3000 )
        {
            return 340;
        }
        if ( rpms > 3000 && rpms <= 4000 )
        {
            return 350;
        }
        return 320;
    }



    //private void HorsePower ( Vector3 velocity )
    //{

    //    float vehicleGear = gearComponent.currentGear;
    //    if ( vehicleThrottle == 0 )
    //    {
    //        horsePower -= Time.deltaTime * horsePowerMultiplier;
    //    }
    //    else if ( SameSign( velocity.z, vehicleThrottle ) )
    //    {
    //        horsePower += Time.deltaTime
    //                      * horsePowerMultiplier
    //                      * gearComponent.GetNormalizedPower( horsePower );
    //    }
    //    else
    //    {
    //        horsePower += Time.deltaTime * horsePowerMultiplier;
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
    //    if ( WheelComponent.wheelsGrounded == WheelsOnGround.REAR_WHEELS_IN_AIR )
    //    {
    //        return;
    //    }
    //    if ( vehicleThrottle > 0.0f )
    //    {
    //        throttleForce = Mathf.Sign( vehicleThrottle ) * horsePower * rigidbody.mass;
    //    }
    //    if ( brake > 0.0f )
    //    {
    //        brakeForce = -brake * ( gearComponent.GetGearValue( 0 ) * rigidbody.mass );
    //    }
    //    rigidbody.AddForce( transform.forward * Time.deltaTime * ( throttleForce + brakeForce ) );
    //    Debug.Log( throttleForce );
    //    engineSpeed = rigidbody.velocity.magnitude;
    //}

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
        //if ( VehiclePhysics.speedCap < ( 160 / 2 ) )
        //{
        //    return minimumTurn;
        //}
        double speedIndex = 1 - ( engineSpeed / ( 160 / 2 ) );
        return minimumTurn + speedIndex * ( maximumTurn - minimumTurn );
    }

    private bool SameSign ( float firstValue, float secondValue )
    {
        return firstValue >= 0 ^ secondValue < 0;
    }
}