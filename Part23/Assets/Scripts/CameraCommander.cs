using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCommander : MonoBehaviour {

    Camera _camera;
    GroupMessageSendScript navGroup;
	// Use this for initialization
	void Start () {
        _camera = GetComponent<Camera>();
        navGroup = GameObject.Find("AgentGroup").GetComponent<GroupMessageSendScript>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ToggleSelection();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CommandToDestination();
        }
	}

    private void ToggleSelection()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject obj = hit.transform.parent.gameObject;
            if (obj.tag == "Agent")
            {
                obj.GetComponent<NavDirectScript>().ToggleActivation();
            }
        }
    }

    void CommandToDestination()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            navGroup.SetGroupDestination(hit.point);
        }
    }
}
