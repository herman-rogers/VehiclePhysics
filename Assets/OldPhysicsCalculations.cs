using UnityEngine;
using System.Collections;

public class OldPhysicsCalculations : MonoBehaviour
{
    //Wheel Suspension Constants
    //private const float suspensionRange = 0.1f;
    //private const float suspensionDamper = 50.0f;
    //private const float suspensionSpringFront = 18500.0f;
    //private const float suspensionSpringRear = 9000.0f;
    //    private const float stabilizierForce = 5000.0f;
    /*******************Wheel Friction*******************/
    //private void WheelFrictionCurve ( )
    //{
    //    wheelFriction = new WheelFrictionCurve( );
    //    wheelFriction.extremumSlip = 1;
    //    wheelFriction.extremumValue = 50;
    //    wheelFriction.asymptoteSlip = 2;
    //    wheelFriction.asymptoteValue = 25;
    //    wheelFriction.stiffness = 1;
    //}

    //private void WheelFriction( )
    //{
    //    Vector3 velocity = mainVehiclePhysics.GetComponent<Rigidbody>().velocity;
    //    float squareVelocity = velocity.x * velocity.x;
    //    wheelFriction.extremumValue = Mathf.Clamp( 300 - squareVelocity, 0, 300 );
    //    wheelFriction.asymptoteValue = Mathf.Clamp( 150 - ( squareVelocity / 2 ), 0, 150 );
    //    foreach ( WheelCollider wheel in vehicleColliders )
    //    {
    //        wheel.sidewaysFriction = wheelFriction;
    //        wheel.forwardFriction = wheelFriction;
    //    }
    //}

    //public float CalculateWheelSlip ( )
    //{
    //    float wheelSlip = 0.0f;
    //    Vector3 vehicleVelocity = mainVehiclePhysics.GetComponent<Rigidbody>().velocity;
    //    float slipRatio = ( mainVehiclePhysics.engineComponent.wheelAngularVelocity
    //                        * vehicleWheels[ i_rearLeftWheel ].wheelRadius
    //                        * -vehicleVelocity.magnitude ) / vehicleVelocity.magnitude;
    //    wheelSlip = tractionConstant
    //                * slipRatio
    //                * mainVehiclePhysics.engineComponent.vehicleThrottle;
    //    wheelFriction.asymptoteSlip = wheelSlip;
    //    return wheelSlip;
    //}


    /*******************Vehicle Weight Calculations*******************/
    //private void WheelWeightDistribution( )
    //{
    //    //Front Wheel Calculations
    //    CalculateWeightDistribution( i_frontLeftWheel,
    //                                 i_frontRightWheel,
    //                                 vehicleWheels[ i_frontLeftWheel ].mainWheelCollider.transform.up,
    //                                 vehicleWheels[ i_frontRightWheel ].mainWheelCollider.transform.up );
    //    //Rear Wheel Calculations
    //    CalculateWeightDistribution( i_rearLeftWheel,
    //                                 i_rearRightWheel,
    //                                 -vehicleWheels[ i_rearLeftWheel ].mainWheelCollider.transform.up,
    //                                 -vehicleWheels[ i_rearRightWheel ].mainWheelCollider.transform.up );
    //}

    //private void CalculateWeightDistribution( int leftWheel, int rightWheel, Vector3 leftWheelForcePosition, Vector3 rightWheelForcePosition )
    //{
    //    float travelLeft = 1.0f;
    //    float travelRight = 1.0f;
    //    float antiRollForce = 0.0f;
    //    travelLeft = CalculateLatitudinalWeight( leftWheel );
    //    travelRight = CalculateLatitudinalWeight( rightWheel );
    //    antiRollForce = ( travelLeft - travelRight ) * stabilizierForce;
    //    ApplyWheelWeight( leftWheel, antiRollForce, leftWheelForcePosition );
    //    ApplyWheelWeight( rightWheel, antiRollForce, rightWheelForcePosition );
    //}

    //This is the weight distribution along the x axis of the car
    //It distributes the center of gravity from the right axis to the left
    //TODO: Improve Latitudinal Weight forces
    //private float CalculateLatitudinalWeight( int wheel )
    //{
    //    if ( vehicleWheels[ wheel ].isWheelOnGround != WheelsOnGround.WHEEL_ON_GROUND )
    //    {
    //        return 0.0f;
    //    }
    //    return ( ( -vehicleWheels[ wheel ].mainWheelCollider.transform.InverseTransformPoint( vehicleWheels[ wheel ].wheelGroundContactPoint.point ).y
    //               - vehicleWheels[ wheel ].wheelRadius ) / vehicleWheels[ wheel ].mainWheelCollider.suspensionDistance );
    //}

