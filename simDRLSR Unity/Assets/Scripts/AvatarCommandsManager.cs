using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.InteropServices;

public class AvatarCommandsManager : MonoBehaviour {

    private Transform avatar = null;   

    private AgentInteraction aI;
    private AgentBehave aB;
    private AgentMovement aM;
    private AgentHeadMovement hM;
    private AgentSpeech sp;
    private Queue<Command> commandsQueue;
    //Armazena comandos 
    private Queue<Command> auxCommandsQueue;
    private Command atCommand;

    private VisionManager avatarVision;
    private SmellManager avatarSmell;
    private HearingManager avatarHearing;
    private TasteManager avatarTaste;
    private TouchManager avatarTouch;

    private HashSet<GameObject> allPerceivedElements;
    private List<GameObject> allFindableGObjs;

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
        avatar = transform;
        avatarVision = avatar.GetComponent<VisionManager>();
        avatarSmell = avatar.GetComponent<SmellManager>();
        avatarHearing = avatar.GetComponent<HearingManager>();
        avatarTaste = avatar.GetComponent<TasteManager>();
        avatarTouch = avatar.GetComponent<TouchManager>();
        locationsOnScene = new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.TAG_KNOWLOCATIONS));
        
        commandsQueue= new Queue<Command>();
        aI = avatar.GetComponent<AgentInteraction>();
        aM = avatar.GetComponent<AgentMovement>();
        hM = avatar.GetComponent<AgentHeadMovement>();
        sp = avatar.GetComponent<AgentSpeech>();
        aB = avatar.GetComponent<AgentBehave>();

        locationsList =new List<GameObject>(locationsOnScene);
        GameObject[] knowLocations = GameObject.FindGameObjectsWithTag(Constants.TAG_KNOWLOCATIONS);
        GameObject[] publicChairs = GameObject.FindGameObjectsWithTag(Constants.TAG_PUBLICCHAIR);
        GameObject[] workChairs = GameObject.FindGameObjectsWithTag(Constants.TAG_WORKCHAIR);
        GameObject[] magazines = GameObject.FindGameObjectsWithTag(Constants.TAG_MAGAZINE);
        GameObject[] doors = GameObject.FindGameObjectsWithTag(Constants.TAG_SIDEDOOR);
        allFindableGObjs = new List<GameObject>();
        allFindableGObjs = allFindableGObjs.Concat(knowLocations).ToList();
        allFindableGObjs = allFindableGObjs.Concat(publicChairs).ToList();
        allFindableGObjs = allFindableGObjs.Concat(workChairs).ToList();
        allFindableGObjs = allFindableGObjs.Concat(magazines).ToList();
        allFindableGObjs = allFindableGObjs.Concat(doors).ToList();
    }
    
    void Update()
    {
        if((avatarVision != null)&&(avatarSmell != null)&&(avatarHearing != null)&&(avatarTaste != null)&&(avatarTouch != null)&&
        (avatarVision.enabled)&&(avatarSmell.enabled)&&(avatarHearing.enabled)&&(avatarTaste.enabled)&&(avatarTouch.enabled))
        {
            List<GameObject> objectsInFildOfVision = avatarVision.getListOfElements();
            List<GameObject> objectsInSmellSense = avatarSmell.getListOfElements();
            List<GameObject> objectsInHearingSense = avatarHearing.getListOfElements();
            List<GameObject> objectsInTasteSense = avatarTaste.getListOfElements();
            List<GameObject> objectsInTouchSense = avatarTouch.getListOfElements();
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
         }else{
             objectsList = new List<GameObject>();
            switchsList = new List<GameObject>();
            //doorsList = new List<GameObject>(getElementsOfTagType(objectsInFildOfVision, Constants.TAG_DOOR));
            locationsList = new List<GameObject>();
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
            //print(transform.name+" action: "+atCommand.getAction()+" animation "+atCommand.getAnimation());  
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


    public void dynamicInteraction()
    {
        
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
            case TypeAction.Behave:
                atCommand = command;
                aB.sendCommand(command);
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
                //Debug.Log("Command>>> " + this.name + " received command " + command.getStringCommand());
                //Debug.Log("Command>>> " + this.name + " command " + command.getId() + " Started!");
                cancelExecutation();
                atCommand = command;
                //Debug.Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
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

    private GameObject getGameObjectByName(string name)
    {
        foreach (GameObject go in allFindableGObjs)
        {

            if (go.name.Equals(name))
            {                
                return go;
            }
        }
        return null;
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
        Command command = new Command(id,hand, action, avatarTaste.tasteSensor);
        if (action == Action.Taste) {
            command = new Command(id, hand, action, avatarTaste.tasteSensor);
            manageCommand(command);
        }else
        if (action == Action.Smell)
        {
            command = new Command(id, hand, action, avatarSmell.smellSensor);
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
        Command command = new Command(id, action, name);
        if ((action != Action.LookFor) && (action != Action.Speak))
        {
            GameObject go = getGameObjectByName(name);

            if (go != null)
            {
                command = new Command(id, action, go.transform);
                manageCommand(command);
            }/*
            else
            {
                print("=======ERROR" + name + "ERROR========");
            }*/
        }
        else
        {
            manageCommand(command);
        }
        return command;
    }

    public Command sendCommand(string id, Hands hand, Action action, string name)
    {
        Command command = new Command(id, action, name);
        GameObject go = getGameObjectByName(name);
        if (go != null)
        {
            command = new Command(id,hand, action, go.transform);
            manageCommand(command);
        }
        else
        {
            print("=======ERROR" + name + "ERROR========");
        }

        return command;
    }

    public Command sendCommand(string id, Action action, Transform reference)
    {
        Command command = new Command(id, action, reference);
        manageCommand(command);        

        return command;
    }

    public Command sendCommand(string id, Action action, float angle)
    {
        float angleAt = Vector3.Angle(Vector3.forward, avatar.forward);
        Vector3 cross = Vector3.Cross(Vector3.forward, avatar.forward);
        if (cross.y < 0) angleAt = -angleAt;
        Command command = new Command(id, action, angle + angleAt, angle.ToString("0") + "º");
        manageCommand(command);
        return command;

    }

    public Command sendCommand(string id, Action action,string animation, string reference, int time)
    {

        
        GameObject go = getGameObjectByName(reference);
        Command command = new Command(id, action, animation, null, time);
        if (go != null)
        {
            command = new Command(id, action, animation, go.transform, time);
           
        }
        else
        {
            if(!reference.Equals(" ")&&!reference.Equals(""))
                print(transform.name+"=======ERROR" + reference + "ERROR========");
        }
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
