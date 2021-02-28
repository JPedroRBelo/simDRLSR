using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OntSenseCSharpAPI;

public class GodModCanvasManager : MonoBehaviour {
    
    public Transform godMode;
    public Text objectText;
    public Transform tGStatus;
   

    private Camera camera;
    private Toggle toggleFalseStatus;
    private Toggle toggleTrueStatus;
    private GodModeControl gmc;
    private Transform aux;
    private GameObject atGO;
    private GameObject lastGO;
    private PhysicalState status;

    public InputField iFieldPositionX;
    public InputField iFieldPositionY;
    public InputField iFieldPositionZ;


    void Start () {
        if (godMode != null)
            camera = godMode.GetComponent<Camera>();
        else
            Debug.Log("RHS>>> ERROR! No GodMode attached to this script.");
        atGO = null;
        lastGO = null;
        gmc = godMode.GetComponent<GodModeControl>();
        if (tGStatus != null)
        {
            toggleFalseStatus = tGStatus.transform.Find("FalseToggle").GetComponent<Toggle>();
            toggleTrueStatus = tGStatus.transform.Find("TrueToggle").GetComponent<Toggle>();
            tGStatus.gameObject.SetActive(false);

        }
        else
        {
            Debug.Log("RHS>>> ERROR! tGStatus is Null! Insert a Toggle Group in Inspector.");
        }
        status = PhysicalState.noneState;
        toggleFalseStatus.isOn = true;
        

    }
	
	// Update is called once per frame
	void Update () {

        atGO = gmc.getHighlightedGObject();
        if (atGO != null)
        {
            objectText.text = buildPropertiesString(atGO);
            if (!iFieldPositionX.isFocused && !iFieldPositionY.isFocused && !iFieldPositionZ.isFocused)
            {
                Vector3 auxPosition = gmc.getTargetPosition();
                if(auxPosition!=atGO.transform.position)
                    setInputPosition(auxPosition);
            }
            if (atGO != lastGO)
            {
                setInputPosition(gmc.getTargetPosition());
                Status auxStatus = atGO.GetComponent<Status>();
                if (auxStatus != null)
                {
                    tGStatus.gameObject.SetActive(true);
                    status = auxStatus.getStatus();
                    if (status == PhysicalState.offState || status == PhysicalState.closeState)
                    {
                        toggleFalseStatus.isOn = true;
                    }
                    else
                    {
                        toggleTrueStatus.isOn = true;
                    }
                }else
                {
                    tGStatus.gameObject.SetActive(false);
                }
                lastGO = atGO;
            }
            
        }
        else
        {
            objectText.text = "";
        }
        
    }

    private string buildPropertiesString(GameObject gO)
    {
        string sensesProperty = "";
        string especificProperty = "";
        string emotionProperty = "";
        SmellProperties auxSmellProperties = gO.GetComponent<SmellProperties>();
        if (auxSmellProperties != null)
        {
            sensesProperty += auxSmellProperties.getSmellStatus();
        }

        TasteProperties auxTasteProperties = gO.GetComponent<TasteProperties>();
        if (auxTasteProperties != null)
        {
            sensesProperty += "\n"+ auxTasteProperties.getTasteStatus();
        }

        HearingProperties auxHearingProperties = gO.GetComponent<HearingProperties>();
        if (auxHearingProperties != null)
        {
            sensesProperty += "\n" + auxHearingProperties.getHearingStatus();
        }

        TouchProperties auxTouchProperties = gO.GetComponent<TouchProperties>();
        if (auxTouchProperties != null)
        {
            sensesProperty += "\n" + auxTouchProperties.getTouchStatus();
        }

        VisionProperties auxVisionProperties = gO.GetComponent<VisionProperties>();
        if (auxVisionProperties != null)
            sensesProperty += "\n" + auxVisionProperties.getVisionStatus();
        EmotionStatus auxEmotionStatus = gO.GetComponent<EmotionStatus>();
        if (auxEmotionStatus != null)
            emotionProperty = auxEmotionStatus.getEmotion().ToString();
        Status auxStatus = gO.GetComponent<Status>();
        if (auxStatus != null)
        {
            especificProperty = auxStatus.getStringStatus();
        }



        string auxText = "ID: " + gO.GetInstanceID() + "\n" + gO.tag + ": " + gO.name +
             "\nPosition: " + gO.transform.position;
        if (!sensesProperty.Equals("None"))
        {
            auxText += "\n" + sensesProperty;
        }
        if (emotionProperty.Equals(""))
        {
            emotionProperty = "None";
        }
        if (especificProperty.Equals(""))
        {
            especificProperty = "None";
        }
        auxText += "\nStatus: " + especificProperty;
        auxText += "\nEmotion: " + emotionProperty;

        return auxText;
    }

    public void apply()
    {
           if (atGO != null)
            {
                Status auxStatus = atGO.GetComponent<Status>();
            if (auxStatus != null)
            {
                if (toggleTrueStatus.isOn)
                {

                    auxStatus.turnOnOpen();
                }
                else
                {
                    auxStatus.turnOffClose();
                }

                
            }
            Debug.Log("RHS>>> " + this.name + " " + " applied changes in " +atGO.name);
            atGO.transform.position = getInputPosition();
            gmc.highlightMode();
        }
    }

   

    private void setInputPosition(Vector3 position)
    {
        iFieldPositionX.text = position.x.ToString();
        iFieldPositionY.text = position.y.ToString();
        iFieldPositionZ.text = position.z.ToString();
    }

    private Vector3 getInputPosition( )
    {
        if (!iFieldPositionX.Equals("") && !iFieldPositionY.Equals("") && !iFieldPositionZ.Equals(""))
        {
            float x;
            float y;
            float z;
            float.TryParse(iFieldPositionX.text, out x);
            float.TryParse(iFieldPositionY.text, out y);
            float.TryParse(iFieldPositionZ.text, out z);
            return new Vector3(x, y, z);
        }
        return Vector3.zero;
    }

}
