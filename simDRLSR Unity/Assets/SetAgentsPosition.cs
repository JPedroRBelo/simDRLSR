using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



public class SetAgentsPosition : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform initialLocations;
    public List<Transform> human_avatas;
      
    public bool randomPosition;

    private List<Transform> locations;
    void Start()
    {
        int index = 0;
        locations = new List<Transform>();
        if(initialLocations!=null){
            foreach (Transform child in initialLocations.transform)
               locations.Add(child);
            var rnd = new System.Random();
            var randomized = locations.OrderBy(item => rnd.Next());
            if(randomPosition){
                foreach(Transform human in human_avatas){
                    Vector3 new_position = randomized.ToList()[index++%randomized.Count()].position;
                    float y_pos = human.transform.position.y+human.transform.position.y;
                    human.transform.position = new Vector3(new_position.x,y_pos,new_position.z);
                }
            }
        }

        //foreach()
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAvatarRandomPositionFlag(bool flag){
        randomPosition = flag;
    }

      
}
