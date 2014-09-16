using UnityEngine;
using System.Collections;

public class WheelComponent
{
    private WheelFrictionCurve wheelFriction;
    private WheelCollider[ ] vehicleWheels;
    private WheelCollider frontRightWheel;
    private WheelCollider frontLeftWheel;
    private WheelCollider rearRightWheel;
    private WheelCollider rearLeftWheel;
    private Rigidbody vehicleRigidBody;
    private float stabilizierForce = 5000.0f;

    //Suspension
    private float suspensionRange = 0.1f;
    private float suspensionDamper = 50.0f;
    private float suspensionSpringFront = 18500;
    private float suspensionSpringRear = 9000.0f;

    public WheelComponent ( WheelCollider[ ] wheels, Rigidbody rigidBody )
    {
        vehicleWheels = wheels;
        frontRightWheel = wheels[ 0 ];
        frontLeftWheel = wheels[ 1 ];
        rearRightWheel = wheels[ 2 ];
        rearLeftWheel = wheels[ 3 ];
        vehicleRigidBody = rigidBody;
        WheelSuspension( frontRightWheel, true );
        WheelSuspension( frontLeftWheel, true );
        WheelSuspension( rearRightWheel, false );
        WheelSuspension( rearLeftWheel, false );
        WheelFrictionCurve( );
    }

    public void WheelFixedUpdate ( Vector3 velocity )
    {
        WheelFriction( velocity );
        FrontStabilizerBar( );
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

    private void WheelSuspension( WheelCollider currentWheel, bool isFrontWheel )
    {
        currentWheel.suspensionDistance = suspensionRange;
        JointSpring wheelJointSpring = currentWheel.suspensionSpring;
        if( isFrontWheel )
        {
            wheelJointSpring.spring = suspensionSpringFront;
        }
        else
        {
            wheelJointSpring.spring = suspensionSpringRear;
        }
        wheelJointSpring.damper = suspensionDamper;
        currentWheel.suspensionSpring = wheelJointSpring;
    }

    private void WheelFriction ( Vector3 velocity )
    {
        float squareVelocity = velocity.x * velocity.x;
        wheelFriction.extremumValue = Mathf.Clamp( 300 - squareVelocity, 0, 300 );
        wheelFriction.asymptoteValue = Mathf.Clamp( 150 - ( squareVelocity / 2 ), 0, 150 );
        foreach ( WheelCollider wheel in vehicleWheels )
        {
            wheel.sidewaysFriction = wheelFriction;
            wheel.forwardFriction = wheelFriction;
        }
    }

    private void FrontStabilizerBar ( )
    {
        WheelHit wheelHit;
        float travelLeft = 1.0f;
        float traveltRight = 1.0f;
        if ( frontLeftWheel.GetGroundHit( out wheelHit ) )
        {
            travelLeft = ( ( -frontLeftWheel.transform.InverseTransformPoint( wheelHit.point ).y
                             - frontLeftWheel.radius ) / frontLeftWheel.suspensionDistance );
        }
        if ( frontRightWheel.GetGroundHit( out wheelHit ) )
        {
            traveltRight = ( ( -frontRightWheel.transform.InverseTransformPoint( wheelHit.point ).y
                               - frontRightWheel.radius ) / frontRightWheel.suspensionDistance );
        }
        float antiRollForce = ( travelLeft - traveltRight ) * stabilizierForce;
        if ( frontLeftWheel.GetGroundHit( out wheelHit ) )
        {
            vehicleRigidBody.AddForceAtPosition( frontLeftWheel.transform.up * -antiRollForce,
                                         frontLeftWheel.transform.position );
        }
        if ( frontRightWheel.GetGroundHit( out wheelHit ) )
        {
            vehicleRigidBody.AddForceAtPosition( frontRightWheel.transform.up * antiRollForce,
                                                 frontRightWheel.transform.position );
        }

    }

    private void RearStabilizerBar ( )
    {

    }
}
