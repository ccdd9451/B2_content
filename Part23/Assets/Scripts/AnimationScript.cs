using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AnimationScript : MonoBehaviour {


    public float animSpeed = 1.5f;
    public float sensity = 1.5f;

    Vector3 oldLoc;
    Animator anim;
    NavMeshAgent agent;
    

	// Use this for initialization
	void Start () {
        oldLoc = transform.position;
        anim = GetComponent<Animator>();
        anim.speed = animSpeed;

        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 curLoc = transform.position;
        Vector3 velo = (curLoc - oldLoc) / Time.deltaTime * sensity;

        if (velo.magnitude > 0.1f)
        {
            anim.SetFloat("Speed", velo.magnitude);
            anim.SetFloat("velox",
                Vector3.Dot(transform.right, velo));
            anim.SetFloat("veloz",
                Vector3.Dot(transform.forward, velo));
        }
        else
        {
            anim.SetFloat("Speed", 0f);
            anim.SetFloat("velox", 0f);
            anim.SetFloat("veloz", 0f);
        }

        if (agent.isOnOffMeshLink)
        {
            anim.SetBool("NavJump", true);
        }
        else
        {
            anim.SetBool("NavJump", false);
        }
        oldLoc = curLoc;
	}
}
