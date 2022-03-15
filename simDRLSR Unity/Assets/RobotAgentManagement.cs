using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class RobotAgentManagement : MonoBehaviour
{
    public Transform robotInitialLocations;
    public GameObject robot;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
     
    public bool randomPosition;

    private List<Transform> locations;
    // Start is called before the first frame update
    void Start()
    {        
        originalPosition = robot.transform.position;
        originalRotation = robot.transform.rotation;
    	StartCoroutine(LateStart(3));
     }
 
     IEnumerator LateStart(float waitTime)
     {
        yield return new WaitForSeconds(waitTime);
        if(randomPosition){
            setRandomPosition();
        }         
     }

     public void setRandomPosition(){     
        randomPosition = false;
        int index = 0;
        locations = new List<Transform>();
        if(robotInitialLocations!=null){
            foreach (Transform child in robotInitialLocations.transform)
            locations.Add(child);

            var rnd = new System.Random();
            var randomized = locations.OrderBy(item => rnd.Next());
            Transform random_position = randomized.ToList()[index++%randomized.Count()];
            robot.transform.position = random_position.position;
            robot.transform.rotation = random_position.rotation; 
        }
     }

    public void setInitPosition(){
        randomPosition = false;
        robot.transform.position = originalPosition;
        robot.transform.rotation = originalRotation;
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
