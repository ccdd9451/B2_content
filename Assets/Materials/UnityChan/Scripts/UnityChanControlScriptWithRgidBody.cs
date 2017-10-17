//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

// 必要なコンポーネントの列記
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]

public class UnityChanControlScriptWithRgidBody : MonoBehaviour
{

	public float lookSmoother = 3.0f;			// a smoothing setting for camera motion
	public bool useCurves = true;				// Mecanimでカーブ調整を使うか設定する
												// このスイッチが入っていないとカーブは使われない
	public float useCurvesHeight = 0.5f;		// カーブ補正の有効高さ（地面をすり抜けやすい時には大きくする）
    public float animspeed = 2.0f;

	public float forwardSpeed = 1.5f;
    public float runSpeed = 1.5f;
    public float strafeSpeed = 0.8f;
	public float rotateSpeed = 2.0f;
	public float jumpPower = 3.0f; 

	private CapsuleCollider col;
	private Rigidbody rb;
	private float orgColHight;
	private Vector3 orgVectColCenter;
    private Vector3 velBeforeJump;
	
	private Animator anim;							
	private AnimatorStateInfo currentBaseState;			

	private GameObject cameraObject;	
		
// アニメーター各ステートへの参照
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


    void FixedUpdate()
    {
        float velox = Input.GetAxis("Horizontal") * strafeSpeed;           
        float veloz = Input.GetAxis("Vertical") * forwardSpeed;				
        Vector3 vel = new Vector3(velox, 0, veloz);
		anim.SetFloat("Speed", vel.magnitude);
        anim.SetFloat("velox", velox);
        anim.SetFloat("veloz", veloz);

		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	
		rb.useGravity = true;	
		
		
		if (Input.GetButtonDown("Jump")) {	
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
            if (vel.magnitude > 0.1)
            {
                rb.velocity = velBeforeJump;
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
