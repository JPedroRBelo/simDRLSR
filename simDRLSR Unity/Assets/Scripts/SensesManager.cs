using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Net.Sockets;
using OntSenseCSharpAPI;
using System;

public class SensesManager : MonoBehaviour {
    
    public string clientName;
    public string host = "localhost";
    public int port = 11000;

    private TcpClient socket;
    private NetworkStream stream;
    //Involuntary informations
    public int sensesRefreshRate = 2;
    //Senses informations of Smell and Taste commands
    public int triggerRefreshRate = 1;

    private Transform robot;
    private SimulatorCommandsManager scm;
    private UniqueIdDistributor uIDD;

    private VisionManager vision;
    private SmellManager smell;
    private HearingManager hearing;
    private TasteManager taste;
    private TouchManager touch;
    private List<GameObject> locationsOnScene;
    private List<GameObject> objectsInFildOfVision;
    private Queue<GameObject> visionQueue;
    private Queue<GameObject> hearKnowQueue;
    private Queue<GameObject> hearUnkQueue;
    private Queue<GameObject> touchUnkQueue;
    private Queue<GameObject> touchKnowQueue;
    private Queue<GameObject> smellUnkQueue;

    private List<GameObject> soundsInHearing;
    private List<GameObject> knowObjInHearing;

    private List<GameObject> odorInSmell;
    private List<GameObject> knowObjInSmell;
    private List<GameObject> objectsInTaste;
    private List<GameObject> touchInTouch;
    private List<GameObject> knowObjInTouch;

    private const string  SEPARATOR = "-----------------------";

    private int count;
    private int batchSize;
    private DateTime atDateTime;

    void Awake()
    {
        
        try
        {
            string ontSenseAPIDir = "OntSenseAPIServer\\OntSenseAPIServer.exe";
            System.Diagnostics.Process.Start(ontSenseAPIDir);
        }catch(Exception e)
        {
            Debug.Log("System>>> " + e.Message);
        }
    }

    // Use this for initialization
    void Start () {
        scm = GetComponent<SimulatorCommandsManager>();
        robot = scm.robot;
        uIDD = GetComponent<UniqueIdDistributor>();
        vision = robot.GetComponent<VisionManager>();
        smell = robot.GetComponent<SmellManager>();
        hearing = robot.GetComponent<HearingManager>();
        taste = robot.GetComponent<TasteManager>();
        touch = robot.GetComponent<TouchManager>();

        locationsOnScene = new List<GameObject>(GameObject.FindGameObjectsWithTag(Constants.TAG_KNOWLOCATIONS));
        objectsInFildOfVision = new List<GameObject>();
        soundsInHearing = new List<GameObject>();
        knowObjInHearing = new List<GameObject>();
        touchInTouch = new List<GameObject>();
        knowObjInTouch = new List<GameObject>();

        odorInSmell = new List<GameObject>();
        knowObjInSmell = new List<GameObject>();
        objectsInTaste = new List<GameObject>();
        count = 0;
        SparqlEndPoint instanceSparql = SparqlEndPoint.getInstance();       // gets the instance for the  singleton object
        instanceSparql.init(host, port);
        visionQueue = new Queue<GameObject>();
        hearKnowQueue = new Queue<GameObject>();
        hearUnkQueue = new Queue<GameObject>();
        touchUnkQueue = new Queue<GameObject>();
        touchKnowQueue = new Queue<GameObject>();
        smellUnkQueue = new Queue<GameObject>();
        atDateTime = DateTime.Now;
    }

    /*void FixedUpdate () {
        
        if (count % sensesRefreshRate == 0)
        {
            //Vision
            objectsInFildOfVision = vision.getListOfElements();
            //Hear
            soundsInHearing = hearing.getListOfElements();
            knowObjInHearing = objectsInFildOfVision.Intersect(soundsInHearing).ToList();
            foreach (GameObject obj in knowObjInHearing)
            {
                soundsInHearing.Remove(obj);
            }
            //Smell
            odorInSmell = smell.getListOfUnknowElements();
            //Touch
            knowObjInTouch = touch.getListOfKnowElements();
            touchInTouch = touch.getListOfUnknowElements();


            insertVision(objectsInFildOfVision);
        }
        if (count % triggerRefreshRate == 0)
        {
            knowObjInSmell = smell.getListOfKnowElements();
            //Taste
            objectsInTaste = taste.getListOfElements();
        }   
        count++;        
    }*/

