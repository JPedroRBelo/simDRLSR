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

    public float faceMaxDistance = 12f;

    private Animator animator;
    private Dictionary<Events,bool> lastStepEvents;
    private RobotInteraction robotHRI;
    private Transform robotHead;

    // Start is called before the first frame update
    void Start()
    {
        stepAt = -1;
        animator = GetComponent<Animator>();
        robotHRI = GetComponent<RobotInteraction>();
        robotHead = robotHRI.getRobotHead();
        initEventsDict(-1);
    }

    // Update is called once per frame
    void Update()
    {
        if(robotHRI.getHandTouch()){
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
        detectFace();

    }

    public bool detectFace(){
        foreach (GameObject person in GameObject.FindGameObjectsWithTag("Person"))
        {
            Transform person_head = person.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
            Vector3 dirFromBtoA = (transform.position - person_head.position ).normalized;
            float robotInHumanVisionDot = Vector3.Dot(dirFromBtoA,  person_head.forward);
  
            if(robotInHumanVisionDot>=0.15){
                float dist = Vector3.Distance(person_head.position, transform.position);
                if(robotHRI.thereIsAFaceInRobotView(person)&&dist<faceMaxDistance){
                    //print("FACE");
                    return true;
                }
            }            
        }
        return false;
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
