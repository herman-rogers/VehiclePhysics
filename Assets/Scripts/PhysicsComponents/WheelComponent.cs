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

public struct VehicleWheel
{
    public WheelPosition wheelFrontRear;
    public WheelPosition wheelLeftRight;
    public WheelsOnGround currenWheelOnGround;
    public WheelHit wheelGroundContactPoint;
    public WheelCollider mainWheelCollider;
    public WheelComponent wheelPhysics;
    public Transform visualWheel;
    public float wheelRadius;

    public VehicleWheel ( WheelCollider vehicleWheel,
                          WheelComponent wheelComponent,
                          WheelPosition frontRear,
                          WheelPosition leftRight )
    {
        wheelFrontRear = frontRear;
        wheelLeftRight = leftRight;
        currenWheelOnGround = WheelsOnGround.WHEEL_ON_GROUND;
        mainWheelCollider = vehicleWheel;
        wheelPhysics = wheelComponent;
        visualWheel = vehicleWheel.GetComponentInChildren<MeshRenderer>( ).transform;
        wheelRadius = ( visualWheel.renderer.bounds.size.y / 2 );
        mainWheelCollider.radius = wheelRadius;
        mainWheelCollider.GetGroundHit( out wheelGroundContactPoint );
    }

    public void UpdateWheel( )
    {
        if( mainWheelCollider.GetGroundHit( out wheelGroundContactPoint ) )
        {
            currenWheelOnGround = WheelsOnGround.WHEEL_ON_GROUND;
            UpdateWheelGraphic( );
            return;
        }
        currenWheelOnGround = WheelsOnGround.WHEEL_IN_AIR;
    }

    private void UpdateWheelGraphic ( )
    {
        Vector3 wheelVelocity = wheelPhysics.mainVehiclePhysics.rigidbody.GetPointVelocity( wheelGroundContactPoint.point );
        Vector3 localWheelVelocity = visualWheel.parent.InverseTransformDirection( wheelVelocity );
        if ( wheelFrontRear == WheelPosition.FRONT_WHEEL )
        {
            wheelPhysics.SteeringRotation( this, localWheelVelocity );
            return;
        }
        wheelPhysics.RotateDrivingWheel( this, localWheelVelocity );
    }
}

public class WheelComponent
{
    public VehicleWheel[ ] vehicleWheels { get; private set; }
    public float distanceBetweenAxles { get; private set; }
    public float getDistanceBetweenRearWheels { get; private set; }
    public static WheelsOnGround wheelsGrounded;
    public VehiclePhysics mainVehiclePhysics;
    private WheelFrictionCurve wheelFriction;
    private WheelCollider[ ] vehicleColliders;
    private float stabilizierForce = 5000.0f;
    private float suspensionRange = 0.1f;
    private float suspensionDamper = 50.0f;
    private float suspensionSpringFront = 18500.0f;
    private float suspensionSpringRear = 9000.0f;
    private const float earthGravitationalConstant = 9.8f;
    private const int i_frontRightWheel = 0;
    private const int i_frontLeftWheel = 1;
    private const int i_rearRightWheel = 2;
    private const int i_rearLeftWheel = 3;

    public WheelComponent ( GameObject vehiclePhysics )
    {
        mainVehiclePhysics = vehiclePhysics.GetComponent<VehiclePhysics>( );
        vehicleColliders = mainVehiclePhysics.vehicleWheels;
        SetupWheelObjects( );
        WheelSuspension( );
        WheelFrictionCurve( );
        CalculateDistanceBetweenAxles( );
        CalculateDistanceBetweenRearWheels( );
    }

    //Wheels in the VehicleWheel array are set up as frontwheels = 0,1. rearwheels = 2,3
    private void SetupWheelObjects ( )
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

    public void WheelUpdate ( Vector3 velocity )
    {
    }

    public void WheelFixedUpdate ( Vector3 velocity )
    {
        WheelFriction( velocity );
        WheelWeightDistribution( );
        UpdateVehicleWheels( );
    }

    private void UpdateVehicleWheels ( )
    {
        vehicleWheels[ i_frontRightWheel ].UpdateWheel( );
        vehicleWheels[ i_frontLeftWheel ].UpdateWheel( );
        vehicleWheels[ i_rearRightWheel ].UpdateWheel( );
        vehicleWheels[ i_rearLeftWheel ].UpdateWheel( );
    }

    //Used to calculate the steering angle
    private void CalculateDistanceBetweenAxles ( )
    {
        Vector3 frontAxlePosition = vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.position;
        Vector3 rearAxlePosition = vehicleWheels[ i_frontLeftWheel ].mainWheelCollider.transform.position;
        distanceBetweenAxles = Vector3.Distance( frontAxlePosition, rearAxlePosition );
    }

