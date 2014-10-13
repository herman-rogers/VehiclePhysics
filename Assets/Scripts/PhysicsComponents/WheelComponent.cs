using UnityEngine;
using System.Collections;

public class WheelComponent
{
    public VehiclePhysics mainVehiclePhysics;
    public VehicleWheel[ ] vehicleWheels { get; private set; }
    public float getDistanceBetweenRearWheels { get; private set; }
    public float wheelBase { get; private set; }
    private WheelFrictionCurve wheelFriction;
    private WheelCollider[ ] vehicleColliders;
    private float wheelAngleDegrees;
    private const int i_frontRightWheel = 0;
    private const int i_frontLeftWheel = 1;
    private const int i_rearRightWheel = 2;
    private const int i_rearLeftWheel = 3;
    //Wheel weight distribution constants
    private const float earthGravitationalConstant = 9.8f;
    private const float stabilizierForce = 5000.0f;
    //Wheel Suspension Constants
    private const float suspensionRange = 0.1f;
    private const float suspensionDamper = 50.0f;
    private const float suspensionSpringFront = 18500.0f;
    private const float suspensionSpringRear = 9000.0f;
    //Wheel Steering Constants
    private const float maxSteeringAngle = 25.0f;

    public WheelComponent( GameObject vehiclePhysics )
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

    public void WheelUpdate( Vector3 velocity )
    {
    }

