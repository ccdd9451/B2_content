#define DVORAK

using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class UnityChanNavController : MonoBehaviour
{

    public float animspeed = 2.0f;
    public float walkSpeed = 1.5f;
    public float runAmp = 2.0f;
    public Vector3 destination;
    public GameObject spotlight;


    private NavMeshAgent agent;
    private Animator anim;
    private AnimatorStateInfo currentBaseState;
    private Rigidbody rb;
    private bool activated = false;

    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int restState = Animator.StringToHash("Base Layer.Rest");

    Vector3 smoothDeltaPosition = Vector3.zero;
    Vector3 velocity = Vector3.zero;

    // 初期化
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        anim.speed = animspeed;
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(destination);
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

    public void toggleActivation()
    {
        activated = !activated;
        spotlight.SetActive(activated);
    }

    public void setDestination(Vector3 dest)
    {
        if (activated)
        {
            agent.SetDestination(dest);
        }
    }

    void FixedUpdate()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        Debug.DrawLine(transform.position, agent.nextPosition);

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dz = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector3 deltaPosition = new Vector3(dx, 0, dz).normalized;

        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector3.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        if (shouldMove)
        {
            SetVelocity(deltaPosition *2);
        }
        else
        {
            SetVelocity(Vector3.zero);
        }

        if (agent.isOnOffMeshLink)
        {
            anim.SetBool("Jump", true);
        }
        else
        {
            anim.SetBool("Jump", false);
        }
    }

    void OnAnimatorMove()
    {
        // Update postion to agent position
        //		transform.position = agent.nextPosition;

        // Update position based on animation movement using navigation surface height
        Vector3 position= agent.nextPosition;
        transform.position = position;
    }
}
