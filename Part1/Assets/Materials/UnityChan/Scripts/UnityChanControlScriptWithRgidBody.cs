//#define DVORAK

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class UnityChanControlScriptWithRgidBody : MonoBehaviour
{
#if DVORAK
    public enum KEYBOARD_INPUT : int
    {
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
        LEFT = KeyCode.LeftArrow,
        RIGHT = KeyCode.RightArrow,
        FORWARD = KeyCode.UpArrow,
        BACKWARD = KeyCode.DownArrow,
        JUMP = KeyCode.Space,
    }
#endif

    public float animspeed = 2.0f;
    public float walkSpeed = 1.5f;
    public float runAmp = 2.0f;

    private CapsuleCollider col;
    private Rigidbody rb;
    private GameObject footRef;
    private float colOffset;
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
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = animspeed;
        rb = GetComponent<Rigidbody>();
        cameraObject = GameObject.FindWithTag("MainCamera");
        col = GetComponent<CapsuleCollider>();
        footRef = GameObject.Find("Character1_RightFoot");
        colOffset = col.center.y;
    }

    void SetVelocity(Vector3 velo)
    {
        anim.SetFloat("Speed", velo.magnitude);
        anim.SetFloat("velox", velo.x);
        anim.SetFloat("veloz", velo.z);

        if (velo.magnitude > 0.1)
        {
            Quaternion desireRot = Quaternion.FromToRotation(Vector3.forward,
                                            velo);
            desireRot = Quaternion.Slerp(Quaternion.identity, desireRot, Time.deltaTime);
            desireRot = Quaternion.Euler(new Vector3(0f, desireRot.eulerAngles.y, 0f));
            rb.rotation = rb.rotation * desireRot;
        }

    }
    void FixedUpdate()
    {
        #region motionCtrl
        float moveSpeed = walkSpeed;
        Vector3 movement = Vector3.zero;
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.ACCEL))
        {
            moveSpeed *= runAmp;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.RIGHT))
        {
            movement.x += walkSpeed;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.LEFT))
        {
            movement.x -= walkSpeed;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.FORWARD))
        {
            movement.z += moveSpeed;
        }
        if (Input.GetKey((KeyCode)KEYBOARD_INPUT.BACKWARD))
        {
            movement.z -= moveSpeed;
        }

        Vector3 dm = movement - inpVel;
        if (dm.magnitude > 0.1)
        {
            inpVel = inpVel + dm.normalized * 10f * Time.deltaTime;
        }
        SetVelocity(inpVel);
        #endregion

        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        rb.useGravity = true;


        if (Input.GetKeyDown((KeyCode)KEYBOARD_INPUT.JUMP))
        {
            if (currentBaseState.fullPathHash != jumpState)
            {
                if (!anim.IsInTransition(0))
                {
                    velBeforeJump = anim.velocity;
                    anim.SetBool("Jump", true);
                }
            }
        }
        else if (currentBaseState.fullPathHash == jumpState)
        {
            cameraObject.SendMessage("setCameraPositionJumpView");
            if (inpVel.magnitude > 0.1)
            {
                rb.velocity = 1.5f * velBeforeJump;

            }
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Jump", false);

            }
            float relFootY = footRef.transform.position.y - transform.position.y;
            if (footRef.transform.position.y > col.center.y - colOffset)
            {
                col.center = new Vector3(0f, relFootY + colOffset, 0.2f);
            }
            else
            {
                Vector3 tempos = transform.position;
                tempos.y = col.center.y - colOffset - relFootY + 0.01f;
                transform.position = tempos;
            }
        }
        else
        {
            Vector3 tempos2 = transform.position + col.center;
            tempos2.y -= colOffset;
            transform.position = Vector3.Lerp(transform.position, tempos2, 10 * Time.deltaTime);
            tempos2.y += colOffset;
            col.center = tempos2 - transform.position;
        }
    }
}