    void FixedUpdate()
    {
        if (count % sensesRefreshRate == 0)
        {
            //Vision
            objectsInFildOfVision = vision.getListOfElements();
            visionQueue = new Queue<GameObject>(objectsInFildOfVision);

            //Hear
            soundsInHearing = hearing.getListOfElements();
            
            knowObjInHearing = objectsInFildOfVision.Intersect(soundsInHearing).ToList();
            hearKnowQueue = new Queue<GameObject>(knowObjInHearing);
            foreach (GameObject obj in knowObjInHearing)
            {
                soundsInHearing.Remove(obj);
            }
            hearUnkQueue = new Queue<GameObject>(soundsInHearing);
            //Smell
            odorInSmell = smell.getListOfUnknowElements();
            smellUnkQueue = new Queue<GameObject>(odorInSmell);
            //Touch
            knowObjInTouch = touch.getListOfKnowElements();
            touchKnowQueue = new Queue<GameObject>(knowObjInTouch);
            //Unknow objects in touch
            touchInTouch = touch.getListOfUnknowElements();
            touchUnkQueue = new Queue<GameObject>(touchInTouch);
            //objQueue = new Queue<GameObject>(objectsInFildOfVision.Union(soundsInHearing).Union(knowObjInHearing).Union(odorInSmell).Union(knowObjInTouch).Union(touchInTouch)); 
            int totalSize = visionQueue.Count + touchKnowQueue.Count + touchUnkQueue.Count + hearUnkQueue.Count + 
            hearKnowQueue.Count + smellUnkQueue.Count; 
            batchSize = (int)Math.Ceiling((float)totalSize/sensesRefreshRate);
            //insertVision(objectsInFildOfVision);
            atDateTime = DateTime.Now;
        }
        for (int i = 0; i < batchSize; i++)
        {
            if (visionQueue.Count > 0)
            {
                insertVision(visionQueue.Dequeue(), atDateTime);
            }
            else if (hearKnowQueue.Count > 0)
            {
                insertHearID(hearKnowQueue.Dequeue(), atDateTime);
            }
            else if (hearUnkQueue.Count > 0)
            {
                insertHearPos(hearUnkQueue.Dequeue(), atDateTime);
            }
            else if (touchKnowQueue.Count > 0)
            {
                insertTouchID(touchKnowQueue.Dequeue(), atDateTime);
            }
            else if (touchUnkQueue.Count > 0)
            {
                insertTouchPos(touchUnkQueue.Dequeue(), atDateTime);
            }else if (smellUnkQueue.Count > 0)
            {
                insertSmellPos(smellUnkQueue.Dequeue(), atDateTime);
            }
        }
        if (count % triggerRefreshRate == 0)
        {
            knowObjInSmell = smell.getListOfKnowElements();
            foreach (GameObject goSmell in knowObjInSmell)
            {
                insertSmellID(goSmell,DateTime.Now);
            }
            //Taste
            objectsInTaste = taste.getListOfElements();
            foreach (GameObject goTaste in objectsInTaste)
            {
                insertTaste(goTaste, DateTime.Now);
            }                       
        }
        count++;
    }

