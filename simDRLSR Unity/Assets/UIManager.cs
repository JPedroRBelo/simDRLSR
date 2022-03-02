using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Text textAction;
    public Text textEmotion;
    public RLAgent agent;
    public EventDetector eventDetector;
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
        string humanEmotion = eventDetector.getCurrentEmotion();
        textAction.text = "\tRobot Action: "+robotAction;
        textEmotion.text = "\tHuman Emotion: "+firstLetterToUpper(humanEmotion);
    }

    public string firstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
}
