using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public bool isOpen = true;
    public float speed = 2f;
    public float offset = 0.7f;

    private Vector3 closedPosition_LeftDoor;
    private Vector3 closedPosition_RightDoor;

    private Vector3 opendedPosition_LeftDoor;
    private Vector3 opendedPosition_RightDoor;

    // Start is called before the first frame update
    void Start()
    {
        closedPosition_LeftDoor = leftDoor.position;
        closedPosition_RightDoor = rightDoor.position;
        opendedPosition_LeftDoor = new Vector3(leftDoor.position.x-offset, leftDoor.position.y, leftDoor.position.z);
        opendedPosition_RightDoor = new Vector3(rightDoor.position.x+offset, rightDoor.position.y, rightDoor.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(isOpen){        
           leftDoor.position = Vector3.Lerp(leftDoor.position, opendedPosition_LeftDoor,Time.deltaTime * speed);
           rightDoor.position = Vector3.Lerp(rightDoor.position, opendedPosition_RightDoor,Time.deltaTime * speed);
        }else{
            leftDoor.position = Vector3.Lerp(leftDoor.position, closedPosition_LeftDoor,Time.deltaTime * speed);
            rightDoor.position = Vector3.Lerp(rightDoor.position, closedPosition_RightDoor,Time.deltaTime * speed);
          
        }
        
    }
}
