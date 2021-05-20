using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManagerKeyboard : MonoBehaviour {

    private enum TimeStates {Started,Paused,Stoped};
    private float timeValue = 1f;
    private TimeStates timeStateAt;

   
	// Use this for initialization
	void Start () {
        //stopButton.interactable = false;
        playSimulation();
	}
	
	// Update is called once per frame
	void Update () {
        if ((Input.GetKeyDown("z"))&&(timeStateAt == TimeStates.Started))
        {
            pauseSimulation();

        }
        else if((Input.GetKeyDown("z")) && (timeStateAt == TimeStates.Paused))
        {
            playSimulation();

        }

    }
    public void playSimulation()
    {
        timeStateAt = TimeStates.Started;
        setTime(1f);
        
    }

    public void pauseSimulation()
    {
        timeStateAt = TimeStates.Paused;
        Time.timeScale = 0;
    }

    public void setTime(float timeValue)
    {
        this.timeValue = timeValue;
        if (timeStateAt == TimeStates.Started)
        {
            
            Time.timeScale = timeValue;
        }
    }

    public float getTime()
    {
       return timeValue;
    }
}
