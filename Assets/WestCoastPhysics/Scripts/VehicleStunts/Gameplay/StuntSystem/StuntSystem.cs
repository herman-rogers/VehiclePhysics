using UnityEngine;
using System.Collections;

public enum StuntType
{
    Null,
    ForwardFlip,
    BackwardFlip,
    LeftRoll,
    RightRoll
}

public class StuntSystem : MonoBehaviour
{
    //public StuntAlertHandler stuntHandler;
    private Vector3 startRotation;
    private bool flipping = false;
    private bool cleanStunt = true;
    private bool crashing = false;
    //private VehicleParent vehicle;
    private VehiclePhysics vehicle;
    private StuntType stunttype;
    private float bankedStuntPoints;
    private float overallStuntPoints;
    private bool WaitingForLanding;

    void Start ( )
    {
        //vehicle = GetComponent<VehicleParent>( );
        vehicle = GetComponent<VehiclePhysics>( );
    }

    void Update ( )
    {
        if ( crashing == true )
        {
            //if ( vehicle.oneGrounded )
            //{
            //    crashing = false;
            //}
        }
        if ( flipping == false && crashing == false )
        {
            CheckInput( );
        }

    }

    private void CheckInput ( )
    {
        //if ( Input.GetAxis( "StuntX_1" ) == 1 && vehicle.oneGrounded == false )
        //{
        //    StartCoroutine( FlipZ( -360, 400 ) );
        //}
        //else if ( Input.GetAxis( "StuntX_1" ) == -1 && vehicle.oneGrounded == false )
        //{
        //    StartCoroutine( FlipZ( 360, 400 ) );
        //}
        //else if ( Input.GetAxis( "StuntY_1" ) == 1 && vehicle.oneGrounded == false )
        //{
        //    StartCoroutine( FlipX( 360, 400 ) );
        //}
        //else if ( Input.GetAxis( "StuntY_1" ) == -1 && vehicle.oneGrounded == false )
        //{
        //    StartCoroutine( FlipX( -360, 400 ) );
        //}
    }

    private IEnumerator FlipX ( float rotation, float rotationSpeed )
    {
        flipping = true;
        Vector3 startRotation = transform.localEulerAngles;

        if ( rotation > 0 )
        {
            float newAngle = startRotation.x + rotation;
            while ( startRotation.x < newAngle )
            {
                if ( startRotation.x + 50 > newAngle )
                {
                    Debug.Log( "Almost done" );
                }
                startRotation.x = Mathf.MoveTowards( startRotation.x, newAngle, rotationSpeed * Time.deltaTime );
                transform.localEulerAngles = startRotation;
                yield return new WaitForSeconds( 0.009f );
            }
            stunttype = StuntType.ForwardFlip;
        }
        else
        {
            float newAngle = startRotation.x + rotation;
            while ( startRotation.x > newAngle )
            {
                if ( startRotation.x - 50 < newAngle )
                {
                    Debug.Log( "Almost done" );
                }
                startRotation.x = Mathf.MoveTowards( startRotation.x, newAngle, rotationSpeed * Time.deltaTime );
                transform.localEulerAngles = startRotation;
                yield return new WaitForSeconds( 0.009f );
            }
            stunttype = StuntType.BackwardFlip;
        }
        flipping = false;
        DisplayStunt( 4 );
    }

    private IEnumerator FlipY ( int rotation, float rotationSpeed )
    {
        flipping = true;
        Vector3 startRotation = transform.localEulerAngles;

        if ( rotation > 0 )
        {
            float newAngle = startRotation.y + rotation;
            while ( startRotation.y < newAngle )
            {
                startRotation.y = Mathf.MoveTowards( startRotation.y, newAngle, rotationSpeed * Time.deltaTime );
                transform.localEulerAngles = startRotation;
                yield return new WaitForSeconds( 0.009f );
            }
        }
        else
        {
            float newAngle = startRotation.y + rotation;
            while ( startRotation.y > newAngle )
            {
                startRotation.y = Mathf.MoveTowards( startRotation.y, newAngle, rotationSpeed * Time.deltaTime );
                transform.localEulerAngles = startRotation;
                yield return new WaitForSeconds( 0.009f );
            }
        }
        flipping = false;
        DisplayStunt( 6 );
    }

