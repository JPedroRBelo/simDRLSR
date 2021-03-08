using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
     
public class CameraDrag:MonoBehaviour {
         public float speed = 3.5f;
         private float X;
         private float Y;

        float mainSpeed = 10.0f; //regular speed
        float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
        float maxShift = 1000.0f; //Maximum speed when holdin gshift
        private float totalRun= 1.0f;

        private bool followCamera = false;
     
        public Transform robot;
        public List<Transform> agents;

        private float zoomFactor = 1f;
        private float yaxisFactor = 2.0f;
        private float xaxisFactor = 0f;

        private float original_zoomFactor = 1.5f;
        private float original_yaxisFactor = 2.0f;
        private float original_xaxisFactor = 0f;

        void Update() {
            if (Input.GetKeyDown(KeyCode.C))
            { 
                followCamera = !followCamera; 
            }
            if(!followCamera){
                Drag();
            }else{

                if((agents != null)&&(agents.Count>0)){
                    Drag();
                    FixedCameraFollowSmooth(transform.GetComponent<Camera>(),robot,agents[0]);
                }else{
                    followCamera = false;
                    Debug.Log("Error: set an agent to list to change camera.");
                }
                
            }
                
        
        }


        private void Drag()
        {
            if(Input.GetMouseButton(0)) {
                transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * speed, Input.GetAxis("Mouse X") * speed, 0));
                X = transform.rotation.eulerAngles.x;
                Y = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(X, Y, 0);
                //Keyboard commands
                float f = 0.0f;
                Vector3 p = GetBaseInput();
                if (Input.GetKey (KeyCode.LeftShift)){
                    totalRun += Time.deltaTime;
                    p  = p * totalRun * shiftAdd;
                    p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                    p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                    p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
                }
                else{
                    totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                    p = p * mainSpeed;
                }
            
                p = p * Time.deltaTime;
                Vector3 newPosition = transform.position;
                if (Input.GetKey(KeyCode.Space)){ //If player wants to move on X and Z axis only
                    transform.Translate(p);
                    newPosition.x = transform.position.x;
                    newPosition.z = transform.position.z;
                    transform.position = newPosition;
                }
                else
                {
                    transform.Translate(p);
                }
            }
        }
    
      
     
    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey (KeyCode.W)){
            p_Velocity += new Vector3(0, 0 , 1);
        }
        if (Input.GetKey (KeyCode.S)){
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey (KeyCode.A)){
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey (KeyCode.D)){
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }


     // Follow Two Transforms with a Fixed-Orientation Camera
    public void FixedCameraFollowSmooth(Camera cam, Transform t1, Transform t2)
    {

        if (Input.GetKey (KeyCode.W)){
            yaxisFactor += 0.1f;
        }
        if (Input.GetKey (KeyCode.S)){
            yaxisFactor -= 0.1f;
        }
        if (Input.GetKey (KeyCode.A)){
            zoomFactor += 0.01f;
        }
        if (Input.GetKey (KeyCode.D)){
            zoomFactor -= 0.01f;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f){
            zoomFactor -= 0.02f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f){
            zoomFactor += 0.02f;
        }
        if (Input.GetMouseButtonDown(2)){
            yaxisFactor = original_yaxisFactor;
            xaxisFactor = original_xaxisFactor;
            zoomFactor = original_zoomFactor;
        }
           

        // How many units should we keep from the players
        //float zoomFactor = 0.8f;
        float followTimeDelta = 0.8f;
    
        // Midpoint we're after
        Vector3 midpoint = (t1.position + t2.position) / 2f;
    
        // Distance between objects
        float distance = (t1.position - t2.position).magnitude;
    
        // Move camera a certain distance
        Vector3 cameraDestination = midpoint - cam.transform.forward * distance * zoomFactor;
        Vector3 cameraOffSet = new Vector3(0,yaxisFactor,xaxisFactor);
        cameraDestination+=cameraOffSet;
    
        // Adjust ortho size if we're using one of those
        if (cam.orthographic)
        {
            // The camera's forward vector is irrelevant, only this size will matter
            cam.orthographicSize = distance;
        }

        

        // You specified to use MoveTowards instead of Slerp
        cam.transform.position = Vector3.Slerp(cam.transform.position, cameraDestination, followTimeDelta);
            
        // Snap when close enough to prevent annoying slerp behavior
        if ((cameraDestination - cam.transform.position).magnitude <= 0.05f)
        {
            cam.transform.position = cameraDestination;
        }
    }

}

    
