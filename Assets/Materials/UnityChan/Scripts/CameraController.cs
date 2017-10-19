//CameraController.cs for UnityChan
//Original Script is here:
//TAK-EMI / CameraController.cs
//https://gist.github.com/TAK-EMI/d67a13b6f73bed32075d
//https://twitter.com/TAK_EMI
//
//Revised by N.Kobayashi 2014/5/15 
//Change : To prevent rotation flips on XY plane, use Quaternion in cameraRotate()
//Change : Add the instrustion window
//Change : Add the operation for Mac
//




using UnityEngine;
using System.Collections;

namespace CameraController
{
    enum MouseButtonDown
    {
        MBD_LEFT = 0,
        MBD_RIGHT,
        MBD_MIDDLE,
    };

    public class CameraController : MonoBehaviour
    {

        private Vector3 focus = Vector3.zero;

        public Camera cam;    
        public GameObject focusObj;
        public GameObject activatedChan;

        private Vector3 oldPos;

        void setupFocusObject(string name)
        {
            GameObject obj = this.focusObj = new GameObject(name);
            obj.transform.position = this.focus;
            obj.transform.LookAt(this.transform.position);

            return;
        }

        void Start()
        {

            Transform trans = this.transform;
            transform.parent = this.focusObj.transform;

            trans.LookAt(this.focus);

            return;
        }

        void Update()
        {
            this.mouseEvent();

            return;
        }



        void mouseEvent()
        {
            float delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta != 0.0f)
                this.mouseWheelEvent(delta);

            if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_LEFT))
                toggleAct();

            if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_RIGHT))
                commandToMove();

        }
        
        void toggleAct()
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject obj = hit.transform.gameObject;


                if (obj.tag == "chan")
                {
                    obj.GetComponent<UnityChanNavController>().toggleActivation();
                }
            }
        }
        void commandToMove()
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 pos = hit.transform.position;
                foreach (UnityChanNavController c 
                    in activatedChan.GetComponentsInChildren<UnityChanNavController>())
                {
                    c.setDestination(pos);
                }
            }
        }

        public void mouseWheelEvent(float delta)
        {
            Vector3 focusToPosition = this.transform.position - this.focus;

            Vector3 post = focusToPosition * (1.0f + delta);

            if (post.magnitude > 0.01)
                this.transform.position = this.focus + post;

            return;
        }

        void cameraTranslate(Vector3 vec)
        {
            Transform focusTrans = this.focusObj.transform;

            vec.x *= -1;

            focusTrans.Translate(Vector3.right * vec.x);
            focusTrans.Translate(Vector3.up * vec.y);

            this.focus = focusTrans.position;

            return;
        }

        public void cameraRotate(Vector3 eulerAngle)
        {
            //Use Quaternion to prevent rotation flips on XY plane
            Quaternion q = Quaternion.identity;

            Transform focusTrans = this.focusObj.transform;
            focusTrans.localEulerAngles = focusTrans.localEulerAngles + eulerAngle;

            //Change this.transform.LookAt(this.focus) to q.SetLookRotation(this.focus)
            q.SetLookRotation(this.focus);

            return;
        }
    }
}
