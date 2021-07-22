using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System.Diagnostics;

public class HumanVisionManager : MonoBehaviour {

    public bool drawLines = false;
    public float horizontalLookAngle = 135f;
    public float verticalLookAngle = 90f;
    private GameObject robot;
    //private bool robotVisible = false;
    private Transform personHead;
    public bool printLog = false;

    void Start () {
        robot = GameObject.FindGameObjectWithTag("Robot");
        personHead = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);


        if (robot == null)
        {
             Log("Robot not found!");
        }
        /*
        else
        {
            robotVisible = false;
           
        }
        */
    }

    private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }

    
	
	void Update () {
        if (robot != null)
        {
           // isRobotVisible(robot);
        }
    }

    public bool isRobotVisible()
    {
        return isRobotVisible(robot);
    }
    
    private bool isRobotVisible(GameObject robot)
    {
       
        float maxRange = 10f;
        RaycastHit hit;

        Transform robotHead = robot.gameObject.GetComponent<RobotInteraction>().getRobotHead();


        if (Physics.Raycast(personHead.position, (robotHead.position - personHead.position), out hit))
        {
            
           /*
            Vector3 angles_robot = get3DAngles(neckOriginalTransform.transform, robotHead, "left");
            float hrobot_angle = (neckOriginalTransform.transform.localEulerAngles - angles_robot)[1];
            float vrobot_angle = -(neckOriginalTransform.transform.localEulerAngles - angles_robot)[2];
           */

            Vector3 angles_camera = get3DAngles(personHead.transform, robotHead, "forward");
            float hcam_angle = angles_camera[1];
            float vcam_angle = angles_camera[2];



            if (drawLines) print(transform.name + " " +hcam_angle);
            if (drawLines) print(transform.name + " " + vcam_angle);


            Debug.DrawRay(personHead.position, (robotHead.position - personHead.position), Color.blue);
            if (hit.transform == robotHead)
            {
                
                if ((Math.Abs(hcam_angle) <= (horizontalLookAngle / 2)) && (Math.Abs(vcam_angle) <= (verticalLookAngle / 2)))
                {



                    if (drawLines) Debug.DrawRay(personHead.position, (hit.transform.position - personHead.position), Color.green);
                    return true;                
                }
                else
                {
                    if (drawLines) Debug.DrawRay(personHead.position, (hit.transform.position - personHead.position), Color.blue);
                }
                
                
            }
            else
            {
                if (drawLines) Debug.DrawRay(personHead.position, (hit.transform.position - personHead.position), Color.yellow);
            }

        }
        else
        {

            if (drawLines) Debug.DrawRay(personHead.position, (hit.transform.position - personHead.position), Color.red);
        }
        
        return false;
    }


    public Vector3 get3DAngles(Transform refPosition, Transform obj, string direction_name = "forward")
    {

        Vector3 direction = refPosition.forward;
        if (direction_name.Equals("back"))
        {
            direction = -refPosition.forward;
        }
        else if (direction_name.Equals("right"))
        {
            direction = refPosition.right;
        }
        else if (direction_name.Equals("left"))
        {
            direction = -refPosition.right;
        }
        else if (direction_name.Equals("up"))
        {
            direction = refPosition.up;
        }
        else if (direction_name.Equals("down"))
        {
            direction = -refPosition.up;
        }

        float angleXZ;
        float angleYZ;
        float angleXY;

        Vector2 objPosXZ = new Vector2(obj.position.x, obj.position.z);
        Vector2 camPosXZ = new Vector2(refPosition.position.x, refPosition.position.z);
        Vector2 camFwrXZ = new Vector2(direction.x, direction.z);

        Vector2 directTargetXZ = objPosXZ - camPosXZ;
        angleXZ = Vector2.SignedAngle(camFwrXZ, directTargetXZ);
        Vector3 crossXZ = Vector3.Cross(camFwrXZ, directTargetXZ);

        /*
        if (crossXZ.y < 0)
        {
            angleXZ = -angleXZ;
        }
        */

        Vector2 objPosYZ = new Vector2(obj.position.y, obj.position.z);
        Vector2 camPosYZ = new Vector2(refPosition.position.y, refPosition.position.z);
        Vector2 camFwrYZ = new Vector2(direction.y, direction.z);

        Vector2 directTargetYZ = objPosYZ - camPosYZ;
        angleYZ = Vector2.SignedAngle(camFwrYZ, directTargetYZ);
        Vector3 crossYZ = Vector3.Cross(camFwrYZ, directTargetYZ);

        /*
        if (crossYZ.y < 0)
        {
            angleYZ = -angleYZ;
        }
        */

        Vector2 objPosXY = new Vector2(obj.position.x, obj.position.y);
        Vector2 camPosXY = new Vector2(refPosition.position.x, refPosition.position.y);
        Vector2 camFwrXY = new Vector2(direction.x, direction.y);

        Vector2 directTargetXY = objPosXY - camPosXY;
        angleXY = Vector2.SignedAngle(camFwrXY, directTargetXY);
        Vector3 crossXY = Vector3.Cross(camFwrXY, directTargetXY);

        /*
        if (crossXY.y < 0)
        {
            angleXY = -angleXY;
        }
        */
        Vector3 angles = new Vector3(angleXY, angleXZ, angleYZ);
        return angles;
    }


}
