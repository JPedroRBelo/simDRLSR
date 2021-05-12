using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AvatarColliderManager))]
public class AgentInteraction : MonoBehaviour {

    //Define o animator
    protected Animator animator;
    
    //Define alguma partes do corpo do agente
    private Transform agentHead;
    private Transform agentRightShoulder;
    private Transform agentRightUpperArm;
    private Transform agentRightForeArm;
    private Transform agentRightHand;
    private Transform agentRightFoot;
    private Transform agentLeftShoulder;
    private Transform agentLeftHand;
    private Transform agentLeftFoot;
    private Transform agentHibs;
    private Transform agentSpine;
    private Transform agentTasteSensor;
    private Transform agentSmellSensor;

    private Quaternion quatHandTasteRotation = new Quaternion(0.1f, 0.1f, -0.6f, -0.8f);
    private Quaternion quatHandGrabRotation = new Quaternion(0.5f, 0.7f, -0.3f, -0.4f);

    private float handGrabYRotation = 180;
    private float handTasteYAngle = 120f;

    public bool ikFoot;
    public float maxCrouchDistance = 0.7f;
    private float minCrouchDistance;
    private Transform anklePosition;
    public Vector3 anklePositionValue = new Vector3(0, 0.1133955f, 0);

    private Hand[] agentHands;
    Rigidbody rigidibody;

    public float turningRate = 0.8f;

    private float distHandShoulder;
    private float distSpineShoulder;
    private float distSpineObject;
    private float distAllArm;

    private float crouchValue;
    private bool itsToChrouch = true;
    //private bool isSitting = false;
    

    public Vector3 spineRotation = new Vector3(0, 0, 0);
    private Vector3 auxSRotation;

    private float maxSpinePosY = 0.99f;
    private float minSpinePosY = 0.35f;

    private float velocityFocus;
    private Command auxCommand;

    private float timer = 0.0f;
    private float timerHandtouchRH = 0.0f;
    public float timeToTaste = 3f;
    private enum SPINE_DIRECTION { Forward, Left, Right, Backward };
    private SPINE_DIRECTION spineDirection;
    private bool isInteracting = false;
    private bool isSpineReset = true;

    public bool debug = false;

    void Awake()
    {
        auxSRotation = new Vector3(0, 0, 0);
        animator = GetComponent<Animator>();
        rigidibody = GetComponent<Rigidbody>();
        agentRightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        agentRightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        agentRightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        agentRightForeArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        agentRightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        agentLeftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        agentLeftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        agentLeftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        agentHead = animator.GetBoneTransform(HumanBodyBones.Head);
        agentTasteSensor = agentHead.Find(Constants.NAME_TASTESENSOR);
        agentSmellSensor = agentHead.Find(Constants.NAME_SMELLSENSOR);
        agentHands = new Hand[2];
        agentHands[(int)Hands.Left] = new Hand(Hands.Left, agentLeftHand, agentLeftShoulder);
        agentHands[(int)Hands.Right] = new Hand(Hands.Right, agentRightHand, agentRightShoulder);

        agentSpine = animator.GetBoneTransform(HumanBodyBones.Spine);
        agentHibs = animator.GetBoneTransform(HumanBodyBones.Hips);
        distAllArm = Vector3.Distance(agentRightHand.position, agentRightForeArm.position);
        distAllArm += Vector3.Distance(agentRightForeArm.position, agentRightUpperArm.position);
        distAllArm += Vector3.Distance(agentRightUpperArm.position, agentRightShoulder.position);

        anklePosition = transform.Find(Constants.POS_ANKLE);
        minCrouchDistance = anklePosition.position.y;
        crouchValue = anklePosition.localPosition.y;
        velocityFocus = Constants.NATURALSPD;
        auxCommand = null;        
    }

    void Start()
    {
        // sendCommand(new Command(Hands.Right,Action.Take,auxTransform)); 
        Vector3 perp = Vector3.Cross(transform.forward, agentSpine.forward);
        float dir = Vector3.Dot(perp, transform.up);
        if (dir < -0.5)
        {
            spineDirection = SPINE_DIRECTION.Left;
        }else if(dir>0.5){
            spineDirection = SPINE_DIRECTION.Right;
        }
        else
        {
            spineDirection = SPINE_DIRECTION.Forward;
        }
        //Debug.Log("RHS>>> "+this.name+" is ready to receive Interaction Commands.");
        
    }

