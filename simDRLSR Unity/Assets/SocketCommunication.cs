using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;




public class SocketCommunication : MonoBehaviour
{

    private int port = 12375;
    private string ip = "127.0.0.1";

    public GameObject robot;
    private RLAgent agent;


    

    private TcpServerClient client;
    private List<TcpServerClient> disconnectList;

    private const int MAXDATASIZE =  4096;    // max number of bytes we can send at once
    private const int BACKLOG = 10;          // how many pending connections queue will hold
    private TcpListener server;
    private bool serverStarted;
    public bool printLog = false;
    private int stepAt;

    private GameObject simManager;

    private TimeManagerKeyboard timeManager;
    // Start is called before the first frame update
    void Start()
    {
        GameObject simulatorManager = GameObject.Find("/SimulatorManager");
        if(simulatorManager == null){
            Debug.Log("Simulator Manager not found...");
        }else{
            ConfigureSimulation cs = simulatorManager.GetComponent<ConfigureSimulation>();
            if(cs == null){
                Debug.Log("Script Configure Simulation not found...");
            }else
            {
                ip = cs.getIPAdress();
                port = cs.getPort();
            }
            simManager = GameObject.FindGameObjectsWithTag("SimulatorManager")[0];
            if(simManager != null){
                timeManager = simManager.GetComponent<TimeManagerKeyboard>();
            }

            

        }


        if(robot!=null){
            agent = robot.GetComponent<RLAgent>();
        }
        stepAt = -1;

        disconnectList = new List<TcpServerClient>();
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            startListening();
            serverStarted = true;
            Log("System>>> Server has been started on port " + port.ToString());
        }
        catch (Exception e)
        {
            Log("System>>> Socket Error:" + e.Message);
        }
    }

    private void Log(string text)
    {
        if(printLog)
        {
            Debug.Log(text);
        }

    }

    // Update is called once per frame
    void Update()
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
                        if(data.ToString().Equals("pause")){
                            sendDataClient("0");
                            pauseSimulation();
                        }else if(data.ToString().Equals("restart")){
                            sendDataClient("0");
                            restartSimulation("Library");
                        }else if(data.ToString().Equals("play")){
                            sendDataClient("0");
                            playSimuation();
                        }else if(data.ToString().Contains("step")){
                            
                            string data_string = data.Replace("step","");
                            int initStep = 0;
                            try{
                                initStep = Int32.Parse(data_string);
                                agent.setInitStep(initStep);
                                print("Init Step: "+initStep);
                                sendDataClient("0");
                            }catch{
                                sendDataClient("1");
                                print("Data error: step");
                            }
                        }else if(data.ToString().Contains("episode")){
                            
                            string data_string = data.Replace("episode","");
                            data_string = data_string.Replace(" ","");
                            try{
                                string episode = data_string;
                                agent.setEpisode(episode);
                                print("Episode folder: "+episode);
                                sendDataClient("0");
                            }catch{
                                sendDataClient("1");
                                print("Data error: episode");   
                            }                                                       
                        }else if(data.ToString().Contains("speed")){
                            
                            string data_string = data.Replace("speed","");
                            data_string = data_string.Replace(" ","");
                            try{
                                float timeSpeed =  float.Parse(data_string);
                                GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
                                if(simManager != null){
                                    simManager[0].GetComponent<TimeManagerKeyboard>().setTime(timeSpeed);
                                }   

                                print("Time x"+timeSpeed);
                                sendDataClient("0");
                            }catch{
                                sendDataClient("1");
                                print("Data error: time");   
                            }                                                       
                         }else if(data.ToString().Contains("fov")){
                            
                            string data_string = data.Replace("fov","");
                            data_string = data_string.Replace(" ","");
                            try{
                                float fov =  float.Parse(data_string);
                                agent.setFov(fov);
                                print("Robot camera Fov: "+fov);
                                sendDataClient("0");
                            }catch{
                                sendDataClient("1");
                                print("Data error: fov");   
                            }                                                       
                            
                                
                            
                        }else if(data.ToString().Contains("workdir")){
                            
                            string data_string = data.Replace("workdir","");
                            try{
                                string workDir = data_string;
                                agent.setWorkDir(workDir);
                                print("Workdir folder: "+workDir);
                                sendDataClient("0");
                            }catch{
                                sendDataClient("1");
                                print("Data error: episode");   
                            }                                                       
                            
                        }else if(data.ToString().Equals("start")){
                            sendDataClient("0");
                            restartSimulation("Library");
                        }else if(data.ToString().Equals("stop")){
                            sendDataClient("0");
                            pauseSimulation();
                            restartSimulation("Empty");
                        }else{
                            stepAt = agent.sendData(data);
                        }
                        
                    }
                }

                if(stepAt != -1)
                {
                    float reward = agent.getReward(stepAt);
                    if(reward != agent.NULL_REWARD)
                    {
                        sendReward(reward.ToString());
                        //print(this+"Reward sended at step "+stepAt);
                        stepAt = -1;                                            
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

 
    private bool pauseSimulation()
    {
        timeManager.pauseSimulation();
        return true;        
    }

    private bool restartSimulation()
    {
        //Time.timeScale = 1;
        timeManager.playSimulation();
        OnApplicationQuit();
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        return true;        
    }

    private bool restartSimulation(string scene)
    {
        timeManager.playSimulation();
        OnApplicationQuit();
        SceneManager.LoadScene(scene);
        return true;        
    }

    private bool playSimuation()
    {
        timeManager.playSimulation();
        return true;        
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

        client = new TcpServerClient(listener.EndAcceptTcpClient(ar));
        startListening();

        // Send a message to everyone, say somene has connected
        Log("System>>> Client connected.");
        //broadcast(clients[clients.Count-1].clientName + " has connected",clients);

        //broadcast("%NAME",new List<ServerClient>() { clients[clients.Count - 1] });
    }

    private void sendDataClient(string data)
    {
        try
        {
            print("Sending data");
            string stringData = data;
            StreamWriter writer = new StreamWriter(client.tcp.GetStream());
            writer.WriteLine(stringData);
            writer.Flush();     
            print("Data sended!");      
        }
        catch (Exception e)
        {
            Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
        }
    }

    

    private void sendReward(string data)
    {
        try
        {
            print("Sending reward");
            string stringData = data;
            StreamWriter writer = new StreamWriter(client.tcp.GetStream());
            writer.WriteLine(stringData);
            writer.Flush();     
            //print("Reward sended!");      
        }
        catch (Exception e)
        {
            Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            client.Close();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        // You must close the tcp listener
        try
        {
            server.Stop();
            serverStarted = false;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

}


[Serializable]
public class TcpServerClient
{
    public TcpClient tcp;
    public string clientName;

    public TcpServerClient(TcpClient clientSocket)
    {
        clientName = "Driver";
        tcp = clientSocket;
    }

    public bool Close()
    {
        tcp.GetStream().Close();
        tcp.Close();
        return true;
    }
}
