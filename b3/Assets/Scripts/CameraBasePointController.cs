using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBasePointController : MonoBehaviour {

    public enum K_INPUT : int
    {
        LEFT = KeyCode.A,
        RIGHT = KeyCode.D,
        FORWARD = KeyCode.W,
        BACKWARD = KeyCode.S,
        LROTATE = KeyCode.Q,
        RLROTATE = KeyCode.E,
    }
    public float cameraVelo = 5;
    public float rotateVelo = 15;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 movement = Vector3.zero;
        float rotateAngle = 0;
        if (Input.GetKey((KeyCode)K_INPUT.RIGHT))
        {
            movement.x += cameraVelo;
        }
        if (Input.GetKey((KeyCode)K_INPUT.LEFT))
        {
            movement.x -= cameraVelo;
        }
        if (Input.GetKey((KeyCode)K_INPUT.FORWARD))
        {
            movement.z += cameraVelo;
        }
        if (Input.GetKey((KeyCode)K_INPUT.BACKWARD))
        {
            movement.z -= cameraVelo;
        }
        if (Input.GetKey((KeyCode)K_INPUT.LROTATE))
        {
            rotateAngle += rotateVelo;
        }
        if (Input.GetKey((KeyCode)K_INPUT.RLROTATE))
        {
            rotateAngle -= rotateVelo;
        }
        transform.position += transform.rotation * movement * Time.deltaTime;
        rotateAngle = rotateAngle * Time.deltaTime;
        transform.forward = 
            Quaternion.AngleAxis(rotateAngle, Vector3.up) * transform.forward;
    }
}

