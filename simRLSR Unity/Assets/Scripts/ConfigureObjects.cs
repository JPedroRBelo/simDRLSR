using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

public class ConfigureObjects : MonoBehaviour
{

    public List<GameObject> knowLocations;

    public List<GameObject> publicChairs;
    public List<GameObject> workChairs;
    public List<GameObject> magazines;
    public List<GameObject> sidedoors;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject gO in knowLocations)
        {
            gO.tag = Constants.TAG_KNOWLOCATIONS;
        }
        foreach (GameObject gO in publicChairs)
        {
            gO.tag = Constants.TAG_PUBLICCHAIR;
        }
        foreach (GameObject gO in workChairs)
        {
            gO.tag = Constants.TAG_WORKCHAIR;
        }
        foreach (GameObject gO in magazines)
        {
            gO.tag = Constants.TAG_MAGAZINE;
        }
        foreach (GameObject gO in sidedoors)
        {
            gO.tag = Constants.TAG_SIDEDOOR;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
