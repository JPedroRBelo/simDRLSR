using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntSenseCSharpAPI;
public class SwitchManager : Status {

    public List<Light> lightList = new List<Light>();
    
    
	void Start () {

	}
	
	void Update () {
        foreach(Light light in lightList)
        {
           light.enabled = status == PhysicalState.onState;
        }		
	}

    public override void turnOnOpen()
    {
        status = PhysicalState.onState;
        Debug.Log("RHS>>> " + this.name + " on.");
    }

    public override void turnOffClose()
    {
        status = PhysicalState.offState;
        Debug.Log("RHS>>> " + this.name + " off.");
    }

}
