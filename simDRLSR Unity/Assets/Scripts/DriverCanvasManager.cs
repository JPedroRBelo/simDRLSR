using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DriverCanvasManager : MonoBehaviour {

    public GameObject simulatorManager = null;
    private SimulatorCommandsManager scm;
    public Dropdown dropdownActions;
    public Dropdown dropdownElements;
    public Slider sliderRotation;
    public Text textRotation;
    public InputField inputFieldLookFor;
    public ToggleGroup groupToggleHand;   
    

    private List<Action> listActions = new List<Action>();
    //Objects e Locations pertencem ao conjunto Elements
    private List<GameObject> listObjects;
    private List<GameObject> listLocations = null;
    private List<GameObject> listSwitchs;
    //private List<GameObject> listDoors;
    private bool dropdownSelected;

    // Use this for initialization
    void Start () {
        scm = simulatorManager.GetComponent<SimulatorCommandsManager>();
        dropdownSelected = false;
        listObjects = new List<GameObject>();
        listSwitchs = new List<GameObject>();
        //listDoors = new List<GameObject>();
        
        
        scm = simulatorManager.GetComponent<SimulatorCommandsManager>();
        setItensInActionDropdown(scm.getAvailableActions());
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
                }else
                {
                   
                        dropdownSelected = false;

                }
            }
        }

        if (scm != null)
        {
            

            if (!isAnyDropdownBusy())
            {
                //setItensInDoorDropdown(scm.getDoorsList());
                setItensInActivateDropdown(scm.getSwitchsList());
                setItensInObjectDropdown(scm.getObjectsList());
                setItensInLocationDropdown(scm.getLocationsList());
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
        }
        dropdownActions.value++;
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

            if (typeParameter.Equals(Constants.PAR_ROTATION))
            {
                setUIElements(false);
                sliderRotation.gameObject.SetActive(true);
            }
            else if (typeParameter.Equals(Constants.PAR_STRING))
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

    private void setUIElements(bool value)
    {
        groupToggleHand.gameObject.SetActive(value);
        dropdownElements.gameObject.SetActive(value);
        sliderRotation.gameObject.SetActive(value);
        inputFieldLookFor.gameObject.SetActive(value);
    }


    public void sendCommand()
    {
        string typeParameter = Command.DictActions[listActions[dropdownActions.value]].typeParameter;
        if (typeParameter.Equals(Constants.PAR_ROTATION))
        {
            scm.sendCommand("",getSelectedActionItem(), sliderRotation.value);
        }
        else if (typeParameter.Equals(Constants.PAR_STRING))
        {
            scm.sendCommand("",getSelectedActionItem(), inputFieldLookFor.text);
        }
        else if (typeParameter.Equals(Constants.PAR_NULL))
        {
            scm.sendCommand("",getSelectedActionItem());
        }else
        {
            Hands hand = Hands.Right;
            if (groupToggleHand.isActiveAndEnabled && groupToggleHand.GetActive().name.Equals(Constants.TGGL_LEFT))
            {
                hand = Hands.Left;
            }
            if (getSelectedActionItem() == Action.Taste)
            {
                scm.sendCommand("", hand, Action.Taste);
            }else
            if (getSelectedActionItem() == Action.Smell)
            {
                scm.sendCommand("", hand, Action.Smell);
            }
            else
            {
                Transform auxTransform = getListOfGameObjects()[dropdownElements.value].transform;
                scm.sendCommand("",hand, getSelectedActionItem(), auxTransform);
            }
        }
    }


    public void setTextRotation(float value)
    {
        textRotation.text = value.ToString("0");
    }
}