    //Used to calculate that angle theta to find the steering angle ( 90 - theta )
    private void CalculateDistanceBetweenRearWheels ( )
    {
        Vector3 rearRightWheel = vehicleWheels[ i_rearRightWheel ].mainWheelCollider.transform.position;
        Vector3 rearLeftWheel = vehicleWheels[ i_rearLeftWheel ].mainWheelCollider.transform.position;
        getDistanceBetweenRearWheels = Vector3.Distance( rearRightWheel, rearLeftWheel );
    }

    public void SteeringRotation ( VehicleWheel frontWheel, Vector3 frontVelocity )
    {
        //Rotate the wheels horizontally
        Vector3 cachedWheelRotation = frontWheel.visualWheel.parent.localEulerAngles;
        cachedWheelRotation.y = SteeringAngle( );
        //float test = SpeedTurnRatio( );
        frontWheel.visualWheel.parent.localEulerAngles = cachedWheelRotation;
        //Rotate the wheels vertically
        frontWheel.visualWheel.Rotate( Vector3.right
                                       * ( frontVelocity.z / frontWheel.wheelRadius )
                                       * Time.deltaTime * Mathf.Rad2Deg );
    }

    public void RotateDrivingWheel ( VehicleWheel rearWheel, Vector3 rearRotation )
    {
        if ( mainVehiclePhysics.engineComponent.vehicleThrottle > 0.0f )
        {
            float rearWheelVelocity = ( ( mainVehiclePhysics.engineComponent.vehicleThrottle * 10 ) *
            rearRotation.z );
            rearWheel.visualWheel.Rotate( Vector3.right
                                          * ( rearWheelVelocity / rearWheel.wheelRadius )
                                          * Time.deltaTime
                                          * Mathf.Rad2Deg );
        }
        else
        {
            rearWheel.visualWheel.Rotate( Vector3.right
                                          * ( rearRotation.z / rearWheel.wheelRadius )
                                          * Time.deltaTime * Mathf.Rad2Deg );
        }
    }

    private float SteeringAngle ( )
    {
        return mainVehiclePhysics.engineComponent.steer * 25;
    }

