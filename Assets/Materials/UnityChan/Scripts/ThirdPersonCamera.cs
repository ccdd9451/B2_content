//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//
using UnityEngine;
using System.Collections;


public class ThirdPersonCamera : MonoBehaviour
{
	public float smooth = 3f;		// カメラモーションのスムーズ化用変数
	Transform standardPos;			// the usual position for the camera, specified by a transform in the game
	Transform frontPos;         // Front Camera locater
    Transform jumpPos;          // Jump Camera locater	
    Transform secondCam;			// Jump Camera locater
    GameObject secondCamObj;

    // スムーズに繋がない時（クイック切り替え）用のブーリアンフラグ
    bool bQuickSwitch = false;	//Change Camera Position Quickly
    bool secondCamEnabled = false;
	
	
	void Start()
	{
		standardPos = GameObject.Find("CamPos").transform;
		
		if(GameObject.Find ("FrontPos"))
			frontPos = GameObject.Find ("FrontPos").transform;

		if(GameObject.Find ("JumpPos"))
			jumpPos = GameObject.Find ("JumpPos").transform;

        if (GameObject.Find("SecondCam"))
            secondCam = GameObject.Find("SecondCam").transform;

        transform.position = standardPos.position;	
		transform.forward = standardPos.forward;	
	}

	
	void FixedUpdate ()
	{
        if (Input.GetKeyDown(KeyCode.C))
        {
            secondCamEnabled = !(secondCamEnabled);
            secondCamObj.SetActive(secondCamEnabled); 
        }
        if (!secondCamEnabled)
        {
            setCameraPositionNormalView();
        } else
        {
            setCameraPositionSecondView();
        }
	}

	void setCameraPositionNormalView()
	{
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;
	}
    void setCameraPositionSecondView()
    {
        transform.position = secondCam.position;
        transform.forward = secondCam.forward;
    }


    void setCameraPositionJumpView()
	{
		transform.position = Vector3.Lerp(transform.position, jumpPos.position, Time.fixedDeltaTime * smooth);	
		transform.forward = Vector3.Lerp(transform.forward, jumpPos.forward, Time.fixedDeltaTime * smooth);		
	}
}