    private void insertVision(GameObject go, DateTime dt)
    {
        CartesianPos cPos = new CartesianPos(go.transform.position.x, go.transform.position.y, go.transform.position.z);
        VisionProperties auxVision = go.GetComponent<VisionProperties>();
        Status auxStatus = go.GetComponent<Status>();
        RGBValue rgb;
        OntSenseCSharpAPI.Material material = OntSenseCSharpAPI.Material.unknownMaterial;
        PhysicalState state = PhysicalState.noneState;
        string tag = go.tag;
        string uri = "";
        if (auxStatus != null)
        {
            state = auxStatus.getStatus();
        }
        if (auxVision != null)
        {
            rgb = auxVision.getRGB();
            material = auxVision.getMaterial();
            if (auxVision == vision)
            {
                tag = "SelfRobot";
            }
            uri = auxVision.getURI();
        }
        else
        {
            rgb = new RGBValue(0, 0, 0);
        }
        EmotionalState emotion = EmotionalState.neutralEmotion;
        EmotionStatus auxEmotion = go.GetComponent<EmotionStatus>();
        if (auxEmotion != null)
        {
            emotion = auxEmotion.getEmotion();
        }
        RobotVision rv;
        switch (go.tag)
        {
            case Constants.TAG_ROBOT:
                Robot auxRobot = new Robot(uIDD.getID(go), go.name, tag, rgb, cPos, state, material, uri);
                rv = new RobotVision(dt, auxRobot);
                break;
            case Constants.TAG_HUMAN:
                Human auxHuman = new Human(uIDD.getID(go), go.name, tag, rgb, cPos, state, material, uri, emotion);
                rv = new RobotVision(dt, auxHuman);
                break;
            default:
                Thing auxThing = new Thing(uIDD.getID(go), go.name, tag, rgb, cPos, state, material, uri);
                rv = new RobotVision(dt, auxThing);
                break;
        }
        try                                         // Try to access a resource.
        {
            rv.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }

    }

    private void insertHearID(GameObject go, DateTime dt)
    {
        HearingProperties auxHearingProperties = go.GetComponent<HearingProperties>();
        HearingAttribute soundType = HearingAttribute.unknownSound;
        float volume = 0;
        string desc = "";
        if (auxHearingProperties != null)
        {
            soundType = auxHearingProperties.getSoundType();
            volume = auxHearingProperties.getVolume();
            desc = auxHearingProperties.getSoundDetail();
        }
        RobotHear rh = new RobotHear(
                dt,                                     // the event occurs now
                uIDD.getID(go),                             // object  identifier
                soundType,           // I heard a beautiful music
                volume,                                    // the volume is in the middle
                desc);              // sound detail

        try                                         // Try to access a resource.
        {
            rh.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }
    }

    private void insertHearPos(GameObject go, DateTime dt)
    {
        CartesianPos cPos = new CartesianPos(go.transform.position.x, go.transform.position.y, go.transform.position.z);
        HearingProperties auxHearingProperties = go.GetComponent<HearingProperties>();
        HearingAttribute soundType = HearingAttribute.unknownSound;
        float volume = 0;
        string desc = "";
        if (auxHearingProperties != null)
        {
            soundType = auxHearingProperties.getSoundType();
            volume = auxHearingProperties.getVolume();
            desc = auxHearingProperties.getSoundDetail();
        }
        RobotHear rh = new RobotHear(
                dt,                                     // the event occurs now
                cPos,                             // object  identifier
                soundType,           // I heard a beautiful music
                volume,                                    // the volume is in the middle
                desc);              // sound detail

        try                                         // Try to access a resource.
        {
            rh.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }
    }

    private void insertTouchID(GameObject go, DateTime dt)
    {
        TouchProperties auxTouchProperties = go.GetComponent<TouchProperties>();
        float hardness = 0;
        float moisture = 0;
        float roughness = 0;
        float pressure = 0;
        float temperature = 0;
        if (auxTouchProperties != null)
        {
            hardness = auxTouchProperties.getHardness();
            moisture = auxTouchProperties.getMoistness();
            roughness = auxTouchProperties.getRoughness();
            pressure = auxTouchProperties.getPressure();
            temperature = auxTouchProperties.getTemperature();
        }
        RobotTouch rTouch = new RobotTouch(
            dt,                                         // the event occurs now
            uIDD.getID(go),                             //  object definition
            hardness,                                   // hardness level 
            moisture,                                   // moisture level 
            pressure,                                   // pressure level
            roughness,                                  // roughness level
            temperature);                               // temperature level 
        try                                         // Try to access a resource.
        {
            rTouch.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }
    }

