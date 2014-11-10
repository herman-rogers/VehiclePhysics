using UnityEngine;
using System.Collections;
public class VehiclePhysics : MonoBehaviour
{
    public WheelCollider frontRightWheel;
    public WheelCollider frontLeftWheel;
    public WheelCollider rearRightWheel;
    public WheelCollider rearLeftWheel;
    public const int numberOfGears = 6;
    public const float speedCap = 180;
    public GearsComponent gearsComponent { get; private set; }
    public EngineComponent engineComponent { get; private set; }
    public WheelComponent wheelComponent { get; private set; }
    public WheelCollider[ ] vehicleWheels { get; private set; }
    private float steer;
    private float motor;
    private float brake;

    private void Start ( )
    {
        vehicleWheels = new WheelCollider[ ]{ frontRightWheel, 
                                              frontLeftWheel, 
                                              rearRightWheel, 
                                              rearLeftWheel };
        gearsComponent = new GearsComponent( 6, 180 );
        wheelComponent = new WheelComponent( gameObject );
        engineComponent = gameObject.AddComponent<EngineComponent>( );
        engineComponent.Start( );
        UnityWheelBugFix( );
    }

    private void UnityWheelBugFix ( )
    {
        foreach ( WheelCollider wheel in vehicleWheels )
        {
            wheel.transform.position = new Vector3( wheel.transform.position.x,
                                                    wheel.transform.position.y,
                                                    wheel.transform.position.z );
        }
    }

    private void Update ( )
    {
        Vector3 vehicleVelocity = transform.InverseTransformDirection( GetComponent<Rigidbody>( ).velocity );
        gearsComponent.UpdateGears( vehicleVelocity );
        engineComponent.EngineUpdate( );
    }

    private void FixedUpdate ( )
    {
        engineComponent.EngineFixedUpdate( );
        wheelComponent.WheelFixedUpdate( );
    }
}