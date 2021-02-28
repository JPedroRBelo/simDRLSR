using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Events
{   
    HandTouch
}

public class EventDetector : MonoBehaviour
{

    private int stepAt;

    public float neutralReward = 0;

    private Animator animator;
    private Dictionary<Events,bool> lastStepEvents;
    private RobotInteraction handTouchIHR;


    // Start is called before the first frame update
    void Start()
    {
        stepAt = -1;
        animator = GetComponent<Animator>();
        handTouchIHR = GetComponent<RobotInteraction>();
        initEventsDict(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if(handTouchIHR.getHandTouch()){
            lastStepEvents[Events.HandTouch] = true;
        }
    }

    public bool detectHandshake(int step){        
        return ((step==stepAt) && lastStepEvents[Events.HandTouch]);
    }

    public void startDetector(int step)
    {
        
        initEventsDict(step);
    }

    private void initEventsDict(int step)
    {
        lastStepEvents = new Dictionary<Events,bool>();
        lastStepEvents[Events.HandTouch] = false;
        stepAt = step;
    }
}
