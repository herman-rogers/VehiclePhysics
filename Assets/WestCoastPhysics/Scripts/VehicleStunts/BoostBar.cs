using UnityEngine;
using System.Collections;

public class BoostBar : MonoBehaviour
{
    //public VehicleParent thisCar;

    void Update ( )
    {
        MoveBoostNeedle( );
    }

    private void MoveBoostNeedle ( )
    {
        //GetComponent<UIProgressBar>( ).value = thisCar.boost;
    }
}
