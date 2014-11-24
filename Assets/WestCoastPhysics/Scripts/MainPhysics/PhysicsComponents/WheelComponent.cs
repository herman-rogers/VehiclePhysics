using UnityEngine;
using System.Collections;

public class WheelComponent
{
    public VehiclePhysics mainVehiclePhysics;
    public VehicleWheel[ ] vehicleWheels { get; private set; }
    public float getDistanceBetweenRearWheels { get; private set; }
    public float wheelBase { get; private set; }
    private WheelCollider[ ] vehicleColliders;
    private const int i_frontRightWheel = 0;
    private const int i_frontLeftWheel = 1;
    private const int i_rearRightWheel = 2;
    private const int i_rearLeftWheel = 3;

    public WheelComponent( GameObject vehiclePhysics )
    {
        mainVehiclePhysics = vehiclePhysics.GetComponent<VehiclePhysics>( );
        vehicleColliders = mainVehiclePhysics.vehicleWheels;
        SetupWheelObjects( );
        CalculateDistanceBetweenAxles( );
        CalculateDistanceBetweenRearWheels( );
    }

    public int GetWheelsOnGround ( )
    {
        int wheelState = FindCurrentWheelState( WheelsOnGround.WHEEL_ON_GROUND );
        return wheelState;
    }

    public int GetWheelInAir ( )
    {
        int wheelState = FindCurrentWheelState( WheelsOnGround.WHEEL_IN_AIR );
        return wheelState;
    }

    //Wheels in the VehicleWheel array are set up as frontwheels = 0,1. rearwheels = 2,3
    private void SetupWheelObjects( )
    {
        VehicleWheel frontRightWheel = new VehicleWheel( vehicleColliders[ i_frontRightWheel ],
                                                         this,
                                                         WheelPosition.FRONT_WHEEL,
                                                         WheelPosition.RIGHT_WHEEL );
        VehicleWheel frontLeftWheel = new VehicleWheel( vehicleColliders[ i_frontLeftWheel ],
                                                        this,
                                                        WheelPosition.FRONT_WHEEL,
                                                        WheelPosition.LEFT_WHEEL );
        VehicleWheel rearRightWheel = new VehicleWheel( vehicleColliders[ i_rearRightWheel ],
                                                        this,
                                                        WheelPosition.REAR_WHEEL,
                                                        WheelPosition.RIGHT_WHEEL );
        VehicleWheel rearLeftWheel = new VehicleWheel( vehicleColliders[ i_rearLeftWheel ],
                                                       this,
                                                       WheelPosition.REAR_WHEEL,
                                                       WheelPosition.LEFT_WHEEL );
        vehicleWheels = new VehicleWheel[ ] { frontRightWheel, frontLeftWheel, rearRightWheel, rearLeftWheel };
    }

    public void WheelFixedUpdate( )
    {
        UpdateVehicleWheels( );
    }

    private void UpdateVehicleWheels( )
    {
        vehicleWheels[ i_frontRightWheel ].UpdateWheel( );
        vehicleWheels[ i_frontLeftWheel ].UpdateWheel( );
        vehicleWheels[ i_rearRightWheel ].UpdateWheel( );
        vehicleWheels[ i_rearLeftWheel ].UpdateWheel( );
    }

    //Used to calculate the wheel base = ( distance between axles )
    private void CalculateDistanceBetweenAxles( )
    {
        Vector3 frontAxlePosition = vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.position;
        Vector3 rearAxlePosition = vehicleWheels[ i_frontLeftWheel ].mainWheelCollider.transform.position;
        wheelBase = Vector3.Distance( frontAxlePosition, rearAxlePosition );
    }

    //Used to calculate that angle theta to find the steering angle ( 90 - theta )
    private void CalculateDistanceBetweenRearWheels( )
    {
        Vector3 rearRightWheel = vehicleWheels[ i_rearRightWheel ].mainWheelCollider.transform.position;
        Vector3 rearLeftWheel = vehicleWheels[ i_rearLeftWheel ].mainWheelCollider.transform.position;
        getDistanceBetweenRearWheels = Vector3.Distance( rearRightWheel, rearLeftWheel );
    }

    private int FindCurrentWheelState ( WheelsOnGround checkForWheelState )
    {
        int wheelOnGroundCount = 0;
        for ( int i = 0; i < vehicleWheels.Length; i++ )
        {
            if ( vehicleWheels[ i ].checkCurrentWheelState == checkForWheelState )
            {
                wheelOnGroundCount++;
            }
        }
        return wheelOnGroundCount;
    }
}