    //private void ApplyWheelWeight( int wheel, float rollForce, Vector3 applyDirection )
    //{
    //    float currentWheelWeightDistribution = 0.0f;
    //    CalculateLongitudinalWeight( wheel, out currentWheelWeightDistribution );
    //    if ( vehicleWheels[ wheel ].isWheelOnGround != WheelsOnGround.WHEEL_ON_GROUND )
    //    {
    //        return;
    //    }
    //    //Vehicle weight position shift based on center of gravity
    //    mainVehiclePhysics.GetComponent<Rigidbody>().AddForceAtPosition( applyDirection * currentWheelWeightDistribution,
    //                                                     vehicleWheels[ wheel ].mainWheelCollider.transform.position );
    //    //anti-roll force
    //    mainVehiclePhysics.GetComponent<Rigidbody>().AddForceAtPosition( vehicleWheels[ wheel ].mainWheelCollider.transform.up
    //                                                     * rollForce,
    //                                                     vehicleWheels[ wheel ].mainWheelCollider.transform.position );
    //}

    //This is the weight distribution along the z axis of the car.
    //It distributes the center of gravity from the hood to the trunk ( boot ).
    //private void CalculateLongitudinalWeight( int currentWheel, out float currentWheelAxis )
    //{
    //    //weight distributed between front and rear axles
    //    float vehicleMass = mainVehiclePhysics.GetComponent<Rigidbody>().mass;
    //    //Weight in Kilograms
    //    float vehicleWeight = vehicleMass * earthGravitationalConstant;
    //    Vector3 centerOfGravityPosition = mainVehiclePhysics.GetComponent<Rigidbody>().worldCenterOfMass;
    //    if ( mainVehiclePhysics.vehicleCenterOfGravity != null )
    //    {
    //        centerOfGravityPosition = mainVehiclePhysics.vehicleCenterOfGravity.transform.position;
    //    }
    //    float centerGravityDistanceToFrontAxle = Vector3.Distance( centerOfGravityPosition, vehicleWheels[ 0 ].visualWheel.position );
    //    float centerGravityDistanceToRearAxle = Vector3.Distance( centerOfGravityPosition, vehicleWheels[ 2 ].visualWheel.position );
    //    //TODO: Improve and use the friction coefficient
    //    float frictionCoefficient = 1.0f;
    //    float wheelFrictionLimit = frictionCoefficient * vehicleWeight;

    //    if( vehicleWheels[ currentWheel ].wheelFrontRear == WheelPosition.FRONT_WHEEL )
    //    {
    //        currentWheelAxis = ( centerGravityDistanceToFrontAxle / wheelBase ) * vehicleWeight
    //                             - ( ( centerOfGravityPosition.y / wheelBase ) *
    //                             vehicleMass * mainVehiclePhysics.engineComponent.vehicleThrottle );
    //        return;
    //    }
    //    currentWheelAxis = ( centerGravityDistanceToRearAxle / wheelBase ) * vehicleWeight
    //                         + ( ( centerOfGravityPosition.y / wheelBase ) *
    //                         vehicleMass * mainVehiclePhysics.engineComponent.vehicleThrottle );
    //}

    //private void WheelSuspension ( )
    //{
    //    foreach ( VehicleWheel wheel in vehicleWheels )
    //    {
    //        WheelCollider currentWheelCollider = wheel.mainWheelCollider;
    //        currentWheelCollider.suspensionDistance = suspensionRange;
    //        JointSpring wheelJointSpring = currentWheelCollider.suspensionSpring;
    //        if ( wheel.wheelFrontRear == WheelPosition.FRONT_WHEEL )
    //        {
    //            wheelJointSpring.spring = suspensionSpringFront;
    //        }
    //        else
    //        {
    //            wheelJointSpring.spring = suspensionSpringRear;
    //        }
    //        wheelJointSpring.damper = suspensionDamper;
    //        currentWheelCollider.suspensionSpring = wheelJointSpring;
    //    }
    //}

    //mainWheelCollider.motorTorque = 500 * Input.GetAxis( "Acceleration" );
    //GetComponent<Rigidbody>().AddForce( transform.forward
    //                    * Time.deltaTime
    //                    * totalLongitudinalForces, ForceMode.Acceleration );
    //GetComponent<Rigidbody>().AddForce( transform.forward
    //                    * Time.deltaTime
    //                    * ( totalBrakeForce ), ForceMode.Acceleration );
    //public void RotateDrivingWheel( Vector3 rearRotation )
    //{
    //    if ( wheelPhysicsComponent.mainVehiclePhysics.engineComponent.vehicleThrottle > 0.0f )
    //    {
    //        float rearWheelVelocity = ( ( wheelPhysicsComponent.mainVehiclePhysics.engineComponent.vehicleThrottle * 10 ) 
    //                                      * rearRotation.z );
    //        visualWheel.Rotate( Vector3.right
    //                            * ( rearWheelVelocity / wheelRadius )
    //                            * Time.deltaTime
    //                            * Mathf.Rad2Deg );
    //    }
    //    else
    //    {
    //        visualWheel.Rotate( Vector3.right
    //                            * ( rearRotation.z / wheelRadius )
    //                            * Time.deltaTime * Mathf.Rad2Deg );
    //    }
    //}
    //public float SteeringAngle ( )
    //{
    //    return mainVehiclePhysics.engineComponent.steer * wheelAngleDegrees;
    //}
    //mainVehiclePhysics.GetComponent<Rigidbody>().AddForceAtPosition( new Vector3( driftForceAcceleration, 0, 0 ),
    //                                                 vehicleWheels[ i_rearLeftWheel ].visualWheel.position );

