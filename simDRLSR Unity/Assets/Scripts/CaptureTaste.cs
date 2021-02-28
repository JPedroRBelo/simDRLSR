using System;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTaste : MonoBehaviour {

    private GameObject actTaste;
    public int updateRate = 30;
    private int count;
    private GameObject lastTaste;
    private bool isObjectInSensor;
    private bool isToCapture = false;

    void Start()
    {
        isObjectInSensor = false;
        actTaste = null;
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (count >= updateRate)
        {
            actTaste = null;
            count = 0;
        }
        else
        {
            count++;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        captureObject(collider);
    }

    void OnTriggerEnter(Collider collider)
    {
        captureObject(collider);
    }

    private void captureObject(Collider collider)
    {
        if (isToCapture)
        {
            isObjectInSensor = true;
            string itemName = collider.gameObject.name;
            GameObject gO = collider.gameObject;
            if (itemName.Contains("Collider", StringComparison.OrdinalIgnoreCase) || itemName.Contains("GameObject", StringComparison.OrdinalIgnoreCase))
            {
                gO = collider.transform.parent.gameObject;
            }
            actTaste = gO;
            lastTaste = gO;
        }
    }
    
    void OnTriggerExit(Collider collider)
    {
        isObjectInSensor = false;
    }

    public GameObject getActTaste()
    {
        return actTaste;
    }

    public void capture()
    {
        isToCapture = true;
    }

    public void stopCature()
    {
        isToCapture = false;
    }
    public GameObject getLastTaste()
    {
        return lastTaste;
    }

    public bool isObjectInTasteSensor()
    {
        return isObjectInSensor;
    }

}