    private float SpeedTurnRatio ( )
    {
        //Low Speed Turning
        //This is the opposite side of our triangle

        Vector3 frontRightWheelRaycast = vehicleWheels[ i_frontRightWheel ].visualWheel.TransformDirection(
                                         vehicleWheels[ i_frontRightWheel ].visualWheel.right );
        Vector3 frontLeftWheelRaycast = vehicleWheels[ i_frontLeftWheel ].visualWheel.TransformDirection(
                                       -vehicleWheels[ i_frontLeftWheel ].visualWheel.right );
        Debug.DrawRay( vehicleWheels[ 0 ].visualWheel.position, vehicleWheels[ 0 ].visualWheel.right * 9, Color.cyan );
        Debug.DrawRay( vehicleWheels[ 1 ].visualWheel.position, -vehicleWheels[ 1 ].visualWheel.right * 9, Color.red );
        Debug.DrawRay( vehicleWheels[ 2 ].visualWheel.position, vehicleWheels[ 2 ].visualWheel.right * 9, Color.magenta );
        Debug.DrawRay( vehicleWheels[ 3 ].visualWheel.position, -vehicleWheels[ 3 ].visualWheel.right * 9, Color.blue );
        float wheelBase = distanceBetweenAxles;

        //This is the adjacent side of our triangle
        float adjacentSide = getDistanceBetweenRearWheels;

        //TODO: Better calculation for the circle radius
        float deltaAngle = Mathf.Atan( wheelBase / adjacentSide );
        float circleRadius = wheelBase / Mathf.Sin( deltaAngle );


        float angluarVelocity = mainVehiclePhysics.engineComponent.vehicleSpeed / circleRadius;

        // add in angular velocity here
        return angluarVelocity;

        //High Speed Turning
        //sideslip angle of the car, the angular rotation of the car around the up axis ( yaw ),
        //and the steering angle

        //cornering for per tire slipangle = corninering stiffness * slip angle

        //Beta = arctan( Vy / Vx );
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

    private void WheelWeightDistribution ( )
    {
        float travelLeft = 1.0f;
        float travelRight = 1.0f;
        float frontWheelWeight = 0.0f;
        float rearWheelWeight = 0.0f;
        bool frontRightWheelGrounded = vehicleWheels[ i_frontRightWheel ].currenWheelOnGround == WheelsOnGround.WHEEL_ON_GROUND;
        bool frontLeftWheelGrounded = vehicleWheels[ i_frontLeftWheel ].currenWheelOnGround == WheelsOnGround.WHEEL_ON_GROUND;
        bool rearRightWheelGrounded = vehicleWheels[ i_rearRightWheel ].currenWheelOnGround == WheelsOnGround.WHEEL_ON_GROUND;
        bool rearLeftWheelGrounded = vehicleWheels[ i_rearLeftWheel ].currenWheelOnGround == WheelsOnGround.WHEEL_ON_GROUND;
        CalculateLongitudinalWeight( out frontWheelWeight, out rearWheelWeight );

        if ( frontLeftWheelGrounded )
        {
            travelLeft = CalculateWheelStabilization( i_frontLeftWheel );
        }
        if ( frontRightWheelGrounded )
        {
            travelRight = CalculateWheelStabilization( i_frontRightWheel );
        }

        float antiRollForce = ( travelLeft - travelRight ) * stabilizierForce;

        if ( frontLeftWheelGrounded )
        {
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ i_frontLeftWheel ].mainWheelCollider.transform.up * frontWheelWeight,
                                                             vehicleWheels[ i_frontLeftWheel ].mainWheelCollider.transform.position );
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ i_frontLeftWheel ].mainWheelCollider.transform.up
                                                             * antiRollForce,
                                                             vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.position );
        }
        if ( frontRightWheelGrounded )
        {
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.up * frontWheelWeight,
                                                             vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.position );
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.up
                                                             * antiRollForce,
                                                             vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.position );
        }

        //Rear Wheel Calculations
        if ( rearLeftWheelGrounded )
        {
            travelLeft = CalculateWheelStabilization( i_rearLeftWheel );
        }
        if ( rearRightWheelGrounded )
        {
            travelRight = CalculateWheelStabilization( i_rearRightWheel );
        }

        antiRollForce = ( travelLeft - travelRight ) * stabilizierForce;

        if ( rearLeftWheelGrounded )
        {
            mainVehiclePhysics.rigidbody.AddForceAtPosition( Vector3.down * rearWheelWeight,
                                                             vehicleWheels[ i_rearLeftWheel ].mainWheelCollider.transform.position );

            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ i_rearLeftWheel ].mainWheelCollider.transform.up
                                                             * antiRollForce,
                                                             vehicleWheels[ i_rearLeftWheel ].mainWheelCollider.transform.position );
        }
        if ( rearRightWheelGrounded )
        {
            //frontal physics
            mainVehiclePhysics.rigidbody.AddForceAtPosition( Vector3.down * rearWheelWeight,
                                                             vehicleWheels[ i_rearRightWheel ].mainWheelCollider.transform.position );
            //anti roll force
            mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ i_rearRightWheel ].mainWheelCollider.transform.up
                                                             * antiRollForce,
                                                             vehicleWheels[ i_rearRightWheel ].mainWheelCollider.transform.position );
        }
    }

    private float CalculateWheelStabilization ( int wheel )
    {
        return ( ( -vehicleWheels[ wheel ].mainWheelCollider.transform.InverseTransformPoint( vehicleWheels[ wheel ].wheelGroundContactPoint.point ).y
                  - vehicleWheels[ wheel ].wheelRadius ) / vehicleWheels[ wheel ].mainWheelCollider.suspensionDistance );
    }

    //This is the weight distribution along the z axis of the car.
    //It distributes the center of gravity from the hood to the trunk ( boot ).
    private void CalculateLongitudinalWeight ( out float frontWheelAxis, out float rearWheelAxis )
    {
        //weight distributed between front and rear axles
        float vehicleMass = mainVehiclePhysics.rigidbody.mass;
        //Weight in Kilograms
        float vehicleWeight = vehicleMass * earthGravitationalConstant;
        Vector3 centerOfGravityPosition = mainVehiclePhysics.rigidbody.worldCenterOfMass;
        float centerGravityDistanceToFrontAxle = Vector3.Distance( centerOfGravityPosition, vehicleWheels[ 0 ].visualWheel.position );
        float centerGravityDistanceToRearAxle = Vector3.Distance( centerOfGravityPosition, vehicleWheels[ 2 ].visualWheel.position );
        float frictionCoefficient = 1.0f;
        float wheelBase = distanceBetweenAxles;
        float wheelFrictionLimit = frictionCoefficient * vehicleWeight;

        frontWheelAxis = ( centerGravityDistanceToFrontAxle / wheelBase ) * vehicleWeight
                             - ( ( centerOfGravityPosition.y / wheelBase ) *
                             vehicleMass * mainVehiclePhysics.engineComponent.vehicleThrottle );

        rearWheelAxis = ( centerGravityDistanceToRearAxle / wheelBase ) * vehicleWeight
                            + ( ( centerOfGravityPosition.y / wheelBase ) *
                            vehicleMass * mainVehiclePhysics.engineComponent.vehicleThrottle );
    }

    //private void ApplySteering ( Vector3 velocity )
    //{
    //    //double turnRadius = 3.0 / Mathf.Sin( ( 90 - ( steer * 30 ) ) * Mathf.Deg2Rad );
    //    //float minMaxTurn = SpeedTurnRatio( );
    //    //float turnSpeed = Mathf.Clamp( velocity.z / ( float )turnRadius, -minMaxTurn / 10, minMaxTurn / 10 );
    //    //transform.RotateAround( transform.position + transform.right * ( float )turnRadius * steer,
    //    //                        transform.up,
    //    //                        turnSpeed * Mathf.Rad2Deg * Time.deltaTime * steer );
    //}

}