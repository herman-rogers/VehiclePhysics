using UnityEngine;
using System.Collections;

public class Observer
{
    public Observer( )
	{
        Subject.AddObserver( this );
    }

    public virtual void OnNotify( Object sender, EventArguments e ){ }

	public virtual void UnityUpdate( ){ }
}

public class EventArguments
{
    public string eventMessage{ get; set; }
    public string extendedMessage{ get; set; }
    public float extendedMessageNumber{ get; private set; }
    public Vector3 objectCoordinates{ get; private set; }

    public EventArguments( string newEventMessage, string newExtendedMessage, Vector3 coordinates )
	{
        float newInteger;
        eventMessage = newEventMessage;
        extendedMessage = newExtendedMessage;
        objectCoordinates = coordinates;
        if( float.TryParse( newExtendedMessage, out newInteger ) )
		{
            extendedMessageNumber = newInteger;
        }
    }
}