using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Subject : MonoBehaviour
{
    private static readonly List<Observer> listOfObservers = new List<Observer>( );
    private static List<GameObject> listOfUnityObservers = new List<GameObject>( );
    private static Vector3 stubbedVector3 = new Vector3( 0, 0, 0 );

    public static void AddObserver ( Observer newObserver )
    {
        if ( !listOfObservers.Contains( newObserver ) )
        {
            listOfObservers.Add( newObserver );
            return;
        }
        Debug.LogError( "Cannot Add The Same Observer Twice" );
    }

    public static void AddUnityObservers ( )
    {
        listOfUnityObservers = ( GameObject.FindGameObjectsWithTag( "UnityObserver" ) ).ToList( );
    }

    public static void RemoveAllObservers ( )
    {
        listOfObservers.Clear( );
    }

    public static int ObserverCount ( )
    {
        return listOfObservers.Count;
    }

    public static int UnityObserverCount ( )
    {
        return listOfUnityObservers.Count( );
    }

    public static void Notify ( string staticEventName )
    {
        var stubbedObject = new Object( );
        SendToObservers( stubbedObject, staticEventName, "NoMessage", stubbedVector3 );
    }

    public static void NotifyExtendedMessage ( string staticEventName, string extendedMessage )
    {
        var stubbedObject = new Object( );
        SendToObservers( stubbedObject, staticEventName, extendedMessage, stubbedVector3 );
    }

    public static void NotifyObject ( Object sender, string staticEventName, string extendedMessage )
    {
        SendToObservers( sender, staticEventName, extendedMessage, stubbedVector3 );
    }

    public static void NotifyCoordinates ( string staticEventName, Vector3 worldCoordinates )
    {
        var stubbedObject = new Object( );
        SendToObservers( stubbedObject, staticEventName, "NoMessage", worldCoordinates );
    }

    private static void SendToObservers ( Object sender, string eventName, string extendedMessage, Vector3 coordinates )
    {
        GarbageCollectObservers( );
        foreach ( var observer in listOfObservers )
        {
            observer.OnNotify( sender, new EventArguments( eventName, extendedMessage, coordinates ) );
        }
        NotifyUnityObservers( sender, eventName, extendedMessage, coordinates );
    }

    private static void NotifyUnityObservers ( Object sender, string unityEventName, string extendedMessage, Vector3 coordinates )
    {
        foreach ( var unityObserver in listOfUnityObservers )
        {
            List<UnityObserver> ObserverObjects = unityObserver.GetComponents<UnityObserver>( ).ToList( );
            ObserverObjects.ForEach( item =>
                                     item.OnNotify( sender,
                                                    new EventArguments( unityEventName, extendedMessage, coordinates ) ) );
        }
    }

    public static void GarbageCollectObservers ( )
    {
        listOfObservers.RemoveAll( item => item == null );
        listOfUnityObservers.RemoveAll( item => item == null );
    }
}