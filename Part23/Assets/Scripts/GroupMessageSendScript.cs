using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupMessageSendScript : MonoBehaviour {


    NavDirectScript[] NavGroup;
	// Use this for initialization
	void Start () {
        NavGroup = GetComponentsInChildren<NavDirectScript>();
	}
	
    public void SetGroupDestination(Vector3 dest)
    {
        foreach (NavDirectScript nav in NavGroup)
        {
            nav.SetDestination(dest);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
