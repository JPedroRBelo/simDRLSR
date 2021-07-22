
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
using System.ComponentModel.Design;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Diagnostics;
//using System.Security.Cryptography;


class Items<T>
{
    public double Probability { get; set; }
    public T Item { get; set; }
}

public class AvatarBehaviors : MonoBehaviour
{

    public bool repeatBehaviors = true;
    public List<TextAsset> behaviorFiles = null;

    public float farDistance = 6;
    public float closeDistance = 2;

    public float visionAngle = 90f;

    public int waitTime = 1000;
    
    private int commandId;


    private List<string> strCommands;

    private List<Command> runningCommands;
    private List<Command> pausedCommands;
    private List<Command> hriCommands;
    private const string DELIMITER = ";";
    private AvatarCommandsManager scm;
    private UniqueIdDistributor uid;
    private HumanVisionManager vision;
    private int count;

    private List<Items<string>> probabilities;
    private GameObject robot;
    private RobotInteraction robotHRI;

    private List<List<Items<HumanActionType>>> engagedProbTab;
    private List<List<Items<HumanActionType>>> human_notEngdProbTab;
    private List<List<Items<HumanActionType>>> robot_notEngdProbTab;


    private bool isHumanEngaged;

    private bool  stagnantBehavior;

    public long maxToleranceTime = 10000;
    private long startToleranceTime;

    
    //private bool ignoreFlag;

    //private float ignoredDistance;

    

    private enum InteractionType
    {
        WaitClose = 0,
        WaitMiddle = 1,
        WaitFar = 2,
        LookClose = 3,
        LookMiddle = 4,
        LookFar = 5,
        WaveClose = 6,
        WaveMiddle = 7,
        WaveFar = 8,
        HSClose = 9,
        HSMiddle = 10,
        HSFar = 11,
    }

    private enum HumanActionType
    {
        None = -1,
        Ignore = 0,
        Wait = 1,
        Look = 2,
        Move = 3,
        Handshake = 4,

    }

    private AgentAction lastRobotAction;
    private HumanActionType lastHumanAction;

    private ((int step,AgentAction action) robotLast, HumanActionType humanAction) lastHumanRobotActions;

    //public CommandType command;   


    /*
        static readonly IDictionary<int, Type> typeLookup = new Dictionary<int, Type>
        {
            {1, typeof(CommandWithId)}, {2, typeof(CommandWithAngle)}, {3, typeof(CommandWithoutPar)},
            { 4, typeof(CommandWithPosition)}, {5, typeof(CommandWithString)}, {6, typeof(Response)}
        };*/

    private void Start()
    {
        startToleranceTime = 0;
        robot = GameObject.FindGameObjectsWithTag("Robot")[0];
        robotHRI = robot.GetComponent<RobotInteraction>();
        //probabilities = getProbabilities();
        scm = transform.GetComponent<AvatarCommandsManager>();
        uid = transform.GetComponent<UniqueIdDistributor>();
        pausedCommands = new List<Command>();
        runningCommands = new List<Command>();
        hriCommands = new List<Command>();
        lastHumanRobotActions = ((-1,AgentAction.None),HumanActionType.None);
        //Flag if human ignored robot in that interaction. Resets when he/she completed one task
        //ignoreFlag = false;
        //ignoredDistance = 0f;
        strCommands = new List<String>();
        vision = transform.GetComponent<HumanVisionManager>();
        if(behaviorFiles!=null)
        {
            foreach(TextAsset file in behaviorFiles)
            {
                string fs = file.text;

                string[] fLines = fs.Split("\n"[0]);
                foreach(string line in fLines)
                {
                    int index = line.IndexOf("#");
                    string cmdText = ""; 
                    if (index > 0)
                    {
                        cmdText = line.Substring(0, index);
                    }
                    else if(index < 0)
                    {
                        cmdText = line;
                    }
                    if (!cmdText.Equals(""))
                    {                    
                        strCommands.Add(cmdText.Replace("\n", "").Replace("\r", ""));
                    }
                }            
            }
        }
        
        GameObject simulatorManager = GameObject.Find("/SimulatorManager");
        string path_config = "Config";
        if(simulatorManager == null){
            Debug.Log("Simulator Manager not found...");
        }else{
            ConfigureSimulation cs = simulatorManager.GetComponent<ConfigureSimulation>();
            if(cs == null){
                Debug.Log("Script Configure Simulation not found...");
            }else
            {
                path_config = cs.getPathConfig();
            }

        }

        string file_engaged_prob = Path.Combine(path_config,"engaged_hri_probabilities.csv");
        string file_human_not_engd_prob = Path.Combine(path_config,"human_notengd_hri_probabilities.csv");
        string file_robot_not_engd_prob = Path.Combine(path_config,"robot_notengd_hri_probabilities.csv");
        engagedProbTab = initProbabilities(file_engaged_prob);
        human_notEngdProbTab = initProbabilities(file_human_not_engd_prob);
        robot_notEngdProbTab = initProbabilities(file_human_not_engd_prob);
        
        isHumanEngaged = false;
        count = 0;    
        commandId = 0;
    }



