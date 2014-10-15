using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngineComponent : MonoBehaviour
{
    public float vehicleSpeed { get; private set; }
    public float steer{ get; private set; }
    public float vehicleThrottle { get; private set; }
    public float brake { get; private set; }
    public float wheelAngularVelocity { get; private set; }
    private WheelComponent wheelComponent;
    private VehiclePhysics vehiclePhysics;
    private const float airDensity = 1.29f; //air density of earth
    private const float vehicleFrontalArea = 2.2f;
    private const float amountOfDragForVehicleShape = 0.30f; //the tunneltest for a corvette
    private const float airFrictionDrag = 0.5f;//constant value for the drag of air on a vehicle
    private const float pi = 3.14f;
    private int gearNumber = 0;
    private float gearRatio = 2.66f;
    private const float transmissionEfficiency = 0.7f;
    private float[ ] engineGearRatios = new float[ ]{ 2.66f, 1.78f,
                                                     1.3f, 1.0f,
                                                     0.74f, 0.50f }; //reverse 2.90f
    private const float differentialRatio = 3.42f;

    public void Start ( )
    {
        vehiclePhysics = GetComponent<VehiclePhysics>( );
        wheelComponent = vehiclePhysics.wheelComponent;
    }

    public void EngineUpdate ( )
    {
        UpdateInput( );
    }

    public void EngineFixedUpdate ( )
    {
        LongitudinalForces( );
        AutomaticGearShift( );
    }

    //TODO: Move this into an Input Component Script
    private void UpdateInput ( )
    {
        steer = Input.GetAxis( "Horizontal" );
        vehicleThrottle = Input.GetAxis( "Acceleration" );
        brake = Input.GetAxis( "Brake" );
    }

    private void LongitudinalForces ( )
    {
        //Vehicle Speed
        vehicleSpeed = rigidbody.velocity.magnitude;
        Vector3 reverseVehicleSpeed = transform.InverseTransformDirection( rigidbody.velocity );
        //Calculating Engine Torque
        float currentEngineRPM = CalculateEngineRPM( );
        float maxTorque = CalculateTorque( currentEngineRPM );
        float engineTorque = vehicleThrottle * maxTorque;
        float engineBrake = ( -brake ) * maxTorque;
        float horsePower = ( engineTorque * currentEngineRPM ) / 5252;

        //The Amount of torque ( force ) we recieve when converting engine torque to the rear wheels
        float driveTorque = engineTorque * gearRatio * differentialRatio * transmissionEfficiency;
        float forceDrive = ( driveTorque / wheelComponent.vehicleWheels[ 0 ].wheelRadius );

        //Brake Torque
        float brakeTorque = engineBrake * gearRatio * differentialRatio * transmissionEfficiency;
        float brakeForce = ( brakeTorque / wheelComponent.vehicleWheels[ 0 ].wheelRadius );

        //**COMPLETE
        //Vehicle Resistance Coefficients
        float coefficientDrag = airFrictionDrag * amountOfDragForVehicleShape 
                                * vehicleFrontalArea * Mathf.Sqrt( vehicleSpeed ); 
        float coefficientRollingResistance = 30 * coefficientDrag;

        //Vehicle Forwards Resistance
        float forwardsDrag = -coefficientDrag * vehicleSpeed;
        float forwardsRollingResistance = -coefficientRollingResistance * vehicleSpeed;
        float totalLongitudinalForces = forceDrive
                                        + forwardsDrag
                                        + forwardsRollingResistance;
                               //         + wheelComponent.CalculateWheelSlip( );
        float totalBrakeForce = brakeForce
                                - forwardsDrag 
                                - forwardsRollingResistance
                                - wheelComponent.CalculateWheelSlip( );

        //TODO: Fix issues when reversing and moving vehicle forward -- causes multiply issues
        if ( brake > 0.0f && reverseVehicleSpeed.z >= -15.0f )
        {
            rigidbody.AddForce( transform.forward 
                                * Time.deltaTime 
                                * ( totalBrakeForce ), ForceMode.Acceleration );
        }
        //TODO: Apply to brake when brake speed is high
        if ( totalLongitudinalForces > 0.0f )
        {
            rigidbody.AddForce( transform.forward 
                                * Time.deltaTime 
                                * ( totalLongitudinalForces ), ForceMode.Acceleration );
        }
    }

    private float CalculateEngineRPM ( )
    {
        float vehicleSpeedMetersPerSecond = ( vehicleSpeed * 1000 ) / 3600;
        //Speed wheel is rotating at a given speed( in Radians/per second )
        wheelAngularVelocity = vehicleSpeedMetersPerSecond / wheelComponent.vehicleWheels[ 0 ].wheelRadius;
        float rpm =  ( ( wheelAngularVelocity * gearRatio * differentialRatio * 60 ) / ( 2 * pi ) );
        if( rpm < 1000 )
        {
            return 1000; 
        }
        return rpm;
    }

    private float CalculateTorque ( float rpms )
    {
        //Torque is in lb-ft
        if ( rpms == 1000 )
        {
            return 300;
        }
        if( rpms > 1000 && rpms <= 2000 )
        {
            return 325;
        }
        if ( rpms > 2000 && rpms <= 3000 )
        {
            return 330;
        }
        if ( rpms > 3000 && rpms <= 4000 )
        {
            return 340;
        }
        if ( rpms > 4000 && rpms <= 5000 )
        {
            return 350;
        }
        return 340;
    }

    private void AutomaticGearShift ( )
    {
        //Vehicle Speed in KM/h
        if ( vehicleSpeed < 25.0f )
        {
            gearNumber = 0;
        }
        else if ( vehicleSpeed >= 25.0f && vehicleSpeed <= 40.0f )
        {
            gearNumber = 1;
        }
        else if ( vehicleSpeed >= 40.0f && vehicleSpeed <= 60.0f )
        {
            gearNumber = 2;
        }
        else if ( vehicleSpeed >= 60.0f && vehicleSpeed <= 90.0f )
        {
            gearNumber = 3;
        }
        else if ( vehicleSpeed >= 90.0f && vehicleSpeed <= 120.0f )
        {
            gearNumber = 4; 
        }
        gearRatio = engineGearRatios[ gearNumber ];
    }
}