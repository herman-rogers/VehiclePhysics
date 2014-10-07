using UnityEngine;
using System.Collections;

public enum WheelsOnGround
{
    REAR_WHEELS_GROUNDED,
    REAR_WHEELS_IN_AIR,
}

public enum WheelPosition
{
    FRONT_WHEEL,
    REAR_WHEEL,
    LEFT_WHEEL,
    RIGHT_WHEEL,
}

public struct VehicleWheel
{
    public Transform visualWheel;
    public WheelCollider mainWheelCollider;
    public float wheelRadius;
    public WheelPosition wheelFrontRear;
    public WheelPosition wheelLeftRight;

    public VehicleWheel ( WheelCollider vehicleWheel,
                         WheelPosition frontRear,
                         WheelPosition leftRight )
    {
        wheelFrontRear = frontRear;
        wheelLeftRight = leftRight;
        mainWheelCollider = vehicleWheel;
        visualWheel = vehicleWheel.GetComponentInChildren<MeshRenderer>( ).transform;
        wheelRadius = ( visualWheel.renderer.bounds.size.y / 2 );
        mainWheelCollider.radius = wheelRadius;
    }
}

public class WheelComponent
{
    public VehicleWheel[ ] vehicleWheels { get; private set; }
	public float getDistanceBetweenAxles{ get; private set; }
    public static WheelsOnGround wheelsGrounded;
    private WheelFrictionCurve wheelFriction;
    private WheelCollider[ ] vehicleColliders;
    private VehiclePhysics mainVehiclePhysics;
    private float stabilizierForce = 5000.0f;
    private float suspensionRange = 0.1f;
    private float suspensionDamper = 50.0f;
    private float suspensionSpringFront = 18500.0f;
    private float suspensionSpringRear = 9000.0f;

    public WheelComponent ( GameObject vehiclePhysics )
    {
        mainVehiclePhysics = vehiclePhysics.GetComponent<VehiclePhysics>( );
        vehicleColliders = mainVehiclePhysics.vehicleWheels;
        SetupWheelObjects( );
        WheelSuspension( );
        WheelFrictionCurve( );
		CalculateDistanceBetweenAxles( );
    }

	//Wheels in the VehicleWheel array are set up as frontwheels = 0,1. rearwheels = 2,3
    private void SetupWheelObjects ( )
    {
        VehicleWheel frontRightWheel = new VehicleWheel( vehicleColliders[ 0 ],
                                                        WheelPosition.FRONT_WHEEL,
                                                        WheelPosition.RIGHT_WHEEL );
        VehicleWheel frontLeftWheel = new VehicleWheel( vehicleColliders[ 1 ],
                                                        WheelPosition.FRONT_WHEEL,
                                                        WheelPosition.LEFT_WHEEL );
        VehicleWheel rearRightWheel = new VehicleWheel( vehicleColliders[ 2 ],
                                                        WheelPosition.REAR_WHEEL,
                                                        WheelPosition.RIGHT_WHEEL );
        VehicleWheel rearLeftWheel = new VehicleWheel( vehicleColliders[ 3 ],
                                                       WheelPosition.REAR_WHEEL,
                                                       WheelPosition.LEFT_WHEEL );
        vehicleWheels = new VehicleWheel[ ] { frontRightWheel, frontLeftWheel, rearRightWheel, rearLeftWheel };
    }

    public void WheelUpdate ( Vector3 velocity )
    {
        UpdateWheelGraphics( velocity );
    }

    public void WheelFixedUpdate ( Vector3 velocity )
    {
        WheelFriction( velocity );
        FrontWheelStabilizers( );
        RearWheelStabilizers( );
        RearWheelsGrounded( );
    }

	private void CalculateDistanceBetweenAxles()
	{
		Vector3 rearAxlePosition = vehicleWheels [0].mainWheelCollider.transform.position;
		Vector3 frontAxlePosition = vehicleWheels [2].mainWheelCollider.transform.position;
		getDistanceBetweenAxles = Vector3.Distance( frontAxlePosition, rearAxlePosition );
	}

    private void UpdateWheelGraphics ( Vector3 velocity )
    {
        foreach ( VehicleWheel currentWheel in vehicleWheels )
        {
            WheelHit wheelHit = new WheelHit( );
            if ( currentWheel.mainWheelCollider.GetGroundHit( out wheelHit ) )
            {
                Vector3 wheelVelocity = mainVehiclePhysics.rigidbody.GetPointVelocity( wheelHit.point );
                Vector3 localWheelVelocity = currentWheel.visualWheel.parent.InverseTransformDirection( wheelVelocity );
                if ( currentWheel.wheelFrontRear == WheelPosition.FRONT_WHEEL )
                {
                    //Steering Rotation
                    Vector3 cachedWheelRotation = currentWheel.visualWheel.parent.localEulerAngles;
                    cachedWheelRotation.y = mainVehiclePhysics.engineComponent.SteeringSpeed( );
                    currentWheel.visualWheel.parent.localEulerAngles = cachedWheelRotation;

                    //Wheel Rotation
                    currentWheel.visualWheel.Rotate( Vector3.right * ( localWheelVelocity.z / currentWheel.wheelRadius )
                                               * Time.deltaTime * Mathf.Rad2Deg );
                }
                else
                {
                    if ( mainVehiclePhysics.engineComponent.GetVehicleThrottle( ) > 0.0f )
                    {
                        float rearWheelVelocity = ( ( mainVehiclePhysics.engineComponent.GetVehicleThrottle( ) * 10 ) *
                                                    localWheelVelocity.z );
                        currentWheel.visualWheel.Rotate( Vector3.right * ( rearWheelVelocity / currentWheel.wheelRadius )
                                                         * Time.deltaTime * Mathf.Rad2Deg );
                    }
                    else
                    {
                        currentWheel.visualWheel.Rotate( Vector3.right * ( localWheelVelocity.z / currentWheel.wheelRadius )
                                   * Time.deltaTime * Mathf.Rad2Deg );
                    }
                }
            }
        }
    }