    private void Update()
    {
        (int step,AgentAction action) robotAction = robotHRI.getActualAction();
        //Verifica se humano está de frente ao robô (se robô é visível)
        HumanActionType hriType = HumanActionType.Ignore;
        if((hriCommands.Count() == 0))
        {       
            GameObject robotAttention = robotHRI.getPersonFocusedByRobot();
            Command hriCommand = null;         
            //Debug.Log("Human Action>>> !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //Debug.Log("Human Action>>> robot attention: "+robotAttention);
            //Verifica foco de atenção do robô
            if((robotAttention==gameObject)||(isHumanEngaged)){
            //if(robotAttention==gameObject){            

                
                float angle = getAngle(transform,robot.transform.position);
                //Debug.Log("Human Action>>> robot angle: "+angle);
                if(angle<=visionAngle){                    
                    //Debug.Log("Human Action>>> human is engaged?: "+isHumanEngaged);
                    List<List<Items<HumanActionType>>> probTab = engagedProbTab;
                    if(!isHumanEngaged){
                        probTab = human_notEngdProbTab;
                    } else if(robotAttention!=gameObject)
                    {
                        probTab = robot_notEngdProbTab;
                    }
                    
                    //print(transform.name+">>> robot attention: "+robotAttention);

                   
                    if((robotAction.action==AgentAction.DoNothing)||(robotAction.action==AgentAction.None)){
                        robotAction.action = AgentAction.Wait;  
                       // Debug.Log("Human Action>>> Robot doing nothing/none");                      
                    }
                    //print(transform.name+">>> robot action: "+robotAction);
                    float distance = Vector3.Distance(robot.transform.position,gameObject.transform.position);
                    /*
                    Debug.Log("Human Action>>> distance: "+distance);
                    if(distance<=closeDistance){                                          
                        Debug.Log("Human Action>>> distancia perto");
                    }else if(distance<= farDistance){                            
                       Debug.Log("Human Action>>> distancia meio");
                    }else{
                        Debug.Log("Human Action>>> distancia longe");                                   
                    }   
                    */  
                    bool auxSelectedCommandFlag = false;
                    
                    if((robotAction.step==lastHumanRobotActions.robotLast.step)&&(lastHumanRobotActions.humanAction==HumanActionType.Ignore)){
                        hriType = HumanActionType.Ignore;
                        auxSelectedCommandFlag = true;
                        //Debug.Log("Human Action>>> ignorando novamente"); 
                    }else if((robotAction.action==lastHumanRobotActions.robotLast.action))                    {

                        //Humano espera até que robô faça algo diferente
                        //caso a ação anterior seja 'Wait' e o robô esteja executando a mesma ação
                        if(!stagnantBehavior){
                            startToleranceTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            stagnantBehavior = true;
                        }
                        long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        float timeSpeed = 1f;
                        GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
                        if(simManager != null){
                            timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
                        }                    
                        if(stagnantBehavior&& ((timeNow-startToleranceTime)<maxToleranceTime/timeSpeed)){
                            
                            if((lastHumanRobotActions.humanAction==HumanActionType.Wait)&&
                                (robotAction.step==lastHumanRobotActions.robotLast.step)){
                                //hriType = getHumanActionByProb(probTab,InteractionType.WaitClose);
                                hriType = HumanActionType.Wait;
                                auxSelectedCommandFlag = true;
                            }else if((lastHumanRobotActions.humanAction==HumanActionType.Look)&&
                                (robotAction.step==lastHumanRobotActions.robotLast.step)){
                                hriType = HumanActionType.Look;
                                auxSelectedCommandFlag = true;
                        
                                    
                            }else if(lastHumanRobotActions.humanAction==HumanActionType.Handshake){
                            
                                hriType = HumanActionType.Ignore;
                                auxSelectedCommandFlag = true;
                                stagnantBehavior = false;
                            }
                        }else{
                            hriType = HumanActionType.Ignore;
                            auxSelectedCommandFlag = true;
                            stagnantBehavior = false;
                        }
                        //Debug.Log("Human Action>>> olhando novamente");                     
                    }else{
                        stagnantBehavior = false;
                    }
                       
                    if(!auxSelectedCommandFlag) 
                    {
                        
                        //Verifica a distância da interação
                        
                        //print(transform.name+ " Distance: "+distance);
                        switch (robotAction.action)
                        {
                            
                            case AgentAction.Wait:

                                if(distance<=closeDistance){                                          
                                    hriType = getHumanActionByProb(probTab,InteractionType.WaitClose);
                                }else if(distance<= farDistance){                            
                                    hriType = getHumanActionByProb(probTab,InteractionType.WaitMiddle);
                                }else{
                                    hriType = getHumanActionByProb(probTab,InteractionType.WaitFar);                                      
                                }                         
                                break;
                            case AgentAction.Look:
                                if(distance<=closeDistance){                                          
                                    hriType = getHumanActionByProb(probTab,InteractionType.LookClose);
                                }else if(distance<= farDistance){                            
                                    hriType = getHumanActionByProb(probTab,InteractionType.LookMiddle);
                                }else{
                                    hriType = getHumanActionByProb(probTab,InteractionType.LookFar);                                      
                                } 
                                break;
                            case AgentAction.Wave:
                                if(distance<=closeDistance){                                          
                                    hriType = getHumanActionByProb(probTab,InteractionType.WaveClose);
                                }else if(distance<= farDistance){                            
                                    hriType = getHumanActionByProb(probTab,InteractionType.WaveMiddle);
                                }else{
                                    hriType = getHumanActionByProb(probTab,InteractionType.WaveFar);                                      
                                } 

                                break;
                            case AgentAction.HandShake:
                                if(distance<=closeDistance){                                          
                                    hriType = getHumanActionByProb(probTab,InteractionType.HSClose);
                                }else if(distance<= farDistance){                            
                                    hriType = getHumanActionByProb(probTab,InteractionType.HSMiddle);
                                }else{
                                    hriType = getHumanActionByProb(probTab,InteractionType.HSFar);                                      
                                }
                                break;
                            default:

                                hriType = HumanActionType.Ignore;
                                break;                    
                        }
                        //Debug.Log("Human Action>>> ação selecionada: "+hriType);
                        //if((robotAction.action!=AgentAction.None)&&(robotAction.action!=AgentAction.DoNothing)){
                        lastHumanRobotActions = (robotAction,hriType);
                        //}
                        //Verifica se humano não ignorou o robô. Caso positivo, ele está engajado na interação
                        isHumanEngaged = !(hriType==HumanActionType.Ignore); 
                        //print(transform.name+" "+hriType);
                        print("Command: "+hriType);
                        if(!isHumanEngaged)
                        {
                            //Debug.Log("Human Action>>> humano nao engajado, resetando posicao cabeca");
                            scm.sendCommand("HRIHeadReset", Action.HeadReset);
                        }
                    }
                    isHumanEngaged = !(hriType==HumanActionType.Ignore);                  
                    
                }
            }
            
        }
        
        if(hriType!=HumanActionType.Ignore)       
        {            
            if (runningCommands.Count() != 0)
            {
                count--;
                scm.sendCommand("IDcancelCommand",Action.Cancel);
            }
            Transform robotHead = robotHRI.getRobotHead();
            Transform robotHand = robotHRI.getRobotHand();
            
            switch (hriType)
            {
                case HumanActionType.Wait:
                    
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Wait","", waitTime));
                    break;
                case HumanActionType.Look:
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.HeadFocus, robotHead));
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Wait","", waitTime));
                    break;
                case HumanActionType.Move:
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.HeadFocus, robotHead));
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Wait","", 1000));
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Move, "Robot"));
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Wait","", 300));
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Turn, robot.transform));
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Wait","", 1000));
                    break;
                case HumanActionType.Handshake:
                    //hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Wait","", 1000));
                    hriCommands.Add(scm.sendCommand(generateCommandId(), Action.HeadFocus, robotHead));
                    if(robotHRI.getActualAction().action==AgentAction.HandShake)
                    {  
                        robotHRI.touchRobotHand();
                        hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Handshake","", 500));
                        hriCommands.Add(scm.sendCommand(generateCommandId(), Hands.Right, Action.Activate, robotHand));
                        
                        hriCommands.Add(scm.sendCommand(generateCommandId(), Action.Animate, "Wait","", 500));
                        scm.sendCommand("HRIHeadReset", Action.HeadReset);
                        isHumanEngaged = false;
                    }                    
                    break;                
                default:
                    break;
            }            
                        
        }

        if(hriCommands.Count() == 0){
            
            if((pausedCommands!=null)||(pausedCommands.Count()>0)){
                //                
            }
            if ((count < strCommands.Count())&&(count >=0))
            {
                if (runningCommands.Count() == 0)
                {
                    //ignoreFlag = false;
                    lastHumanRobotActions.humanAction = HumanActionType.None;
                    lastHumanRobotActions.robotLast.action = AgentAction.None;
                    processCommand(strCommands[count]);
                    count++;
                }
            }
            else
            {
                if(repeatBehaviors) 
                    count = 0;
            }  
            
        }
        removeExecutedCommands(runningCommands);
        removeExecutedCommands(hriCommands);     
             
    }

    private string generateCommandId()
    {
        return (commandId++).ToString();
    }
    private void removeExecutedCommands(List<Command> list)
    {
        List<Command> auxList = new List<Command>(list);
        foreach (Command c in auxList)
        {
            switch (c.getCommandStatus())
            {
                case CommandStatus.Success:                  
                    list.Remove(c);
                    break;
                case CommandStatus.Fail:
                    list.Remove(c);
                    break;
                default:
                    break;
            }
        }
    }

    private HumanActionType getHumanActionByProb(List<List<Items<HumanActionType>>> probTab,InteractionType hri)
    {
        var rnd = new System.Random();
        var probability = rnd.NextDouble();
        var selectedHumanAction = probTab[(int)hri].SkipWhile(i => i.Probability < probability).First();
        return selectedHumanAction.Item;  
    }

    private List<List<float>> readProbTable(string file)
    {
        List<List<float>> probTable = new List<List<float>>();
        using(var reader = new StreamReader(file))
        {
            
            var line = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                List<string> values = line.Split(';').OfType<string>().ToList();
                List<float> row = new List<float>();
                for(int i = 1; i<values.Count ;i++)
                {                    
                    float value = float.Parse(values[i]);                
                    row.Add(value);
                }                
                probTable.Add(row);
            }
        }
        return probTable;
    }


    /* Inicia Probabilidades
    *  Lê arquivo csv com a probabilidade do humano agir de acordo com a distancia e comportamento do robo
    */
    private List<List<Items<HumanActionType>>> initProbabilities(string file)
    {
        List<List<Items<HumanActionType>>> probTab = new List<List<Items<HumanActionType>>>();
        List<List<float>> tab = readProbTable(file);
        for(int i = 0; i < tab.Count;i++)
        {
            List<Items<HumanActionType>> auxRow = new List<Items<HumanActionType>>();
            for(int j = 0; j < tab[i].Count;j++)
            {
                float probValue = tab[i][j]/100;
                auxRow.Add(new  Items<HumanActionType> {Probability = probValue, Item = (HumanActionType)j});
            }
            List<Items<HumanActionType>> converted = new List<Items<HumanActionType>>(auxRow.Count);
            double sum = 0.0;
            foreach (var item in auxRow.Take(auxRow.Count - 1))
            {
                sum += item.Probability;
                converted.Add(new Items<HumanActionType> { Probability = sum, Item = item.Item });
            }
            converted.Add(new Items<HumanActionType> { Probability = 1.0, Item = auxRow.Last().Item });
            probTab.Add(converted);
        }
        return probTab;
    }


    private float getAngle(Transform obj, Vector3 position){
        Vector3 dir = obj.position - position;
        dir = obj.InverseTransformDirection(dir); 
        //float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        //float angle = Vector3.SignedAngle(dir, transform.forward, Vector3.up);
        dir.y = 0;
        float angle = Vector3.Angle(-Vector3.forward,dir);
        return angle;
    }



    private ActionHand getActionAndHand(CommandType commandType)
    {
        ActionHand actionHand = null;
        switch (commandType)
        {
            case CommandType.ActivateLeft:
                actionHand = new ActionHand(Action.Activate, Hands.Left);
                break;
            case CommandType.ActivateRight:
                actionHand = new ActionHand(Action.Activate, Hands.Right);
                break;
            case CommandType.DeactivateLeft:
                actionHand = new ActionHand(Action.Deactivate, Hands.Left);
                break;
            case CommandType.DeactivateRight:
                actionHand = new ActionHand(Action.Deactivate, Hands.Right);
                break;
            case CommandType.LeaveLeft:
                actionHand = new ActionHand(Action.Release, Hands.Left);
                break;
            case CommandType.LeaveRight:
                actionHand = new ActionHand(Action.Release, Hands.Right);
                break;
            case CommandType.LookAt:
                actionHand = new ActionHand(Action.HeadFocus);
                break;
            case CommandType.Speech:
                actionHand = new ActionHand(Action.Speak);
                break;
            case CommandType.LookFor:
                actionHand = new ActionHand(Action.LookFor);
                break;
            case CommandType.Move:
                actionHand = new ActionHand(Action.Move);
                break;
            case CommandType.TakeLeft:
                actionHand = new ActionHand(Action.Take, Hands.Left);
                break;
            case CommandType.TakeRight:
                actionHand = new ActionHand(Action.Take, Hands.Right);
                break;
            case CommandType.Turn:
                actionHand = new ActionHand(Action.Turn);
                break;
            case CommandType.HeadReset:
                actionHand = new ActionHand(Action.HeadReset);
                break;
            case CommandType.Rotate:
                actionHand = new ActionHand(Action.Rotate);
                break;
            case CommandType.TasteLeft:
                actionHand = new ActionHand(Action.Taste, Hands.Left);
                break;
            case CommandType.TasteRight:
                actionHand = new ActionHand(Action.Taste, Hands.Right);
                break;
            case CommandType.SmellLeft:
                actionHand = new ActionHand(Action.Smell, Hands.Left);
                break;
            case CommandType.SmellRight:
                actionHand = new ActionHand(Action.Smell, Hands.Right);
                break;
            case CommandType.CancelCommands:
                actionHand = new ActionHand(Action.Cancel);
                break;
            case CommandType.Animate:
                actionHand = new ActionHand(Action.Animate);
                break;
            default:
                actionHand = null;
                break;
        }

        return actionHand;
    }

    float toFloat(string str)
    {
        return float.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
    }

   

    int toInt(string str)
    {
        int number;
        Int32.TryParse(str, out number);
        return number;
    }

    //Get substring before DELIMITER
    string getNext(string str)
    {
        int pos = str.IndexOf(DELIMITER); //First Delimiter
        if (pos > 0)
        {
            return str.Substring(0, pos);
        }
        return str;
    }
    //Get substring after first DELIMITER
    string getAfter(string str)
    {
        int pos = str.IndexOf(DELIMITER); //First Delimiter
        string subData = str.Substring(pos + DELIMITER.Length);
        return subData;
    }

    private void processCommand(string data)
    {
        string idCommand = getCommandId(data);
        CommandType commandType = getCommandType(data);
        ParameterType parameterType = getParameterType(data);

        if (checkCmdParType(commandType, parameterType))
        {
            ActionHand ah = getActionAndHand(commandType);
            if (ah != null)
            {
                switch (parameterType)
                {
                    case ParameterType.WithId:
                        int id = getId(data);
                        GameObject gO = getGameObjectById(id);
                        runningCommands.Add(scm.sendCommand(idCommand, ah.hand, ah.action, gO.transform));
                        break;
                    case ParameterType.WithPos:
                        Position3 position = getPosition(data);
                        Vector3 auxPosition = new Vector3(position.x, position.y, position.z);
                        runningCommands.Add(scm.sendCommand(idCommand, ah.hand, ah.action, auxPosition));
                        break;
                    case ParameterType.WithString:
                        string str = getText(data).Replace("\"", "");
                        if (ah.isToUseHand())
                        {
                            runningCommands.Add(scm.sendCommand(idCommand,ah.hand, ah.action, str));
                        }
                        else
                        {
                            runningCommands.Add(scm.sendCommand(idCommand, ah.action, str));
                        }                        
                        break;
                    case ParameterType.WithAngle:
                        
                    case ParameterType.WithoutParameter:
                        if (ah.isToUseHand())
                            runningCommands.Add(scm.sendCommand(idCommand, ah.hand, ah.action));
                        else
                            runningCommands.Add(scm.sendCommand(idCommand, ah.action));
                        break;
                    case ParameterType.WithAnimate:
                        string animation = getAnimation(data);
                        string reference = getAnimationReference(data);
                        int time = getTime(data);
                        runningCommands.Add(scm.sendCommand(idCommand, ah.action, animation,reference, time));

                        break;
                    default:
                        {
                            break;
                        }
                }
            }
        }
        else
        {
            Debug.Log("It's not possible execute command!");
        }
    }
    private GameObject getGameObjectById(int id)
    {
        GameObject gO = null;
        if (uid.isValidId(id))
        {
            gO = uid.getGameObjectById(id);
            if (scm.getAllPerceivedElements().Contains(gO))
            {
                return gO;
            }
            else
            {
                return null;
            }
        }
        return null;
    }

   

    string getCommandId(string data)
    {
        string subData = getNext(data); //First Parameter
        return subData;
    }

    CommandType getCommandType(string data)
    {
        string subData = getAfter(data); //After first Parameter
        subData = getNext(subData);
        CommandType cmdType = (CommandType)toInt(subData);
        return cmdType;
        //return subData;
    }

    ParameterType getParameterType(string data)
    {
        string subData = getAfter(data); //After first Parameter
        subData = getAfter(subData); //After second Parameter
        subData = getNext(subData);
        ParameterType parType = (ParameterType)toInt(subData);
        return parType;
    }

    int getId(string data)
    {
        string subData = getAfter(data); ////After first Parameter -> CommandType, ParameterType and Parameter
        subData = getAfter(subData); //After second Parameter -> ParameterType and Parameter
        subData = getAfter(subData); //After third Parameter -> Only Parameters
        subData = getNext(subData); //Third Parameter -> Parameter of command, the ID
        int id = toInt(subData);
        return id;
    }

    float getAngle(string data)
    {
        string subData = getAfter(data); ////After first Parameter -> CommandType, ParameterType and Parameter
        subData = getAfter(subData); //After second Parameter -> ParameterType and Parameter
        subData = getAfter(subData); //After third Parameter -> Only Parameters
        subData = getNext(subData); //Third Parameter -> Parameter of command, the Angle
        float angle = toFloat(subData);
        return angle;
    }

    string getAnimation(string data)
    {
        string subData = getAfter(data); ////After first Parameter -> CommandType, ParameterType and Parameter
        subData = getAfter(subData); //After second Parameter -> ParameterType and Parameter
        subData = getAfter(subData); //After third Parameter -> Only Parameters
        subData = getNext(subData); //Third Parameter -> Parameter of command, the Angle
        string animation = subData;
        return animation;
    }

    string getAnimationReference(string data)
    {
        string subData = getAfter(data); ////After first Parameter -> CommandType, ParameterType and Parameter
        subData = getAfter(subData); //After second Parameter -> ParameterType and Parameter
        subData = getAfter(subData); //After third Parameter -> Only Parameters
        subData = getAfter(subData); //Third Parameter -> Parameter of command
        subData = getNext(subData);
        return subData;
    }

    int getTime(string data)
    {
        string subData = getAfter(data); ////After first Parameter -> CommandType, ParameterType and Parameter
        subData = getAfter(subData); //After second Parameter -> ParameterType and Parameter
        subData = getAfter(subData); //After third Parameter -> Only Parameters
        subData = getAfter(subData); //Third Parameter -> Parameter of command
        subData = getAfter(subData);
        subData = getNext(subData);
        int time = toInt(subData);
        return time;
    }

    string getText(string data)
    {
        string subData = getAfter(data); ////After first Parameter -> CommandType, ParameterType and Parameter
        subData = getAfter(subData); //After second Parameter -> ParameterType and Parameter
        subData = getAfter(subData); //After third Parameter -> Only Parameters
        subData = getNext(subData); //Third Parameter -> Parameter of command, the Text
        return subData;
    }


    Position3 getPosition(string data)
    {
        string subData = getAfter(data); ////After first Parameter -> CommandType, ParameterType and Parameter
        subData = getAfter(subData); //After second Parameter -> ParameterType and Parameter
        subData = getAfter(subData); //After third Parameter -> Only Parameters
        string x = getNext(subData); //Third Parameter -> Parameter of command, the X, Y and Z axis of position
        subData = getAfter(subData); // Y and Z
        string y = getNext(subData); //Fourth Parameter -> Parameter of command, the y axis of position
        subData = getAfter(subData); // Z
        string z = getNext(subData); //Fifth Parameter -> Parameter of command, the Z axis of position

        Position3 position = new Position3();
        position.x = toFloat(x);
        position.y = toFloat(y);
        position.z = toFloat(z);
        return position;
    }

    private bool checkCmdParType(CommandType cmdType, ParameterType parType)
    {
        switch (cmdType)
        {
            case CommandType.ActivateLeft: return parType == ParameterType.WithId || parType == ParameterType.WithString;
            case CommandType.ActivateRight: return parType == ParameterType.WithId || parType == ParameterType.WithString; ;
            case CommandType.DeactivateLeft: return parType == ParameterType.WithId || parType == ParameterType.WithString; ;
            case CommandType.DeactivateRight: return parType == ParameterType.WithId || parType == ParameterType.WithString; ;
            case CommandType.HeadReset: return parType == ParameterType.WithoutParameter;
            case CommandType.LeaveLeft: return parType == ParameterType.WithId || parType == ParameterType.WithPos;
            case CommandType.LeaveRight: return parType == ParameterType.WithId || parType == ParameterType.WithPos;
            case CommandType.LookAt: return parType == ParameterType.WithId || parType == ParameterType.WithPos || parType == ParameterType.WithString;
            case CommandType.LookFor: return parType == ParameterType.WithString;
            case CommandType.Move: return parType == ParameterType.WithId || parType == ParameterType.WithPos || parType == ParameterType.WithString;
            case CommandType.Rotate: return parType == ParameterType.WithAngle;
            case CommandType.SmellLeft: return parType == ParameterType.WithoutParameter;
            case CommandType.SmellRight: return parType == ParameterType.WithoutParameter;
            case CommandType.Speech: return parType == ParameterType.WithString;
            case CommandType.TakeLeft: return parType == ParameterType.WithId || parType == ParameterType.WithString;
            case CommandType.TakeRight: return parType == ParameterType.WithId || parType == ParameterType.WithString;
            case CommandType.TasteLeft: return parType == ParameterType.WithoutParameter;
            case CommandType.TasteRight: return parType == ParameterType.WithoutParameter;
            case CommandType.Turn: return parType == ParameterType.WithId || parType == ParameterType.WithPos || parType == ParameterType.WithString;
            case CommandType.CancelCommands: return parType == ParameterType.WithoutParameter;
            case CommandType.Animate: return parType == ParameterType.WithAnimate;
            default: return false;
        }
    }

    private class ActionHand
    {
        public Action action { get; set; }
        public Hands hand { get; set; }
        private bool useHand;

        public ActionHand(Action action, Hands hand)
        {
            this.action = action;
            this.hand = hand;
            useHand = true;
        }

        public ActionHand(Action action)
        {
            this.action = action;
            this.hand = Hands.Right;
            useHand = false;
        }

        public bool isToUseHand()
        {
            return useHand;
        }
    }

     /*#################Comportamento Humano ao ver o robô#####################
     * 1º Olha em direção ao robô. Definir tempo do olhar
     * 2º Verifica se robô está disponível (não interagindo) com outra pessoa
     * 3º Verifica interesse em interagir (efeito novidade), a principio, probablidade de 0% (assumir desinteresse)
     * 4º Verifica se robô está olhando para humano: chance de olhar de volta de 40%, chance de ir até ao robô de 10%
     * 5º Verifica se robô interage, "wave hand": probabilidade de olhar de 100% (a principio), prob. de ir de 100%
     * 6º Ao se aproximar, verifica de robô interage "hand shake": probabilidade de retribuir de 100%, e 0% caso o humano esteja com algo na mao ou distante
     * 7º Finaliza interação, retorna a tarefa inicial (não envolvendo robô)
     */
    private void interactWithRobot()
    {
        //verificar se robo é visivel, usando Raycast
        //verificar se raycast esta no angulo de visao
        bool isVisible = vision.isRobotVisible();

          
        

        //verificar quem é o foco do robô
        //Verificar se
    }

    public static T CloneList<T>(T item)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, item);
        stream.Seek(0, SeekOrigin.Begin);
        T result = (T)formatter.Deserialize(stream);
        stream.Close();
        return result;
    }
    
    public bool isHumanEngagedWithRobot(){
        return isHumanEngaged;
    }
    
}
