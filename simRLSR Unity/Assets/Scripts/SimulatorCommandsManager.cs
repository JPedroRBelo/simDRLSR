using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SimulatorCommandsManager : MonoBehaviour {

    public Transform robot = null;   

    private AgentInteraction aI;
    private AgentMovement aM;
    private AgentHeadMovement hM;
    private AgentSpeech sp;
    private Queue<Command> commandsQueue;
    private Command atCommand;

    private VisionManager robotVision;
    private SmellManager robotSmell;
    private HearingManager robotHearing;
    private TasteManager robotTaste;
    private TouchManager robotTouch;

    private HashSet<GameObject> allPerceivedElements;

    //private enum Verbs {Move,Rotate,Grab,Leave,Activate,Open,Close,Taste,Gaze,ResetGaze};
    private Dictionary<Action, string> dictVerbs;

    private List<GameObject> locationsOnScene;

    private List<GameObject> locationsList = null;
    private List<GameObject> switchsList = null;
    private List<GameObject> objectsList = null;
    //private List<GameObject> doorsList = null;

    private void Awake()
    {
        dictVerbs = constructEnglishDict();
        locationsList = new List<GameObject>();
        switchsList = new List<GameObject>();
        objectsList = new List<GameObject>();
        //doorsList = new List<GameObject>();
    }
    // Use this for initialization222
    void Start () {
        robotVision = robot.GetComponent<VisionManager>();
        robotSmell = robot.GetComponent<SmellManager>();
        robotHearing = robot.GetComponent<HearingManager>();
        robotTaste = robot.GetComponent<TasteManager>();
        robotTouch = robot.GetComponent<TouchManager>();
        locationsOnScene = new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.TAG_KNOWLOCATIONS));
        
        commandsQueue= new Queue<Command>();
        aI = robot.GetComponent<AgentInteraction>();
        aM = robot.GetComponent<AgentMovement>();
        hM = robot.GetComponent<AgentHeadMovement>();
        sp = robot.GetComponent<AgentSpeech>();

        locationsList =new List<GameObject>(locationsOnScene);
        Debug.Log("RHS>>> " + this.name + " ready.");
    }
    
    void Update()
    {
        if (robotVision != null)
        {
            List<GameObject> objectsInFildOfVision = robotVision.getListOfElements();
            List<GameObject> objectsInSmellSense = robotSmell.getListOfElements();
            List<GameObject> objectsInHearingSense = robotHearing.getListOfElements();
            List<GameObject> objectsInTasteSense = robotTaste.getListOfElements();
            List<GameObject> objectsInTouchSense = robotTouch.getListOfElements();
            List<GameObject> allObjectsInSenses = objectsInFildOfVision.Union(objectsInSmellSense).ToList();
            allObjectsInSenses = allObjectsInSenses.Union(objectsInHearingSense).ToList();
            allObjectsInSenses = allObjectsInSenses.Union(objectsInTouchSense).ToList();
            allPerceivedElements = new HashSet<GameObject>(allObjectsInSenses);
            List<GameObject> auxGOLocations = new List<GameObject>( getElementsOfTagType(allObjectsInSenses, Constants.TAG_LOCATION));
            auxGOLocations.AddRange(locationsOnScene);
            objectsList = new List<GameObject>(getElementsOfTagType(allObjectsInSenses, Constants.TAG_OBJECT));
            switchsList = new List<GameObject>(getElementsOfTagType(allObjectsInSenses, Constants.TAG_SWITCH));
            //doorsList = new List<GameObject>(getElementsOfTagType(objectsInFildOfVision, Constants.TAG_DOOR));
            locationsList = new List<GameObject>(auxGOLocations);
         }
        if (commandsQueue.Count > 0 & manageExecution())
        {
            execute(commandsQueue.Dequeue());
        }
    }

    private List<GameObject> getElementsOfTagType(List<GameObject> elementsList,string tag)
    {
        List<GameObject> auxList = new List<GameObject>();
        foreach (GameObject item in elementsList)
        {
            if (Constants.getTypeOfTag(item.tag) == tag)
            {
                auxList.Add(item);
            }else if (getInternalTypeOfTag(item.tag) == tag)
            {
                auxList.Add(item);
            }
        }
        return auxList;
    }


    public static string getInternalTypeOfTag(string tag)
    {
        switch (tag)
        {
            case Constants.TAG_DOOR:
                return Constants.TAG_SWITCH;
            case Constants.TAG_PIE:
                return Constants.TAG_OBJECT;
            case Constants.TAG_GLASS:
                return Constants.TAG_OBJECT;
            case Constants.TAG_MUG:
                return Constants.TAG_OBJECT;
            case Constants.TAG_PLATE:
                return Constants.TAG_OBJECT;
            case Constants.TAG_BOOK:
                return Constants.TAG_OBJECT;
            case Constants.TAG_CRACKER:
                return Constants.TAG_OBJECT;
            case Constants.TAG_PHOTOFRAME:
                return Constants.TAG_OBJECT;
            case Constants.TAG_FRYINGPAN:
                return Constants.TAG_OBJECT;
            case Constants.TAG_COOKINGPOT:
                return Constants.TAG_OBJECT;
            case Constants.TAG_SALMON:
                return Constants.TAG_OBJECT;
            case Constants.TAG_MEAT:
                return Constants.TAG_OBJECT;
            case Constants.TAG_BOX:
                return Constants.TAG_OBJECT;
            case Constants.TAG_CUP_CAKE:
                return Constants.TAG_OBJECT;
            case Constants.TAG_DRAWER:
                return Constants.TAG_SWITCH;
            case Constants.TAG_FURNITURE:
                return Constants.TAG_LOCATION;
            case Constants.TAG_TABLE:
                return Constants.TAG_LOCATION;
            case Constants.TAG_BOOK_SHELF:
                return Constants.TAG_LOCATION;
            case Constants.TAG_SOFA:
                return Constants.TAG_LOCATION;
            case Constants.TAG_CARPET:
                return Constants.TAG_LOCATION;
            case Constants.TAG_STOVE:
                return Constants.TAG_LOCATION;
            case Constants.TAG_COUNTER:
                return Constants.TAG_LOCATION;
            case Constants.TAG_CABINET:
                return Constants.TAG_LOCATION;
            case Constants.TAG_FLOOR:
                return Constants.TAG_LOCATION;
            case Constants.TAG_WALL:
                return Constants.TAG_LOCATION;
            case Constants.TAG_HUMAN:
                return Constants.TAG_LOCATION;
            case Constants.TAG_WATER:
                return Constants.TAG_LOCATION;
            case Constants.TAG_TAP:
                return Constants.TAG_SWITCH;
            default:
                break;
        }
        return tag;
    }

    //Camando já foi finalizado?
    private bool manageExecution()
    {
        if (atCommand != null)
        {            
            switch (atCommand.getCommandStatus())
            {                
                case CommandStatus.Success:

                    return true;
                case CommandStatus.Running:
                    return false;
                case CommandStatus.Fail:
                    return true;
                default:
                    return false;
            }
        }else
        {
            return true;
        }
    }

    private void execute(Command command)
    {
        
        
        switch (Command.DictActions[command.getAction()].typeAction)
        {
            case TypeAction.Interaction:
                atCommand = command;
                aI.sendCommand(command);
                break;
            case TypeAction.Movementation:
                atCommand = command;
                aM.sendCommand(command);
                break;
            case TypeAction.HeadFocusing:
                atCommand = command;
                hM.sendCommand(command);
                break;
            case TypeAction.Communication:
                atCommand = command;
                sp.sendCommand(command);
                break;
            case TypeAction.Operational:
                //This Script will take care of this
                doTask(command);
                break;
            default:
                break;
        }
        
    }

    private void doTask(Command command)
    {
        switch (command.getAction())
        {
            case Action.Cancel:
                Debug.Log("Command>>> " + this.name + " received command " + command.getStringCommand());
                Debug.Log("Command>>> " + this.name + " command " + command.getId() + " Started!");
                cancelExecutation();
                atCommand = command;
                Debug.Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
                atCommand.success();
                break;
            default:
                break;
        }
    }

   public void startExecutation()
    {        
        execute(commandsQueue.Dequeue());        
    }


   /* private Dictionary<Verbs, string> constructPortugueseDict()
    {
        Dictionary<Verbs, string> dict = new Dictionary<Verbs, string>();

        dict.Add(Verbs.Move, "Movimentar");
        dict.Add(Verbs.Rotate, "Rotacionar");
        dict.Add(Verbs.Grab, "Pegar");
        dict.Add(Verbs.Leave, "Soltar");
        dict.Add(Verbs.Activate, "Ativar");
        dict.Add(Verbs.Open, "Abrir");
        dict.Add(Verbs.Close, "Fechar");
        dict.Add(Verbs.Gaze, "Olhar");
        dict.Add(Verbs.ResetGaze, "Desfitar");
        return dict;
    }*/

    private Dictionary<Action, string> constructEnglishDict()
    {
        Dictionary<Action, string> dict = new Dictionary<Action, string>();
        dict.Add(Action.Move, "Move");
        dict.Add(Action.Turn, "Turn");
        dict.Add(Action.Rotate, "Rotate");
        //dict.Add(Action.OpenDoor, "Open Door");
        //dict.Add(Action.CloseDoor, "Close Door");
        dict.Add(Action.Take, "Grab");
        dict.Add(Action.Taste, "Taste");
        dict.Add(Action.Smell, "Smell");
        dict.Add(Action.Release, "Leave");
        dict.Add(Action.Activate, "Act/Open");
        dict.Add(Action.Deactivate, "Deact/Close");
        dict.Add(Action.HeadFocus, "Look At");
        dict.Add(Action.HeadReset, "Look Off");
        dict.Add(Action.LookFor, "Look For");
        dict.Add(Action.Speak, "Speak");
        //dict.Add(Action.Cancel, "Cancel");
        return dict;
    }
    
    public HashSet<GameObject> getAllPerceivedElements()
    {
        return allPerceivedElements;
    }

    public Command sendCommand(string id, Hands hand, Action action, Transform transform)
    {
        Command command = new Command(id, hand, action, transform);
        manageCommand(command);
        return command;
    }

    public Command sendCommand(string id, Hands hand, Action action, Vector3 position)
    {
        Command command = new Command(id, hand, action, position);
        manageCommand(command);
        return command;
    }

    public Command sendCommand(string id, Action action)
    {
        Command command = new Command(id,action);
        manageCommand(command);
        return command;
    }

    public Command sendCommand(string id, Hands hand,Action action)
    {
        Command command = new Command(id,hand, action, robotTaste.tasteSensor);
        if (action == Action.Taste) {
            command = new Command(id, hand, action, robotTaste.tasteSensor);
            manageCommand(command);
        }else
        if (action == Action.Smell)
        {
            command = new Command(id, hand, action, robotSmell.smellSensor);
            manageCommand(command);
        }
        else
        {
            command.fail();
            manageCommand(command);
        }
        return command;
    }

    public Command sendCommand(string id, Action action, string name)
    {
        Command command = new Command(id,action, name);
        manageCommand(command);
        return command;
    }

    public Command sendCommand(string id, Action action, float angle)
    {
        float angleAt = Vector3.Angle(Vector3.forward, robot.forward);
        Vector3 cross = Vector3.Cross(Vector3.forward, robot.forward);
        if (cross.y < 0) angleAt = -angleAt;
        Command command = new Command(id,action, angle + angleAt, angle.ToString("0") + "º");
        manageCommand(command);
        return command;

    }

    private void manageCommand(Command command)
    {
        if (command.getAction() != Action.Cancel)
        {

            commandsQueue.Enqueue(command);
        }else
        {
            execute(command);
        }
    }

    public void cancelExecutation()
    {
        
        if (atCommand != null)
        {
            atCommand.fail();            
        }
        while (commandsQueue.Count != 0)
        {
            commandsQueue.Dequeue().fail();
        }

    }
    
    

    public Dictionary<Action, string>   getAvailableActions()
    {
        return dictVerbs;
    }


    public string getAtCommandName()
    {
        if (atCommand != null)
        {
            return atCommand.getAction().ToString() + " " + atCommand.getRefName();
        }
        else
        {
            return "";
        }
    }

    public CommandStatus getCurrentCommandStatus()
    {
        if (atCommand != null)
        {
            return atCommand.getCommandStatus();
        }
        else
        {
            return CommandStatus.Success;
        }
    }

    public List<GameObject> getLocationsList()
    {
        return locationsList;
    }

    public List<GameObject> getObjectsList()
    {
        return objectsList;
    }
    public List<GameObject> getSwitchsList()
    {
        return switchsList;
    }

    /*public List<GameObject> getDoorsList()
    {
        return doorsList;
    }*/



}
