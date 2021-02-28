using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasteProperties : MonoBehaviour {

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float sweetness;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float sourness;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float saltiness;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float bitterness;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float umami;
    
    void Start () {
		
	}

    public float getSweetness()
    {
        return sweetness;
    }

    public float getSourness()
    {
        return sourness;
    }

    public float getSaltiness()
    {
        return saltiness;
    }

    public float getBitterness()
    {
        return bitterness;
    }

    public float getUmami()
    {
        return umami;
    }

    public string getTasteStatus()
    {
        string str = "Swetness: "+ sweetness;
        str += " Sourness: " + sourness;
        str += " Saltiness: " + saltiness;
        str += " Bitterness: " + bitterness;
        str += " Umami: " + umami;
        return str;
    }
}