    //vehicleTransform.RotateAround( mainVehiclePhysics.vehicleCenterOfGravity.transform.position + mainVehiclePhysics.vehicleCenterOfGravity.transform.right * driftForceAcceleration * mainVehiclePhysics.engineComponent.steer,
    //                               mainVehiclePhysics.vehicleCenterOfGravity.transform.position,
    //                               driftForceAcceleration * Mathf.Rad2Deg * Time.deltaTime );
    /*******************Vehicle Steering Calculations*******************/


    //private void SpeedTurnRatio( )
    //{
    //float vehicleMass = mainVehiclePhysics.GetComponent<Rigidbody>().mass;
    //float vehicleWeight = vehicleMass * earthGravitationalConstant;
    //float vehicleVelocity = mainVehiclePhysics.engineComponent.vehicleSpeed;
    ////** Low Speed Turning **//
    ////Wheel Turning Radius "R" from Meters to Feet ( 3.28 )
    //float turningRadius = ( wheelBase / Mathf.Cos( maxSteeringAngle ) * 3.28f );
    //float wheelAngle = Mathf.Acos( wheelBase / turningRadius );
    //wheelAngleDegrees = 57.29f / wheelAngle;
    //float angularVelocity =  ( vehicleVelocity / ( 2 * 3.14f * turningRadius ) );
    ////Applying steering to the vehicle turning
    //Vector3 angularVelocityVector = new Vector3( 0, angularVelocity, 0 );
    //Quaternion deltaAngularRotation = Quaternion.Euler( angularVelocityVector * Time.deltaTime );
    ////Low speed calculations
    //mainVehiclePhysics.GetComponent<Rigidbody>().MoveRotation( mainVehiclePhysics.GetComponent<Rigidbody>().rotation
    //                                           * deltaAngularRotation );

    ////** High Speed Turning **//
    //float velocitySquared = vehicleVelocity * vehicleVelocity;
    //Transform vehicleTransform = mainVehiclePhysics.transform;

    //float centripedalForce = ( vehicleMass * velocitySquared ) / turningRadius;

    ////0.7 is the friction of rubber 
    //float coefficientOfFriction = ( 0.7f * ( wheelBase / turningRadius ) * velocitySquared ) 
    //                              / wheelBase;
    //float excessForce = ( 0.7f * ( wheelBase / turningRadius ) * velocitySquared )
    //                    / ( wheelBase - coefficientOfFriction );
    ////The amount of acceleration caused by drifting
    //float driftForceAcceleration = excessForce / vehicleMass;
    //}
    //private void UpdateVehicleDrag ( Vector3 vehicleVelocity )
    //{
    //    Vector3 relativeDrag = new Vector3( -vehicleVelocity.x * Mathf.Abs( vehicleVelocity.x ),
    //                                        -vehicleVelocity.y * Mathf.Abs( vehicleVelocity.y ),
    //                                        -vehicleVelocity.z * Mathf.Abs( vehicleVelocity.z ) );
    //    Vector3 drag = Vector3.Scale( dragMultiplier, relativeDrag );
    //    //Add in Hand brake here, adding code that runs when handbrake is not activated
    //    drag.x *= speedCap / vehicleVelocity.magnitude;
    //    if ( Mathf.Abs( vehicleVelocity.x ) < 5 )
    //    {
    //        drag.x = -vehicleVelocity.x * dragMultiplier.x;
    //    }
    //    GetComponent<Rigidbody>().AddForce( transform.TransformDirection( drag ) * GetComponent<Rigidbody>().mass * Time.deltaTime );
    //}
    //Vector3 wheelVelocity = wheelPhysicsComponent.mainVehiclePhysics.GetComponent<Rigidbody>( ).GetPointVelocity( wheelGroundContactPoint.point );
    //Transform visualWheel = mainWheelCollider.transform.GetChild( 0 );
    //Vector3 localWheelVelocity = visualWheel.parent.InverseTransformDirection( wheelVelocity );
    //public void VerticalWheelRotation ( Vector3 frontVelocity )
    //{
    //    Vector3 cachedWheelRotation = visualWheel.parent.localEulerAngles;
    //    visualWheel.parent.localEulerAngles = cachedWheelRotation;
    //    visualWheel.Rotate( Vector3.right
    //                        * ( frontVelocity.z / wheelRadius )
    //                        * Time.deltaTime * Mathf.Rad2Deg );
    //}
}