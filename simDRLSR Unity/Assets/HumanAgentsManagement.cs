using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]


public enum EmotionModes
{
    Random,
}

public class HumanAgentsManagement : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform initialLocations;
    public List<GameObject> avatars;

    public EmotionModes emotionMode;
      
    public bool randomPosition;

    private List<Transform> locations;
    void Start()
    {
        enableDefaultHumanOnly();
    	StartCoroutine(LateStart(1));
     }
 
     IEnumerator LateStart(float waitTime)
     {
        yield return new WaitForSeconds(waitTime);
        setRandomEmotion();
   
     }

    public void enableDefaultHumanOnly(){
        int r = 0;
        for (int i = 0; i < avatars.Count; i++)
        {
            if(r!=i){
                avatars[i].SetActive(false);
            }else{
                avatars[i].SetActive(true);
            }
        }
    }

    public void enableOneRandomHuman(){
        var rnd = new System.Random();
        int r = rnd.Next(avatars.Count);
        
        for (int i = 0; i < avatars.Count; i++)
        {
            if(r!=i){
                avatars[i].SetActive(false);
            }else{
                avatars[i].SetActive(true);
            }
        }

    }

    private void setRandomEmotion(){
        int index = 0;
        locations = new List<Transform>();
        if(initialLocations!=null){
            foreach (Transform child in initialLocations.transform)
               locations.Add(child);
            var rnd = new System.Random();
            var randomized = locations.OrderBy(item => rnd.Next());   
                     
            foreach(GameObject human in avatars){
                
                if(human.active){
                    EkmanEmotions randomEmotion =  chooseHumanEmotion(human,emotionMode);
                    setHumanEmotion(human,randomEmotion);
                    if(randomPosition){
                        Vector3 new_position = randomized.ToList()[index++%randomized.Count()].position;
                        float y_pos = human.transform.position.y+human.transform.position.y;
                        human.transform.position = new Vector3(new_position.x,y_pos,new_position.z);
                    }
                }
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setAvatarRandomPositionFlag(bool flag){
        randomPosition = flag;
    }

    private void setHumanEmotion(GameObject avatar,EkmanEmotions emotion){
        FaceBehave face = avatar.GetComponent<FaceBehave>();
        face.setConstantEmotion(emotion);
    }
    private void setHumanEmotion(GameObject avatar,string emotion){
        FaceBehave face = avatar.GetComponent<FaceBehave>();
        face.setConstantEmotion(emotion);
    }

    private EkmanEmotions chooseHumanEmotion(GameObject avatar,EmotionModes emotionMode){
        EkmanEmotions randomEmotion = EkmanEmotions.Neutral;
        EkmanGroupEmotions randomGroup = EkmanGroupEmotions.Neutral;
        FaceBehave face = avatar.GetComponent<FaceBehave>();
        if(emotionMode == EmotionModes.Random){
            Array values = Enum.GetValues(typeof(EkmanGroupEmotions));
            System.Random random = new System.Random();
            randomGroup = (EkmanGroupEmotions)values.GetValue(random.Next(values.Length));
            random = new System.Random();
            randomEmotion = face.GroupedEmotions[randomGroup][random.Next(face.GroupedEmotions[randomGroup].Count)];
                   
        }
        return randomEmotion;
    }

    public void setEmotionToHumans(string emotion){
        if(emotion.ToLower()!="random"){
            foreach(GameObject human in avatars){
                setHumanEmotion(human,emotion);
            }
        }
    }
      
}