    void OnAnimatorIK()
    {
        if (animator)
        {            
            for (int i = 0; i < agentHands.Length; i++)
            {
                if (agentHands[i].focus != null)
                {
                    AvatarIKGoal avatarIKGoal;
                    if(agentHands[i].getActHand() == Hands.Left)
                    {
                        avatarIKGoal = AvatarIKGoal.LeftHand;
                    }else
                    {
                        avatarIKGoal = AvatarIKGoal.RightHand;
                    }
                    //isReachable(agentHands[i]) && 
                    if (agentHands[i].ik)
                    {
                        animator.SetFloat(agentHands[i].getAnimatorParam(), 1, 0.1f, Time.deltaTime * 0.02f);
                        animator.SetIKPositionWeight(avatarIKGoal, animator.GetFloat(agentHands[i].getAnimatorParam()));
                        animator.SetIKRotationWeight(avatarIKGoal, 1);
                        animator.SetIKPosition(avatarIKGoal, agentHands[i].focus.position);
                        animator.SetIKRotation(avatarIKGoal, agentHands[i].focus.rotation);
                    }
                    else
                    {
                        animator.SetFloat(agentHands[i].getAnimatorParam(), 0, 0.1f, Time.deltaTime * 0.3f);
                        animator.SetIKPositionWeight(avatarIKGoal, animator.GetFloat(agentHands[i].getAnimatorParam()));
                        animator.SetIKRotationWeight(avatarIKGoal, 0);
                        animator.SetIKPosition(avatarIKGoal, agentHands[i].focus.position);
                        animator.SetIKRotation(avatarIKGoal, agentHands[i].focus.rotation);

                    }
                }
            }
            if(anklePosition != null)
            {
                if (ikFoot)
                {
                    // Set the right hand target position and rotation, if one has been assigned

                    Vector3 auxRPosition = new Vector3(agentRightFoot.position.x, anklePosition.position.y, agentRightFoot.position.z);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, auxRPosition);

                    //animator.SetIKRotation(AvatarIKGoal.RightFoot, objFoot.rotation);

                    Vector3 auxLPosition = new Vector3(agentLeftFoot.position.x, anklePosition.position.y, agentLeftFoot.position.z);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, auxLPosition);
                    //animator.SetIKRotation(AvatarIKGoal.RightFoot, objFoot.rotation);
                }
            }
        }
    }

    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }

    void LateUpdate()
    {
        if (isInteracting)
        {
            isSpineReset = false;
        }
          
           // print(Vector3.SqrMagnitude(agentSpine.localEulerAngles - spineRotation));

        if (isSpineReset == false)
        {
            agentSpine.localEulerAngles = new Vector3(spineRotation.x, agentSpine.localEulerAngles.y, spineRotation.z);
        }

        //agentSpine.localEulerAngles = spineRotation;
        /*
        if (animator.GetBool("Sitting"))
        {
            crouchValue = agentRightFoot.position.y;
            isSitting = true;
            anklePosition.localPosition = Vector3.Lerp(anklePosition.localPosition, new Vector3(anklePosition.localPosition.x, crouchValue, anklePosition.localPosition.z), 10 * Time.deltaTime);
        }
        else
        {
            if (isSitting)
            {
                resetCrouchValue();
                isSitting = false;
            }
            anklePosition.localPosition = Vector3.Lerp(anklePosition.localPosition, new Vector3(anklePosition.localPosition.x, crouchValue, anklePosition.localPosition.z), turningRate * Time.deltaTime);

        }*/
        anklePosition.localPosition = Vector3.Lerp(anklePosition.localPosition, new Vector3(anklePosition.localPosition.x, crouchValue, anklePosition.localPosition.z), turningRate * Time.deltaTime);

    }

    // Update is called once per frame
    void Update()
    {        
        updateSpineRotation();
        for (int i = 0; i < agentHands.Length; i++)
        { 
            if (agentHands[i].getCommand() != null)
            {
                switch (agentHands[i].getCommand().getAction())
                {
                    case Action.Take:
                        updateFocusPosition(agentHands[i]);
                        switch (agentHands[i].getCommand().getActionStateID())
                        {
                            case (int)Take.Start:
                                if(debug) Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Started!");
                                isInteracting = true;
                                agentHands[i].getCommand().next();
                                break;
                            case (int)Take.Position:
                                velocityFocus = Constants.NATURALSPD*1;
                                agentHands[i].ik = true;
                                configureCrouchAndSpineInclination(agentHands[i]);
                                //O foco está na posição
                                if (isInPosition(agentHands[i].focus.position, agentHands[i].getCommand().getNearestDesiredPosition(transform.position)))
                                {
                                    //A mão está posicionada no foco? O IK já terminou de executar?
                                    if (isInPosition(agentHands[i].hand.position, agentHands[i].focus.position) && animator.GetFloat(agentHands[i].getAnimatorParam()) > 0.4)
                                    {
                                        agentHands[i].getCommand().next();
                                    }
                                    else
                                    if (!isReachable(agentHands[i]))
                                    {
                                        Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Failed! Object not reachable.");
                        
                                        agentHands[i].getCommand().fail();
                                    }
                                }                          
                            break;
                            case (int)Take.Grab:
                                bool success = agentHands[i].hold(agentHands[i].getCommand().getReference());
                                if (success)
                                    agentHands[i].getCommand().next();
                                else
                                    agentHands[i].getCommand().fail();
                                if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Failed! Could not grab the object!");
                                break;
                            case (int)Take.Approximate:
                                if (agentHands[i].getCommand().isToResetHand())
                                {
                                    resetCrouchValue();
                                    resetSpineRotation();
                                }
                                velocityFocus = Constants.NATURALSPD;
                                agentHands[i].getCommand().setLocation(agentHands[i].getRestPosition());
                                //O foco está na posição?
                                if (isInPosition(agentHands[i].focus.position, agentHands[i].getRestPosition().position))
                                {
                                    agentHands[i].getCommand().success();
                                    if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Success!");
                                }
                                break;
                            case (int)Take.End:
                                //Continua atualizando o IK
                                
                                //Verifica houve falha                                
                                    if (agentHands[i].isHandFree())
                                    {
                                        agentHands[i].ik = false;
                                    }                                
                                break;
                            default:
                                break;
                        }
                        break;
                    case Action.Activate:
                    case Action.Deactivate:
                        velocityFocus = Constants.NATURALSPD * 2;
                        updateFocusPosition(agentHands[i]);
                        
                        switch (agentHands[i].getCommand().getActionStateID())
                        {
                            case (int)Activate.Start:
                                
                                if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Started!");
                                Status objectStatus = agentHands[i].getCommand().getReference().GetComponent<Status>();
                                if (objectStatus != null)
                                {
                                    if((agentHands[i].getCommand().getAction() == Action.Activate) == (objectStatus.getIsActivated()))
                                    {
                                        agentHands[i].getCommand().success();
                                        if(debug)Debug.Log("Command>>> " + this.name + " command " + agentHands[i].getCommand().getId() + " Success!");
                                    }
                                }
                                agentHands[i].getCommand().next();
                                break;
                            case (int)Activate.Position:
                                velocityFocus = Constants.NATURALSPD * 10;
                                timerHandtouchRH += Time.deltaTime;
                                float seconds = timerHandtouchRH % 60;
                                if (seconds < 3f)
                                {
                                    manageHandPosition(agentHands[i]);
                                }else{
                                    agentHands[i].ik = false;
                                    agentHands[i].getCommand().next();
                                }
                                break;
                            case (int)Activate.Trigger:
                                bool success = agentHands[i].setActivate(agentHands[i].getCommand().getReference(), agentHands[i].getCommand().getAction()==Action.Activate);
                                if (success)
                                {
                                    agentHands[i].getCommand().success();
                                    if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Success!");
                                }
                                else
                                {
                                    if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Failed! ERROR!");
                                    agentHands[i].getCommand().fail();
                                }
                                break;
                            case (int)Activate.End:
                                timerHandtouchRH = 0;
                                velocityFocus = Constants.NATURALSPD * 3;
                                agentHands[i].ik = false;
                                manageEndOfActivate(agentHands[i]);
                                break;
                            default:
                                if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Failed! Object not reachable.");
                                agentHands[i].getCommand().fail();
                                break;

                        }
                        break;                    

                    case Action.Release:
                        velocityFocus = Constants.NATURALSPD * 3;
                        switch (agentHands[i].getCommand().getActionStateID())
                        {
                            case (int)Release.Start:
                                if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Started!");
                                isInteracting = true;
                                agentHands[i].getCommand().next();
                                break;
                            case (int)Release.Position:
                                if (isLocationReachable(agentHands[i]))
                                {
                                    updateFocusToOtherPosition(agentHands[i], agentHands[i].getCommand().getNearestDesiredContainer(transform.position));
                                    configureCrouchAndSpineInclination(agentHands[i]);
                                }
                                else
                                {
                                    updateFocusToOtherPosition(agentHands[i], agentHands[i].getRestPosition().position);
                                    if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Failed! Location is not reachable.");
                                    agentHands[i].getCommand().fail();

                                }
                                //O Foco está na posição?
                                if (isInPosition(agentHands[i].focus.position, agentHands[i].getCommand().getNearestDesiredContainer(transform.position)) && isInPosition(agentHands[i].focus.position, agentHands[i].hand.position))
                                {
                                    agentHands[i].getCommand().next();
                                }
                                break;
                            case (int)Release.Leave:
                                velocityFocus = Constants.NATURALSPD;
                                bool success = agentHands[i].drop(agentHands[i].getCommand().getNearestDesiredContainer(transform.position));
                                if (success)
                                {
                                    if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Success!");
                                    agentHands[i].getCommand().success();
                                }
                                else
                                {
                                    if(debug)Debug.Log("Command>>> "+this.name+" command " + agentHands[i].getCommand().getId() + " Failed! ERROR!");
                                    agentHands[i].getCommand().fail();
                                }
                                break;
                            case (int)Release.End:
                                if (agentHands[i].getCommand().isToResetHand())
                                {
                                    resetCrouchValue();
                                    resetSpineRotation();
                                    
                                }
                                if (agentHands[i].getCommand().isFail())
                                {
                                    updateFocusToOtherPosition(agentHands[i], agentHands[i].getRestPosition().position);
                                    
                                }
                                else
                                {
                                    agentHands[i].ik = false;
                                }
                                if (agentHands[i].isHandFree())
                                {
                                    agentHands[i].ik = false;
                                }
                                break;
                        }
                        break;
                    case Action.Taste:
                        velocityFocus = Constants.NATURALSPD * 3;
                        switch (agentHands[i].getCommand().getActionStateID())
                        {
                            case (int)Taste.Start:
                                if(debug)Debug.Log("Command>>> " + this.name + " command " + agentHands[i].getCommand().getId() + " Started!");
                                isInteracting = true;
                                auxCommand = GetComponent<AgentHeadMovement>().getCurrentCommand();
                                GetComponent<AgentHeadMovement>().sendCommand(new Command("", Action.HeadReset));
                                agentHands[i].getCommand().next();
                                break;
                            case (int)Taste.Position:
                                if (!agentHands[i].isHandFree())
                                {
                                    updateFocusToOtherPosition(agentHands[i], agentHands[i].getCommand().getNearestDesiredPosition(agentHands[i].hand.position));
                                    int auxFlag = 1;
                                    if (agentHands[i].getActHand() == Hands.Right)
                                        auxFlag = -1;
                                    rotateFocus(agentHands[i], (handTasteYAngle * auxFlag));
                                    //O Foco está na posição?
                                    CaptureTaste captureTaste = agentTasteSensor.GetComponent<CaptureTaste>();
                                    captureTaste.capture();
                                    if (captureTaste.isObjectInTasteSensor() && captureTaste.getActTaste() == agentHands[i].objInHand.gameObject)
                                    {
                                        agentHands[i].getCommand().next();
                                    }
                                }
                                else
                                {
                                    if(debug)Debug.Log("Command>>> " + this.name + " command " + agentHands[i].getCommand().getId() + " Failed! Hand is empty. ");
                                    agentHands[i].getCommand().fail();
                                }
                                break;

                            case (int)Taste.Taste:

                                timer += Time.deltaTime;
                                float seconds = timer % 60;
                                velocityFocus = Constants.NATURALSPD;
                                if (seconds > timeToTaste)
                                {
                                    CaptureTaste captureTaste = agentTasteSensor.GetComponent<CaptureTaste>();
                                    captureTaste.stopCature();
                                    agentHands[i].getCommand().next();
                                }
                                break;
                            case (int)Taste.ReturnHeadTask:
                                if (auxCommand != null)
                                {
                                    auxCommand.resetState();
                                    GetComponent<AgentHeadMovement>().sendCommand(auxCommand);
                                }
                                if(debug)Debug.Log("Command>>> " + this.name + " command " + agentHands[i].getCommand().getId() + " Success!");
                                agentHands[i].getCommand().success();
                                break;
                            case (int)Taste.End:
                                if (agentHands[i].getCommand().isToResetHand())
                                {
                                    resetCrouchValue();
                                    resetSpineRotation();
                                }
                                resetRotationFocus(agentHands[i]);
                                timer = 0f;
                                updateFocusToOtherPosition(agentHands[i], agentHands[i].getRestPosition().position);
                                if (agentHands[i].isHandFree())
                                {
                                    agentHands[i].ik = false;
                                }
                                break;
                        }
                        break;
                    case Action.Smell:
                        velocityFocus = Constants.NATURALSPD * 3;
                        switch (agentHands[i].getCommand().getActionStateID())
                        {
                            case (int)Smell.Start:
                                if(debug)Debug.Log("Command>>> " + this.name + " command " + agentHands[i].getCommand().getId() + " Started!");
                                isInteracting = true;
                                auxCommand = GetComponent<AgentHeadMovement>().getCurrentCommand();
                                GetComponent<AgentHeadMovement>().sendCommand(new Command("", Action.HeadReset));
                                agentHands[i].getCommand().next();
                                break;
                            case (int)Smell.Position:
                                if (!agentHands[i].isHandFree())
                                {
                                    updateFocusToOtherPosition(agentHands[i], agentHands[i].getCommand().getNearestDesiredPosition(agentHands[i].hand.position));
                                    int auxFlag = 1;
                                    if (agentHands[i].getActHand() == Hands.Right)
                                        auxFlag = -1;
                                    rotateFocus(agentHands[i], (handTasteYAngle * auxFlag));
                                    //O Foco está na posição?
                                    CaptureSmell captureSmell = agentSmellSensor.GetComponent<CaptureSmell>();
                                    if (agentHands[i].objInHand.gameObject.GetComponentInChildren<ParticleSystem>() == null)
                                    {
                                        captureSmell.putObjInSmell(agentHands[i].objInHand.gameObject);
                                    }
                                    if (captureSmell.isObjectInSmellInSensor() && captureSmell.getActSmell() == agentHands[i].objInHand.gameObject)
                                    {
                                         agentHands[i].getCommand().next();
                                    }                                                                                            
                                }
                                else
                                {
                                    if(debug)Debug.Log("Command>>> " + this.name + " command " + agentHands[i].getCommand().getId() + " Failed! Hand is empty. ");
                                    agentHands[i].getCommand().fail();
                                }
                                break;

                            case (int)Smell.Smell:

                                timer += Time.deltaTime;
                                float seconds = timer % 60;
                                velocityFocus = Constants.NATURALSPD;
                                if (seconds > timeToTaste)
                                {
                                    agentHands[i].getCommand().next();
                                }
                                break;
                            case (int)Smell.ReturnHeadTask:
                                if (auxCommand != null)
                                {
                                    auxCommand.resetState();
                                    GetComponent<AgentHeadMovement>().sendCommand(auxCommand);
                                }
                                if(debug)Debug.Log("Command>>> " + this.name + " command " + agentHands[i].getCommand().getId() + " Success!");
                                agentHands[i].getCommand().success();
                                break;
                            case (int)Smell.End:
                                if (agentHands[i].getCommand().isToResetHand())
                                {
                                    resetCrouchValue();
                                    resetSpineRotation();
                                }
                                resetRotationFocus(agentHands[i]);
                                timer = 0f;
                                updateFocusToOtherPosition(agentHands[i], agentHands[i].getRestPosition().position);
                                if (agentHands[i].isHandFree())
                                {
                                    agentHands[i].ik = false;
                                }
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    
    private void manageHandPosition(Hand hand)
    {
        if (hand.isHandFree())
        {
            if (isReachable(hand))
            {
                hand.ik = true;
                configureCrouchAndSpineInclination(hand);
                //O foco está na posição
                if (isInPosition(hand.focus.position, hand.focusDesiredPosition) && animator.GetFloat(hand.getAnimatorParam()) > 0.5)
                {
                    //A mão está posicionada no foco? O IK já terminou de executar?
                    hand.getCommand().next();
                }
            }
            else if (isInPosition(hand.focus.position, hand.getCommand().getNearestDesiredPosition(transform.position)))
            {
                hand.getCommand().fail();
                if(debug)Debug.Log("Command>>> "+this.name+" command " + hand.getCommand().getId() + " Failed! Location is not reachable.");
            }
        }
        else
        {
            if(debug)Debug.Log("Command>>> "+this.name+" command " + hand.getCommand().getId() + " Failed! Hand is busy.");
            hand.getCommand().fail();
        }
    }

    private void manageEndOfActivate(Hand hand)
    {
        /*if (hand.getCommand().isToResetHand())
        {
            resetCrouchValue();
            resetSpineRotation();
        }*/
        if (hand.isHandFree())
        {
            
            hand.ik = false;
            //updateFocusToOtherPosition(hand, hand.getRestPosition().position);
        }
    }

    private void configureCrouchAndSpineInclination(Hand agentHand)
    {
        itsToChrouch = true;
        distHandShoulder = Vector3.Distance(agentRightHand.position, agentRightShoulder.position);

        distSpineShoulder = Vector3.Distance(agentRightShoulder.position, agentSpine.position);
        distSpineObject = Vector3.Distance(agentSpine.position, agentHand.focusDesiredPosition);
        float distHibsAnkle = Vector3.Distance(agentHibs.position, anklePosition.position);
        float distAnkleObject = Vector3.Distance(anklePosition.position, agentHand.focusDesiredPosition);

        Vector3 vectorHandShoulder = (agentHand.focusDesiredPosition - agentRightShoulder.position).normalized;
        Debug.DrawRay(agentRightShoulder.position, vectorHandShoulder * distHandShoulder, Color.cyan);

        Vector3 vectorShoulderSpine = (agentRightShoulder.position - agentSpine.position).normalized;
        Debug.DrawRay(agentSpine.position, vectorShoulderSpine * distSpineShoulder, Color.magenta);

        Vector3 vectorObjectSpine = (agentHand.focusDesiredPosition  - agentSpine.position).normalized;
        Debug.DrawRay(agentSpine.position, vectorObjectSpine * distSpineObject, Color.green);

        if (isReachable(agentHand))
        {
            if (itsToChrouch)
            {
                Vector3 vectorAnkleHibs = (agentHibs.position - anklePosition.position).normalized;
                Debug.DrawRay(anklePosition.position, vectorAnkleHibs * distHibsAnkle, Color.red);

                Vector3 vectorObjectAnkle = (agentHand.focusDesiredPosition - anklePosition.position).normalized;
                Debug.DrawRay(anklePosition.position, vectorObjectAnkle * distAnkleObject, Color.grey);

                Vector3 vectorHandHibs = (agentHand.focusDesiredPosition - agentHibs.position).normalized;
               Debug.DrawRay(agentHibs.position, vectorHandHibs * distAllArm, Color.blue);

                float angle = Vector3.Angle(vectorObjectAnkle, vectorAnkleHibs);
                float auxCrouchValue = calcCrouchValue(sideSSATriangle(distAnkleObject, distAllArm, angle));
                crouchValue = !System.Single.IsNaN(auxCrouchValue) ? auxCrouchValue : crouchValue;
            }
            
            float angle1 = angleOfTriangle(distSpineShoulder, distSpineObject, distHandShoulder);
            float angle2 = Vector3.Angle(transform.up, vectorHandShoulder);

            float angleZ = Mathf.LerpAngle(spineRotation.z, angle1, turningRate * Time.deltaTime);
            Quaternion rotation = Quaternion.LookRotation(vectorObjectSpine);
            

            if (!System.Single.IsNaN(angleZ))
            {
                switch (spineDirection)
                {
                    case SPINE_DIRECTION.Left:
                        setSpineRotation(new Vector3(rotation.eulerAngles.x, spineRotation.y, angle1 - angle2));
                        break;
                    case SPINE_DIRECTION.Right:
                        setSpineRotation(new Vector3(rotation.eulerAngles.x, spineRotation.y, angle2 - angle1));
                        break;
                    default:
                        setSpineRotation(new Vector3(angle2 - angle1, spineRotation.y, rotation.eulerAngles.z));
                        break;
                }
                   
            }
        }
        else
        {
            resetCrouchValue();
            resetSpineRotation();
        }
    }

    private void setSpineRotation(Vector3 vector3)
    {
        auxSRotation = vector3;
    }

    private void resetSpineRotation()
    {
        auxSRotation = Vector3.zero;
        isInteracting = false;
    }

    public void resetCrouchValue()
    {
        crouchValue = anklePositionValue.y;

    }

    private void updateSpineRotation()
    {
        if (isSpineReset==false)
        {
            float angleZ = Mathf.LerpAngle(spineRotation.z, auxSRotation.z, turningRate * Time.deltaTime);
            float angleX = Mathf.LerpAngle(spineRotation.x, auxSRotation.x, turningRate * Time.deltaTime);
            if((angleZ< 10) && (angleX < 10))
            {
                isSpineReset = true;
            }
            
            if (!System.Single.IsNaN(angleZ))
            {
                spineRotation = new Vector3(angleX, spineRotation.y, angleZ);
            }
        }
    }

    private void updateFocusPosition(Hand hand)
    {
        updateFocusToOtherPosition(hand, hand.getCommand().getNearestDesiredPosition(transform.position));
    }

    private bool updateFocusToOtherPosition(Hand hand, Vector3 position)
    {
            hand.focusDesiredPosition = position;
            checkSpeedFocus(hand);
            hand.focus.parent.position = Vector3.Lerp(hand.focus.parent.position,position, hand.spdFocus);
            if (hand.getCommand().getReference() != null)
            {
                if (hand.getCommand().isRefObject())
                {
                    Vector3 targetPostition = new Vector3(transform.position.x, hand.focus.parent.position.y, transform.position.z);
                    hand.focus.parent.LookAt(targetPostition);
                }else 
                {
                    hand.focus.parent.rotation = Quaternion.LookRotation(-transform.forward);
                }                
            }
            else
            {
                return false;
            }
        
        return true;
    }

    private void rotateFocus(Hand hand, float angle)
    {
        Quaternion startRotation = hand.focus.localRotation;
        Quaternion endRotation = Quaternion.Euler(new Vector3(hand.focus.localEulerAngles.x, angle - handGrabYRotation, hand.focus.localEulerAngles.z));
        hand.focus.localRotation= Quaternion.Lerp(startRotation, endRotation, Time.deltaTime * turningRate);
    }

    private void resetRotationFocus(Hand hand)
    {
        rotateFocus(hand, 0);
    }

    private float normalizeAngle(float angle)
    {
        float newAngle = angle;
        while (newAngle <= -180) newAngle += 360;
        while (newAngle > 180) newAngle -= 360;
        return newAngle;
    }

    private bool isInPosition(Vector3 position1, Vector3 position2)
    {
        if ((position1 - position2).magnitude <= 0.5f)
        {
            return true;
        }
        return false;
    }

    private bool isReachable(Hand hand)
    {
        Vector3 auxSpinePossiblePosition = new Vector3(agentSpine.position.x, anklePosition.position.y, agentSpine.position.z);
        float diffMinMaxSpinePosition = maxSpinePosY - minSpinePosY;
        for (int i = 0; i <=10; i++)
        {
            Vector3 directionToTarget;
            float angleHand;
            float distance;
            float distSpineAndArm = distAllArm;
            float auxIndex = (diffMinMaxSpinePosition / 10 * i) + minSpinePosY;
            auxSpinePossiblePosition = new Vector3(agentSpine.position.x, anklePosition.position.y + auxIndex, agentSpine.position.z);
            directionToTarget = hand.focus.position - auxSpinePossiblePosition;
            if (hand.getActHand() == Hands.Left)
            {
                distSpineAndArm += Vector3.Distance(agentSpine.position, agentLeftShoulder.position);
                angleHand = Vector3.Angle(transform.forward - transform.right, directionToTarget);
            }
            else
            {
                distSpineAndArm += Vector3.Distance(agentSpine.position, agentRightShoulder.position);
                angleHand = Vector3.Angle(transform.forward + transform.right, directionToTarget);
            }
            distance = directionToTarget.magnitude;
            if (Mathf.Abs(angleHand) < 125 && (distance <= (distSpineAndArm * 0.9f)))
            {
                return true;
            }
        }
        return false;
    }

    private bool isLocationReachable(Hand hand)
    {
        Vector3 auxSpinePossiblePosition = new Vector3(agentSpine.position.x, anklePosition.position.y, agentSpine.position.z);
        float diffMinMaxSpinePosition = maxSpinePosY - minSpinePosY;
        for (int i = 0; i < 10; i++)
        {
            Vector3 directionToTarget;
            float angleHand;
            float distance;
            float distSpineAndArm = distAllArm;
            float auxIndex = (diffMinMaxSpinePosition / 10 * i) + minSpinePosY;
            auxSpinePossiblePosition = new Vector3(agentSpine.position.x, anklePosition.position.y + auxIndex, agentSpine.position.z);
            directionToTarget = hand.getCommand().getNearestDesiredPosition(transform.position) - auxSpinePossiblePosition;
            if (hand.getActHand() == Hands.Left)
            {
                distSpineAndArm += Vector3.Distance(agentSpine.position, agentLeftShoulder.position);
                angleHand = Vector3.Angle(transform.forward - transform.right, directionToTarget);
            }
            else
            {
                distSpineAndArm += Vector3.Distance(agentSpine.position, agentRightShoulder.position);
                angleHand = Vector3.Angle(transform.forward + transform.right, directionToTarget);
            }
            distance = directionToTarget.magnitude;
            if (Mathf.Abs(angleHand) < 125 && (distance <= distSpineAndArm))
            {
                return true;
            }
        }
        return false;
    }

    private bool isVisible(Transform focusObj)
    {
        Vector3 directionToTarget;
        float angleForward;
        float distance;

        directionToTarget = focusObj.position - transform.position;
        angleForward = Vector3.Angle(transform.forward, directionToTarget);
        distance = directionToTarget.magnitude;
        if (Mathf.Abs(angleForward) < 90 && distance < 10)
        {
            return true;
        }     
        return false;
    }

    //Verifica a velocidade adequada do Focus de acordo com a posição da mão (IK)
    private void checkSpeedFocus(Hand hand)
    {
        float auxParameter = animator.GetFloat(hand.getAnimatorParam());
        if (auxParameter < 0.2)
        {
            hand.spdFocus = Constants.MAXSPD;
        }
        else
        {
            hand.spdFocus = velocityFocus;                       
        }
    }

    public bool sendCommand(Command command)
    {
        agentHands[(int)command.getActHand()].setCommand(command);
        if(debug)Debug.Log("Command>>> "+this.name+ " received command " + command.getStringCommand());
        return true;
    } 

    private float angleOfTriangle(float a, float b, float c)
    {
        double angleC = (a * a + b * b - c * c) / (2 * a * b);
        angleC = Math.Acos(angleC);
        return (float)radianToDegree(angleC);
    }

    private float sideSSATriangle(double sideA, double sideB, double angleB)
    {
        angleB = degreeToRadian(angleB);
        //Primeiro, encontrar angulo de A
        double angleA = (sideA * Math.Sin(angleB)) / sideB;
        angleA = Math.Asin(angleA);

        double angleC = 180 - radianToDegree(angleA) - radianToDegree(angleB);
       
        angleC = degreeToRadian(angleC);
        double sideC = (Math.Sin(angleC) * sideB) / Math.Sin(angleB);
        
        return (float)sideC;
    }

    private double degreeToRadian(double angle)
    {
        return (float)(Math.PI * angle / 180.0);
    }

    private double radianToDegree(double angle)
    {
        return angle * (180.0 / Math.PI);
    }

    private float calcCrouchValue(float auxCrouch)
    {
        auxCrouch = 1 - auxCrouch;
        auxCrouch = Math.Max(auxCrouch, minCrouchDistance);
        return auxCrouch;
    }

    public Hand[] getHands()
    {
        return agentHands;
    }
}