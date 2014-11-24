using UnityEngine;
using System.Collections;

public class PositionHolder : MonoBehaviour {
    Vector3 position;
    Quaternion rotation;

    void Start()
    {
        position = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetButton("Pause")){
            position = transform.position;
            rotation = transform.rotation;
            Debug.Log("Position Saved");
        }else if(Input.GetButton("Back")){
            transform.position = position;
            transform.rotation = rotation;
            Debug.Log("Position Loaded");
        }

	}
}