    private void insertTouchPos(GameObject go, DateTime dt)
    {
        CartesianPos cPos = new CartesianPos(go.transform.position.x, go.transform.position.y, go.transform.position.z);
        TouchProperties auxTouchProperties = go.GetComponent<TouchProperties>();
        float hardness = 0;
        float moisture = 0;
        float roughness = 0;
        float pressure = 0;
        float temperature = 0;
        if (auxTouchProperties != null)
        {
            hardness = auxTouchProperties.getHardness();
            moisture = auxTouchProperties.getMoistness();
            roughness = auxTouchProperties.getRoughness();
            pressure = auxTouchProperties.getPressure();
            temperature = auxTouchProperties.getTemperature();
        }
        RobotTouch rTouch = new RobotTouch(
            dt,                                         // the event occurs now
            cPos,                             //  object definition
            hardness,                                   // hardness level 
            moisture,                                   // moisture level 
            pressure,                                   // pressure level
            roughness,                                  // roughness level
            temperature);                               // temperature level 
        try                                         // Try to access a resource.
        {
            rTouch.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }
    }
    private void insertTaste(GameObject go, DateTime dt)
    {
        TasteProperties auxTasteProperties = go.GetComponent<TasteProperties>();
        float bitter = 0;
        float salt = 0;
        float sour = 0;
        float sweet = 0;
        float umami = 0;
        if (auxTasteProperties != null)
        {
            bitter = auxTasteProperties.getBitterness();
            salt = auxTasteProperties.getSaltiness();
            sour = auxTasteProperties.getSourness();
            sweet = auxTasteProperties.getSweetness();
            umami = auxTasteProperties.getUmami();
        }
        RobotTaste rTaste = new RobotTaste(
           dt,                           // the event occurs now
           uIDD.getID(go),                             // object  identifier
           bitter,                                   // bitter level 
           salt,                                   // salt level 
           sour,                                   // sour level 
           sweet,                                   // sweet level 
           umami);                                  // umani level 

        try                                         // Try to access a resource.
        {
            rTaste.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }
    }

    private void insertSmellID(GameObject go, DateTime dt)
    {
        SmellProperties auxSmellProperties = go.GetComponent<SmellProperties>();
        OlfactoryAttribute smellType = OlfactoryAttribute.noSmell;
        if (auxSmellProperties != null)
        {
            smellType = auxSmellProperties.getSmellType();
        }
            RobotSmell rs = new RobotSmell(
            dt,                           // the event occurs now
            uIDD.getID(go),                                   // source position
            smellType);           // it is a putrid odor


        try                                         // Try to access a resource.
        {
            rs.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }
    }

    private void insertSmellPos(GameObject go, DateTime dt)
    {
        CartesianPos cPos = new CartesianPos(go.transform.position.x, go.transform.position.y, go.transform.position.z);
        SmellProperties auxSmellProperties = go.GetComponent<SmellProperties>();
        OlfactoryAttribute smellType = OlfactoryAttribute.noSmell;
        if (auxSmellProperties != null)
        {
            smellType = auxSmellProperties.getSmellType();
        }
        RobotSmell rs = new RobotSmell(
           dt,                           // the event occurs now
           cPos,                                   // source position
           smellType);           // it is a putrid odor


        try                                         // Try to access a resource.
        {
            rs.insert();                       // using dotNetRDF library inserts the information in the triple store
        }
        catch (Exception e)
        {
            Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
        }
    }

