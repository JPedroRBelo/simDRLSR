using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ConfigureSaveImage : MonoBehaviour
{
    // Start is called before the first frame update
    private ImageSynthesis imSynthesis;
    public float timeBeweenCapturures = 1f;
    public int numberOfPictures = 8;
    private float nextUpdate;
    private bool capturing = false;

    private List<ImageToSaveProperties> imgProp;
    private Dictionary<int, bool> stateCaptured;
    private int stepAt;
    private bool isToCapture = false;
    private List<List<byte[]>> lastState;
    private bool save_image_in_disc;
    private SocketCommunication socket;


    private int interator;
    void Start()
    {
        save_image_in_disc = true;
        lastState = new List<List<byte[]>>();
    	float timeSpeed = 1f;
        GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");

        if(simManager != null){
		    timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
            socket = simManager[0].GetComponent<SocketCommunication>();
	    }    
        interator = numberOfPictures;
        nextUpdate = timeBeweenCapturures/timeSpeed;
        imSynthesis = GetComponent<ImageSynthesis>();
        imgProp = new List<ImageToSaveProperties>();
        stateCaptured = new Dictionary<int, bool>();
        stateCaptured[-1] = true;
        stepAt = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.time >= nextUpdate)
        {           
            // Change the next update (current second+1)
            nextUpdate = Time.time + timeBeweenCapturures;
            // Call your fonction
            //UpdateEverySecond();
            CaptureLoop();
        }

    }

    private void CaptureLoop()
    {
        if(isToCapture){
            
            if (interator <= numberOfPictures)
            {   
                if(!imSynthesis.isCapturing())
                {
                    
                    capturing = true;
                    //Vector2 gameViewSize = Handles.GetMainGameViewSize();
                    //imSynthesis.Save(filename, width: (int)gameViewSize.x, height: (int)gameViewSize.y, imSynthesis.filepath);
                    for(int i = 0; i < imgProp.Count;i++)
                    {
                        imgProp[i].filename = imgProp[i].basename+interator.ToString()+".png";
                                        
                    }
                    if(save_image_in_disc){
                        imSynthesis.Save(imgProp);
                    }else{
                        List<byte[]> gray_depth = imSynthesis.GetImage(imgProp);
                        lastState.Add(gray_depth);
                    }
                    
                    interator = interator+1;
                }

            }
            else
            {   
                bool flag = true;
                if(save_image_in_disc){
                    for(int i = 0; i < imgProp.Count;i++)
                    {
                        string filename = Path.Combine(imgProp[i].path,imgProp[i].basename+(numberOfPictures)+".png");
                        //print(filename);
                        flag = (flag && File.Exists(filename));
                        if(!flag){
                            break;
                        }             
                    }
                }else{
                    flag = true;
                    socket.sendImageClient(lastState);
                }                
                if(flag){
                    
                    stateCaptured[stepAt] = true;
                    capturing = false;
                    isToCapture = false;
                }
               
            }
        }
    }

    public void CaptureImages(List<ImageToSaveProperties> imgProp,int step,bool saveInDisc = true)
    {
        save_image_in_disc = saveInDisc;
        lastState = new List<List<byte[]>>();
        interator = 1;
        isToCapture = true;
        stateCaptured[step] = false;
        stepAt = step;
        this.imgProp = imgProp;
    }

    public void CaptureImages()
    {
        lastState = new List<List<byte[]>>();
        isToCapture = true;
        interator = 0;
        this.imgProp = imgProp;
    }

    public bool IsCaptureFinished(int step)
    {
        return (stateCaptured.ContainsKey(step) && stateCaptured[step]);   
    }

    public List<List<byte[]>> GetLastState(){
        return lastState;
    }

    public bool IsCaptureFinished()
    {
        return !capturing;
    }
     

}
