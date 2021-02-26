using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueIdDistributor : MonoBehaviour {


    private Dictionary<int, GameObject> dictIds;
    private Dictionary<GameObject, int> dictObjs;

    public bool printLog = false;
    // Use this for initialization
    void Start () {
        dictIds = new Dictionary<int, GameObject>();
        dictObjs = new Dictionary<GameObject,int>();
        foreach (GameObject gO in Object.FindObjectsOfType(typeof(GameObject)))
        {
            dictIds.Add(gO.GetInstanceID(), gO);
            dictObjs.Add(gO,gO.GetInstanceID());
        }
        Log("RHS>>> " + this.name + " generated the IDs.");
    }

	
    public bool isValidId(int id)
    {
        return dictIds.ContainsKey(id);
    }

    public GameObject getGameObjectById(int id)
    {
        if (dictIds.ContainsKey(id))
        {
            return dictIds[id];
        }
        return null;
    }

    public int getID(GameObject obj)
    {
        if (dictObjs.ContainsKey(obj))
        {
            return dictObjs[obj];
        }
        return 0;
    }

    public bool insertGameObject(GameObject gO)
    {
        if (!dictIds.ContainsKey(gO.GetInstanceID()))
        {
            dictIds.Add(gO.GetInstanceID(), gO);
            return true;
        }else
        {
            return false;
        }
    }

    private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }
}
