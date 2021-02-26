using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OntSenseCSharpAPI;

public class UserCanvasManager : MonoBehaviour {

    
    public Transform avatar;


    private AvatarManager aM;
    private AgentSpeech speech;
    
    public Dropdown dropdownActions;
    public Dropdown dropdownElements;
    public InputField inputFieldLookFor;
    public ToggleGroup groupToggleHand;
    public Dropdown dropdownEmotions;

    private List<Action> listActions = new List<Action>();
    //Objects e Locations pertencem ao conjunto Elements
    private List<GameObject> listObjects;
    private List<GameObject> listLocations = null;
    private List<GameObject> listSwitchs;
    //private List<GameObject> listDoors;
    private bool dropdownSelected;

    private EmotionStatus emotionStatus;

    // Use this for initialization
    void Start()
    {
        if (avatar != null) {
            aM = avatar.GetComponent<AvatarManager>();
            speech = avatar.GetComponent<AgentSpeech>();
            dropdownSelected = false;
            listObjects = new List<GameObject>();
            listSwitchs = new List<GameObject>();                
            setItensInActionDropdown(aM.getAvailableActions());
            setItensInEmotionsDropdown();
            emotionStatus = avatar.GetComponent<EmotionStatus>();
            if (emotionStatus != null)
            {
                dropdownEmotions.value = (int)emotionStatus.getEmotion();
            }else
            {
                Debug.Log("RHS>>> Avatar " + avatar.name+" doesn't have Emotions");
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                GameObject aux = EventSystem.current.currentSelectedGameObject;
                if (aux != null)
                {
                    //print(aux.name);
                    if (aux.tag == Constants.TAG_DROPDOWN)
                    {
                        dropdownSelected = true;
                    }
                    if (aux.name == "Blocker" || aux.name == "ButtonOK")
                    {
                        dropdownSelected = false;
                    }
                }
                else
                {
                    dropdownSelected = false;
                }
            }
        }
        if (aM != null)
        {
            if (!isAnyDropdownBusy())
            {
                //setItensInDoorDropdown(scm.getDoorsList());
                setItensInActivateDropdown(aM.getSwitchsList());
                setItensInObjectDropdown(aM.getObjectsList());
                setItensInLocationDropdown(aM.getLocationsList());
                if (emotionStatus != null)
                {
                    dropdownEmotions.value = (int)emotionStatus.getEmotion();
                }

            }
            // setItensInActivateDropdown(scm.get)
        }
    }

    public bool isAnyDropdownBusy()
    {
        return dropdownSelected;
    }

    public bool setItensInActionDropdown(Dictionary<Action, string> dictVerbs)
    {
        listActions = new List<Action>();
        dropdownActions.options.Clear();
        foreach (Action act in System.Enum.GetValues(typeof(Action)))
        {
            if (dictVerbs.ContainsKey(act))
            {
                dropdownActions.options.Add(new Dropdown.OptionData() { text = dictVerbs[act] });
                listActions.Add(act);
            }
        };
        dropdownActions.value++;
        return true;

    }
    public bool setItensInEmotionsDropdown()
    {
        dropdownEmotions.options.Clear();
        foreach (EmotionalState emotion in System.Enum.GetValues(typeof(EmotionalState)))
        {
            dropdownEmotions.options.Add(new Dropdown.OptionData() { text = emotion.ToString() });
        }
        return true;

    }

    public bool setItensInLocationDropdown(List<GameObject> listLocations)
    {
        this.listLocations = new List<GameObject>(listLocations);
        return changeItensList();
    }

    public bool setItensInObjectDropdown(List<GameObject> listObjects)
    {
        this.listObjects = new List<GameObject>(listObjects);
        return changeItensList();
    }

    public bool setItensInActivateDropdown(List<GameObject> listSwitchs)
    {
        this.listSwitchs = new List<GameObject>(listSwitchs);
        return changeItensList();
    }

    /*public bool setItensInDoorDropdown(List<GameObject> listDoors)
    {
        this.listDoors = new List<GameObject>(listDoors);
        return changeItensList();
    }*/

    private bool changeItensList()
    {
        dropdownElements.options.Clear();
        List<GameObject> auxList = getListOfGameObjects();
        if (auxList != null)
        {
            foreach (GameObject item in auxList)
            {
                dropdownElements.options.Add(new Dropdown.OptionData() { text = item.name });
            }
            dropdownElements.value++;
            dropdownElements.value--;
            return true;
        }
        return false;
    }

    public List<GameObject> getListOfGameObjects()
    {
        int index = dropdownActions.value;

        string typeParameter = Command.DictActions[listActions[index]].typeParameter;

        List<GameObject> auxList = null;
        switch (typeParameter)
        {
            case Constants.TAG_LOCATION:
                if (listLocations != null)
                    auxList = new List<GameObject>(listLocations);
                else
                    auxList = new List<GameObject>();
                auxList.AddRange(listObjects);
                auxList.AddRange(listSwitchs);
                //auxList.AddRange(listDoors);
                break;
            case Constants.TAG_OBJECT:
                auxList = listObjects;
                break;
            case Constants.TAG_SWITCH:
                auxList = listSwitchs;
                break;
            /*case Constants.TAG_DOOR:
                auxList = listDoors;
                break;*/
            default:
                break;
        }
        return auxList;
    }

    public Action getSelectedActionItem()
    {
        return listActions[dropdownActions.value];
    }

    public GameObject getSelectedElementItem()
    {
        return getListOfGameObjects()[dropdownElements.value];
    }

    public void OnActionValueChanged(int index)
    {
        if (listActions.Count > 0)
        {
            string typeParameter = Command.DictActions[listActions[dropdownActions.value]].typeParameter;

            if (typeParameter.Equals(Constants.PAR_STRING))
            {
                setUIElements(false);
                inputFieldLookFor.gameObject.SetActive(true);
            }
            else if (typeParameter.Equals(Constants.PAR_NULL))
            {
                setUIElements(false); ;
            }
            else
            {
                changeItensList();
                setUIElements(false);
                dropdownElements.gameObject.SetActive(true);
                TypeAction typeAction = Command.DictActions[listActions[dropdownActions.value]].typeAction;
                if (typeAction == TypeAction.Interaction)
                {
                    groupToggleHand.gameObject.SetActive(true);
                    if (listActions[dropdownActions.value] == Action.Taste)
                    {
                        dropdownElements.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void OnEmotionChange(int i)
    {
        emotionStatus.setEmotion((EmotionalState)i);
    }

    private void setUIElements(bool value)
    {
        groupToggleHand.gameObject.SetActive(value);
        dropdownElements.gameObject.SetActive(value);
        inputFieldLookFor.gameObject.SetActive(value);
    }

    public void sendCommand()
    {
        string typeParameter = Command.DictActions[listActions[dropdownActions.value]].typeParameter;
        if (typeParameter.Equals(Constants.PAR_STRING))
        {
            aM.sendCommand("",getSelectedActionItem(), inputFieldLookFor.text);
        }
        else
        {
            Hands hand = Hands.Right;
            if (groupToggleHand.isActiveAndEnabled && groupToggleHand.GetActive().name.Equals(Constants.TGGL_LEFT))
            {
                hand = Hands.Left;
            }
            Transform auxTransform = getListOfGameObjects()[dropdownElements.value].transform;
            aM.sendCommand("",hand, getSelectedActionItem(), auxTransform);
        }
        
    }

}
