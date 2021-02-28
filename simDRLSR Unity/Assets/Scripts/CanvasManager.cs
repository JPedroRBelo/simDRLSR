using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {

    public Transform sensesPanel;
    public Transform visionComponent;
    public Transform smellComponent;
    public Transform hearingComponent;
    public Transform tasteComponent;
    public Transform touchComponent;
    public Transform speechComponent;
    public  GameObject simulatorManager = null;
    public RectTransform statusPanel = null;
    public Color successColor;
    public Color runningColor;
    public Color failColor;
    private Text textVision;
    private Text textSmell;
    private Text textHearing;
    private Text textTaste;
    private Text textTouch;
    private Text textSpeech;
    private SimulatorCommandsManager scm;
    private SensesManager sm;
    public Camera robotCamera;
    public HearingProperties robotHProp;   

    public Camera[] userCamera;

    private Text commandTextField;
    private Text statusTextField;

    private enum TimeStates {Started, Paused, Stoped };
    private float timeValue = 1f;
    private TimeStates timeStateAt;

    public ToggleGroup groupToggleSmellParticles;
    public ToggleGroup groupToggleSensesPanel;

    public RectTransform timePanel = null;
    private  Button playButton;
    private Button pauseButton;
    private Button stopButton;
    private Slider timeSlider;
    public Text timeText;
    
    private int height;
    private int width;
    
    private VisionManager vision;
    private SmellManager smell;
    private HearingManager hearing;
    private TasteManager taste;
    private TouchManager touch;

    private UniqueIdDistributor uid;

    void Start()
    {        
        scm = simulatorManager.GetComponent<SimulatorCommandsManager>();
        sm = simulatorManager.GetComponent<SensesManager>();
        vision = scm.robot.GetComponent<VisionManager>();
        smell = scm.robot.GetComponent<SmellManager>();
        hearing = scm.robot.GetComponent<HearingManager>();
        taste = scm.robot.GetComponent<TasteManager>();
        touch = scm.robot.GetComponent<TouchManager>();
        robotHProp = scm.robot.GetComponent<HearingProperties>();

        uid = scm.GetComponent<UniqueIdDistributor>();

        height = robotCamera.pixelHeight;
        width = robotCamera.pixelWidth;
        textVision = visionComponent.GetComponent<Text>();
        textSmell = smellComponent.GetComponent<Text>();
        textHearing = hearingComponent.GetComponent<Text>();
        textTaste = tasteComponent.GetComponent<Text>();
        textTouch = touchComponent.GetComponent<Text>();
        textSpeech = speechComponent.GetComponent<Text>();
        commandTextField = statusPanel.Find("CommandText").GetComponent<Text>();
        statusTextField = statusPanel.Find("StatusText").GetComponent<Text>();
        textTaste.text = "";
        textVision.text = "";
        textSmell.text = "";
        textHearing.text = "";
        textTouch.text = "";
        textSpeech.text = "";
        
        if (timePanel != null)
        {
            playButton = timePanel.Find("PlayButton").GetComponent<UnityEngine.UI.Button>();
            pauseButton = timePanel.Find("PauseButton").GetComponent<UnityEngine.UI.Button>();
            stopButton = timePanel.Find("StopButton").GetComponent<UnityEngine.UI.Button>();
            //stopButton.interactable = false;
            pauseSimulation();
        }
        Debug.Log("RHS>>> " + this.name + " is ready.");
    }

    void Update()
    {
        
        //dropObjects.options.Clear();
        textVision.text = sm.getStringVisionInformation();
        textSmell.text = sm.getStringSmellInformation();
        textHearing.text = sm.getStringHearInformation();
        textTaste.text = sm.getStringTasteInformation();
        textTouch.text = sm.getStringTouchInformation();
        textSpeech.text = robotHProp.getSoundDetail();
        Toggle auxSmellParToogle = groupToggleSmellParticles.GetActive();
        sensesPanel.gameObject.active = groupToggleSensesPanel.GetActive();
        if (auxSmellParToogle != null && auxSmellParToogle.gameObject.name.Equals(Constants.TGGL_SMLLPON))
        {
            activateSmellParticles(true);
        }
        else
        {
            activateSmellParticles(false);
        }

        if (scm != null)
        {
            commandTextField.text = scm.getAtCommandName();
            CommandStatus commandStatus = scm.getCurrentCommandStatus();
            statusTextField.text = commandStatus.ToString();
            switch (commandStatus)
            {
                case CommandStatus.Success:
                    statusPanel.GetComponent<Image>().color = successColor;
                    break;
                case CommandStatus.Running:
                    statusPanel.GetComponent<Image>().color = runningColor;
                    break;
                case CommandStatus.Fail:
                    statusPanel.GetComponent<Image>().color = failColor;
                    break;
                default:
                    break;
            }
        }

    }



    /*private string buildOutputString(HashSet<GameObject> elements,System.Type type)
    {
        string auxText = "";
        foreach (GameObject item in elements)
        {           
            string sensesProperty = "None";
            string especificProperty = "None";
            string emotionProperty = "None";
            if (type == typeof(SmellProperties))
            {
                SmellProperties auxSmellProperties = item.GetComponent<SmellProperties>();
                if (auxSmellProperties != null)
                {
                    sensesProperty = auxSmellProperties.getSmellStatus();
                }
            }
            else
            if (type == typeof(TasteProperties))
            {
                TasteProperties auxTasteProperties = item.GetComponent<TasteProperties>();
                if (auxTasteProperties != null)
                {
                    sensesProperty = auxTasteProperties.getTasteStatus();
                }
            }
            else
            if (type == typeof(HearingProperties))
            {
                HearingProperties auxHearingProperties = item.GetComponent<HearingProperties>();
                if (auxHearingProperties != null)
                {
                    sensesProperty = auxHearingProperties.getHearingStatus();
                }
            }
            else
            if (type == typeof(TouchProperties))
            {
                TouchProperties auxTouchProperties = item.GetComponent<TouchProperties>();
                if (auxTouchProperties != null)
                {
                    sensesProperty = auxTouchProperties.getTouchStatus();
                }
            }else
            if (type == typeof(VisionProperties))
            {
                VisionProperties auxVisionProperties = item.GetComponent<VisionProperties>();
                if (auxVisionProperties != null)
                    sensesProperty = auxVisionProperties.getVisionStatus();
                EmotionStatus auxEmotionStatus = item.GetComponent<EmotionStatus>();
                if (auxEmotionStatus != null)
                    emotionProperty = auxEmotionStatus.getEmotion().ToString();
                
            }
                Status auxStatus = item.GetComponent<Status>();
                if (auxStatus != null)
                {
                    especificProperty = auxStatus.getStringStatus();
                }

            auxText = auxText + "\n---------\nID: "+item.GetInstanceID()+"\n" + item.tag + ": " + item.name +
                "\nPosition: " + item.transform.position;
            if (!sensesProperty.Equals("None"))
            {
                auxText += "\n" + sensesProperty;
            }
            if (!emotionProperty.Equals("None"))
            {
                auxText += "\nEmotion: " + emotionProperty;
            }
            if (!especificProperty.Equals("None"))
            {
                auxText+= "\nStatus: " + especificProperty;
            }
        }
        return auxText + "\n\n";       
    }*/

    private void activateSmellParticles(bool activate)
    {
        foreach (Camera cam in userCamera)
        {
            CameraCullingMask auxCameraManager = cam.GetComponent<CameraCullingMask>();
            if (auxCameraManager != null)
            {
                if (activate)
                {
                    auxCameraManager.turnOnCullingMask(Constants.LAYER_SMELLPART);
                }
                else
                {
                    auxCameraManager.turnOffCullingMask(Constants.LAYER_SMELLPART);
                }
            }
        }
    }

    public void playSimulation()
    {
        playButton.interactable = false;
        pauseButton.interactable = true;
        stopButton.interactable = true;
        timeStateAt = TimeStates.Started;
        setTime(timeValue);
        Debug.Log("RHS>>> simulation started.");
    }


    public void stopSimulation()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        pauseSimulation();
        Debug.Log("RHS>>> simulation stopped.");
    }

    public void pauseSimulation()
    {
        playButton.interactable = true;
        pauseButton.interactable = false;
        timeStateAt = TimeStates.Paused;
        Time.timeScale = 0;
        Debug.Log("RHS>>> simulation paused.");
    }

    public void setTime(float timeValue)
    {
        timeText.text = timeValue.ToString();
        this.timeValue = timeValue;
        if (timeStateAt == TimeStates.Started)
        {
            Time.timeScale = timeValue;
            Debug.Log("RHS>>> simulation speed changed: "+timeValue);
        }
    }

}
