using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using UnityEngine;


public class Vector3Wrapper
{
    public Vector3 vector;

    public Vector3Wrapper(Vector3 vector){
        this.vector = vector;
    }
}

[RequireComponent(typeof(Animator))]
public class RobotInteraction : MonoBehaviour
{
    
    [HideInInspector] 
    public float handshakeReward = 50f;
    [HideInInspector] 
    public float failHandshakeReward = -0.1f;
    [HideInInspector]
    public float  successEyeGazeReward = 50f;
    [HideInInspector]
    public float failEyeGazeReward = -0.1f;
    [HideInInspector] 
    public float neutralReward = 0f;
    [HideInInspector] 
    public float NULL_REWARD = -Mathf.Infinity;

    [Header("Configuration")]
    public int actionDuration = 3;

    public bool drawLines = false;

    private const string CONST_TRYHANDSHAKE = "Try Handshake";

    private Animator animator;
    private LookHeadController lookHead;
    private long timeStart;
    private long actionTimeStart;
    public int maxRandomLookTime = 15;
    public int minRandomLookTime = 5;
    public int waitVerticalLookAngle = 135;
    public int waitHorizontalLookAngle = 45;
    public int verticalLookAngle = 135;
    public int horizontalLookAngle = 45;
    public int verticalSearchAngle = 135;
    public int horizontalSearchAngle = 45;
    public int verticalSearchOffSet = 90;
    public int horizontalSearchOffSet = 60;
    public float lookAngleOffSet = 15;

    public float speed = 0.1f;
    public Transform robotSpine;
    public Transform robotTorso;
    public Transform robotNeck;
    public Transform rightHand;
    public Vector3 torsoOffsetRotation;
    private Transform robotHead;
    private Transform rgbCamera;
    private Camera camera;

    private GameObject neckOriginalTransform;
    private GameObject headOriginalTransform;

    private GameObject personFocused;

    
    public bool handTouchInspector = false;

    private bool handTouch;


    public AgentAction agentAction = AgentAction.None;
    private AgentAction rAction;

    private AgentAction queueAction;
    private int queueStep;

    //private AgentAction lastAction;
    private AgentAction actualAction;

    private Vector3 aux_offsetRotation;

    private int step;
    //Armazena a ultima vez que cada uma das ações foi 
   // private Dictionary<AgentAction, ulong> lastStepAction;


    private int randomTime;
    private Vector3 neckRotation;

    private Vector3 lastNeckRotation;
    
    private enum ActionStages
    {
        Start,
        SetAction,
        Running,        
        WaitReward,
        End
    }

    private ActionStages actionStage;
    private Vector3 neckOriginalRotation;
    private Dictionary<int,float> dictStepRewards;
    private Dictionary<int,AgentAction> dictStepActions;
    private bool actionFromRL;

    private EventDetector eventDetector;

    void Start()
    {
        lookHead = gameObject.GetComponent<LookHeadController>();
        eventDetector = GetComponent<EventDetector>();
        randomTime = minRandomLookTime;
        step = 0;
        handTouch = false;
        actionFromRL = false;
        dictStepRewards = new  Dictionary<int,float>();
        dictStepActions = new  Dictionary<int,AgentAction>();
        personFocused = null;
        timeStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        actionTimeStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        animator = GetComponent<Animator>();
        //lastAction = rAction;
        rAction = AgentAction.DoNothing;
        queueAction = AgentAction.None;
        queueStep = 0;
        
        /*
        lastStepAction = new Dictionary<AgentAction, ulong>();
        lastStepAction.Add(AgentAction.DoNothing, 0);
        lastStepAction.Add(AgentAction.Wait, 0);
        lastStepAction.Add(AgentAction.Look, 0);
        lastStepAction.Add(AgentAction.Wave, 0);
        lastStepAction.Add(AgentAction.HandShake, 0);
        */
        actionStage = ActionStages.Start;
        aux_offsetRotation = robotSpine.localEulerAngles;

        if (robotNeck != null)
        {
            neckOriginalRotation = robotNeck.localEulerAngles;
            robotHead = robotNeck.Find("head");
            rgbCamera = robotNeck.Find("head/RGB Camera");
            
            if (rgbCamera != null)
            {
                camera = rgbCamera.GetComponent<Camera>();
                neckOriginalTransform = new GameObject("neckOriginalTransform");
                neckOriginalTransform.transform.SetParent(robotNeck.parent);
                neckOriginalTransform.transform.forward = robotNeck.forward;
                neckOriginalTransform.transform.localScale = robotNeck.localScale;
                neckOriginalTransform.transform.localPosition = robotNeck.localPosition;
                neckOriginalTransform.transform.localRotation = robotNeck.localRotation;
                lastNeckRotation = neckOriginalTransform.transform.localEulerAngles;


                headOriginalTransform = new GameObject("headOriginalTransform");
                headOriginalTransform.transform.SetParent(neckOriginalTransform.transform);
                headOriginalTransform.transform.forward = robotHead.forward;
                headOriginalTransform.transform.localScale = robotHead.localScale;
                headOriginalTransform.transform.localPosition = robotHead.localPosition;
                headOriginalTransform.transform.localRotation = robotHead.localRotation;
            }
        }
    }

