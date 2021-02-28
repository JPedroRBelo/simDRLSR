using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TouchManager : MonoBehaviour {

    private HashSet<GameObject> knowObjects;
    private HashSet<GameObject> unknowObjects;
    private HashSet<GameObject> updKnowElementsList;
    private HashSet<GameObject> updUnknowElementsList;
    private CaptureTouch[] touchSensors;
    public int refreshRate = 30;

    public bool printLog = false;
    private int count;
    private Hand[] hands;
    void Start () {
        hands = GetComponent<AgentInteraction>().getHands();
        touchSensors = GetComponentsInChildren<CaptureTouch>();
        knowObjects = new HashSet<GameObject>();
        updKnowElementsList = new HashSet<GameObject>();
        unknowObjects = new HashSet<GameObject>();
        updUnknowElementsList = new HashSet<GameObject>();
        Log("RHS>>> " + this.name + " touch was configured with success");
    }


     private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }
	
	// Update is called once per frame
	void Update () {
        foreach (CaptureTouch cT in touchSensors)
        {
            GameObject auxGO = cT.getActTouch();
            if(auxGO != null)
            {
                bool flag = false;
                foreach(Hand hand in hands)
                {
                    flag = GameObject.ReferenceEquals(hand.objInHand, auxGO.transform);
                    if (flag) break;               
                }
                if (flag)
                {
                    knowObjects.Add(auxGO);
                }else
                {
                    unknowObjects.Add(auxGO);
                }

               
            }
        }
        updKnowElementsList = new HashSet<GameObject>(knowObjects);
        knowObjects = new HashSet<GameObject>();
        updUnknowElementsList = new HashSet<GameObject>(unknowObjects);
        unknowObjects = new HashSet<GameObject>();
    }

    public List<GameObject> getListOfElements()
    {
        if (updUnknowElementsList != null || updKnowElementsList != null)
            return updKnowElementsList.Union(updUnknowElementsList).ToList();
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
    public List<GameObject> getListOfKnowElements()
    {
        if (updKnowElementsList != null)
            return updKnowElementsList.ToList();
        else
            return new List<GameObject>();
    }



}
