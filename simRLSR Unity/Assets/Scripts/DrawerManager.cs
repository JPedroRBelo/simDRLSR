using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntSenseCSharpAPI;
public class DrawerManager : Status {

    //public bool opened = false;
    public float posOpened = 0.6f;
    public float posClosed = -0.026f;

    private Vector3 vector3Opened;
    private Vector3 vector3Closed;

    private Transform outClosed;
    public Transform outOpened;
    private Transform locationReference;
    private Transform positionReference;
    //public float speed = 2f;
    // Use this for initialization
    void Start () {
        outClosed = transform.Find(Constants.DOOR_OUT_CLOSED);
        outOpened = transform.Find(Constants.DOOR_OUT_OPEN);
        locationReference = transform.Find(Constants.REF_LOCATION);
        positionReference = transform.Find(Constants.REF_POSITION);
        vector3Opened = new Vector3(posOpened, transform.localPosition.y, transform.localPosition.z);
        vector3Closed = new Vector3(posClosed, transform.localPosition.y, transform.localPosition.z);
        if (status == PhysicalState.openState)
        {
            transform.localPosition = vector3Opened;
        }else
        {
            transform.localPosition = vector3Closed;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (status == PhysicalState.openState)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, vector3Opened, Time.deltaTime * speed);
            changeLocationsReferences();
        }else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, vector3Closed, Time.deltaTime * speed);
            changeLocationsReferences();
        }
	}

    private void changeLocationsReferences()
    {
        if (locationReference != null && positionReference != null && outClosed != null && outOpened != null)
        {
            if (status == PhysicalState.openState)
            {
                locationReference.position = outOpened.position;
            }
            else
            {
                locationReference.position = outClosed.position;
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