    /*
    private void insertVision(List<GameObject> listGO)
    {
        foreach(GameObject go in listGO)
        {
            CartesianPos cPos = new CartesianPos(go.transform.position.x, go.transform.position.y, go.transform.position.z);
            VisionProperties auxVision = go.GetComponent<VisionProperties>();
            Status auxStatus = go.GetComponent<Status>();           
            RGBValue rgb;
            OntSenseCSharpAPI.Material material = OntSenseCSharpAPI.Material.unknownMaterial;
            PhysicalState state = PhysicalState.noneState;
            string tag = go.tag;
            string uri = "";
            if (auxStatus != null)
            {
                state = auxStatus.getStatus();
            }
            if (auxVision != null)
            {
                rgb = auxVision.getRGB();
                material = auxVision.getMaterial();
                if(auxVision == vision)
                {
                   tag = "SelfRobot";
                }
                uri = auxVision.getURI();
            }
            else
            {
                rgb = new RGBValue(0, 0, 0);
            }
            EmotionalState emotion = EmotionalState.neutralEmotion;
            EmotionStatus auxEmotion = go.GetComponent<EmotionStatus>();
            if (auxEmotion != null)
            {
                emotion = auxEmotion.getEmotion();
            }
                
            RobotVision rv;
            switch (go.tag)
            {
                case Constants.TAG_ROBOT:
                    Robot auxRobot = new Robot(uIDD.getID(go), go.name, tag,rgb, cPos, state, material, uri);
                    rv = new RobotVision(DateTime.Now,auxRobot);
                    break;
                case Constants.TAG_HUMAN:
                    Human auxHuman = new Human(uIDD.getID(go), go.name, tag, rgb, cPos, state, material, uri,emotion);
                    rv = new RobotVision(DateTime.Now, auxHuman);
                    break;
                default:
                     Thing auxThing = new Thing(uIDD.getID(go), go.name, tag, rgb, cPos, state, material, uri);
                    rv = new RobotVision(DateTime.Now, auxThing);
                    break;
            }
            try                                         // Try to access a resource.
            {
                rv.insert();                       // using dotNetRDF library inserts the information in the triple store
            }
            catch (Exception e)
            {
                Debug.Log("System>>> " + e.Message);                  // change for your: LogError(e);     // Call a custom error logging procedure.
                throw;                                  // Re-throw the error. It is likely to interrupt the simulator
            }


        }
    }*/

    private bool isHiddenObject(GameObject obj)
    {
        switch (obj.tag)
        {
            case Constants.TAG_BODYPART:
                return true;
            case Constants.TAG_BODYSENSOR:
                return true;
        }
        return false;
    }

    public string getStringVisionInformation()
    {
        string str = "";
        foreach (GameObject obj in objectsInFildOfVision)
        {
            if (!isHiddenObject(obj))
            {
                str += SEPARATOR;
                str += "\nID: " + uIDD.getID(obj);
                str += "\nName: " + obj.name;
                if (obj.GetComponent<VisionManager>() != null && obj.GetComponent<VisionManager>() == vision)
                {
                    str += "\nType: " + "SelfRobot";
                }
                else
                {
                    str += "\nType: " + obj.tag;
                }
                str += "\nPosition: " + obj.transform.position;
                VisionProperties auxVisionProperties = obj.GetComponent<VisionProperties>();
                if (auxVisionProperties != null)
                {
                    str += "\n" + auxVisionProperties.getVisionStatus();
                }
                else
                {
                    str += "\nRGB: 0, 0, 0";
                    str += "\nType: " + OntSenseCSharpAPI.Material.unknownMaterial;
                }
                Status auxStatus = obj.GetComponent<Status>();
                if (auxStatus != null)
                {
                    string aux = auxStatus.getStringStatus();
                    str += "\nStatus: " + aux;
                }
                else
                {
                    //str += "\nSatus: " + ObjectStatus.None;
                }
                EmotionStatus auxEmotionStatus = obj.GetComponent<EmotionStatus>();
                if (auxEmotionStatus != null)
                    str += "\nEmotion: " + auxEmotionStatus.getEmotion().ToString();
                str += "\n";
            }
        }
        return str;
    }

