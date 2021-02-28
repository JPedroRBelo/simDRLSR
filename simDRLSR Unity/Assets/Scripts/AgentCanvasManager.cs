using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentCanvasManager : MonoBehaviour {

    public RectTransform containerAgents = null;
    public ToggleGroup toggleGroup = null;
    public Agent robot;
    public Agent human;
    public Agent godMode;
    // Use this for initialization
    void Start()
    {
        toggleGroup.GetActive().onValueChanged.Invoke(true);
        setGodMode();
    }

    private void disableModes()
    {
        AvatarControl ac = human.getAgent().GetComponent<AvatarControl>();
        if (ac != null)
            ac.desactivate();
        robot.disableForUser();
        human.disableForUser();
        godMode.disableForUser();
    }



    public void setRobotMode()
    {
        disableModes();
        robot.enableForUser();
        containerAgents.sizeDelta = new Vector2(containerAgents.sizeDelta.x, 30f + robot.getPanelHeight());
    }

    public void setHumanMode()
    {
        disableModes();
        AvatarControl ac = human.getAgent().GetComponent<AvatarControl>();
        if (ac != null)
            ac.activate();
        human.enableForUser();
        containerAgents.sizeDelta = new Vector2(containerAgents.sizeDelta.x, 30f + human.getPanelHeight());
    }

    public void setGodMode()
    {
        disableModes();
        godMode.enableForUser();
        containerAgents.sizeDelta = new Vector2(containerAgents.sizeDelta.x, 30f + godMode.getPanelHeight());
    }


    // Use this for initialization
   

}
