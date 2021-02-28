using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectsManager : MonoBehaviour {

    // Use this for initialization

    public Transform component;
    private Text text;
    public Camera cam;
    
    private int height;
    private int width;
    private HashSet<GameObject> gameObjects;
    //private int step = 50;

    private SimulatorCommandsManager scm;
    private VisionManager vision;

    Renderer[] renderers;
    private ArrayList objectsInScene;
    private GameObject[] locationsOnScene;

	void Start () {

        locationsOnScene = GameObject.FindGameObjectsWithTag(Constants.TAG_LOCATION);
        scm = GetComponent<SimulatorCommandsManager>();
        vision = scm.robot.GetComponent<VisionManager>();

        height = cam.pixelHeight;
        width = cam.pixelWidth;
        gameObjects = new HashSet<GameObject>();
        text = component.GetComponent<Text>();
        text.text = "";
        
        foreach (GameObject item in locationsOnScene)
        {
            //dropObjects.options.Add(new Dropdown.OptionData() { text = item.name });
        }
        

    }

    void Update()
    {
        HashSet<GameObject> elementsSeenByRobot = new HashSet<GameObject>(vision.getListOfElements());
        //dropObjects.options.Clear();
        string auxText = "";
        foreach (GameObject item in elementsSeenByRobot)
        {
            if (item.tag == Constants.TAG_OBJECT )
            {
                //dropObjects.options.Add(new Dropdown.OptionData() { text = item.name });
            }
            auxText = auxText + "\n---\n" + item.tag + ": " + item.name + " " + item.transform.position;
        
        }
        text.text = auxText + "\n\n";
        
    }

    /*
	// Update is called once per frame
	void Update () {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        print(planes[4].distance+" "+planes[5].distance);
        renderers = (Renderer[])Object.FindObjectsOfType(typeof(Renderer));
        string auxText = "";
        foreach (Renderer item in renderers)
        {
            if (item.IsVisibleFrom(camera))
            {
                

                RaycastHit hit;
                Ray landingRay = new Ray(camera.transform.position, item.transform.position - camera.transform.position);
                


                if (Physics.Raycast(landingRay,out hit, 100))
                {
                    string itemName = hit.collider.transform.name;
                    Transform gO = hit.collider.transform;
                    if(itemName == "Collider" || itemName == "GameObject")
                    {
                        gO = hit.collider.transform.parent;
                    }
                    
                    if (gO == item.transform)
                    {

                        auxText = auxText + "\n---\n" + item.tag + ": " + item.name + " " + item.transform.position;
                    //    auxText = auxText + "\n" + item.transform.name;
                    }
                }
                

            }
           
        }
        text.text = auxText + "\n\n";
    }

    */
}
