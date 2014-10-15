using UnityEngine;
using System.Collections;

public class GearsComponent
{
    public float currentGear { get; private set; }
    public int gearCount { get; private set; }
    private float speedCap;
    private float[ ] gearSpeeds;
    private float[ ] engineForceValues;

    public GearsComponent ( int numberOfGears, float topSpeed )
    {
        gearCount = numberOfGears;
        speedCap = topSpeed;
        SetupGears( );
    }

    public void UpdateGears ( Vector3 vehicleVelocity )
    {
        ChangeGears( vehicleVelocity );
    }

    public float GetGearValue ( float gearNumber )
    {
        return engineForceValues[ ( int )gearNumber ];
    }

    public float GetNormalizedPower ( float power )
    { 
        float normalizedHorsePower = ( power / engineForceValues[ engineForceValues.Length - 1 ] ) * 2;
        if ( normalizedHorsePower < 1 )
        {
            return 10.0f - normalizedHorsePower * 9.0f;
        }
        else
        {
            return 1.9f - normalizedHorsePower - 0.9f;
        }
    }

    private void SetupGears ( )
    {
        engineForceValues = new float[ gearCount ];
        gearSpeeds = new float[ gearCount ];
        float tempSpeed = speedCap;
        for ( int i = 0; i < gearCount; i++ )
        {
            if ( i == 0 )
            {
                gearSpeeds[ 0 ] = tempSpeed / gearCount;
            }
            else
            {
                gearSpeeds[ i ] = tempSpeed / gearCount + gearSpeeds[ i - 1 ];
            }
            tempSpeed -= tempSpeed / gearCount;
        }
        float engineSpeeds = speedCap / gearSpeeds[ gearSpeeds.Length - 1 ];
        for ( int i = 0; i < gearCount; i++ )
        {
            float maxLinearDrag = gearSpeeds[ i ] * gearSpeeds[ i ];
            engineForceValues[ i ] = maxLinearDrag * engineSpeeds;
        }
    }

    private void ChangeGears ( Vector3 vehicleVelocity )
    {
        currentGear = 0;
        for ( int i = 0; i < ( gearCount - 1 ); i++ )
        {
            if ( vehicleVelocity.z > gearSpeeds[ i ] )
            {
                currentGear = i + 1;
            }
        }
    }
}
