using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OntSenseCSharpAPI;
//public enum Emotions { Neutral, Anger, Disgust, Fear, Happiness, Sadness, Surprise}
public class EmotionStatus : MonoBehaviour {

    public EmotionalState emotion;

    void Start()
    {
        
    }
    public EmotionalState getEmotion()
    {
        return emotion;
    }
                
    public void setEmotion(EmotionalState emotion)
    {
        Debug.Log("RHS>>> " + this.name + " emotion is changed to "+emotion+".");
        this.emotion = emotion;
    }

}
