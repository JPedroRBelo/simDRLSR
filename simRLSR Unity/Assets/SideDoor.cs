using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntSenseCSharpAPI;

public class SideDoor : Status {
    public Transform topDoor;
    public Transform bottomDoor;
    public bool isOpen = true;
    public float angleOpened = 90;
    public float angleClosed = 0;
    //public float speed = 2f;

    private float initialAngle_topDoor;
    private float initialAngle_bottomDoor;
    private Quaternion initialQuaternion_topDoor;
    private Quaternion initialQuaternion_bottomDoor;
    private Quaternion closedQuaternion_topDoor;
    private Quaternion closedQuaternion_bottomDoor;
    private Quaternion openedQuaternion_topDoor;
    private Quaternion openedQuaternion_bottomDoor;
    private float angleOpened_bottomDoor;
    private float angleClosed_bottomDoor;
    private float angleOpened_topDoor;
    private float angleClosed_topDoor;







    // Start is called before the first frame update
    void Start()
    {
        angleOpened_bottomDoor = angleOpened;
        angleClosed_bottomDoor = angleClosed;
        angleOpened_topDoor = angleOpened;
        angleClosed_topDoor = angleClosed;
        initialAngle_topDoor = topDoor.rotation.eulerAngles.z;
        initialQuaternion_topDoor = topDoor.rotation;
        closedQuaternion_topDoor = topDoor.rotation;
        //openedQuaternion_topDoor = Quaternion.Euler(topDoor.rotation.x, (initialAngle + angleOpened), topDoor.rotation.z);
        openedQuaternion_topDoor = Quaternion.Euler(initialQuaternion_topDoor.eulerAngles.x,  initialQuaternion_topDoor.eulerAngles.y, initialAngle_topDoor - angleOpened);


        initialAngle_bottomDoor = bottomDoor.rotation.eulerAngles.y;
        initialQuaternion_bottomDoor = bottomDoor.rotation;
        closedQuaternion_bottomDoor = bottomDoor.rotation;
        //openedQuaternion = Quaternion.Euler(bottomDoor.rotation.x, (initialAngle + angleOpened), bottomDoor.rotation.z);
        openedQuaternion_bottomDoor = Quaternion.Euler(initialQuaternion_bottomDoor.eulerAngles.x, initialAngle_bottomDoor - angleOpened_bottomDoor, initialQuaternion_bottomDoor.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (status == PhysicalState.openState)
        {
            
            openedQuaternion_topDoor = Quaternion.Euler(initialQuaternion_topDoor.eulerAngles.x,  initialQuaternion_topDoor.eulerAngles.y, initialAngle_topDoor - angleOpened);
            topDoor.rotation = Quaternion.Lerp(topDoor.rotation, openedQuaternion_topDoor, Time.deltaTime * speed);
            openedQuaternion_bottomDoor = Quaternion.Euler(initialQuaternion_bottomDoor.eulerAngles.x, initialAngle_bottomDoor - angleOpened_bottomDoor, initialQuaternion_bottomDoor.eulerAngles.z);
            bottomDoor.rotation = Quaternion.Lerp(bottomDoor.rotation, openedQuaternion_bottomDoor, Time.deltaTime * speed);
        }else{
            topDoor.rotation = Quaternion.Lerp(topDoor.rotation, closedQuaternion_topDoor, Time.deltaTime * speed);
            bottomDoor.rotation = Quaternion.Lerp(bottomDoor.rotation, closedQuaternion_bottomDoor, Time.deltaTime * speed);
        }        
    }



    public override void turnOnOpen()
    {
        status = PhysicalState.openState;

    }

    public override void turnOffClose()
    {
        status = PhysicalState.closeState;
    }
}
