using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Text textAction;
    public RLAgent agent;
    public GameObject panelAgent;
    private bool doShow = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) { doShow = !doShow; }
        panelAgent.SetActive(doShow);
        string robotAction = agent.getAction().ToString();
        textAction.text = "\tRobot Action: "+robotAction;
    }
}
