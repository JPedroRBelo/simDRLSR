using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class AvatarManager : MonoBehaviour {

    private AgentSpeech speech;
    private AgentInteraction aI;
    private VisionManager avatarVision;
    private Queue<Command> commandsQueue;
    private Command atCommand;
    private Animator animator;


   // private enum Verbs {Grab, Leave, Activate, Open, Close,Speak};
    private Dictionary<Action, string> dictVerbs;
    
    private List<GameObject> locationsList = null;
    private List<GameObject> switchsList = null;
    private List<GameObject> objectsList = null;

    private void Awake()
    {
        dictVerbs = constructEnglishDict();
        locationsList = new List<GameObject>();
        switchsList = new List<GameObject>();
        objectsList = new List<GameObject>();
        //doorsList = new List<GameObject>();
    }
    // Use this for initialization
    void Start()
    {
        avatarVision = GetComponent<VisionManager>();

        animator = GetComponent<Animator>();
        commandsQueue = new Queue<Command>();
        aI = GetComponent<AgentInteraction>();
        speech = GetComponent<AgentSpeech>();
        locationsList = new List<GameObject>();
        Debug.Log("RHS>>> " + this.name +" "+ this.GetType()+ " is ready.");
    }


	// Update is called once per frame
	void Update () {
        if (avatarVision != null)
        {
            List<GameObject> objectsInFildOfVision = avatarVision.getListOfElements();           
           
            List<GameObject> auxGOLocations = new List<GameObject>(getElementsOfTagType(objectsInFildOfVision, Constants.TAG_LOCATION));
            objectsList = new List<GameObject>(getElementsOfTagType(objectsInFildOfVision, Constants.TAG_OBJECT));
            switchsList = new List<GameObject>(getElementsOfTagType(objectsInFildOfVision, Constants.TAG_SWITCH));
            //doorsList = new List<GameObject>(getElementsOfTagType(objectsInFildOfVision, Constants.TAG_DOOR));
            locationsList = new List<GameObject>(auxGOLocations);
        }

        if (commandsQueue.Count > 0 & manageExecution())
        {
            execute(commandsQueue.Dequeue());
        }

    }

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
        }
        else
        {
            return true;
        }
    }

    public void cancelExecutation()
    {
        if (atCommand != null)
        {
            atCommand.fail();
            commandsQueue.Clear();
        }
    }

    private void execute(Command command)
    {
        atCommand = command;
        switch (Command.DictActions[command.getAction()].typeAction)
        {
            case TypeAction.Interaction:
                aI.sendCommand(command);
                break;
            case TypeAction.Communication:
                speech.sendCommand(command);
                break;
            default:
                break;
        }
    }

    private List<GameObject> getElementsOfTagType(List<GameObject> elementsList, string tag)
    {
        List<GameObject> auxList = new List<GameObject>();
        foreach (GameObject item in elementsList)
        {
            if (Constants.getTypeOfTag(item.tag) == tag)
            {
                auxList.Add(item);
            }
        }
        return auxList;
    }

    private Dictionary<Action, string> constructEnglishDict()
    {
        Dictionary<Action, string> dict = new Dictionary<Action, string>();
        dict.Add(Action.Speak, "Speak");
        dict.Add(Action.Take, "Grab");
        dict.Add(Action.Release, "Leave");
        dict.Add(Action.Activate, "Act/Open");        
        dict.Add(Action.Deactivate, "Deact/Close");
        
        return dict;
    }

    public void sendCommand(string id,Hands hand, Action action, Transform transform)
    {
        commandsQueue.Enqueue(new Command(id,hand, action, transform));
    }

    public void sendCommand(string id, Action action, string speech)
    {
        commandsQueue.Enqueue(new Command(id,action, speech));
    }


    /*private string getTypeOfTag(string tag)
    {
        switch (tag)
        {
            case Constants.TAG_DOOR:
                return Constants.TAG_SWITCH;
            case Constants.TAG_PIE:
                return Constants.TAG_OBJECT;
            case Constants.TAG_DRAWER:
                return Constants.TAG_SWITCH;
            case Constants.TAG_FURNITURE:
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
    */
    public Dictionary<Action, string> getAvailableActions()
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
}
