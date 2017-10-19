using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavEvaScript : MonoBehaviour {

    NavMeshAgent agent;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(transform.parent.position);
	}
	
	// Update is called once per frame
	void Update () {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            transform.gameObject.SetActive(false);
        }

    }
}
