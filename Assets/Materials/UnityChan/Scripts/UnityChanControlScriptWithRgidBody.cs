#define DVORAK

using UnityEngine;
using System.Collections;

// 必要なコンポーネントの列記
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]

public class UnityChanControlScriptWithRgidBody : MonoBehaviour
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
	private float orgColHight;
	private Vector3 orgVectColCenter;
    private Vector3 inpVel = Vector3.zero;
    private Vector3 velBeforeJump;
	
	private Animator anim;							
	private AnimatorStateInfo currentBaseState;			

	private GameObject cameraObject;	
		
	static int idleState = Animator.StringToHash("Base Layer.Idle");
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");
	static int jumpState = Animator.StringToHash("Base Layer.Jump");
	static int restState = Animator.StringToHash("Base Layer.Rest");

// 初期化
	void Start ()
	{
		anim = GetComponent<Animator>();
        anim.speed = animspeed;
		col = GetComponent<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();
		cameraObject = GameObject.FindWithTag("MainCamera");
		orgColHight = col.height;
		orgVectColCenter = col.center;
}

    void SetVelocity(Vector3 velo)
    {
        anim.SetFloat("Speed", velo.magnitude);
        anim.SetFloat("velox", velo.x);
        anim.SetFloat("veloz", velo.z);

        Quaternion desireRot = Quaternion.FromToRotation(Vector3.forward,
            velo);
        desireRot = Quaternion.Slerp(Quaternion.identity, desireRot, Time.deltaTime);
        desireRot = Quaternion.Euler(new Vector3(0f, desireRot.eulerAngles.y, 0f));

        rb.rotation = rb.rotation * desireRot;


    }
    void FixedUpdate()
    {
        float moveSpeed = walkSpeed;
        Vector3 movement = Vector3.zero;
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.RIGHT))
        {
            movement.x += 1;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.LEFT))
        {
            movement.x -= 1;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.FORWARD))
        {
            movement.z += 1;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.BACKWARD))
        {
            movement.z -= 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed *= runAmp;
        }
        inpVel = Vector3.Lerp(inpVel, movement * moveSpeed, Time.deltaTime);
        SetVelocity(inpVel);

        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	
		rb.useGravity = true;	
		
		
		if (Input.GetKeyDown((KeyCode) KEYBOARD_INPUT.JUMP)) {	
			if (currentBaseState.fullPathHash != jumpState){
				if(!anim.IsInTransition(0))
				{
                    velBeforeJump = anim.velocity;
                    anim.SetBool("Jump", true);		
				}
			}
		}
		
		else if(currentBaseState.fullPathHash == jumpState)
		{
			cameraObject.SendMessage("setCameraPositionJumpView");
            if (inpVel.magnitude > 0.1)
            {
                rb.velocity = 1.5f * velBeforeJump;
            }
			if(!anim.IsInTransition(0))
			{
				anim.SetBool("Jump", false);
			}
		}
	}


	void resetCollider()
	{
		col.height = orgColHight;
		col.center = orgVectColCenter;
	}
}