    public void WheelFixedUpdate( Vector3 velocity )
    {
        WheelFriction( velocity );
        WheelWeightDistribution( );
        SpeedTurnRatio( );
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

    private void WheelFrictionCurve( )
    {
        wheelFriction = new WheelFrictionCurve( );
        wheelFriction.extremumSlip = 1;
        wheelFriction.extremumValue = 50;
        wheelFriction.asymptoteSlip = 2;
        wheelFriction.asymptoteValue = 25;
        wheelFriction.stiffness = 1;
    }

    private void WheelSuspension( )
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

    private void WheelFriction( Vector3 velocity )
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

    /*Vehicle Steering Calculations*/
    public float SteeringAngle( )
    {
        return mainVehiclePhysics.engineComponent.steer * wheelAngleDegrees;
    }

    private void SpeedTurnRatio( )
    {
        float vehicleMass = mainVehiclePhysics.rigidbody.mass;
        float vehicleWeight = vehicleMass * earthGravitationalConstant;
        float vehicleVelocity = mainVehiclePhysics.engineComponent.vehicleSpeed;
        //Low Speed Turning
        //Wheel Turning Radius "R" from Meters to Feet ( 3.28 )
        float turningRadius = ( wheelBase / Mathf.Cos( maxSteeringAngle ) * 3.28f );
        float wheelAngle = Mathf.Acos( wheelBase / turningRadius );
        wheelAngleDegrees = 57.29f / wheelAngle;
        float angularVelocity =  ( vehicleVelocity / ( 2 * 3.14f * turningRadius ) );
        //High Speed Turning
        float velocitySquared = vehicleVelocity * vehicleVelocity;
        //0.7 is the fiction of rubber
        float centripedalForce = ( vehicleMass * velocitySquared / turningRadius );
        float coefficientOfFriction = ( ( 0.7f * ( wheelBase / turningRadius ) 
                                        * velocitySquared ) / wheelBase ) * vehicleWeight;
        float driftForce = centripedalForce / coefficientOfFriction;

        float slipRatio = ( mainVehiclePhysics.engineComponent.wheelAngularVelocity
                            * vehicleWheels[ i_rearLeftWheel ].wheelRadius
                            - vehicleVelocity ) / Mathf.Abs( vehicleVelocity ) ;

        Debug.Log( slipRatio );

        //Applying steering to the vehicle turning
        Vector3 angularVelocityVector = new Vector3( 0, angularVelocity, 0 );
        Quaternion deltaAngularRotation = Quaternion.Euler( angularVelocityVector * Time.deltaTime );
        //Low speed calculations
        mainVehiclePhysics.rigidbody.MoveRotation( mainVehiclePhysics.rigidbody.rotation 
                                                   * deltaAngularRotation );
        //mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ i_rearLeftWheel ].visualWheel.transform.right
        //                                                  * driftForce,
        //                                                  vehicleWheels[ i_rearLeftWheel ].visualWheel.position );

        // add in angular velocity here
        //return angluarVelocity;

        //High Speed Turning
        //sideslip angle of the car, the angular rotation of the car around the up axis ( yaw ),
        //and the steering angle

        //cornering for per tire slipangle = corninering stiffness * slip angle

        //Beta = arctan( Vy / Vx );
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

    /*Vehicle Weight Calculations*/
    private void WheelWeightDistribution( )
    {
        //Front Wheel Calculations
        CalculateWeightDistribution( i_frontLeftWheel,
                                     i_frontRightWheel,
                                     vehicleWheels[ i_frontLeftWheel ].mainWheelCollider.transform.up,
                                     vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.up );
        //Rear Wheel Calculations
        CalculateWeightDistribution( i_rearLeftWheel,
                                     i_rearRightWheel,
                                     Vector3.down,
                                     Vector3.down );
    }

    private void CalculateWeightDistribution( int leftWheel, int rightWheel, Vector3 leftWheelForcePosition, Vector3 rightWheelForcePosition )
    {
        float travelLeft = 1.0f;
        float travelRight = 1.0f;
        float antiRollForce = 0.0f;
        travelLeft = CalculateLatitudinalWeight( leftWheel );
        travelRight = CalculateLatitudinalWeight( rightWheel );
        antiRollForce = ( travelLeft - travelRight ) * stabilizierForce;
        ApplyWheelWeight( leftWheel, antiRollForce, leftWheelForcePosition );
        ApplyWheelWeight( rightWheel, antiRollForce, rightWheelForcePosition );
    }

    //This is the weight distribution along the x axis of the car
    //It distributes the center of gravity from the right axis to the left
    //TODO: Improve Latitudinal Weight forces
    private float CalculateLatitudinalWeight( int wheel )
    {
        if ( vehicleWheels[ wheel ].isWheelOnGround != WheelsOnGround.WHEEL_ON_GROUND )
        {
            return 0.0f;
        }
        return ( ( -vehicleWheels[ wheel ].mainWheelCollider.transform.InverseTransformPoint( vehicleWheels[ wheel ].wheelGroundContactPoint.point ).y
                   - vehicleWheels[ wheel ].wheelRadius ) / vehicleWheels[ wheel ].mainWheelCollider.suspensionDistance );
    }

    private void ApplyWheelWeight( int wheel, float rollForce, Vector3 applyDirection )
    {
        float currentWheelWeightDistribution = 0.0f;
        CalculateLongitudinalWeight( wheel, out currentWheelWeightDistribution );
        if ( vehicleWheels[ wheel ].isWheelOnGround != WheelsOnGround.WHEEL_ON_GROUND )
        {
            return;
        }
        //Vehicle weight position shift based on center of gravity
        mainVehiclePhysics.rigidbody.AddForceAtPosition( applyDirection * currentWheelWeightDistribution,
                                                         vehicleWheels[ wheel ].mainWheelCollider.transform.position );
        //anti-roll force
        mainVehiclePhysics.rigidbody.AddForceAtPosition( vehicleWheels[ wheel ].mainWheelCollider.transform.up
                                                         * rollForce,
                                                         vehicleWheels[ wheel ].mainWheelCollider.transform.position );
    }

    //This is the weight distribution along the z axis of the car.
    //It distributes the center of gravity from the hood to the trunk ( boot ).
    private void CalculateLongitudinalWeight( int currentWheel, out float currentWheelAxis )
    {
        //weight distributed between front and rear axles
        float vehicleMass = mainVehiclePhysics.rigidbody.mass;
        //Weight in Kilograms
        float vehicleWeight = vehicleMass * earthGravitationalConstant;
        Vector3 centerOfGravityPosition = mainVehiclePhysics.rigidbody.worldCenterOfMass;
        float centerGravityDistanceToFrontAxle = Vector3.Distance( centerOfGravityPosition, vehicleWheels[ 0 ].visualWheel.position );
        float centerGravityDistanceToRearAxle = Vector3.Distance( centerOfGravityPosition, vehicleWheels[ 2 ].visualWheel.position );

        //TODO: Improve and use the friction coefficient
        float frictionCoefficient = 1.0f;
        float wheelFrictionLimit = frictionCoefficient * vehicleWeight;

        if( vehicleWheels[ currentWheel ].wheelFrontRear == WheelPosition.FRONT_WHEEL )
        {
            currentWheelAxis = ( centerGravityDistanceToFrontAxle / wheelBase ) * vehicleWeight
                                 - ( ( centerOfGravityPosition.y / wheelBase ) *
                                 vehicleMass * mainVehiclePhysics.engineComponent.vehicleThrottle );
            return;
        }
        currentWheelAxis = ( centerGravityDistanceToRearAxle / wheelBase ) * vehicleWeight
                             + ( ( centerOfGravityPosition.y / wheelBase ) *
                             vehicleMass * mainVehiclePhysics.engineComponent.vehicleThrottle );
    }
}