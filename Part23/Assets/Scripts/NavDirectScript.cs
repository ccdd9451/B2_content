using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavDirectScript : MonoBehaviour {

    public GameObject[] activate_indicate_light;
    public Queue<Vector3> points = new Queue<Vector3>();
    public int destPoint;

    int isActivated;
    NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();

        isActivated = 0;
        foreach (GameObject l in activate_indicate_light)
        {
            l.SetActive(false);
        }
	}
	
    public void ToggleActivation()
    {
        isActivated = (isActivated + 1) % 4;
        if (isActivated != 0)
        {
            activate_indicate_light[isActivated-1].SetActive(true);
            agent.speed = isActivated * 1.5f;
            agent.ResetPath() ;
        }
        else
        {
            foreach (GameObject l in activate_indicate_light)
            {
                l.SetActive(false);
            }
        }
        points.Clear();
    }

    public void SetDestination(Vector3 destination)
    {
        if (isActivated != 0)
        {
            points.Enqueue(destination);
        }
    }

    void GotoNextPoint()
    {
        if (points.Count == 0)
            return;
        agent.destination = points.Dequeue();

    }

    void Update () {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            GotoNextPoint();
    }
}