    private IEnumerator FlipZ ( int rotation, float rotationSpeed )
    {
        flipping = true;
        Vector3 startRotation = transform.localEulerAngles;
        if ( rotation > 0 )
        {
            float newAngle = startRotation.z + rotation;
            while ( startRotation.z < newAngle )
            {
                if ( startRotation.z + 50 > newAngle )
                {
                    Debug.Log( "Almost done" );
                }
                startRotation.z = Mathf.MoveTowards( startRotation.z, newAngle, rotationSpeed * Time.deltaTime );
                transform.localEulerAngles = startRotation;
                yield return new WaitForSeconds( 0.009f );
            }
            stunttype = StuntType.LeftRoll;
        }
        else
        {
            float newAngle = startRotation.z + rotation;
            while ( startRotation.z > newAngle )
            {
                if ( startRotation.z - 50 < newAngle )
                {
                    Debug.Log( "Almost done" );
                }
                startRotation.z = Mathf.MoveTowards( startRotation.z, newAngle, rotationSpeed * Time.deltaTime );
                transform.localEulerAngles = startRotation;
                yield return new WaitForSeconds( 0.009f );
            }
            stunttype = StuntType.RightRoll;
        }
        flipping = false;
        DisplayStunt( 2 );
    }

    private void OnCollisionEnter ( Collision col )
    {
        if ( flipping || col.gameObject.tag != "Terrain" )
        {
            crashing = true;
            StopAllCoroutines( );
            //stuntHandler.Wipeout( );
            bankedStuntPoints = 0;
            flipping = false;
        }
        else if ( !( flipping ) )
        {
            if ( bankedStuntPoints > 0 )
            {
                WaitingForLanding = true;
            }
        }

    }

    private void OnCollisionStay ( Collision col )
    {
        if ( WaitingForLanding )
        {
            //if ( vehicle.allGrounded )
            //{
            //    cleanStunt = true;
            //}
            //if ( vehicle.onlyOneGrounded )
            //{
            //    cleanStunt = false;
            //}

            ScoreStunts( );
            bankedStuntPoints = 0;
            WaitingForLanding = false;
        }
    }

    private void DisplayStunt ( float stuntScore )
    {
        bankedStuntPoints += stuntScore;
        //stuntHandler.DisplayStunt( stunttype );
        //stuntHandler.UpdateHighScore( stuntScore );
    }

    private void ScoreStunts ( )
    {
        //if ( cleanStunt )
        //{
        //    vehicle.boost += ( bankedStuntPoints / 2 ) / 10;
        //}
        //else
        //{
        //    vehicle.boost += ( bankedStuntPoints / 4 ) / 10;
        //}
        //stuntHandler.SetQuality( cleanStunt );
        cleanStunt = true;
    }

    private bool AlmostLanded ( )
    {
        bool almostLanded = false;
        //Debug.Log("x "+transform.rotation.x);
        //Debug.Log("y "+transform.rotation.y);
        //Debug.Log("z "+transform.rotation.z);
        if ( ( transform.rotation.x >= 0 && transform.rotation.x <= 50 ) || ( transform.rotation.x <= 0 && transform.rotation.x >= -50 ) )
        {
            if ( ( transform.rotation.y >= 0 && transform.rotation.y <= 50 ) || ( transform.rotation.y <= 0 && transform.rotation.y >= -50 ) )
            {
                if ( ( transform.rotation.z >= 0 && transform.rotation.z <= 50 ) || ( transform.rotation.z <= 0 && transform.rotation.z >= -50 ) )
                {
                    almostLanded = true;
                }
            }
        }
        return almostLanded;
    }
}

