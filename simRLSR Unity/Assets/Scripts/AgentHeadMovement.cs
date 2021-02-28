using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentHeadMovement : MonoBehaviour {

    protected Animator animator;

    //Define alguma partes do corpo do agente
    private Transform agentHead;
    private Transform agentSpine;
    private Command command;
    Rigidbody rigidibody;
    private bool ik;
    private Transform focus;
    //Velocidade do foco
    public float spdFocus = 8f;
    public float headRotationValue = 45f;

    public bool printLog = false;

    private Quaternion originalFocusRotation;
    private List<Vector3> listOfRotations;
    

    private int idListOfRot;
    private VisionManager robotVision;


    private Transform lookTo;

    void Awake()
    { 
        ik = false;
        animator = GetComponent<Animator>();
        rigidibody = GetComponent<Rigidbody>();
        agentHead = animator.GetBoneTransform(HumanBodyBones.Head);
        agentSpine = animator.GetBoneTransform(HumanBodyBones.Spine);

        

        focus = agentSpine.Find("CenterFocus/HeadFocusPosition");
        originalFocusRotation = focus.parent.localRotation;
        listOfRotations = new List<Vector3>();
        listOfRotations.Add(new Vector3(0f, 0f, 0f));
        listOfRotations.Add(new Vector3(-headRotationValue, -headRotationValue, 0));
        listOfRotations.Add(new Vector3(0f, -headRotationValue, 0));        
        listOfRotations.Add(new Vector3(headRotationValue, -headRotationValue, 0));
        listOfRotations.Add(new Vector3(headRotationValue, 0f,0));
        listOfRotations.Add(new Vector3(headRotationValue, headRotationValue, 0));
        listOfRotations.Add(new Vector3(0f, headRotationValue, 0));
        listOfRotations.Add(new Vector3(-headRotationValue, headRotationValue, 0));
        listOfRotations.Add(new Vector3(-headRotationValue, 0f,0));

        command = null;
    }

    void Start () {
        idListOfRot = 0;
        robotVision = transform.GetComponent<VisionManager>();
        Log("RHS>>> " + this.name + " is ready to receive Head Movement Commands.");
    }

    private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }

    void OnAnimatorIK()
    {
        if (animator)
        {
            if (focus != null)
            {
                if (ik && focus != null)
                {
                    if (isVisible(focus))
                    {
                        animator.SetFloat("HeadReach", 1, 0.5f, Time.deltaTime * 0.6f);
                        animator.SetLookAtWeight(animator.GetFloat("HeadReach"));
                        animator.SetLookAtPosition(focus.position);
                    }
                }
                else
                {
                    animator.SetFloat("HeadReach", 0, 0.1f, Time.deltaTime * 0.3f);
                    animator.SetLookAtWeight(animator.GetFloat("HeadReach"));
                    animator.SetLookAtPosition(focus.position);

                }
            }           
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (command != null)
        {           
            switch (command.getAction())
            {
                case Action.HeadFocus:
                    updateFocus(command.getNearestDesiredPosition(transform.position));
                    switch (command.getActionStateID())
                    {
                        case (int)HeadFocus.Start:
                            Log("Command>>> " + this.name + " command " + command.getId() + " Started!");
                            command.next();
                            break;
                        case (int)HeadFocus.Position:
                            ik = true;
                            //if (animator.GetFloat("HeadReach") > 0.9)
                            //{
                                Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
                                command.success();
                            //}else
                            if (command.getReference() != null && isFocusInPosition(focus, command.getReference().position) && !isVisible(command.getReference()))
                            {
                                Log("Command>>> " + this.name + " command " + command.getId() + " Failed! Obj. is not visible.");
                                command.fail();
                            }
                            break;
                        case (int)HeadFocus.End:
                            if (command.isFail()) {
                                ik = false;
                            }
                            break;
                    }
                    break;
                case Action.HeadReset:
                    switch (command.getActionStateID())
                    {
                        case (int)HeadReset.Start:
                            Log("Command>>> " + this.name + " command " + command.getId() + " Started!");
                            command.next();
                            break;
                        case (int)HeadReset.Position:
                            moveFocus(originalFocusRotation);
                            if (isFocusMoved(focus.parent.localRotation, originalFocusRotation))
                            {
                                ik = false;                                
                            }
                            if ((animator.GetFloat("HeadReach") < 0.5))
                            {
                                Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
                                command.success();
                            }
                            break;
                        case (int)HeadReset.End:
                            break;
                        default:
                            break;
                    }
                    break;
                case Action.LookFor:
                    switch (command.getActionStateID())
                    {
                        case (int)LookFor.Start:
                            Log("Command>>> " + this.name + " command " + command.getId() + " Started!");
                            command.next();
                            break;
                        case (int)LookFor.Position:
                            ik = true;
                            Quaternion auxQuater = Quaternion.Euler(listOfRotations[0]);
                            moveFocus(originalFocusRotation);                            
                            if (isFocusMoved(focus.parent.localRotation, originalFocusRotation))
                            {
                                idListOfRot = 0;
                                command.next();
                            }
                            break;
                        case (int)LookFor.Search:
                            if (idListOfRot < listOfRotations.Count)
                            {
                                Quaternion auxQuater2 = Quaternion.Euler(listOfRotations[idListOfRot]);
                                moveFocus(originalFocusRotation * auxQuater2);
                                if (isFocusMoved(focus.parent.localRotation, originalFocusRotation * auxQuater2))
                                {
                                   idListOfRot++;
                                   if(isOnList(robotVision.getListOfElements(), command.getRefName()))
                                    {
                                        command.next();
                                    }
                                }
                            }else
                            {
                                Log("Command>>> " + this.name + " command " + command.getId() + " Failed! " + command.getRefName() + " Not found.");
                                command.fail();
                            }
                            break;
                        case (int)LookFor.Focus:
                            if (command.getReference() == null)
                            {
                                GameObject auxGameObject = getOnList(robotVision.getListOfElements(), command.getRefName());
                                if (auxGameObject != null)
                                {
                                    command.setReference(auxGameObject.transform);
                                    if (isFocusInPosition(focus, auxGameObject.transform.position) && !isVisible(auxGameObject.transform))
                                    {
                                        Log("Command>>> " + this.name + " command " + command.getId() + " Failed! Obj. is not visible.");
                                        command.fail();
                                    }
                                }
                                else
                                {
                                    command.resetState();
                                }
                            }else
                            {
                                updateFocus(command.getNearestDesiredPosition(transform.position));
                                if (isFocusInPosition(focus, command.getNearestDesiredPosition(transform.position)) && (animator.GetFloat("HeadReach") > 0.9))
                                {
                                    
                                    command.next();
                                }
                            }                            
                            break;
                        case (int)LookFor.Turn:
                            Command auxCommand = new Command("",Action.Turn, command.getReference());
                            GetComponent<AgentMovement>().sendCommand(auxCommand);
                            Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
                            command.success();
                            break;
                        case (int)LookFor.End:
                            if (command.isFail())
                            {
                                moveFocus(originalFocusRotation);
                                if (isFocusMoved(focus.parent.localRotation, originalFocusRotation))
                                {
                                    ik = false;
                                }
                            }else
                            {
                                updateFocus(command.getNearestDesiredPosition(transform.position));
                            }
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
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

    public bool sendCommand(Command command)
    {

        this.command = command;
        Log("Command>>> " + this.name + " received command " + command.getStringCommand());
        return true;
    }

    private bool updateFocus(Vector3 position)
    {
        if (ik)
        {
            Vector3 direction = position - focus.parent.position;
            Quaternion rotation  = Quaternion.LookRotation(direction);
            focus.parent.rotation = Quaternion.Lerp(focus.parent.rotation, rotation, Constants.NATURAL_LOOK_SPD  * Time.deltaTime);            
        }
        return true;
    }

    private bool moveFocus(Quaternion rotation)
    {
        if (ik)
        {
            focus.parent.localRotation = Quaternion.Lerp(focus.parent.localRotation, rotation, Constants.NATURAL_LOOK_SPD * Time.deltaTime);
        }
        return true;
    }

    private bool isFocusMoved(Quaternion focusRotation, Quaternion targetRotation)
    {
        float angle = Quaternion.Angle(focusRotation, targetRotation);
        if (angle < 5)
        {
            return true;
        }
        return false;
    }

    private bool isFocusInPosition(Transform focus, Vector3 position)
    {
        Vector3 direction = position - focus.parent.position;
        if (Vector3.Angle(focus.parent.forward, direction) < 1f)
        {
            return true;
        }
        return false;
    }

    private GameObject getOnList(List<GameObject> listGameObjects, string name)
    {
        foreach (GameObject item in listGameObjects)
            if (item.name.Equals(name, StringComparison.OrdinalIgnoreCase) || item.tag.Equals(name, StringComparison.OrdinalIgnoreCase))
                return item;
        return null;
    }

    private bool isOnList(List<GameObject> listGameObjects, string name)
    {
        foreach (GameObject item in listGameObjects)
            if (item.name.Equals(name, StringComparison.OrdinalIgnoreCase)|| item.tag.Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    public Transform getRobotHead()
    {
        return agentHead;
    }

    public Command getCurrentCommand()
    {
        return command;
    }
}
