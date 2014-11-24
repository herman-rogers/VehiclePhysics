using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftFrontWheel;
    public WheelCollider rightFrontWheel;
    public bool motor;
    public bool steering;
}

public class TestController : MonoBehaviour
{
    public List<AxleInfo> axleInformation;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    private void Start ( )
    {
        UnityWheelBugFix( );
    }

    private void UnityWheelBugFix ( )
    {
        foreach ( AxleInfo wheel in axleInformation )
        {
            wheel.leftFrontWheel.transform.position = new Vector3( wheel.leftFrontWheel.transform.position.x,
                                                                   wheel.leftFrontWheel.transform.position.y,
                                                                   wheel.leftFrontWheel.transform.position.z );
        }
    }

    public void FixedUpdate ( )
    {
        //float motor = maxMotorTorque * Input.GetAxis( "Vertical" );
        //float steering = maxSteeringAngle * Input.GetAxis( "Horizontal" );

        foreach ( AxleInfo infor in axleInformation )
        {
            //if ( infor.steering )
            //{
            //    infor.leftFrontWheel.steerAngle = steering;
            //    infor.rightFrontWheel.steerAngle = steering;
            //}
            if ( infor.motor )
            {
                //infor.leftFrontWheel.motorTorque = motor;
                //infor.rightFrontWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals( infor.leftFrontWheel );
            ApplyLocalPositionToVisuals( infor.rightFrontWheel );
        }
    }

    public void ApplyLocalPositionToVisuals ( WheelCollider wheel )
    {
        if ( wheel.transform.childCount == 0 )
        {
            return;
        }
        //Transform visualWheel = wheel.transform.GetChild( 0 );
        //Vector3 position = new Vector3( 0, 0, 0 );
        //Quaternion rotation = Quaternion.identity;
        //wheel.GetLocalPose( out position, out rotation );
        //visualWheel.transform.position = wheel.transform.parent.TransformPoint( position );
        //visualWheel.transform.rotation = wheel.transform.parent.rotation * rotation;
    }
}
