using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HearingProperties))]
public class AgentSpeech : MonoBehaviour {

    // Use this for initialization
    public bool printLog = false;
    private HearingProperties hearingProperties;
    private AudioSource audioSource;
    private Command command;
    void Start () {
        command = null;
        hearingProperties = GetComponent<HearingProperties>();
        audioSource = hearingProperties.getAudioSource();
        Log("RHS>>> " + this.name + " is ready to receive Speech Commands.");
    }

    private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }   
    

    void Update()
    {
        if (command != null)
        {
            switch (command.getAction())
            {
                case Action.Speak:
                    switch (command.getActionStateID())
                    {
                        case (int)Speak.Start:
                            Log("Command>>> " + this.name + " command " + command.getId() + " Started!");
                            command.next();
                            break;
                        case (int)Speak.Position:
                            hearingProperties.setSoundDetail(command.getRefName());
                            audioSource.Play();
                            Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
                            command.success();
                            break;
                        case (int)Speak.End:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public bool sendCommand(Command command)
    {
        this.command = command;
        Log("Command>>> " + this.name + " received command " + command.getStringCommand());
        return true;
    }
}
