using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;
using System.Globalization;




public class SocketCommunication : MonoBehaviour
{

    private int port = 12375;
    private string ip = "127.0.0.1";

    public GameObject robot;
    private RLAgent agent;

    public bool sendImages = false;

    
    private Queue<byte[]> imagesQueue = new Queue<byte[]> ();
    private Queue<byte[]> sizesQueue = new Queue<byte[]> ();
    private Queue<bool> facesQueue = new Queue<bool>();
    private Queue<byte[]> last_imagesQueue = new Queue<byte[]> ();
    private Queue<byte[]> last_sizesQueue = new Queue<byte[]> ();
    private TcpServerClient client;
    private List<TcpServerClient> disconnectList;

    private List<byte> lastImage;

    private const int MAXDATASIZE =  4096;    // max number of bytes we can send at once
    private const int BACKLOG = 10;          // how many pending connections queue will hold
    private TcpListener server;
    private bool serverStarted;
    public bool printLog = false;
    private int stepAt;
    private bool waitingImages = false;
    private GameObject simManager;

    private bool waitingImageSize;
    private bool waitingImageFile;
    private bool waitingLastImage;
    private bool waitingFaceState;

    private bool last_waitingImageSize;
    private bool last_waitingImageFile;

    private TimeManagerKeyboard timeManager;
    // Start is called before the first frame update
    void Start()
    {
        lastImage = new List<byte>();
        waitingImageSize = false;
        waitingImageFile = false;
        waitingLastImage = false;
        waitingFaceState = false;
        //facesQueue = new Queue<bool>();
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

                                print("speed "+timeSpeed);
                                sendDataClient("0");
                            }catch{
                                sendDataClient("1");
                                print("Data error: "+data.ToString());   
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
                        }else if(data.ToString().Contains("reward")){                            
                            string data_string = data.Replace("reward","");
                            string data_aux = data_string.Replace(" ","");
                            try{
                                string[] sub_string = data_aux.Split(':');
                                var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                                ci.NumberFormat.NumberDecimalSeparator = ".";
                                float reward =  float.Parse(sub_string[1],ci);
                                agent.configReward(sub_string[0],reward);
                                sendDataClient("0");
                            
                            }catch{
                                sendDataClient("1");
                                print("Data error: "+data_string);   
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
                        }else if(data.ToString().Contains("use_depth")){
                            
                            string data_string = data.Replace("use_depth","");
                            data_string = data_string.Replace(" ","");
                            try{
                                bool use_depth = false;
                                if(data_string.Equals("True")||data_string.Equals("true")){
                                    use_depth = true;
                                }
                                agent.setUseDepth(use_depth);
                                sendDataClient("0");
                            }catch{
                                sendDataClient("1");
                                print("Data error: fov");   
                            } 
                        }else if(data.ToString().Equals("get_screen")){

                            waitingImageSize = true;
                            waitingImageFile = false;
                            waitingFaceState = false;
                            imagesQueue= new Queue<byte[]>();
                            facesQueue = new Queue<bool>();
                            sizesQueue = new Queue<byte[]>();
                            sendDataClient("0");
                            agent.CaptureStates();                    

                            
                            //sendImageClient(lastState);
                        }else if(data.ToString().Equals("reset")){
                            sendDataClient("0");

                            restartSimulation("Library");
                        }else if(data.ToString().Equals("next_size")){

                            waitingImageSize = true;
                        }else if(data.ToString().Equals("last_image")){

                            waitingLastImage = true;

                        }else if(data.ToString().Equals("next_image")){
                             waitingImageFile = true;

                        }else if(data.ToString().Equals("next_face")){
                             waitingFaceState = true;

                        }
                        else if(data.ToString().Equals("stop")){
                            sendDataClient("0");
                            pauseSimulation();
                            restartSimulation("Empty");
                        }else{
                            stepAt = agent.sendData(data);
                            sendDataClient("1");
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
        if(waitingImageSize){
            if(sizesQueue.Count>0){
                sendImageSize();
                waitingImageSize = false;
            }
         }
         if (waitingImageFile){
             if(imagesQueue.Count>0){
                 sendImageClient();
                 waitingImageFile = false;
             }
         }
         if(waitingFaceState){
             if(facesQueue.Count>0){
                 sendFaceState();
                 waitingFaceState = false;
             }
         }
         if (waitingLastImage){
             if(lastImage != null){
                 sendLastImage();
                 waitingLastImage = false;
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
            //print("Sending data");
            string stringData = data;
            StreamWriter writer = new StreamWriter(client.tcp.GetStream());
            writer.WriteLine(stringData);
            writer.Flush();     
            //print("Data sended!");      
        }
        catch (Exception e)
        {
            Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
        }
    }

    private void sendLastImage(){
        try
        {
            
            byte[] image = lastImage.ToArray();
            NetworkStream stream = client.tcp.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Flush();
            writer.Write(image, 0, image.Length);   
        }
        catch (Exception e)
        {
            Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
        }
    }


    private void sendFaceState(){

            bool face = facesQueue.Dequeue();
            sendDataClient(face.ToString());
    }
    
    private void sendImageSize(){
        try
        {
            
            byte[] size = sizesQueue.Dequeue();
            NetworkStream stream = client.tcp.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Flush();
            //print("Size: "+System.Text.Encoding.UTF8.GetString(size));
            writer.Write(size, 0, size.Length);
            writer.Flush();    
            //print("Size sended!");      
        }
        catch (Exception e)
        {
            Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
        }
        
    }

    private void sendImageClient(){
        try
        {  
            byte[] image = imagesQueue.Dequeue();
            lastImage = new List<byte>(image);
            NetworkStream stream = client.tcp.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(image, 0, image.Length);
            //writer.Flush();    
            //print("Image sended!");      
        }
        catch (Exception e)
        {
            Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
        } 
    }



 /*   
 public void sendImageClient(List<List<byte[]>>  data)
    {
        if(waitingImages){
            //byte[][][] stringData = data.ToArray();
            //byte[] image =  BitConverter.GetBytes(data);
            print("Sending images");
            
            
            
            foreach (List<byte[]> gray_depth in data)
            {
                foreach (byte[] image in gray_depth)
                {
                    try
                    {
                        NetworkStream stream = client.tcp.GetStream();
                        BinaryWriter writer = new BinaryWriter(stream);
                        // Send the message to the connected TcpServer.
                        int byteSize = 6;

                        byte[] size = Encoding.ASCII.GetBytes(image.Length.ToString());
                        int extraSize = byteSize - size.Length;
                        if(extraSize<0){
                            extraSize = 0;
                        }
                        string aux = "";
                        for(int i = 0; i < extraSize;i++){
                            aux = aux+"0";
                        }
                        size = Encoding.ASCII.GetBytes(aux+image.Length.ToString());
                        writer.Write(size,0,size.Length);
                        writer.Flush();
                        //sendDataClient()
                        //stream.Write(image.Length,0,4);
                        writer.Write(image, 0, image.Length);
                        writer.Flush(); 

                        //int total = SendVarData(client,image);

                        print("Data sended!");  
                    }
                    catch (Exception e)
                    {
                        Log("System>>> Write error: " + e.Message + " to client " + client.clientName);
                    }
                    
                }                
            }
            waitingImages = false;
        }else{
            Debug.Log("Socket Client does not required theses images");
        }
    }
*/
    public void setQueueImages(List<List<byte[]>> images){
        
        foreach (List<byte[]> gray_depth in images)
        {
            foreach(byte[] image in gray_depth){
                int byteSize = 6;

                byte[] size = Encoding.ASCII.GetBytes(image.Length.ToString());
                int extraSize = byteSize - size.Length;
                if(extraSize<0){
                    extraSize = 0;
                }
                string aux = "";
                for(int i = 0; i < extraSize;i++){
                    aux = aux+"0";
                }
                size = Encoding.ASCII.GetBytes(aux+image.Length.ToString());
                sizesQueue.Enqueue(size);
                imagesQueue.Enqueue(image);
            }
        }
    }

    public void SetQueueFaces(List<bool> faceState){
        
        foreach (bool face in faceState)
        {            
            facesQueue.Enqueue(face);
        }
        
    }



    private int SendVarData(Socket s, byte[] data)
    {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
           
            
            sent = s.Send(datasize);

            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            
            return total;
    }
    

    

    private void sendReward(string data)
    {
        try
        {
            string stringData = "reward "+data;
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
