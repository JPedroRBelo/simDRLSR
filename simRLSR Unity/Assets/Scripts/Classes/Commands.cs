using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Enum originalmente utilizado no SocketCommands
public enum CommandType
{
    ActivateLeft = 1,
    ActivateRight = 2,
    DeactivateLeft = 3,
    DeactivateRight = 4,
    HeadReset = 5,
    LeaveLeft = 6,
    LeaveRight = 7,
    LookAt = 8,
    LookFor = 9,
    Move = 10,
    Rotate = 11,
    SmellLeft = 12,
    SmellRight = 13,
    Speech = 14,
    TakeLeft = 15,
    TakeRight = 16,
    TasteLeft = 17,
    TasteRight = 18,
    Turn = 19,
    CancelCommands = 20,
    Animate = 21
};
//Enum originalmente utilizado no SocketCommands
public enum ParameterType
{
    WithId = 1,
    WithPos = 2,
    WithAngle = 3,
    WithString = 4,
    WithoutParameter = 5,
    WithAnimate = 6
}


public enum Hands {Left,Right};
public enum TypeAction {Interaction,Movementation,HeadFocusing,Communication,Operational,Behave};

public enum Action {Take, Release,Activate, Deactivate,Taste,Smell,Move,Turn,Rotate,HeadFocus,HeadReset,LookFor,Speak,Cancel,Animate};

/*estados de cada Action. Está em ordem de execução.
* Caso queira adicionar um estado intermediário, colocar na ordem de chamada
* End deve estar sempre depois de todos estados e antes de estados especiais (ex. Fail), 
* pois o next() faz um ++ no Enum 
*/

public enum Take {Start,Position,Grab,Approximate,End};
public enum Activate {Start, Position,Trigger,End};
public enum Deactivate {Start, Position,Trigger,End };
public enum Release {Start, Position, Leave,End};
public enum OpenDoor {Start, Position,Open,End};
public enum CloseDoor {Start, Position,Close, End};
public enum Taste {Start, Position,Taste,ReturnHeadTask,End};
public enum Smell { Start, Position, Smell, ReturnHeadTask, End};

public enum Move { Start, Position, Turn, End};
public enum Rotate { Start, Position, End};
public enum Turn { Start, Position, End};

public enum HeadFocus { Start, Position,End};
public enum HeadReset { Start, Position,End};
public enum LookFor { Start, Position,Search,Focus,Turn,End};

public enum Speak { Start, Position,End};

public enum Cancel { Start, Position, End };

public enum Animate { Start, Position, End };

//Fim ações
public enum CommandStatus {Running, Success, Fail};

public class Command{
    //Ações

    const int NEXT = 1;
    const int PREV = -1;

     public static  Dictionary<Action, Tuple<TypeAction, string>> DictActions
     = new Dictionary<Action, Tuple<TypeAction, string>>
     {
        { Action.Take,new Tuple<TypeAction,string>(TypeAction.Interaction,Constants.TAG_OBJECT)},
        { Action.Activate,new Tuple<TypeAction,string>(TypeAction.Interaction,Constants.TAG_SWITCH)},
        { Action.Deactivate,new Tuple<TypeAction,string>(TypeAction.Interaction,Constants.TAG_SWITCH)},
        { Action.Release,new Tuple<TypeAction,string>(TypeAction.Interaction,Constants.TAG_LOCATION)},
         {Action.Taste, new Tuple<TypeAction, string>(TypeAction.Interaction,Constants.TAG_OBJECT)},
          {Action.Smell, new Tuple<TypeAction, string>(TypeAction.Interaction,Constants.TAG_OBJECT)},
        //{ Action.OpenDoor,new Tuple<TypeAction,string>(TypeAction.Interaction,Constants.TAG_DOOR)},
        //{ Action.CloseDoor,new Tuple<TypeAction,string>(TypeAction.Interaction,Constants.TAG_DOOR)},
        { Action.Move,new Tuple<TypeAction,string>(TypeAction.Movementation,Constants.TAG_LOCATION)},
        { Action.Turn,new Tuple<TypeAction,string>(TypeAction.Movementation,Constants.TAG_LOCATION)},
        { Action.Rotate,new Tuple<TypeAction,string>(TypeAction.Movementation,Constants.PAR_ROTATION)},
        { Action.HeadFocus,new Tuple<TypeAction,string>(TypeAction.HeadFocusing,Constants.TAG_LOCATION)},
        { Action.HeadReset,new Tuple<TypeAction,string>(TypeAction.HeadFocusing,Constants.PAR_NULL)},
        { Action.LookFor,new Tuple<TypeAction,string>(TypeAction.HeadFocusing,Constants.PAR_STRING)},
         { Action.Speak,new Tuple<TypeAction,string>(TypeAction.Communication,Constants.PAR_STRING)},
         {Action.Cancel,new Tuple<TypeAction, string>(TypeAction.Operational,Constants.PAR_NULL)},
         {Action.Animate,new Tuple<TypeAction, string>(TypeAction.Behave,Constants.PAR_ANIMATIONTIME)}
     };

