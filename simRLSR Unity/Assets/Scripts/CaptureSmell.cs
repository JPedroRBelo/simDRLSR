using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSmell : MonoBehaviour {
    
    private GameObject actSmell;
    public  int updateRate = 30;
    private int count;
    private bool isObjectInSensor;
    void Start () {
        isObjectInSensor = false;
        actSmell = null;
        count = 0;
    }
	
	// Update is called once per frame
    void Update()
    {
        if (count >= updateRate)
        {
            actSmell = null;
            count = 0;
        }
        else
        {
            count++;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        count = 0;
        actSmell = other.transform.parent.gameObject;
        float dist = Vector3.Distance(actSmell.transform.position, transform.position);
        isObjectInSensor = dist < 0.2f;
    }
    //Objetos sem cheiro
    public void putObjInSmell(GameObject other)
    {
        count = 0;
        actSmell = other;
        float dist = Vector3.Distance(actSmell.transform.position, transform.position);
        isObjectInSensor = dist < 0.2f;
    }

    public GameObject getActSmell()
    {
        return actSmell;
    }

    public bool isObjectInSmellInSensor()
    {
        return isObjectInSensor;
    }

}
