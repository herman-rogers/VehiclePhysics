using UnityEngine;
using System.Collections;

public class VehiclePhysicsExtension : Observer
{
    public const string VEHICLE_PHYSICS = "VEHICLE_PHYSICS";
    public const string ENGINE_TORQUE = "DRIVE_TORQUE";
    public const string HORSE_POWER = "HORSE_POWER";
    public const string CURRENT_RPM = "CURRENT_RPM";
    public const string MAX_TORQUE = "MAX_TORQUE";
    public const string TLONG_FORCE = "TLONG_FORCE";
    public const string BRAKE_FORCE = "BRAKE_FORCE";
    public const string AIR_RESISTANCE = "AIR_RESISTANCE";
    public const string ROLLING_RESISTANCE = "ROLLING_RESISTANCE";
    public const string CURRENT_GEAR = "CURRENT_GEAR";
    public const string GEAR_RATIO = "GEAR_RATIO";
    private const int screenHorizontalPosition = 1000;
    private const int screenVerticalPosition = 230;
    private const int screenWidth = 700;
    private const int screenHeight = 450;
    private Rect dragWindow = new Rect( Screen.width - screenHorizontalPosition,
                                        screenVerticalPosition, 
                                        screenWidth, screenHeight );
    private bool showVehicleStats;
    private float vehicleSpeed;
    private float gearNumber;
    private float gearRatio;
    private float driveTorque;
    private float horsePower;
    private float currentRPM;
    private float maxTorque;
    private float airResistance;
    private float rollingResistance;
    //Longitudinal Forces
    private float tLongForce;
    private float brakeForce;

    public override void OnNotify( Object sender, EventArguments e )
    {
        if( sender != SceneTransformTracker.currentlyTrackedObject )
        {
            showVehicleStats = false;
            return;
        }
        showVehicleStats = true;
        MonitorVehicle( e );
    }

    public void ShowMainGUI ( )
    {
        dragWindow = GUI.Window( 1, dragWindow, VehicleStatsDragWindow, "Vehicle Physics Stats" );
    }

    private void MonitorVehicle( EventArguments args )
    {
        switch( args.eventMessage )
        {
            case VEHICLE_PHYSICS:
                vehicleSpeed = args.extendedMessageNumber;
                break;
            case CURRENT_GEAR:
                gearNumber = args.extendedMessageNumber;
                break;
            case GEAR_RATIO:
                gearRatio = args.extendedMessageNumber;
                break;
            case MAX_TORQUE:
                maxTorque = args.extendedMessageNumber;
                break;
            case ENGINE_TORQUE:
                driveTorque = args.extendedMessageNumber;
                break;
            case HORSE_POWER:
                horsePower = args.extendedMessageNumber;
                break;
            case CURRENT_RPM:
                currentRPM = args.extendedMessageNumber;
                break;
            case AIR_RESISTANCE:
                airResistance = args.extendedMessageNumber;
                break;
            case ROLLING_RESISTANCE:
                rollingResistance = args.extendedMessageNumber;
                break;
            case TLONG_FORCE:
                tLongForce = args.extendedMessageNumber;
                break;
            case BRAKE_FORCE:
                brakeForce = args.extendedMessageNumber;
                break;
        }
    }

    private void VehicleStatsDragWindow ( int windowID )
    {
        if( showVehicleStats )
        {
            GUI.Label( new Rect( 10, 20, 400, 100 ), "*** Engine Physics ***" );
            DisplayVehicleStats( );
        }
        GUI.DragWindow( );
    }

    private void DisplayVehicleStats( )
    {
        GUI.Label( new Rect( 10, 40, 400, 100 ), "Vehicle Speed( KM/h ): " + vehicleSpeed );
        GUI.Label( new Rect( 10, 60, 400, 100 ), "Current Gear: " + gearNumber );
        GUI.Label( new Rect( 10, 80, 400, 100 ), "Gear Ratio: " + gearRatio );
        GUI.Label( new Rect( 10, 100, 400, 100 ), "Max Torque: " + maxTorque );
        GUI.Label( new Rect( 10, 120, 400, 100 ), "Engine Torque( N/m ): " + driveTorque );
        GUI.Label( new Rect( 10, 140, 400, 100 ), "Horse Power: " + horsePower );
        GUI.Label( new Rect( 10, 160, 400, 100 ), "Current RPM: " + currentRPM );
        GUI.Label( new Rect( 10, 180, 400, 100 ), "Air Resistance: " + airResistance );
        GUI.Label( new Rect( 10, 200, 400, 100 ), "Rolling Resistance: " + rollingResistance );
        GUI.Label( new Rect( 10, 220, 400, 100 ), "TLong Force: " + tLongForce );
        GUI.Label( new Rect( 10, 240, 400, 100 ), "Brake Force: " + brakeForce );
    }
}
