using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using OntSenseCSharpAPI;

public class SinkManager : Status {
    
    private ParticleSystem waterParticles;
    private PhysicalState atValue;
    AudioSource auxAudioSource;
    // Use this for initialization
    void Start()
    {
        atValue = status;
        waterParticles = GetComponentsInChildren<ParticleSystem>()[0];
        auxAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (status == PhysicalState.openState) {

                if (waterParticles != null && !waterParticles.isPlaying)
                {
                    waterParticles.Play();
                }
                                
                if (auxAudioSource != null && !auxAudioSource.isPlaying)
                {
                    auxAudioSource.Play();
                }
                atValue = status;
            
        }else
        {
           

                if (waterParticles != null && waterParticles.isPlaying)
                {
                    waterParticles.Stop();
                }

                
                if (auxAudioSource != null && auxAudioSource.isPlaying)
                {
                    auxAudioSource.Stop();
                }
                atValue = status;
            
        }
    }
    public override void turnOnOpen()
    {
        status = PhysicalState.openState;
        Debug.Log("RHS>>> " + this.name + " opened.");

    }

    public override void turnOffClose()
    {
        status = PhysicalState.closeState;
        Debug.Log("RHS>>> " + this.name + " closed.");
    }

}
