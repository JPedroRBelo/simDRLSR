
   using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;
//using ProtoBuf;
using System.Linq;
using UnityEngine.UI;
using System.Text;

/*public enum CommandType
{
    ActivateLeft, ActivateRight, DeactivateLeft, DeactivateRight,
    HeadReset, LeaveLeft, LeaveRight, LookAt, LookFor, Move,
    Rotate, SmellLeft, SmellRight, Speech, TakeLeft, TakeRight,
    TasteLeft, TasteRight, Turn, CancelCommands
}
*/


public class SocketCommands : MonoBehaviour
{    
    private ServerClient client;
    private List<ServerClient> disconnectList;
    private List<Command> runningCommands;
    //public GameObject messageContainer;
    //public GameObject messagePrefab;

    public int port = 6321;
    private const int MAXDATASIZE =  4096;    // max number of bytes we can send at once
    private const int BACKLOG = 10;          // how many pending connections queue will hold
    private const string DELIMITER = "<|>";
    private TcpListener server;
    private bool serverStarted;
    private SimulatorCommandsManager scm;
    private UniqueIdDistributor uid;
    

/*
    static readonly IDictionary<int, Type> typeLookup = new Dictionary<int, Type>
    {
        {1, typeof(CommandWithId)}, {2, typeof(CommandWithAngle)}, {3, typeof(CommandWithoutPar)},
        { 4, typeof(CommandWithPosition)}, {5, typeof(CommandWithString)}, {6, typeof(Response)}
    };*/

    private void Start()
    {
        scm = transform.GetComponent<SimulatorCommandsManager>();
        uid = transform.GetComponent<UniqueIdDistributor>();
        disconnectList = new List<ServerClient>();
        runningCommands = new List<Command>();
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            startListening();
            serverStarted = true;
            Debug.Log("System>>> Server has been started on port " + port.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("System>>> Socket Error:" + e.Message);
        }
    }



    private void Update()
    {
        if (!serverStarted)
            return;
        if (client != null)
        {
            //Is the client still connected?
            if (!isConnected(client.tcp))
            {
                client.tcp.Close();
                disconnectList.Add(client);
            }
            else
            {
                NetworkStream s = client.tcp.GetStream();
                if (s.DataAvailable)
                {
                    /*
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                    System.String recv = new System.String(chars);
                    //StreamReader reader = new StreamReader(s, true);
                    BinaryReader reader = new BinaryReader(s);
                     string data = reader.ReadString();
                     print("Tamanho> "+data.Length);*/

                    byte[] myReadBuffer = new byte[1024];
                    StringBuilder myCompleteMessage = new StringBuilder();
                    int numberOfBytesRead = 0;

                    // Incoming message may be larger than the buffer size.
                    do
                    {
                        numberOfBytesRead = s.Read(myReadBuffer, 0, myReadBuffer.Length);

                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                    }
                    while (s.DataAvailable);
                    string data = myCompleteMessage.ToString(); 
                    if (data != null && data != "")
                    {
                        print(">" + data.ToString() + "<");
                        onIncomingData(client, data);
                    }
                }
                List<Command> auxList = new List<Command>(runningCommands);
                foreach(Command c in auxList)
                {
                    Response response;
                    switch (c.getCommandStatus())
                    {
                        case CommandStatus.Success:
                            response= new Response { idCommand = c.getId(), executed = true };
                            sendResponse(response);
                            runningCommands.Remove(c);
                            break;
                        case CommandStatus.Fail:
                             response = new Response { idCommand = c.getId(), executed = false };
                            runningCommands.Remove(c);
                            sendResponse(response);
                            break;
                        default:
                            break;
                    }
                }
                
            }
            //Check for message from the client


            for (int i = 0; i < disconnectList.Count - 1; i++)
            {

                disconnectList.RemoveAt(i);
            }
        }
    }

