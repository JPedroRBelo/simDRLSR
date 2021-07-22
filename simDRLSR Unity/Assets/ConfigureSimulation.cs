using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


[XmlRoot]
public class Configure
{   
    [XmlElement]
    public string simulation_quality { get; set; }
    [XmlElement]
    public int fps { get; set; }
    [XmlElement]
    public int width { get; set; }
    [XmlElement]
    public int height { get; set; }
    [XmlElement]
    public bool fullscreen { get; set; }
    [XmlElement]
    public string ip_address { get; set; }
    [XmlElement] 
    public int port { get; set; }
    [XmlElement]
    public string path_prob_folder { get; set; }

    [XmlElement]
    public string path_work_dir  { get; set; }

    [XmlElement]
    public int total_steps { get; set; }
}

public class ConfigureSimulation : MonoBehaviour
{
    
    public string fileName = "config.xml";

    private string dir_fileName;
    
    [Header("Video Settings")]
    public string simulationQuality = "Medium";
    public int fps = 60;
    public int width = 1024;
    public int height = 768;
    public bool fullscreen = false;

    [Header("Socker Communication")]
    public string ipAddress = "172.17.0.3";
    public int port = 12375;

    //[Header("HRI Probabilities")]

    
    public string pathProbFolder = "Config";
    private string xmlPathProbFolder;
    /*
    public string file_prob_engaged = "engaged_hri_probabilities";
    public string file_prob_human_notengd = "human_notengd_hri_probabilities";
    public string file_prob_robot_notengd = "robot_notengd_hri_probabilities";
    */

    [Header("Work Folder")]
    public string pathWorkDir = "simMDQN/DataGeneration-Phase/";

    

    [Header("RL Configuration")]
    public int totalSteps = 2050; 

    public bool saveToXML = false;
    
    private Configure xmlConfigure;

    //seted by Socket communication
    private string general_work_dir;

    // Start is called before the first frame update
    void Start()
    {      
       
        
    }



    void Awake()
    {
        dir_fileName = Path.Combine(Application.dataPath,"..",fileName);


        string auxQuality = simulationQuality;
        int auxFPS = fps;
        int auxWidth = width;
        int auxHeight = height;
        bool auxFullscreen = fullscreen;
        if(!File.Exists(dir_fileName)){
           xmlConfigure =  saveConfig(inspectorConfiguration(),dir_fileName); 
        }else
        {
            xmlConfigure = loadConfig(dir_fileName);
            auxQuality = xmlConfigure.simulation_quality;
            auxFPS = xmlConfigure.fps;
            auxWidth = xmlConfigure.width;
            auxHeight = xmlConfigure.height;
            auxFullscreen = xmlConfigure.fullscreen;
        }
        Screen.SetResolution(auxWidth, auxHeight, auxFullscreen, auxFPS);
        //print(xmlConfigure.path_work_dir);
        string[] names = QualitySettings.names;
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Equals(auxQuality))
            {
                QualitySettings.SetQualityLevel(i, true);
            }
        }
        general_work_dir = xmlConfigure.path_work_dir;
    }

    void Update(){
        if(saveToXML)
        {               
            xmlConfigure = saveConfig(inspectorConfiguration(),dir_fileName); 
            Debug.Log("XML Saved");
            saveToXML = false;
        }
    }

    private Configure inspectorConfiguration()
    {
        return new Configure {fps=fps,width=width,height=height,fullscreen=fullscreen,simulation_quality=simulationQuality, ip_address= ipAddress,port=port,path_prob_folder = pathProbFolder, path_work_dir = pathWorkDir ,total_steps=totalSteps};
    }

    public string getIPAdress(){
        return xmlConfigure.ip_address;
    }

    public int getPort(){
        return xmlConfigure.port;
    }


    private Configure saveConfig(Configure configuration,string file_name)
    {

        XmlSerializer xmls = new XmlSerializer(typeof(Configure));

        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        XmlWriterSettings settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true };
        using (var stream = File.OpenWrite(file_name))
        {
            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                xmls.Serialize(xmlWriter, configuration, ns);
            }
        }
        return configuration;
    }

    private Configure loadConfig(string file_name)
    {   
        XmlSerializer xmls = new XmlSerializer(typeof(Configure));
        Configure config = null;
        using (var stream = File.OpenRead(file_name))
        {
            config = xmls.Deserialize(stream) as Configure;
        }
        return config;
    }

    public string getPathConfig()
    {
        return Path.Combine(Application.dataPath,"..",xmlConfigure.path_prob_folder);
    }

    public string getWorkDir()
    {
        return Path.Combine(general_work_dir);
    }

    public void setWorkDir(string workDir){
        general_work_dir = workDir;
    }

    public int getTotalSteps()
    {
        return xmlConfigure.total_steps;
    }

   
}
