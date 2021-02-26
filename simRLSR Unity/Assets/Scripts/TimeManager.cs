using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    private enum TimeStates {Started,Paused,Stoped};
    private float timeValue = 1f;
    private TimeStates timeStateAt;

    public Button playButton;
    public Button pauseButton;
    public Button stopButton;
    public Slider timeSlider;
    public Text timeText;
	// Use this for initialization
	void Start () {
        stopButton.interactable = false;
        playSimulation();
	}
	
	// Update is called once per frame
	void Update () {

	}
    public void playSimulation()
    {
        playButton.interactable = false;
        pauseButton.interactable = true;
        timeStateAt = TimeStates.Started;
        setTime( timeValue);
        
    }

    public void pauseSimulation()
    {
        playButton.interactable = true;
        pauseButton.interactable = false;
        timeStateAt = TimeStates.Paused;
        Time.timeScale = 0;
    }

    public void setTime(float timeValue)
    {
        timeText.text = timeValue.ToString();
        this.timeValue = timeValue;
        if (timeStateAt == TimeStates.Started)
        {
            
            Time.timeScale = timeValue;
        }
    }
}