    //Atualiza qual a ultima ação executada, antes da atual
    /*
    public void updateLastAction()
    {
        step++;
        if (rAction != actualAction)
        {
            lastAction = actualAction;
            actualAction = rAction;
            lastStepAction[rAction] = step;
            //print("Step Action "+lastStepAction[rAction]);
        }
        
    }*/

    // Update is called once per frame
    void Update()
    {
        //updateLastAction();       
            switch (actionStage)
            {
                
                case ActionStages.Start:
                    actionTimeStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    handTouch = false;
                    if(queueAction==AgentAction.None){
                        rAction = agentAction;
                        agentAction = AgentAction.None;
                        actionFromRL = false;
                    }else{
                        rAction = queueAction;
                        step=queueStep;
                        queueStep = 0;
                        queueAction = AgentAction.None;
                        actionFromRL = true;
                    }

                    if(rAction!=AgentAction.None)
                    {
                        actionStage = ActionStages.SetAction;                    
                        eventDetector.startDetector(step);
                         if(!actionFromRL)
                        {
                            eventDetector.startDetector(-1);
                        }
                        dictStepRewards[step] = NULL_REWARD;
                    }                    
                   
                    break;
                case ActionStages.SetAction:
                    switch (rAction)
                    {
                        case AgentAction.Wait:
                            ActionWait();
                            break;
                        case AgentAction.Look:
                            ActionLook();
                            break;
                        case AgentAction.Wave:
                            ActionLook();
                            ActionWave();
                            break;
                        case AgentAction.HandShake:
                            ActionLook();
                            ActionHandshake();
                            break;
                        case AgentAction.DoNothing:
                            break;
                        default:
                            break;
                    }
                    actionStage = ActionStages.Running;
                    break;
                case ActionStages.Running:
                    int timeToWait = actionDuration;
                    switch (rAction)
                    {
                        case AgentAction.Wait:
                            //print("wating");
                            ActionWait();
                            break;
                        case AgentAction.Look:
                            //print("looking");
                            ActionLook();
                            break;
                        case AgentAction.Wave:
                            //print("waving");
                            ActionLook();
                            timeToWait = 3;
                            break;
                        case AgentAction.HandShake:
                            //print("hand shaking");
                            ActionLook();
                            timeToWait = 3;
                            break;
                        case AgentAction.DoNothing:
                            timeToWait = 0;
                            break;
                        default:
                            break;
                        
                    }
                    float timeSpeed = 1f;
                    GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
                    if(simManager != null){
                        timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
                    }                    
                    long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    if (timeNow - actionTimeStart >= timeToWait/timeSpeed * 1000)
                    {                    
                        if(detectEndOfAction()){
                            
                            actionStage = ActionStages.WaitReward;
                        }                                        
                    }                
                    break;
                case ActionStages.WaitReward:                
                    if(actionFromRL){
                        dictStepRewards[step] = calcReward(rAction,step);
                        print("Reward: "+dictStepRewards[step]);
                    }else{
                        if(rAction!=AgentAction.DoNothing)
                            print("Debug Reward: "+calcReward(rAction,-1));
                    }


                        
                    actionStage = ActionStages.End;              
                    
                    break;
                case ActionStages.End:
                    //rAction = AgentAction.DoNothing;
                    resetHandTouch();
                    if(actionFromRL){
                        print(rAction+" executed!");
                    }
                
                    
                    
                    actionStage = ActionStages.Start;
                    
                    

                    break;
                default:
                    break;
            }
        

        if (handTouchInspector)
        {
             touchRobotHand();
        }   
       
    }

    private float calcReward(AgentAction action,int step)
    {
        if(action == AgentAction.HandShake)
        {
            if(eventDetector.detectHandshake(step)){
                return handshakeReward;
            }else{
                return failHandshakeReward;
            }
                
        }else if(action == AgentAction.Wave)
        {
            if(eventDetector.detectEyeGaze(step)){
                return successEyeGazeReward;
            }else{
                return failEyeGazeReward;
            }
                
        }else
        {
            return neutralReward;
        }
    }

