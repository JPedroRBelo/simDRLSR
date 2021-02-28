using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SmellManager : MonoBehaviour {

    public Transform smellSensor;

    public bool printLog = false;
    private CaptureSmell captureSmell;
    private HashSet<GameObject> knowObjects;
    private HashSet<GameObject> unknowObjects;
    private HashSet<GameObject> updKnowElementsList;
    private HashSet<GameObject> updUnknowElementsList;
    public int refreshRate = 300;
    private int count;
    // Use this for initialization
    void Start()
    {
        if (smellSensor == null)
        {
            Log("RHS>>> ERROR! Smell Sensor is missing!");
        }else
        {
            captureSmell = smellSensor.GetComponent<CaptureSmell>();
        }        
        count = 0;
        knowObjects = new HashSet<GameObject>();
        updKnowElementsList = new HashSet<GameObject>();
        unknowObjects = new HashSet<GameObject>();
        updUnknowElementsList = new HashSet<GameObject>();
        Log("RHS>>> " + this.name + " smell was configured with success.");
    }
    
    private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (captureSmell != null) {
            if (count < refreshRate)
            {
                GameObject auxGameObject = captureSmell.getActSmell();
                if (auxGameObject != null)
                {
                    if (captureSmell.isObjectInSmellInSensor())
                    {
                        knowObjects.Add(auxGameObject);
                        count = 0;
                    }
                    else
                        unknowObjects.Add(auxGameObject);
                }
                count++;
            }
            else
            {
                updKnowElementsList = new HashSet<GameObject>(knowObjects);
                knowObjects = new HashSet<GameObject>();
                updUnknowElementsList = new HashSet<GameObject>(unknowObjects);
                unknowObjects = new HashSet<GameObject>();
                count = 0;
            }
        }
    }    

    public string getAllSmellStrings()
    {
        string aux = "";
        foreach (GameObject smell in knowObjects)
        {
            aux += " " + smell.name;
        }
        return aux;
    }

    public List<GameObject> getListOfElements()
    {
        if (updUnknowElementsList != null || updKnowElementsList != null)
            return updKnowElementsList.Union(updUnknowElementsList).ToList();
        else
            return new List<GameObject>();
    }

    public List<GameObject> getListOfKnowElements()
    {
        if (updKnowElementsList != null)
            return updKnowElementsList.ToList();
        else
            return new List<GameObject>();
    }

    public List<GameObject> getListOfUnknowElements()
    {
        if (updUnknowElementsList != null)
            return updUnknowElementsList.ToList();
        else
            return new List<GameObject>();
    }


}
