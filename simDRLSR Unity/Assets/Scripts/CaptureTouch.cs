using System;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTouch : MonoBehaviour {

    private GameObject actTouch;
    public int updateRate = 30;
    private int count;

    // Use this for initialization
    void Start () {
        actTouch = null;
        count = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (count >= updateRate)
        {
            actTouch = null;
            count = 0;
        }
        else
        {
            count++;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        string itemName = collider.gameObject.name;
        GameObject gO = collider.gameObject;
        if (itemName.Contains("Collider", StringComparison.OrdinalIgnoreCase) || itemName.Contains("GameObject", StringComparison.OrdinalIgnoreCase))
        {
            gO = collider.transform.parent.gameObject;
        }
        bool auxBool = gO.transform.parent != null;
        if (auxBool)
            auxBool = gO.transform.parent.gameObject != transform.gameObject;
        else
            auxBool = true;

        if (!gO.tag.Equals(Constants.TAG_BODYSENSOR)  && transform.parent.gameObject != gO && auxBool)
        {
            actTouch = gO;
        }
    }

    void OnTriggerExit(Collider collider)
    {

    }

    public GameObject getActTouch()
    {
        return actTouch;
    }   
}