    private bool isConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }

    }

    private void startListening()
    {
        server.BeginAcceptTcpClient(acceptTcpClient, server);
    }

    private void acceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        client = new ServerClient(listener.EndAcceptTcpClient(ar));
        startListening();

        // Send a message to everyone, say somene has connected
        Debug.Log("System>>> Client connected.");
        //broadcast(clients[clients.Count-1].clientName + " has connected",clients);

        //broadcast("%NAME",new List<ServerClient>() { clients[clients.Count - 1] });
    }

    private void onIncomingData(ServerClient c, string data)
    {
        /*if (data.Contains("&NAME"))
        {
            c.clientName = data.Split('|')[1];
            broadcast(c.clientName + " has connected!",clients);
            return;
        }*/
        Debug.Log("RHS>>> socket incoming data.");
        processCommand(data);
        //sendResponse();
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
                        string str = getText(data);
                        runningCommands.Add(scm.sendCommand(idCommand, ah.action, str));
                        break;
                    case ParameterType.WithAngle:
                        float angle = getAngle(data);
                        runningCommands.Add(scm.sendCommand(idCommand, ah.action, angle));

                        break;
                    case ParameterType.WithoutParameter:
                        if (ah.isToUseHand())
                            runningCommands.Add(scm.sendCommand(idCommand, ah.hand, ah.action));
                        else
                            runningCommands.Add(scm.sendCommand(idCommand, ah.action));
                        break;
                    default:
                        {
                            break;
                        }
                }
            }
            else
            {
                Response response = new Response { idCommand = idCommand, executed = false };
                sendResponse(response);
            }

        }
    }
    private GameObject getGameObjectById(int id)
    {
        GameObject gO = null;
        if (uid.isValidId(id)){
            gO = uid.getGameObjectById(id);
            if (scm.getAllPerceivedElements().Contains(gO))
            {
                return gO;
            }else
            {
                return null;
            }
        }
        return null;
    }

    private void sendResponse(Response data)
    {
        try
        {
            //Type type = data.GetType();
            //int field = typeLookup.Single(pair => pair.Value == type).Key;
         //Serializer.NonGeneric.SerializeWithLengthPrefix(client.tcp.GetStream(), data, PrefixStyle.Base128, field);
            string stringData = data.idCommand + DELIMITER + Convert.ToInt32(data.executed);
            StreamWriter writer = new StreamWriter(client.tcp.GetStream());
            writer.WriteLine(stringData);
            writer.Flush();
            if(data.executed)
                Debug.Log("RHS>>>  response sended to cliente: " + data.idCommand + " command finalized with success.");
            else
                Debug.Log("RHS>>>  response sended to cliente: " + data.idCommand + " command finalized with fail.");
        }
        catch (Exception e)
        {
            Debug.Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
        }
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
                actionHand = new ActionHand(Action.Release);
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
                actionHand = new ActionHand(Action.Take,Hands.Left);
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
                actionHand = new ActionHand(Action.Taste,Hands.Left);
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

    /*
        .=======================================.===================.===================.=================.
        |               CommandID               |   CommandType     |   ParameterType   |     Params      |
        :=================================================================================================:
        | 1E009820-008C-2E00-756C-E0CB4EE729FE  |   ActivateLeft    |       WithId      |   123345654654  |
        |---------------------------------------|-------------------|-------------------|-----------------|
        | 1E009820-008C-2E00-756C-E0CB4EE729FE  |       Move        |       WithId      |   123345654654  |
        |---------------------------------------|-------------------|-------------------|-----.-----.-----|
        | 1E009820-008C-2E00-756C-E0CB4EE729FE  |       Move        |       WithPos     | 3.4 | 5.6 |-5.3 |
        |---------------------------------------|-------------------|-------------------|-----'-----'-----|
        | 1E009820-008C-2E00-756C-E0CB4EE729FE  |      Rotate       |      WithAngle    |      -50.5      |
        |---------------------------------------|-------------------|-------------------|-----------------|
        | 1E009820-008C-2E00-756C-E0CB4EE729FE  |      Speech       |      WithString   |"Example of par."|
        |---------------------------------------|-------------------|-------------------|-----------------|
        | 1E009820-008C-2E00-756C-E0CB4EE729FE  |  CancelCommands   |  WithoutParameter |                 |
        '---------------------------------------'-------------------'-------------------'-----------------'



    */

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
            case CommandType.ActivateLeft: return parType == ParameterType.WithId;
            case CommandType.ActivateRight: return parType == ParameterType.WithId; ;
            case CommandType.DeactivateLeft: return parType == ParameterType.WithId; ;
            case CommandType.DeactivateRight: return parType == ParameterType.WithId; ;
            case CommandType.HeadReset: return parType == ParameterType.WithoutParameter;
            case CommandType.LeaveLeft: return parType == ParameterType.WithId || parType == ParameterType.WithPos;
            case CommandType.LeaveRight: return parType == ParameterType.WithId || parType == ParameterType.WithPos;
            case CommandType.LookAt: return parType == ParameterType.WithId || parType == ParameterType.WithPos;
            case CommandType.LookFor: return parType == ParameterType.WithString;
            case CommandType.Move: return parType == ParameterType.WithId || parType == ParameterType.WithPos;
            case CommandType.Rotate: return parType == ParameterType.WithAngle;
            case CommandType.SmellLeft: return parType == ParameterType.WithoutParameter;
            case CommandType.SmellRight: return parType == ParameterType.WithoutParameter;
            case CommandType.Speech: return parType == ParameterType.WithString;
            case CommandType.TakeLeft: return parType == ParameterType.WithId;
            case CommandType.TakeRight: return parType == ParameterType.WithId;
            case CommandType.TasteLeft: return parType == ParameterType.WithoutParameter;
            case CommandType.TasteRight: return parType == ParameterType.WithoutParameter;
            case CommandType.Turn: return parType == ParameterType.WithId || parType == ParameterType.WithPos;
            case CommandType.CancelCommands: return parType == ParameterType.WithoutParameter;
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

}

class Position3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public Position3()
    {

    }

    public Position3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;        
    }


    public override string ToString()
    {
        return "Position: " + x + ", " + y + ", " + z;
    }
}

[Serializable]
public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Driver";
        tcp = clientSocket;
    }
}

class Response
{
    public string idCommand { get; set; }
    public bool executed { get; set; }

    public override string ToString()
    {
        string auxString = "success";
        if (!executed)
        {
            auxString = "fail";
        }
        return "Command Id: " + idCommand + " " + auxString + "!";
    }
}



