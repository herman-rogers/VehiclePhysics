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
    public WheelsOnGround isWheelOnGround;
    public WheelHit wheelGroundContactPoint;
    public WheelCollider mainWheelCollider;
    public WheelComponent wheelPhysicsComponent;
    public Transform visualWheel;
    public float wheelRadius;

    public VehicleWheel( WheelCollider vehicleWheel,
                          WheelComponent wheelComponent,
                          WheelPosition frontRear,
                          WheelPosition leftRight )
    {
        wheelFrontRear = frontRear;
        wheelLeftRight = leftRight;
        isWheelOnGround = WheelsOnGround.WHEEL_ON_GROUND;
        mainWheelCollider = vehicleWheel;
        wheelPhysicsComponent = wheelComponent;
        visualWheel = vehicleWheel.GetComponentInChildren<MeshRenderer>( ).transform;
        wheelRadius = ( visualWheel.GetComponent<Renderer>().bounds.size.y / 2 );
        mainWheelCollider.radius = wheelRadius;
        mainWheelCollider.GetGroundHit( out wheelGroundContactPoint );
    }

    public void UpdateWheel( )
    {
        if ( mainWheelCollider.GetGroundHit( out wheelGroundContactPoint ) )
        {
            isWheelOnGround = WheelsOnGround.WHEEL_ON_GROUND;
            UpdateWheelGraphic( );
            return;
        }
        isWheelOnGround = WheelsOnGround.WHEEL_IN_AIR;
    }

    private void UpdateWheelGraphic( )
    {
        //Vector3 wheelVelocity = wheelPhysics.mainVehiclePhysics.GetComponent<Rigidbody>().GetPointVelocity( wheelGroundContactPoint.point );
        //Vector3 localWheelVelocity = visualWheel.parent.InverseTransformDirection( wheelVelocity );

        Transform visualWheel = mainWheelCollider.transform.GetChild( 0 );
        Vector3 position = new Vector3( 0, 0, 0 );
        Quaternion rotation = Quaternion.identity;
        mainWheelCollider.GetLocalPose( out position, out rotation );

        if ( wheelFrontRear == WheelPosition.FRONT_WHEEL )
        {
            mainWheelCollider.steerAngle = 30 * ( Input.GetAxis( "Horizontal" ) );
            mainWheelCollider.motorTorque = 400 * ( Input.GetAxis( "Acceleration" ) * 2 );
            //mainWheelCollider.motorTorque = 10 * Input.GetAxis( "Brake" );
            //SteeringRotation( localWheelVelocity );
            //return;
        }
        //RotateDrivingWheel( localWheelVelocity );
        visualWheel.transform.position = mainWheelCollider.transform.parent.TransformPoint( position );
        visualWheel.transform.rotation = mainWheelCollider.transform.parent.rotation * rotation;
    }

    public void SteeringRotation( Vector3 frontVelocity )
    {
        //Rotate the wheels horizontally
        Vector3 cachedWheelRotation = visualWheel.parent.localEulerAngles;
        cachedWheelRotation.y = wheelPhysicsComponent.SteeringAngle( );
        //float test = SpeedTurnRatio( );
        visualWheel.parent.localEulerAngles = cachedWheelRotation;
        //Rotate the wheels vertically
        visualWheel.Rotate( Vector3.right
                            * ( frontVelocity.z / wheelRadius )
                            * Time.deltaTime * Mathf.Rad2Deg );
    }

    public void RotateDrivingWheel( Vector3 rearRotation )
    {
        if ( wheelPhysicsComponent.mainVehiclePhysics.engineComponent.vehicleThrottle > 0.0f )
        {
            float rearWheelVelocity = ( ( wheelPhysicsComponent.mainVehiclePhysics.engineComponent.vehicleThrottle * 10 ) 
                                          * rearRotation.z );
            visualWheel.Rotate( Vector3.right
                                * ( rearWheelVelocity / wheelRadius )
                                * Time.deltaTime
                                * Mathf.Rad2Deg );
        }
        else
        {
            visualWheel.Rotate( Vector3.right
                                * ( rearRotation.z / wheelRadius )
                                * Time.deltaTime * Mathf.Rad2Deg );
        }
    }
}