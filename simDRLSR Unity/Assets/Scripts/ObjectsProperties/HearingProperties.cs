using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntSenseCSharpAPI;
//public enum SoundType { NoSound, BarkingSound, BellSound, BirdsSound, LiquidFlowingSound, MotorSound, MusicSound, TvSound, HumanVoiceSound, RobotVoiceSound }

[RequireComponent(typeof(AudioSource))]
public class HearingProperties : MonoBehaviour {

    private AudioSource audioSource;
    [SerializeField]
    private string soundDetail;
    [SerializeField]
    private HearingAttribute soundType;
    private float volume;
	// Use this for initialization
	void Awake () {
        audioSource = GetComponent<AudioSource>();
        volume = audioSource.volume;
    }
	
	// Update is called once per frame
	void Update () {
        volume = audioSource.volume;
	}

    public float getVolume()
    {
        return volume;
    }

    public string getHearingStatus()
    {
        string str = "Sound Type: " + soundType;
        str += "\nVolume: " + volume;
        if (soundDetail != null && !soundDetail.Equals(""))
        {
            str += "\nDesc: " + soundDetail;
        }
        return str;
    }

    public string getSoundDetail()
    {
        return soundDetail;
    }

    public void setSoundDetail(string soundDetail)
    {
        this.soundDetail = soundDetail;
    }

    public AudioSource getAudioSource()
    {
        return audioSource;
    }

    public HearingAttribute getSoundType()
    {
        return soundType;
    }
}
