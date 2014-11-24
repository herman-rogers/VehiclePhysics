using UnityEngine;
using System.Collections;

public enum WheelsOnGround
{
    WHEEL_ON_GROUND, //individual wheel on ground
    WHEEL_IN_AIR, //individual wheel in air
}

public enum WheelPosition
{
    FRONT_WHEEL,
    REAR_WHEEL,
    LEFT_WHEEL,
    RIGHT_WHEEL,
}

public class VehicleWheel
{
    //Is Wheel Front or Rear
    public WheelPosition wheelFrontRear;
    //Is Wheel Left or Right
    public WheelPosition wheelLeftRight;
    public WheelsOnGround checkCurrentWheelState;
    public WheelHit wheelGroundContactPoint;
    public WheelCollider mainWheelCollider;
    public WheelComponent wheelPhysicsComponent;
    public Transform visualWheel;
    public float wheelRadius;

    public VehicleWheel(  WheelCollider vehicleWheel,
                          WheelComponent wheelComponent,
                          WheelPosition frontRear,
                          WheelPosition leftRight )
    {
        wheelFrontRear = frontRear;
        wheelLeftRight = leftRight;
        checkCurrentWheelState = WheelsOnGround.WHEEL_ON_GROUND;
        mainWheelCollider = vehicleWheel;
        wheelPhysicsComponent = wheelComponent;
        visualWheel = vehicleWheel.GetComponentInChildren<MeshRenderer>( ).transform;
        wheelRadius = ( visualWheel.GetComponent<Renderer>().bounds.size.y / 2 );
        mainWheelCollider.radius = wheelRadius;
        mainWheelCollider.GetGroundHit( out wheelGroundContactPoint );
    }

    public void UpdateWheel( )
    {
        UpdateWheelGraphic( );
        UpdateWheelPosition( );
    }

    private void UpdateWheelPosition ( )
    {
        if ( mainWheelCollider.GetGroundHit( out wheelGroundContactPoint ) )
        {
            checkCurrentWheelState = WheelsOnGround.WHEEL_ON_GROUND;
            return;
        }
        checkCurrentWheelState = WheelsOnGround.WHEEL_IN_AIR;
    }

    private void UpdateWheelGraphic ( )
    {
        if ( mainWheelCollider.GetGroundHit( out wheelGroundContactPoint ) )
        {
            ChangeTireTransform( );
        }
    }

    private void ChangeTireTransform( )
    {
        Vector3 position = new Vector3( 0, 0, 0 );
        Quaternion rotation = Quaternion.identity;
        mainWheelCollider.GetWorldPose( out position, out rotation );
        if ( wheelFrontRear == WheelPosition.FRONT_WHEEL )
        {
            mainWheelCollider.steerAngle = 25 * ( Input.GetAxis( "Horizontal" ) );
        }
        visualWheel.transform.localPosition = mainWheelCollider.transform.InverseTransformPoint( position );
        visualWheel.transform.localRotation = Quaternion.Inverse( mainWheelCollider.transform.rotation ) * rotation;
    }
}