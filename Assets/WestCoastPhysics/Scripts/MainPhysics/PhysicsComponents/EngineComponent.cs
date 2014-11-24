using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngineComponent : MonoBehaviour
{
    public float steer { get; private set; }
    public float vehicleThrottle { get; private set; }
    public float brake { get; private set; }
    public float wheelAngularVelocity { get; private set; }
    public float vehicleSpeed { get; private set; } //TODO: find a better way to access vehicle speed
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
    //**Important Engine Monitor Values **//
    private float horsePower;
    private float totalLongitudinalForces;
    private float currentEngineRPM;
    private float maxTorque;
    private float engineTorque;
    private float totalBrakeForce;
    private float forwardsDrag;
    private float forwardsRollingResistance;

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
        EngineMonitor( );
    }

    //TODO: Move this into an Input Component Script
    private void UpdateInput ( )
    {
        steer = Input.GetAxis( "Horizontal" );
        vehicleThrottle = Input.GetAxis( "Acceleration" );
        brake = Input.GetAxis( "Brake" );
    }

    private void LongitudinalForces ( )
    {   //VehicleSpeed in KM/h
        vehicleSpeed = GetComponent<Rigidbody>( ).velocity.magnitude;
        //Calculating Engine Torque
        currentEngineRPM = CalculateEngineRPM( );
        maxTorque = CalculateTorque( currentEngineRPM );
        engineTorque = vehicleThrottle * maxTorque;

        float engineBrake = ( -brake ) * maxTorque;
        horsePower = ( engineTorque * currentEngineRPM ) / 5252;

        //The Amount of torque ( force ) we recieve when converting engine torque to the rear wheels
        float driveTorque = engineTorque * 9.1f * transmissionEfficiency;
        float forceDrive = ( driveTorque / wheelComponent.vehicleWheels[ 0 ].wheelRadius );

        //Brake Torque
        float brakeTorque = engineBrake * GearMultiplier( ) * transmissionEfficiency;
        float brakeForce = ( brakeTorque / wheelComponent.vehicleWheels[ 0 ].wheelRadius );
        VehicleResistanceCoefficients( );
        totalLongitudinalForces = forceDrive
                                  + forwardsDrag
                                  + forwardsRollingResistance;
        totalBrakeForce = brakeForce
                                + forwardsDrag
                                + forwardsRollingResistance;
        //The Total Forces to move the vehicle forwards
        wheelComponent.vehicleWheels[ 2 ].mainWheelCollider.motorTorque = totalLongitudinalForces + totalBrakeForce;
        wheelComponent.vehicleWheels[ 3 ].mainWheelCollider.motorTorque = totalLongitudinalForces + totalBrakeForce;
    }

    private void VehicleResistanceCoefficients ( )
    {
        float coefficientDrag = airFrictionDrag * amountOfDragForVehicleShape
                                * vehicleFrontalArea * Mathf.Sqrt( vehicleSpeed );
        float coefficientRollingResistance = 30 * coefficientDrag;

        //Vehicle Forwards Resistance
        forwardsDrag = -coefficientDrag * vehicleSpeed;
        forwardsRollingResistance = -coefficientRollingResistance * vehicleSpeed;
    }

    private float GearMultiplier ( )
    {
        float gearMultiplier = gearRatio * differentialRatio;
        //Approximating 30% loss of energy
        float lossOfEnergy = gearMultiplier * 0.30f;
        gearMultiplier = ( gearMultiplier - lossOfEnergy );
        return gearMultiplier;
    }

    private float CalculateEngineRPM ( )
    {
        float vehicleSpeedMetersPerSecond = ( vehicleSpeed * 1000 ) / 3600;
        //WheelAngularVelocity:
        //Speed wheel is rotating at a given speed( in Radians/per second )
        wheelAngularVelocity = ( vehicleSpeedMetersPerSecond / wheelComponent.vehicleWheels[ 0 ].wheelRadius );
        float rpm = ( ( wheelAngularVelocity * gearRatio * differentialRatio * 60 ) / ( 2 * pi ) );
        if ( rpm < 1000 )
        {
            return 1000;
        }
        return rpm;
    }

    private float CalculateTorque ( float rpms )
    {
        //based off of y = mx + b. Graph Coordinates are
        //( x1, y1 ) = ( 0, 5000 ), ( x2, y2 ) = ( 4000, 0 )
        //Torque Curve is in Newton Meters
        float torqueValue = ( -1.25f * rpms ) + 5000;
        return torqueValue;
    }

    private void AutomaticGearShift ( )
    {
        if ( currentEngineRPM <= 1200.0f )
        {
            gearNumber = 0;
        }
        else if ( currentEngineRPM > 1200.0f && currentEngineRPM <= 2000.0f )
        {
            gearNumber = 1;
        }
        else if ( currentEngineRPM > 2000.0f && currentEngineRPM <= 2500.0f )
        {
            gearNumber = 2;
        }
        else if ( currentEngineRPM > 2500.0f && currentEngineRPM <= 3500.0f )
        {
            gearNumber = 3;
        }
        else if ( vehicleSpeed > 3500.0f )
        {
            gearNumber = 4;
        }
        gearRatio = engineGearRatios[ gearNumber ];
    }

    private void EngineMonitor ( )
    {
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.VEHICLE_PHYSICS,
                              vehicleSpeed.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.CURRENT_GEAR,
                              gearNumber.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.GEAR_RATIO,
                              gearRatio.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.CURRENT_RPM,
                              currentEngineRPM.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.MAX_TORQUE,
                              maxTorque.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.ENGINE_TORQUE,
                              engineTorque.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.HORSE_POWER,
                              horsePower.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.AIR_RESISTANCE,
                              forwardsDrag.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.ROLLING_RESISTANCE,
                              forwardsRollingResistance.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.TLONG_FORCE,
                              totalLongitudinalForces.ToString( ) );
        Subject.NotifyObject( gameObject,
                              VehiclePhysicsExtension.BRAKE_FORCE,
                              totalBrakeForce.ToString( ) );
    }
}