    private string id;
    private string refName;
    private Transform reference;
    private float angleRotation;
    private Transform auxReference;
    private Vector3 position;
    private Hands actHand;
    private Action action;
    private Take takeState;
    private Activate activateState;
    private Deactivate deactivateState;
    private Release releaseState;
    private Taste tasteState;
    private Smell smellState;
    //Time milliseconds...
    private int time;
    private string animation;

    /* private OpenDoor openDoorState;
     private CloseDoor closeDoorState;*/

    private Move moveState;
    private Turn turnState;
    private Rotate rotateState;

    private HeadFocus headFocusState;
    private HeadReset headResetState;
    private LookFor lookForState;

    private Speak speakState;

    private Cancel cancelState;
    private Animate animateState;

    private CommandStatus commandStatus;

    private bool reset;

    //Action with Hands and objects or locations
    public Command(string id, Hands actHand, Action action, Transform reference)
    {
        constructor(id, actHand, action, reference, reference.position);
    }

 
    //Leave/Release, move head 
    public Command(string id, Hands actHand, Action action, Vector3 position)
    {
        constructor(id, actHand, action, null, position);
    }

    private void constructor(string id, Hands actHand, Action action, Transform reference, Vector3 position)
    {
        this.id = id;
        this.actHand = actHand;
        this.action = action;
        this.reference = reference;
        this.position = position;
        if (reference != null)
        {
            refName = reference.name;
        }
        else 
        {
            refName = "Position: "+position.x + ", " +position.y + ", " +position.z;
        }

        initStates();
        if (action == Action.Taste)
        {
            refName = "Obj. in " + actHand.ToString() + " Hand";
        }
    }

    //Move,turn ...
    public Command(string id, Action action, Transform reference)
    {
        this.id = id;
        actHand = Hands.Right;
        this.action = action;
        this.reference = reference;
        refName = reference.name;
        initStates();
    }

    //Animate
    public Command(string id, Action action, string animation,Transform reference,  int time)
    {
        this.id = id;
        actHand = Hands.Right;
        this.action = action;
        this.reference = reference;
        refName = "";
        this.time = time;
        initStates();
        this.animation = animation;
        
    }

    //Look Off
    public Command(string id,Action action)
    {
        this.id = id;
        actHand = Hands.Right;
        this.action = action;
        reference = null;
        refName =  "";
        initStates();
    }

    //Rotate
    public Command(string id, Action action, float angleRotation, string refName = "")
    {
        this.id = id;
        initStates();
        actHand = Hands.Right;
        this.action = action;
        reference = null;
        this.angleRotation = angleRotation;
        this.refName = refName;
        if(this.refName.Equals(""))
            this.refName = "Pos. At. + ("+angleRotation.ToString("0")+ "º)";
    }

    //Look For or Speak
    public Command(string id,Action action, string str)
    {
        this.id = id;
        initStates();
        actHand = Hands.Right;
        this.action = action;
        reference = null;
        refName = str;
        animation = "none";
    }

    //Taste or Smell
    public Command(string id,Hands actHand,Action action)
    {
        this.id = id;
        initStates();
        this.actHand = actHand;
        this.action = action;
        reference = null;
        refName = "Obj. in " + actHand.ToString();
    }

    private void  initStates()
    {
        takeState = Take.Start;
        activateState = Activate.Start;
        deactivateState = Deactivate.Start;
        releaseState = Release.Start;
        tasteState = Taste.Start;
        smellState = Smell.Start;

        /*openDoorState = OpenDoor.Start;
        closeDoorState = CloseDoor.Start;*/
        moveState = Move.Start;
        rotateState = Rotate.Start;
        turnState = Turn.Start;
        commandStatus = CommandStatus.Running;
        headFocusState = HeadFocus.Start;
        headResetState = HeadReset.Start;
        lookForState = LookFor.Start;

        speakState = Speak.Start;

        cancelState = Cancel.Start;
        animateState = Animate.Start;

        reset = true;           
    }

