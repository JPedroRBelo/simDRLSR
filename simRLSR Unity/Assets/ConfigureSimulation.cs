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

    public string file_name = "config.xml";

    public string simulation_quality = "Medium";

    [Header("Socker Communication")]
    public string ip_address = "172.17.0.3";
    public int port = 12375;

    [Header("HRI Probabilities")]

    
    public string path_prob_folder = "Config";
    private string xml_path_prob_folder;
    /*
    public string file_prob_engaged = "engaged_hri_probabilities";
    public string file_prob_human_notengd = "human_notengd_hri_probabilities";
    public string file_prob_robot_notengd = "robot_notengd_hri_probabilities";
    */

    [Header("Work Folder")]
    public string path_work_dir = "simMDQN/DataGeneration-Phase/";

    [Header("RL Configuration")]
    public int total_steps = 2001; 

    public bool saveToXML = false;
    
    private Configure xmlConfigure;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }



    void Awake()
    {
        string aux_quality = simulation_quality;
        if(!File.Exists(file_name)){
           xmlConfigure =  saveConfig(inspectorConfiguration(),file_name); 
        }else
        {
            xmlConfigure = loadConfig(file_name);
            aux_quality = xmlConfigure.simulation_quality;
        }
        //print(xmlConfigure.path_work_dir);
        string[] names = QualitySettings.names;
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Equals(aux_quality))
            {
                QualitySettings.SetQualityLevel(i, true);
            }
        }
    }

    void Update(){
        if(saveToXML)
        {               
            xmlConfigure = saveConfig(inspectorConfiguration(),file_name); 
            Debug.Log("XML Saved");
            saveToXML = false;
        }
    }

    private Configure inspectorConfiguration()
    {
        return new Configure {simulation_quality=simulation_quality, ip_address= ip_address,port=port,path_prob_folder = path_prob_folder, path_work_dir = path_work_dir ,total_steps=total_steps};
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
        return xmlConfigure.path_prob_folder;
    }

    public string getWorkDir()
    {
        return xmlConfigure.path_work_dir;
    }

    public int getTotalSteps()
    {
        return xmlConfigure.total_steps;
    }

   
}
