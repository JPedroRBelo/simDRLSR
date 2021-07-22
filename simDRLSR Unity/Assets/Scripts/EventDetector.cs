using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Events
{   
    HandTouch,
    EyeGaze
}

public class EventDetector : MonoBehaviour
{

    private int stepAt;

    public float neutralReward = 0;

    private Animator animator;
    private Dictionary<Events,bool> lastStepEvents;
    private RobotInteraction robotIHR;


    // Start is called before the first frame update
    void Start()
    {
        stepAt = -1;
        animator = GetComponent<Animator>();
        robotIHR = GetComponent<RobotInteraction>();
        initEventsDict(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if(robotIHR.getHandTouch()){
            lastStepEvents[Events.HandTouch] = true;
        }
         

        foreach (GameObject person in GameObject.FindGameObjectsWithTag("Person"))
        {
            if(person.GetComponent<AvatarBehaviors>().isHumanEngagedWithRobot()){
                GameObject robotAttention = GetComponent<RobotInteraction>().getPersonFocusedByRobot();
                if(robotAttention==person){
                    lastStepEvents[Events.EyeGaze] = true;
                }
            }
        }


    }

    public bool detectHandshake(int step){        
        return ((step==stepAt) && lastStepEvents[Events.HandTouch]);
    }

    public bool detectEyeGaze(int step){        
        return ((step==stepAt) && lastStepEvents[Events.EyeGaze]);
    }

    public void startDetector(int step)
    {
        
        initEventsDict(step);
    }

    private void initEventsDict(int step)
    {
        lastStepEvents = new Dictionary<Events,bool>();
        lastStepEvents[Events.HandTouch] = false;
        lastStepEvents[Events.EyeGaze] = false;
        stepAt = step;
    }
}
