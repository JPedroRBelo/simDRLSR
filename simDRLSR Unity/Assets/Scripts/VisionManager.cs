using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System.Diagnostics;

public class VisionManager : MonoBehaviour {

    public Camera cam = null;
    private int height;
    private int width;
    private HashSet<GameObject> gameObjects;
    private HashSet<GameObject> updatedElementsList;
    private HashSet<GameObject> allGameObjects;
    private int step = 10;
    public bool verifyVisibleObjects = false;
    




    void Start () {
        if(cam == null)
        {
            Transform head = null;
            Animator animator = GetComponent<Animator>();
            if(animator != null)
            {
               head = animator.GetBoneTransform(HumanBodyBones.Head);
            }
            if(head != null)
            {
                cam = head.Find("CameraHead").GetComponent<Camera>();
            }
        }
        if(cam != null)
        {
            height = cam.pixelHeight;
            width = cam.pixelWidth;
        }
        else
        {
            height = 480;
            width = 640;
        }
        gameObjects = new HashSet<GameObject>();
        updatedElementsList = new HashSet<GameObject>();
        Debug.Log("RHS>>> " + this.name + " vision was configured with success."); 
    }
	
	// Update is called once per frame
	void Update () {
        if (verifyVisibleObjects)
        {
            gameObjects = new HashSet<GameObject>();
            gameObjects.Add(transform.gameObject);
            for (int i = 0; i < width / step; i++)
            {
                for (int j = 0; j < height / step; j++)
                {
                    Ray ray = cam.ScreenPointToRay(new Vector3(i * step, j * step, 0));
                    RaycastHit hit;
                    Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        string itemName = hit.collider.gameObject.name;
                        GameObject gO = hit.collider.gameObject;
                        if (itemName.Contains("Collider", StringComparison.OrdinalIgnoreCase) || itemName.Contains("GameObject", StringComparison.OrdinalIgnoreCase))
                        {
                            gO = hit.collider.transform.parent.gameObject;
                        }
                        gameObjects.Add(gO);
                    }
                }
            }
            updatedElementsList = new HashSet<GameObject>(gameObjects);
        }
    }

    public List<GameObject> getListOfElements()
    {
        if (verifyVisibleObjects)
        {
            if (updatedElementsList != null)
                return updatedElementsList.ToList();
            else
                return new List<GameObject>();
        }       
        else
        {
            allGameObjects = new HashSet<GameObject>();
            foreach (GameObject gO in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                allGameObjects.Add(gO);
            }
            return allGameObjects.ToList();
        }
    }

}