    private void WheelFrictionCurve ( )
    {
        wheelFriction = new WheelFrictionCurve( );
        wheelFriction.extremumSlip = 1;
        wheelFriction.extremumValue = 50;
        wheelFriction.asymptoteSlip = 2;
        wheelFriction.asymptoteValue = 25;
        wheelFriction.stiffness = 1;
    }

    private void WheelSuspension ( )
    {
        foreach ( VehicleWheel wheel in vehicleWheels )
        {
            WheelCollider currentWheelCollider = wheel.mainWheelCollider;
            currentWheelCollider.suspensionDistance = suspensionRange;
            JointSpring wheelJointSpring = currentWheelCollider.suspensionSpring;
            if ( wheel.wheelFrontRear == WheelPosition.FRONT_WHEEL )
            {
                wheelJointSpring.spring = suspensionSpringFront;
            }
            else
            {
                wheelJointSpring.spring = suspensionSpringRear;
            }
            wheelJointSpring.damper = suspensionDamper;
            currentWheelCollider.suspensionSpring = wheelJointSpring;
        }
    }

    private void WheelFriction ( Vector3 velocity )
    {
        float squareVelocity = velocity.x * velocity.x;
        wheelFriction.extremumValue = Mathf.Clamp( 300 - squareVelocity, 0, 300 );
        wheelFriction.asymptoteValue = Mathf.Clamp( 150 - ( squareVelocity / 2 ), 0, 150 );
        foreach ( WheelCollider wheel in vehicleColliders )
        {
            wheel.sidewaysFriction = wheelFriction;
            wheel.forwardFriction = wheelFriction;
        }
    }

    private void FrontWheelStabilizers ( )
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRight = 1.0f;
        bool rightWheelGrounded = vehicleWheels[ 0 ].mainWheelCollider.GetGroundHit( out hit );
        bool leftWheelGrounded = vehicleWheels[ 1 ].mainWheelCollider.GetGroundHit( out hit );
        if ( leftWheelGrounded )
        {
            travelLeft = ( ( -vehicleWheels[ 1 ].mainWheelCollider.transform.InverseTransformPoint( hit.point ).y
                           - vehicleWheels[ 1 ].wheelRadius ) / vehicleWheels[ 1 ].mainWheelCollider.suspensionDistance );
        }
        if ( rightWheelGrounded )
        {
            travelRight = ( ( -vehicleWheels[ 0 ].mainWheelCollider.transform.InverseTransformPoint( hit.point ).y
                              - vehicleWheels[ 0 ].wheelRadius ) / vehicleWheels[ 0 ].mainWheelCollider.suspensionDistance );
        }

        float antiRollForce = ( travelLeft - travelRight ) * stabilizierForce;

        if ( leftWheelGrounded )
        {
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ 1 ].mainWheelCollider.transform.up
                                                             * -antiRollForce,
                                                             vehicleWheels[ 1 ].mainWheelCollider.transform.position );
        }
        if ( rightWheelGrounded )
        {
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ 0 ].mainWheelCollider.transform.up
                                                             * antiRollForce,
                                                             vehicleWheels[ 0 ].mainWheelCollider.transform.position );
        }
    }

    private void RearWheelStabilizers ( )
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRight = 1.0f;
        bool rightWheelGrounded = vehicleWheels[ 2 ].mainWheelCollider.GetGroundHit( out hit );
        bool leftWheelGrounded = vehicleWheels[ 3 ].mainWheelCollider.GetGroundHit( out hit );
        if ( leftWheelGrounded )
        {
            travelLeft = ( ( -vehicleWheels[ 3 ].mainWheelCollider.transform.InverseTransformPoint( hit.point ).y
                           - vehicleWheels[ 3 ].wheelRadius ) / vehicleWheels[ 3 ].mainWheelCollider.suspensionDistance );
        }
        if ( rightWheelGrounded )
        {
            travelRight = ( ( -vehicleWheels[ 2 ].mainWheelCollider.transform.InverseTransformPoint( hit.point ).y
                              - vehicleWheels[ 2 ].wheelRadius ) / vehicleWheels[ 2 ].mainWheelCollider.suspensionDistance );
        }

        float antiRollForce = ( travelLeft - travelRight ) * stabilizierForce;

        if ( leftWheelGrounded )
        {
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ 3 ].mainWheelCollider.transform.up
                                                             * -antiRollForce,
                                                             vehicleWheels[ 3 ].mainWheelCollider.transform.position );
        }
        if ( rightWheelGrounded )
        {
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ 2 ].mainWheelCollider.transform.up
                                                             * antiRollForce,
                                                             vehicleWheels[ 2 ].mainWheelCollider.transform.position );
        }
    }

    private void RearWheelsGrounded ( )
    {
        WheelHit wheelHit;
        foreach ( VehicleWheel wheel in vehicleWheels )
        {
            if ( wheel.wheelFrontRear == WheelPosition.FRONT_WHEEL )
            {
                continue;
            }
            if ( wheel.mainWheelCollider.GetGroundHit( out wheelHit ) )
            {
                wheelsGrounded = WheelsOnGround.REAR_WHEELS_GROUNDED;
                return;
            }
            wheelsGrounded = WheelsOnGround.REAR_WHEELS_IN_AIR;
        }
    }
}