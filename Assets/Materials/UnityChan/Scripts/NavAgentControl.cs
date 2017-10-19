#define DVORAK

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]

public class NavAgentControl : MonoBehaviour
{
	#if DVORAK
	public enum KEYBOARD_INPUT : int {
		ACCEL = KeyCode.LeftShift,
		LEFT = KeyCode.A,
		RIGHT = KeyCode.E,
		FORWARD = KeyCode.Comma,
		BACKWARD = KeyCode.O,
		JUMP = KeyCode.Space,
	}
	#else
	public enum KEYBOARD_INPUT : int
	{
	ACCEL = KeyCode.LeftShift,
	LEFT = KeyCode.A,
	RIGHT = KeyCode.D,
	FORWARD = KeyCode.W,
	BACKWARD = KeyCode.S,
	JUMP = KeyCode.Space,
	}
	#endif

	public float animspeed = 2.0f;

	public float walkSpeed = 1.5f;
	public float runAmp = 2.0f;

	private CapsuleCollider col;
	private Rigidbody rb;
	private NavMeshAgent Agent;
	private Vector3 velBeforeJump;
	private float movespeed;
	private Animator anim;							
	private AnimatorStateInfo currentBaseState;			


	void Start ()
	{
		anim = GetComponent<Animator>();
		anim.speed = animspeed;
		rb = GetComponent<Rigidbody>();
		Agent = GetComponent<NavMeshAgent>(); 

		Agent.updatePosition = false;
		Agent.updateRotation = false;
		Agent.updateUpAxis = false;
	}

	void SetVelocity(Vector3 velo)
	{
		anim.SetFloat("Speed", velo.magnitude);
		anim.SetFloat("velox", velo.x);
		anim.SetFloat("veloz", velo.z);

		if (velo.magnitude > 0.1) {
			Quaternion desireRot = Quaternion.FromToRotation (Vector3.forward,
				velo);
			desireRot = Quaternion.Slerp (Quaternion.identity, desireRot, Time.deltaTime);
			desireRot = Quaternion.Euler (new Vector3 (0f, desireRot.eulerAngles.y, 0f));
			rb.rotation = rb.rotation * desireRot;
		}

	}
	void FixedUpdate()
	{
		if (Input.GetKey ((KeyCode)KEYBOARD_INPUT.ACCEL)) {
			movespeed = Mathf.Lerp (movespeed, walkSpeed * runAmp, Time.deltaTime);
		} else {
			movespeed = Mathf.Lerp (movespeed, walkSpeed, Time.deltaTime);
		}
		Vector3 nextPos = Agent.nextPosition;
		Vector3 velo = (transform.position - nextPos).normalized * movespeed * Time.deltaTime;
		SetVelocity (velo);
		Vector3 tempos = rb.position;
		tempos.y = nextPos.y;
		rb.position = tempos;
		if (Agent.isOnOffMeshLink) {
			anim.SetBool ("Jump", true);
		} else {
			anim.SetBool ("Jump", false);
		}
				
	}
	public void SetDestination(Vector3 dest)
	{
		//Agent.ResetPath();
		Agent.nextPosition = transform.position;
		Agent.SetDestination(dest);
	}
}
