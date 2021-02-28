using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Agent {
    
    [SerializeField]
    private Transform agent;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private RectTransform panel;
    [SerializeField]
    private bool hiddenPanel;

    private bool active;


    public Agent(Transform agent, Camera camera)
    {
        this.agent = agent;
        this.camera = camera;
    }

    public void setAgent(Transform agent)
    {
        this.agent = agent;
    }

    public Transform getAgent()
    {
        return agent;
    }

    public void setCamera(Camera camera)
    {
        this.camera = camera;
    }

    public Camera getCamera()
    {
        return camera;
    }

    public void disableForUser()
    {
        activate(false);
    }

    private void activate(bool flag)
    {
        camera.enabled = flag;
        active = flag;
        panel.gameObject.SetActive(flag&&!hiddenPanel);
    }

    public bool isActive()
    {
        return active;
    }
    public void enableForUser()
    {
        activate(true);
    }

    public float getPanelHeight()
    {
        if (!hiddenPanel)
            return panel.rect.height;
        else
            return 0;
    }
}