    public int getActionStateID()
    {
        switch (action)
        {
            case Action.Take:
                return (int)takeState;
            case Action.Activate:
                return (int)activateState;
            case Action.Deactivate:
                return (int)deactivateState;
            case Action.Release:
                return (int)releaseState;
            case Action.Taste:
                return (int)tasteState;
            case Action.Smell:
                return (int)smellState;
            case Action.Move:
                return (int)moveState;
            case Action.Rotate:
                return (int)rotateState;
            case Action.Turn:
                return (int)turnState;
            case Action.HeadFocus:
                return (int)headFocusState;
            case Action.HeadReset:
                return (int)headResetState;
            case Action.LookFor:
                return (int)lookForState;
            case Action.Speak:
                return (int)speakState;
            case Action.Cancel:
                return (int)cancelState;
            case Action.Animate:
                return (int)animateState;
            default:
                return -1;
        }
    }

    public CommandStatus getCommandStatus()
    {
        return commandStatus;
    }

    public Action  getAction()
    {
        return action;
    }

    public bool isRefObject()
    {
        if (reference != null)
        {
            return reference.tag == Constants.TAG_OBJECT;
        }
        return false;
    }

    public bool isRefLocation()
    {
        if (reference != null)
        {
            return reference.tag == Constants.TAG_LOCATION || reference.tag == Constants.TAG_POSITION;
        }
        return false;
    }

    public void setLocation(Transform local)
    {
        reference = local;
    }

    public float getDesiredRotation()
    {
        if (reference != null)
        {
            return reference.rotation.eulerAngles.y;
        }
        return 0;
    }



    private Vector3 getNearest(Vector3 position,string name)
    {
        Vector3 nearPosition = this.position;
        if (reference != null)
        {
            nearPosition = reference.position;
            int auxCont = 0;
            foreach (Transform t in reference)
            {

                if (t.name.Equals(name))
                {
                    if (auxCont == 0)
                        nearPosition = t.position;
                    else
                    {
                        float distance1 = Vector3.Distance(nearPosition, position);
                        float distance2 = Vector3.Distance(t.position, position);
                        if (distance1 > distance2)
                        {
                            nearPosition = t.position;
                        }
                    }
                    auxCont++;
                }
            }
        }
        return nearPosition;
    }

    public Vector3 getNearestDesiredLocation(Vector3 position)
    {
        Vector3 nearPosition = this.position;
        if (reference != null)
        {
            nearPosition = getNearest(position, Constants.REF_LOCATION);
            if (v3Equal(nearPosition, reference.position))
            {
                nearPosition = getNearest(position, Constants.REF_POSITION);
            }
        }
        return nearPosition;
    }


    public Vector3 getNearestDesiredContainer(Vector3 position)
    {
        Vector3 nearPosition = this.position;
        if (reference != null)
        {
            nearPosition = getNearest(position, Constants.REF_CONTAINER);
            if (v3Equal(nearPosition, reference.position))
            {
                nearPosition = getNearest(position, Constants.REF_POSITION);
            }
        }
        return nearPosition;
    }

    public Vector3 getNearestDesiredPosition(Vector3 position)
    {
        Vector3 newPosition = getNearest(position, Constants.REF_POSITION);
        return newPosition;
    }

    public Vector3 getOutputDesiredPosition(Vector3 position)
    {
        Vector3 nearPosition = this.position;
        if (reference != null)
        {
            nearPosition = reference.Find(Constants.REF_OUTPUT).position;
            if (nearPosition == null)
                nearPosition = getNearestDesiredPosition(position);
        }
        return nearPosition;
    }

    public void next()
    {
        changeStatus(NEXT);
    }

    public void prev()
    {
        changeStatus(PREV);
    }

    private void changeStatus(int value)
    {
        if (commandStatus != CommandStatus.Fail && commandStatus != CommandStatus.Success)
        {
            switch (action)
            {
                case Action.Take:
                    if (takeState != Take.End)
                        takeState+=value;
                    break;
                case Action.Activate:
                    if (activateState != Activate.End)
                        activateState += value;
                    break;
                case Action.Deactivate:
                    if (deactivateState != Deactivate.End)
                        deactivateState += value;
                    break;
                case Action.Release:
                    if (releaseState != Release.End)
                        releaseState += value;
                    break;
                case Action.Taste:
                    if (tasteState != Taste.End)
                        tasteState += value;
                    break;
                case Action.Smell:
                    if (smellState != Smell.End)
                        smellState += value;
                    break;
                case Action.Move:
                    if (moveState != Move.End)
                        moveState+=value;
                    break;
                case Action.Rotate:
                     if (rotateState != Rotate.End)
                        rotateState+=value;
                    break;
                case Action.Turn:
                    if (turnState != Turn.End)
                         turnState+=value;
                   break;
                case Action.HeadFocus:
                    if (headFocusState != HeadFocus.End)
                        headFocusState+=value;
                    break;
                case Action.HeadReset:
                    if (headResetState != HeadReset.End)
                        headResetState+=value;
                    break;
                case Action.LookFor:
                    if (lookForState != LookFor.End)
                        lookForState += value;
                    break;
                case Action.Speak:
                    if (speakState != Speak.End)
                       speakState += value;
                    break;
                case Action.Cancel:
                    if (cancelState != Cancel.End)
                        cancelState += value;
                    break;
                case Action.Animate:
                    if (animateState != Animate.End)
                        animateState += value;
                    break;
                default:
                    break;
            }
            commandStatus = CommandStatus.Running;
        }
    }

