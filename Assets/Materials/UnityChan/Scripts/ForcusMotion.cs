#define DVORAK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcusMotion : MonoBehaviour {

#if DVORAK
    public enum KEYBOARD_INPUT : int
    {
        LEFT = KeyCode.A,
        RIGHT = KeyCode.E,
        FORWARD = KeyCode.Comma,
        BACKWARD = KeyCode.O,
    }
#else
    public enum KEYBOARD_INPUT : int
    {
        LEFT = KeyCode.A,
        RIGHT = KeyCode.D,
        FORWARD = KeyCode.W,
        BACKWARD = KeyCode.S,
    }
#endif

    public float cameraVelo = 5;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.RIGHT))
        {
            movement.x += cameraVelo;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.LEFT))
        {
            movement.x -= cameraVelo;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.FORWARD))
        {
            movement.z += cameraVelo;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.BACKWARD))
        {
            movement.z -= cameraVelo;
        }
        transform.position += movement * Time.deltaTime;
    }
}
