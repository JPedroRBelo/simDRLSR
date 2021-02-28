using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmellPerception: MonoBehaviour
{
    private GameObject obj;
    private bool isSmelled;

	public SmellPerception(GameObject obj, bool isSmelled)
    {
        this.obj = obj;
        this.isSmelled = isSmelled;
    }

    public GameObject getObject()
    {
        return obj;
    }

    public bool objWasSmelled()
    {
        return isSmelled;
    }
}