    private void end()
    {
        switch (action)
        {
            case Action.Take:
                takeState = Take.End;
                break;
            case Action.Activate:
                activateState = Activate.End;
                break;
            case Action.Deactivate:
                deactivateState = Deactivate.End;
                break;
            case Action.Release:
                releaseState = Release.End;
                break;
            case Action.Taste:
                tasteState = Taste.End;
                break;
            case Action.Smell:
                smellState = Smell.End;
                break;
            case Action.Move:
                moveState = Move.End;
                break;                
            case Action.Rotate:
                rotateState = Rotate.End;
                break;
            case Action.Turn:
                  turnState = Turn.End;
                  break;
            case Action.HeadFocus:
                headFocusState = HeadFocus.End;
                break;
            case Action.HeadReset:
                headResetState = HeadReset.End;
                break;
            case Action.LookFor:
                lookForState = LookFor.End;
                break;
            case Action.Speak:
                speakState = Speak.End;
                break;
            case Action.Cancel:
                cancelState = Cancel.End;
                break;
            case Action.Animate:
                animateState = Animate.End;
                break;
            default:
                break; ;
        }   
    }

    public void success()
    {
        end();
        commandStatus = CommandStatus.Success;
    }

    public void fail()
    {
        end();
        commandStatus = CommandStatus.Fail;        
    }

    public bool isFail()
    {
        return (commandStatus == CommandStatus.Fail);
    }  
    
    public Hands getActHand()
    {
        return actHand;
    }

    public Transform getReference()
    {
        return reference;
    }
 
    public string getRefName()
    {
        return refName;      
    }   

    public int getTime()
    {
        return time;
    }

    public string getAnimation()
    {
        return animation;
    }

    public float getAngleRotation()
    {
        if (reference != null)
        {
            return reference.rotation.eulerAngles.y;
        }else
        {
            return angleRotation;
        }
    }

    public void resetState()
    {
        initStates();
    }

    public bool setReference(Transform transform)
    {
        reference = transform;
        return true;
    }

    public bool isToResetHand()
    {
        if (reset)
        {
            reset = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool v3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }

    public string getId()
    {
        return id;
    }

    public string getStringCommand()
    {
        string commandName = "ID: "+id+ " "+ action.ToString();

        switch (action)
        {
            case Action.Take:
                commandName += ", with "+actHand.ToString()+" Hand, obj. "+ reference.name;
                break;
            case Action.Activate:
                commandName += ", with " + actHand.ToString() + " Hand, obj. " + reference.name;
                break;
            case Action.Deactivate:
                commandName += ", with " + actHand.ToString() + " Hand, obj. " + reference.name;
                break;
            case Action.Release:
                if(reference==null)
                    commandName += ", with " + actHand.ToString() + " Hand, in " + reference;
                else
                    commandName += ", with " + actHand.ToString() + " Hand, in " + position;
                break;
            case Action.Taste:
                commandName += " obj. in " + actHand.ToString() + " Hand ";
                break;
            case Action.Smell:
                commandName += " obj. in " + actHand.ToString() + " Hand ";
                break;
            case Action.Move:
                if (reference == null)
                    commandName += " to " + reference;
                else
                    commandName += " to " + position;
                break;
            case Action.Rotate:
                if (reference == null)
                    commandName += " to " + reference;
                else
                    commandName += " to " + position;
                break;
            case Action.Turn:
                    commandName += " "+ refName;
                break;
            case Action.HeadFocus:
               if (reference == null)
                    commandName += " to " + reference;
                else
                    commandName += " to " + position;
                break;
            case Action.HeadReset:
                
                break;
            case Action.LookFor:
                commandName += " to " + refName;
                break;
            case Action.Speak:
                commandName += ": " + refName;
                break;
            case Action.Cancel:
                break;
            case Action.Animate:
                break;
            default:
                break; ;
        }
        return commandName;
    }
}
