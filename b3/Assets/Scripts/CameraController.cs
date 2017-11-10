using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Transform focus;
	// Use this for initialization
	void Start () {
        focus = transform.parent;
        Debug.Log("focus location" + focus.ToString());
        transform.LookAt(focus.position);
	}

    void Update()
    {
        float delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0.0f)
            this.mouseWheelEvent(delta);
    }

    void mouseWheelEvent(float delta)
    {
        Vector3 focusToPosition = transform.position - focus.position;
        Vector3 post = focusToPosition * (1.0f + delta);
        if (post.magnitude > 0.01)
            transform.position = focus.position + post;
    }


}