    public bool getHandTouch()
    {
        return handTouch;
    }

    public GameObject getPersonFocusedByRobot(){
        return personFocused;
    }

    public void setAction(AgentAction action,int step)
    {
        resetPersonFocused();
        queueAction = action;
        queueStep = step;
        dictStepRewards[step] = NULL_REWARD;
        dictStepActions[step] = action;
        actionFromRL = true;
    }

    private void resetHandTouch(){
        handTouch = false;
    }

    public float getReward(int step)
    {
        if(dictStepRewards.ContainsKey(step))
        {
            return dictStepRewards[step];
        }else
        {
            return NULL_REWARD;
        }
        
    }
        
    public (int step, AgentAction action) getActualAction(){
        //Verifica se o Handshake foi finalizado ou não
        int auxStep = step;
        AgentAction atAction = rAction;
        if((rAction==AgentAction.HandShake)&&(!animator.GetCurrentAnimatorStateInfo(0).IsName(CONST_TRYHANDSHAKE )))
        {
            atAction =  AgentAction.Look;
        }
        return (auxStep,atAction);        
    }


    public (int step, AgentAction action) getLastAction(){
        
        int lastStep = step-1;
        AgentAction lastAction =  AgentAction.DoNothing;
        if(dictStepActions.ContainsKey(lastStep))
        {
            lastAction = dictStepActions[lastStep];
        }
        
        return (lastStep,lastAction);        
    }

    public void touchRobotHand()
    {
        if(rAction==AgentAction.HandShake)
        {
            handTouch = true;
            animator.SetTrigger("TouchHand");            
        }        
    }

    private void ActionWait()
    {
    	float timeSpeed = 1f;
	GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
	if(simManager != null){
		timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
	}    
        
        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        //Vector3 randomPosition = transform.forward;
        if (timeNow - timeStart >= (randomTime * 1000)/timeSpeed)
        {
            neckRotation = randomAnglesToLook(waitHorizontalLookAngle, waitVerticalLookAngle);
            //randomTime = UnityEngine.Random.Range(minRandomLookTime, maxRandomLookTime);
            timeStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            
        }
        
        rotateNeck(neckRotation);
        //getNearestPersonFacePosition();
    }
    
    private void ActionLook()
    {
        Vector3Wrapper posNearestPerson = null;
        if (personFocused != null)
        {
            //GameObject person = getNearestPerson();
            posNearestPerson = getPersonPosition(personFocused);
            if (posNearestPerson != null)
            {
                rotateNeck(posNearestPerson.vector);
            }
            else
            {
                resetPersonFocused();
                rotateNeck(lastNeckRotation);
            }
        }
        else
        {
            GameObject person = getNearestPerson();
            posNearestPerson = getPersonPosition(personFocused);
            if (posNearestPerson != null)
            {
                personFocused = person;                        
            }
        }     
    }

    private void resetPersonFocused(){
        personFocused = null;
    }

    private void ActionWave()
    {

        animator.SetTrigger("Wave");
        /*if(lastStepAction[AgentAction.Wait] > lastStepAction[AgentAction.Look])
        {
            rAction = AgentAction.Wait;
        }
        else
        {
            rAction = AgentAction.Look;

        }*/
    }

    private void ActionHandshake()
    {

        animator.SetTrigger("Handshake");
        
        /*
        if (lastStepAction[AgentAction.Wait] > lastStepAction[AgentAction.Look])
        {
            rAction = AgentAction.Wait;
        }
        else
        {
            rAction = AgentAction.Look;
        }
        */
    }


