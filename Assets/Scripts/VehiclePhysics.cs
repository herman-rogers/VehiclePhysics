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
    private Vector3 dragMultiplier = new Vector3( 2, 5, 1 );
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
        rigidbody.centerOfMass += new Vector3( 0, 0, 1.0f );
    }
    private void Update ( )
    {
        Vector3 vehicleVelocity = transform.InverseTransformDirection( rigidbody.velocity );
        gearsComponent.UpdateGears( vehicleVelocity );
        engineComponent.EngineUpdate( );
        wheelComponent.WheelUpdate( vehicleVelocity );
    }

    private void FixedUpdate ( )
    {
        Vector3 vehicleVelocity = transform.InverseTransformDirection( rigidbody.velocity );
        engineComponent.EngineFixedUpdate( vehicleVelocity );
        wheelComponent.WheelFixedUpdate( vehicleVelocity );
        UpdateVehicleDrag( vehicleVelocity );
    }

    private void UpdateVehicleDrag ( Vector3 vehicleVelocity )
    {
        Vector3 relativeDrag = new Vector3( -vehicleVelocity.x * Mathf.Abs( vehicleVelocity.x ),
                                            -vehicleVelocity.y * Mathf.Abs( vehicleVelocity.y ),
                                            -vehicleVelocity.z * Mathf.Abs( vehicleVelocity.z ) );
        Vector3 drag = Vector3.Scale( dragMultiplier, relativeDrag );
        //Add in Hand brake here, adding code that runs when handbrake is not activated
        drag.x *= speedCap / vehicleVelocity.magnitude;
        if ( Mathf.Abs( vehicleVelocity.x ) < 5 )
        {
            drag.x = -vehicleVelocity.x * dragMultiplier.x;
        }
        rigidbody.AddForce( transform.TransformDirection( drag ) * rigidbody.mass * Time.deltaTime );
    }
}