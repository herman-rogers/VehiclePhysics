﻿using UnityEngine;
using System.Collections;

public class VehicleExtension : MonoBehaviour
{
    private static Rect dragWindow = new Rect( Screen.width - 220, 200, 250, 450 );

    public static void ShowMainGUI ( )
    {
        dragWindow = GUI.Window( 1, dragWindow, VehicleStatsDragWindow, "Vehicle Stats" );
    }

    private static void VehicleStatsDragWindow ( int windowID )
    {
        GUI.Label( new Rect( 10, 20, 200, 100 ), EngineComponent.GetEngineSpeed( ).ToString( ) );
        //GUI.Label( new Rect( 10, 40, 200, 100 ), "Obs: " + Subject.ObserverCount( ) );
        //GUI.Label( new Rect( 10, 60, 200, 100 ), "UnityObs: " + Subject.UnityObserverCount( ) );
        //GUI.Label( new Rect( 10, 80, 200, 100 ), "Reverser Target: " + currentReverserTarget );
        GUI.DragWindow( );
    }
}