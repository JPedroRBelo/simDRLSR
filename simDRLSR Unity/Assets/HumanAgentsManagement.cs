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
        
    	StartCoroutine(LateStart(3));
     }
 
     IEnumerator LateStart(float waitTime)
     {
        yield return new WaitForSeconds(waitTime);
        
        int index = 0;
        locations = new List<Transform>();
        if(initialLocations!=null){
            foreach (Transform child in initialLocations.transform)
               locations.Add(child);
            var rnd = new System.Random();
            var randomized = locations.OrderBy(item => rnd.Next());
            if(randomPosition){
                foreach(GameObject human in avatars){
                    EkmanEmotions randomEmotion =  chooseHumanEmotion(human,emotionMode);
                    setHumanEmotion(human,randomEmotion);
                    Vector3 new_position = randomized.ToList()[index++%randomized.Count()].position;
                    float y_pos = human.transform.position.y+human.transform.position.y;
                    human.transform.position = new Vector3(new_position.x,y_pos,new_position.z);
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
      
}
