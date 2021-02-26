using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using OntSenseCSharpAPI;
public class DoorManager : Status {

    // Use this for initialization


    private const int IN = 1;
    private const int OUT = 0;
   
    public float angleOpened = -135f;
    public float angleClosed = 0f;

    private float initialAngle;
    
    private Quaternion initialQuaternion;
    private Quaternion closedQuaternion;
    private Quaternion openedQuaternion;

    private List<Transform> locationsReferences;

    private Transform inClosed;
    private Transform inOpened;
    private Transform outClosed;
    private Transform outOpened;

    void Start () {
        initialAngle = transform.rotation.eulerAngles.y;
        initialQuaternion = transform.rotation;
        closedQuaternion = transform.rotation;
        openedQuaternion = Quaternion.Euler(transform.rotation.x, (initialAngle + angleOpened), transform.rotation.z);

        inClosed = transform.Find(Constants.DOOR_IN_CLOSED);
        inOpened = transform.Find(Constants.DOOR_IN_OPEN);
        outClosed = transform.Find(Constants.DOOR_OUT_CLOSED);
        outOpened = transform.Find(Constants.DOOR_OUT_OPEN);
        locationsReferences = new List<Transform>();
        foreach(Transform t in transform)
        {
            if (t.name.Equals(Constants.REF_LOCATION))
            {
                locationsReferences.Add(t);
            }
        }
        openedQuaternion = Quaternion.Euler(initialQuaternion.eulerAngles.x, initialAngle + angleOpened, initialQuaternion.eulerAngles.x);
        if (status == PhysicalState.openState)
        {
            transform.rotation = openedQuaternion;
            changeLocationsReferences();
        }
        else
        {
            transform.rotation = closedQuaternion;
            changeLocationsReferences();
        }
    }
	
	// Update is called once per frame
	void Update () {
        openedQuaternion = Quaternion.Euler(initialQuaternion.eulerAngles.x, initialAngle + angleOpened, initialQuaternion.eulerAngles.x);
        if (status == PhysicalState.openState)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openedQuaternion, Time.deltaTime * speed);
            changeLocationsReferences();
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closedQuaternion, Time.deltaTime * speed);
            changeLocationsReferences();
        }
	}

    private void changeLocationsReferences()
    {
        if (locationsReferences.Count == 2)
        {
            if (status == PhysicalState.openState)
            {
                locationsReferences[IN].position = inOpened.position;
                locationsReferences[OUT].position = outOpened.position;
            }
            else
            {
                locationsReferences[IN].position = inClosed.position;
                locationsReferences[OUT].position = outClosed.position;
            }
        }
        
    }


    public override void turnOnOpen()
    {
        status = PhysicalState.openState;
        Debug.Log("RHS>>> " + this.name + " opened.");

    }

    public override void turnOffClose()
    {
        status = PhysicalState.closeState;
        Debug.Log("RHS>>> " + this.name + " closed.");
    }

}