    private bool detectEndOfAction()
    {
        return (!animator.GetCurrentAnimatorStateInfo(0).IsName("Try Handshake") &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Handshake") &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Wave")
        );        
    }


    private void rotateNeck(Vector3 rotation)
    {
        float maxVerticalLookAngle = verticalLookAngle/2;
        float maxHorizontalLookAngle = horizontalLookAngle/2;

        float auxHorizontal = rotation.y;
        //float auxVertical = rotation.z;
        float auxVertical = rotation.x;

        
       
        if(auxHorizontal < -maxHorizontalLookAngle)
        {
            auxHorizontal = -maxHorizontalLookAngle;
        }else if(auxHorizontal>maxHorizontalLookAngle)
        {
            auxHorizontal = maxHorizontalLookAngle;
        }

        if(auxVertical < -maxVerticalLookAngle)
        {
            auxVertical = -maxVerticalLookAngle;
        }else if(auxVertical>maxVerticalLookAngle)
        {
            auxVertical = maxVerticalLookAngle;
        }
        
        //rotation = new Vector3(rotation.x,auxHorizontal,auxVertical);
        
        rotation = new Vector3(auxVertical,auxHorizontal,neckOriginalRotation.z);
        //print("neck1: "+rotation);
        Vector3 nkRotation  = neckOriginalTransform.transform.localEulerAngles;
        nkRotation = new Vector3(nkRotation.x,nkRotation.y,nkRotation.z);

        //print("neck2: "+nkRotation);
        rotation = new Vector3(-nkRotation.x+rotation.x,nkRotation.y-rotation.y,rotation.z);
        //rotation = rotation + origNkRot;
        
        if (robotNeck != null)
        {
            Vector3 currentAngle = new Vector3(
                        Mathf.LerpAngle(WrapAngle(robotNeck.localEulerAngles.x), rotation.x, Time.deltaTime * speed),
                        Mathf.LerpAngle(WrapAngle(robotNeck.localEulerAngles.y), rotation.y, Time.deltaTime * speed),
                        Mathf.LerpAngle(WrapAngle(robotNeck.localEulerAngles.z), rotation.z, Time.deltaTime * speed));
            robotNeck.localEulerAngles = currentAngle;
        }
    }
    
    private Vector3 randomAnglesToLook(int hAngle, int vAngle)
    {
        int randH = UnityEngine.Random.Range((-hAngle / 2), (hAngle / 2));
        int randV = UnityEngine.Random.Range((-vAngle / 2), (vAngle / 2));
        return new Vector3(0, randH, randV);
    }


    private Vector3Wrapper getPersonPosition(GameObject person)
    {
        RaycastHit hit = new RaycastHit();
        return getPersonPosition(person, out hit);
    }


     private float WrapAngle(float angle)
        {
            angle%=360;
            if(angle >180)
                return angle - 360;
 
            return angle;
        }

    private Vector3 getAngles(Transform target1,Transform target2)
    {
        var targetDirection = target1.position - target2.position;
        targetDirection = target2.InverseTransformDirection(targetDirection);
        var angleOnX = Mathf.Atan2( targetDirection.y, targetDirection.z ) * Mathf.Rad2Deg;  
        var angleOnY = Mathf.Atan2( targetDirection.x, targetDirection.z ) * Mathf.Rad2Deg;        
        var angleOnZ = Mathf.Atan2( targetDirection.x, targetDirection.y ) * Mathf.Rad2Deg;  
        //print("Angleonx:"+ (-angleOnX-WrapAngle(neckOriginalTransform.transform.localEulerAngles.x)));
        Vector3 angles = new Vector3(angleOnX,angleOnY,angleOnZ);
        return angles;
    }

    private Vector3Wrapper getPersonPosition(GameObject person, out RaycastHit hit)
    {
        Vector3Wrapper angleNearestPerson = null;

        hit = new RaycastHit();
        if (person != null)
        {
            Transform person_head = person.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
            
            if (Physics.Raycast(rgbCamera.position, (person_head.position - rgbCamera.transform.position), out hit))
            {

                Vector3 robot_angle = getAngles(person_head,headOriginalTransform.transform);               

                Vector3 camera_angle = getAngles(person_head,robotHead.transform);
                Vector3 angles_robotHead = getAngles( person_head,rgbCamera);                

                if (hit.transform == person_head)
                {
                    if ((Math.Abs(robot_angle.y) <= (horizontalLookAngle / 2)) && (Math.Abs(robot_angle.x) <= (verticalLookAngle / 2)))
                    {
                        
                        float vFov = camera.fieldOfView;
                        float radAngle = camera.fieldOfView * Mathf.Deg2Rad;
                        float radHFOV = 2 * (float)(Math.Atan(Mathf.Tan(radAngle / 2) * camera.aspect));
                        float hFov = Mathf.Rad2Deg * radHFOV;    
                       
                        if ((Math.Abs(camera_angle.x) <= ((vFov+verticalSearchOffSet) / 2)) && (Math.Abs(camera_angle.y) <= ((hFov+horizontalSearchOffSet) / 2)))
                        {
                            
                            if (drawLines) Debug.DrawRay(rgbCamera.position, (hit.transform.position - rgbCamera.position), Color.green);  
                            Vector3 calcBodyOffSet = aux_offsetRotation - robotSpine.localEulerAngles;
                            float offsetValue = 1 +(Mathf.Abs(robot_angle.y)/90); 
                            angleNearestPerson = new Vector3Wrapper(new Vector3((robot_angle.x/offsetValue)-10f, robot_angle.y, 0));
                                                       
                        }
                        else
                        {
                           if (drawLines) Debug.DrawRay(rgbCamera.position, (hit.transform.position - rgbCamera.position), Color.blue);
                        }
                    }
                    else
                    {
                        if (drawLines) Debug.DrawRay(rgbCamera.position, (hit.transform.position - rgbCamera.position), Color.yellow);
                    }
                    if (drawLines) Debug.DrawRay(rgbCamera.position, rgbCamera.forward * 10f, Color.magenta);
                }
                else
                {

                    if (drawLines) Debug.DrawRay(rgbCamera.position, (hit.transform.position - rgbCamera.position), Color.red);
                }
            }
        }
        return angleNearestPerson;
    }

    private GameObject getNearestPerson()
    {
        float maxRange = 10f;
        RaycastHit hit;

        float minDist = 10000f;

        GameObject[] people = GameObject.FindGameObjectsWithTag("Person");
        GameObject auxFocusedPerson = null;
        foreach (GameObject p in people)
        {

            Vector3Wrapper angleToPerson = getPersonPosition(p,out hit);
            if (angleToPerson  != null)
            {
                if (hit.distance < minDist)
                {
                    minDist = hit.distance;
                    Vector3 angleNearestPerson = new Vector3(angleToPerson.vector.x, angleToPerson.vector.y, 0);
                    lastNeckRotation = angleNearestPerson;
                    auxFocusedPerson = p;
                }
            }
        }
        personFocused = auxFocusedPerson;
        return personFocused;
    }


    public Vector3 get3DAngles(Transform refPosition, Transform obj, string direction_name = "forward")
    {

        Vector3 direction = refPosition.forward;
        if (direction_name.Equals("back"))
        {
            direction = -refPosition.forward;
        }else if (direction_name.Equals("right"))
        {
            direction = refPosition.right;
        }
        else if (direction_name.Equals("left"))
        {
            direction = -refPosition.right;
        }else if (direction_name.Equals("up"))
        {
            direction = refPosition.up;
        }
        else if (direction_name.Equals("down"))
        {
            direction = -refPosition.up;
        }

        float angleXZ;
        float angleYZ;
        float angleXY;

        Vector2 objPosXZ = new Vector2(obj.position.x, obj.position.z);
        Vector2 camPosXZ = new Vector2(refPosition.position.x, refPosition.position.z);
        Vector2 camFwrXZ = new Vector2(direction.x, direction.z);

        Vector2 directTargetXZ = objPosXZ - camPosXZ;
        angleXZ = Vector2.SignedAngle(camFwrXZ, directTargetXZ);
        Vector3 crossXZ = Vector3.Cross(camFwrXZ, directTargetXZ);
        
        /*
        if (crossXZ.y < 0)
        {
            angleXZ = -angleXZ;
        }
        */

        Vector2 objPosYZ = new Vector2(obj.position.y, obj.position.z);
        Vector2 camPosYZ = new Vector2(refPosition.position.y, refPosition.position.z); 
        Vector2 camFwrYZ = new Vector2(direction.y, direction.z);

        Vector2 directTargetYZ = objPosYZ - camPosYZ;
        angleYZ = Vector2.SignedAngle(camFwrYZ, directTargetYZ);
        Vector3 crossYZ = Vector3.Cross(camFwrYZ, directTargetYZ);

        /*
        if (crossYZ.y < 0)
        {
            angleYZ = -angleYZ;
        }
        */

        Vector2 objPosXY = new Vector2(obj.position.x, obj.position.y);
        Vector2 camPosXY = new Vector2(refPosition.position.x, refPosition.position.y);
        Vector2 camFwrXY = new Vector2(direction.x, direction.y);

        Vector2 directTargetXY = objPosXY - camPosXY;
        angleXY = Vector2.SignedAngle(camFwrXY, directTargetXY);
        Vector3 crossXY = Vector3.Cross(camFwrXY, directTargetXY);
        
        /*
        if (crossXY.y < 0)
        {
            angleXY = -angleXY;
        }
        */
        Vector3 angles = new Vector3(angleYZ, angleXZ, angleXY);
        return angles;
    }       

    public Transform getRobotHand()
    {
        return rightHand;
    } 
    
    

    public Transform getRobotHead()
    {
        if (robotHead == null)
        {
            Debug.Log("É nullllllllllllllllllllllllllllllllllllllllllll");
        }
        return robotHead;
    }
}
