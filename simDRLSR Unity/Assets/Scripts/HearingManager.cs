using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class HearingManager : MonoBehaviour {


    public AudioListener hearing;

    public bool printLog = false;

    private int qSamples = 4096;
    private float[] samples;
    private AudioSource[] sources;
    private HashSet<GameObject> gameObjects;
    private HashSet<GameObject> updatedElementsList;
    // Use this for initialization
    void Start () {
        samples = new float[qSamples];
        sources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        gameObjects = new HashSet<GameObject>();
        updatedElementsList = new HashSet<GameObject>();
        Log("RHS>>> " + this.name + " hearing  was configured with success.");
    }

    private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }

    private float getRMS(int channel)
    {
        AudioListener.GetOutputData(samples, channel);
        float sum = 0;
        for(int i = 0;i < qSamples; i++)
        {
            sum = samples[i] * samples[i];
        }
        return Mathf.Sqrt(sum / qSamples);
    }
	
	// Update is called once per frame
	void Update () {

        gameObjects = new HashSet<GameObject>();
        foreach (AudioSource audioSource in sources)
        {
            if (audioSource.isPlaying)
            {
                gameObjects.Add(audioSource.gameObject);
            }
        }
        updatedElementsList = new HashSet<GameObject>(gameObjects);
    }

    public List<GameObject> getListOfElements()
    {
        return updatedElementsList.ToList();
    }

}