    public string getStringHearInformation()
    {
        string str = "";
        foreach (GameObject obj in soundsInHearing)
        {
            str += SEPARATOR;
            str += "\nPosition: " + obj.transform.position;
            HearingProperties auxHearingProperties = obj.GetComponent<HearingProperties>();
            if (auxHearingProperties != null)
            {
                str += "\n" + auxHearingProperties.getHearingStatus();
            }
            else
            {
                str += "\nSound Type: " + HearingAttribute.unknownSound;
                str += "\nVolume: " + 0.0f;
            }
            str += "\n";
        }
        foreach (GameObject obj in knowObjInHearing)
        {
            str += SEPARATOR;
            str += "\nID: " + uIDD.getID(obj);
            HearingProperties auxHearingProperties = obj.GetComponent<HearingProperties>();
            if (auxHearingProperties != null)
            {
                str += "\n" + auxHearingProperties.getHearingStatus();
            }
            else
            {
                str += "\nSound Type: " + HearingAttribute.unknownSound;
                str += "\nVolume: " + 0.0f;
            }
            str += "\n";
        }
        return str;
    }

    public string getStringTouchInformation()
    {
        string str = "";
        foreach (GameObject obj in touchInTouch)
        {
            str += SEPARATOR;
            str += "\nPosition: " + obj.transform.position;
            TouchProperties auxTouchProperties = obj.GetComponent<TouchProperties>();
            if (auxTouchProperties != null)
            {
                str += "\n" + auxTouchProperties.getTouchStatus();
            }
            else
            {
                str += "\nTemperature: " + Constants.ROOM_TEMPERATURE + " ºC";
                str += "\nPressure: " + 0;
                str += "\nPressure: " + 0;
                str += "\nRoughness: " + 0;
                str += "\nMoistness: " + 0;
                str += "\nHardness: " + 0;
            }
            str += "\n";
        }
        foreach (GameObject obj in knowObjInTouch)
        {
            str += SEPARATOR;
            str += "\nID: " + uIDD.getID(obj);
            TouchProperties auxTouchProperties = obj.GetComponent<TouchProperties>();
            if (auxTouchProperties != null)
            {
                str += "\n" + auxTouchProperties.getTouchStatus();
            }
            else
            {
                str += "\nTemperature: " + Constants.ROOM_TEMPERATURE + " ºC";
                str += "\nPressure: " + 0;
                str += "\nPressure: " + 0;
                str += "\nRoughness: " + 0;
                str += "\nMoistness: " + 0;
                str += "\nHardness: " + 0;
            }
            str += "\n";
        }
        return str;
    }

    public string getStringSmellInformation()
    {
        string str = "";
        foreach (GameObject obj in odorInSmell)
        {
            str += SEPARATOR;
            str += "\nPosition: " + obj.transform.position;
            SmellProperties auxSmellProperties = obj.GetComponent<SmellProperties>();           
            if (auxSmellProperties != null)
            {
                str += "\n" + auxSmellProperties.getSmellStatus();
            }
            else
            {
                str += "Smell Type: "+OlfactoryAttribute.noSmell.ToString();
            }
            str += "\n";
        }
        foreach (GameObject obj in knowObjInSmell)
        {
            str += "\n-----------------------";
            str += "\nID: " + uIDD.getID(obj);
            SmellProperties auxSmellProperties = obj.GetComponent<SmellProperties>();
            if (auxSmellProperties != null)
            {
                str += "\n" + auxSmellProperties.getSmellStatus();
            }
            else
            {
                str += "Smell Type: None";
            }
            str += "\n";
        }
        return str;
    }


    public string getStringTasteInformation()
    {
        string str = "";
        foreach (GameObject obj in objectsInTaste)
        {
            str += SEPARATOR;
            str += "\nID: " + uIDD.getID(obj);
            TasteProperties auxTasteProperties = obj.GetComponent<TasteProperties>();
            if (auxTasteProperties != null)
            {
                str += "\n" + auxTasteProperties.getTasteStatus();
            }
            else
            {
                str += "\nSwetness: " + 0;
                str += "\nSourness: " + 0;
                str += "\nSaltiness: " + 0;
                str += "\nBitterness: " + 0;
                str += "\nUmami: " + 0;
            }
            str += "\n";
        }
        
        return str;
    }
}
