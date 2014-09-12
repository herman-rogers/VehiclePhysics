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
    public int extendedMessageNumber{ get; private set; }
    public Vector3 objectCoordinates{ get; private set; }

    public EventArguments( string newEventMessage, string newExtendedMessage, Vector3 coordinates )
	{
        int newInteger;
        eventMessage = newEventMessage;
        extendedMessage = newExtendedMessage;
        objectCoordinates = coordinates;
        if( int.TryParse( newExtendedMessage, out newInteger ) )
		{
            extendedMessageNumber = newInteger;
        }
    }
}