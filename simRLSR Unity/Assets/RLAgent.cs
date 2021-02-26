using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;


    public enum AgentAction
    {
        None = -1,
        DoNothing = 0,
        Wait = 1,
        Look = 2,
        Wave = 3,
        HandShake = 4
    };

    public enum RLStages
    {
        WaitStart,
        GetState,
        SetAction,
        GetReward,
        WaitState,
        SendReward,
        FinishStep
    }

    [RequireComponent(typeof(RobotInteraction))]
    public class RLAgent : MonoBehaviour
    {
        [Header("Rewards")]
        public float failHandshakeReward = -0.1f;
        public float handshakeReward = 50f;
        public float neutralReward = 0f;        
        public  float NULL_REWARD = -Mathf.Infinity;

        [Header("RL Configuration")]
        public int totalSteps = 1000;
        private string workDir = "simMDQN/DataGeneration-Phase/";

        private RobotInteraction hri;
        [Header("Temporary Variables")]
        public bool saveImage = false;


        [Header("Game Objects Setup")]
        public GameObject cameraGameObject;


        private ConfigureSaveImage imageSaver;

        private int stepAt;
        private RLStages rlStage;
        private AgentAction dataAction;
        private bool flagNewActionData;

        private float tempReward;
        private float reward;


        public void setInitStep(int initStep){
            stepAt = initStep;
        }

        void Start()
        {

            

            hri = gameObject.GetComponent<RobotInteraction>();
            hri.handshakeReward = handshakeReward;
            hri.neutralReward = neutralReward;
            hri.failHandshakeReward = failHandshakeReward;
            hri.NULL_REWARD = NULL_REWARD;
            tempReward = NULL_REWARD;
            reward =  NULL_REWARD;
            if (cameraGameObject)
            {
                imageSaver = cameraGameObject.GetComponent<ConfigureSaveImage> ();
            }
            stepAt = 0;
            rlStage = RLStages.WaitStart;
            dataAction = AgentAction.DoNothing;
            flagNewActionData = false;

            GameObject simulatorManager = GameObject.Find("/SimulatorManager");
            if(simulatorManager == null){
                Debug.Log("Simulator Manager not found...");
            }else{
                ConfigureSimulation cs = simulatorManager.GetComponent<ConfigureSimulation>();
                if(cs == null){
                    Debug.Log("Script Configure Simulation not found...");
                }else
                {
                    workDir = cs.getWorkDir();
                    totalSteps = cs.getTotalSteps();
                }
            }
            print("Total Steps: "+totalSteps);
        }

        // Update is called once per frame
        void Update()
        {
            if(stepAt<totalSteps){
                //print(this+" stage: "+rlStage);
                switch (rlStage)
                {
                    case RLStages.WaitStart:
                        tempReward = NULL_REWARD;
                        //reward =  NULL_REWARD;
                        if(flagNewActionData){
                            if(System.Enum.IsDefined(typeof(AgentAction), dataAction))
                            {
                                rlStage = RLStages.GetState;
                            }
                        }
                        break;
                    case RLStages.GetState:
                        GetStates(stepAt);
                        rlStage = RLStages.WaitState;
                        break;
                    case RLStages.WaitState:
                        if(IsStateCaptured(stepAt)){
                            
                            rlStage = RLStages.SetAction;                           
                        }     
                        break;
                    case RLStages.SetAction:
                        SendAction(dataAction,stepAt);
                        flagNewActionData = false;
                        rlStage = RLStages.GetReward;
                        break;
                    case RLStages.GetReward:
                        tempReward = hri.getReward(stepAt);
                        if(tempReward!=NULL_REWARD){
                            rlStage = RLStages.SendReward;
                        }                        
                        break;
                    
                        
                    case RLStages.SendReward:
                        reward = tempReward;
                        rlStage = RLStages.FinishStep;
                        break;                    
                    case RLStages.FinishStep:
                        stepAt++;
                        rlStage = RLStages.WaitStart;
                        break;
                    default:
                        break;

                }
            }
            /*if (saveImage)
            {
                GetImages();
                saveImage = false;
            }
            */

        }

        

        void SendAction(AgentAction action, int step)
        {
            hri.setAction(action,step);
        }

        public float getReward(int step)
        {
            return reward;
        }

        
        public int sendData(string data)
        { 
            if(data.Equals("-")){
                dataAction = AgentAction.DoNothing;
                flagNewActionData = true;                
            }
            else
            {
                try 
                {
                    dataAction = (AgentAction)Convert.ToInt32(data);
                    print("Command received: "+dataAction);
                    flagNewActionData = true;
                }
                catch (FormatException) {
                    Debug.Log("Invalid type");
                    dataAction = AgentAction.DoNothing;
                    flagNewActionData = false;
                }
            }
            reward = NULL_REWARD;
            if(flagNewActionData)
                print(this+" received commmand: "+dataAction);
            return stepAt;
        }            
        
        public bool IsStateCaptured(int step)
        {
            return imageSaver.IsCaptureFinished(step);
        }

         public bool IsStateCaptured()
        {
            return imageSaver.IsCaptureFinished();
        }

        public void GetStates(int step)
        {
            //if(IsStateCaptured()){
                //string dir = Path.Combine(Application.dataPath,("/../../"+workDir) );
                string dir = Path.Combine(Application.dataPath, "..", "..");
                dir = Path.Combine(dir,workDir);
                StreamReader reader = new StreamReader(dir+"files/episode.txt");
                //string ep = reader.ReadToEnd();
                string ep = "";
                while (!reader.EndOfStream){
                    ep = reader.ReadLine();
                    //print("Episode: "+ep);
                }
                reader.Close();

                string save_path1 = Path.Combine(dir, "dataset");
                save_path1 = Path.Combine(save_path1, "RGB");
                save_path1 = Path.Combine(save_path1, "ep"+ep);
                
                string save_path2 = Path.Combine(dir, "dataset");
                save_path2 = Path.Combine(save_path2, "Depth");
                save_path2 = Path.Combine(save_path2, "ep"+ep);

                //string save_path1="dataset/RGB/ep"+ep[2]+"/";
                //string save_path2="dataset/Depth/ep"+ep[2]+"/";
                string img_1 = "image_"+(step+1)+"_";
                string img_2 = "depth_"+(step+1)+"_";
                List<ImageToSaveProperties> imgProp = new List<ImageToSaveProperties>();
                imgProp.Add(new ImageToSaveProperties(img_1,save_path1,width: 320,height:240,ImageType.Grey));
                imgProp.Add(new ImageToSaveProperties(img_2,save_path2,width: 320,height:240,ImageType.Depth));
                imageSaver.CaptureImages(imgProp,step);
            //}
        }


    }

