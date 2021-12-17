using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using System.Linq;
 
public class ConfigBlendShapes : MonoBehaviour
{
    int blendShapeCount;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    float blendSpeed = 1f;
    string[] blendNames;
    float speed;
    
    private int index;
    [HideInInspector]
    public Dictionary<string, FaceEmotion> dictEmotions =new Dictionary<string, FaceEmotion>();
    public Dictionary<string,bool> emotionExecution;

    //public string[] blendShapes;
    private FaceBehave faceBehave;

    void Awake ()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();        
        if(skinnedMeshRenderer==null){
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        skinnedMesh = skinnedMeshRenderer.sharedMesh;
        faceBehave = GetComponent<FaceBehave>();
        //dictEmotions = new Dictionary<string, FaceEmotion>();
        blendShapeCount = skinnedMesh.blendShapeCount; 
        blendNames = getBlendShapeNames(skinnedMesh);
        emotionExecution = new Dictionary<string, bool>();
        emotionExecution["neutral"] = true;

    }


    void Start ()
    {
        
        index = 0;
        speed = 0;
    }

    public void setGUIBlendNames(){
        blendNames = getBlendShapeNames();
    }

    void Update ()
    {   
        /*
        FaceEmotion emotion = emotions[index%emotions.Count];
        for(int j = 0; j < emotion.shapes.Count; j++){
            int shapeId = emotion.shapes[j].shape;
            float blend = emotion.shapes[j].blend;
            float weight = emotion.shapes[j].range;
            /*if(!activate){
                weight = 0;
            }*/
            /*
            skinnedMeshRenderer.SetBlendShapeWeight(shapeId, weight);
        } 

        index = index + 1;
           */    
       
        
    }

    IEnumerator ExecuteBlendShape(CustomBlendShape blendShape,string emotion, bool activate ,float duration,float xEyeOffset, float yEyeOffset, string resetMod = "reset")
    {   
        Vector2 eyeOffSet = new Vector2(xEyeOffset,yEyeOffset);
        float blendTime = 1/blendShape.blend;
        float elapsed = 0.0f;
        float range = 0f;
        float startBlendValue = 0f;

        float  originalBlendValue = 0;
        Vector2 originalEyeOffSet = faceBehave.getDefaultEyeOffset();

        if(resetMod =="continue")
        {
            originalBlendValue = skinnedMeshRenderer.GetBlendShapeWeight(blendShape.shape);
            originalEyeOffSet = faceBehave.GetEyeOffset();

        }else
        if(resetMod =="without_eyegaze")
        {
            originalBlendValue = skinnedMeshRenderer.GetBlendShapeWeight(blendShape.shape);
            originalEyeOffSet = faceBehave.GetEyeOffset();
            eyeOffSet = faceBehave.GetEyeOffset();
            
        }       

        bool condition = true;
        if(activate){
            if(!emotion.Equals("blink")){
                emotionExecution[emotion] = true;
            }                
            faceBehave.SetEyeOffset(eyeOffSet);   
            condition = true;
            while (condition)
            {                       
                if((blendShape.range>originalBlendValue))
                {
                    if(blendShape.range <= skinnedMeshRenderer.GetBlendShapeWeight(blendShape.shape))
                    {                   
                        condition = false;
                        continue;
                    }
                    
                }else
                if(blendShape.range >= skinnedMeshRenderer.GetBlendShapeWeight(blendShape.shape))
                {
                        condition = false;
                        continue;
                }
                
                speed = Mathf.Lerp( originalBlendValue, blendShape.range , elapsed / blendTime );
                elapsed += Time.deltaTime;
                skinnedMeshRenderer.SetBlendShapeWeight(blendShape.shape, speed);
                yield return null;
            }               
          

            elapsed = 0.0f;
            while (elapsed < duration )
            {
                elapsed += Time.deltaTime; 
                yield return null;
            }
        } 
        elapsed = 0.0f;            
        condition = true;
        while (condition)
        {
            if((blendShape.range<0))
            {
                if(originalBlendValue <= skinnedMeshRenderer.GetBlendShapeWeight(blendShape.shape)){                
                    condition = false;
                    continue;
                }                
            }else
            if(originalBlendValue >= skinnedMeshRenderer.GetBlendShapeWeight(blendShape.shape)){
                
                condition = false;
                continue;
            }
            

            speed = Mathf.Lerp( blendShape.range, originalBlendValue, elapsed / blendTime );
            elapsed += Time.deltaTime;
            skinnedMeshRenderer.SetBlendShapeWeight(blendShape.shape, speed);
            yield return null;
        }
        if(emotionExecution.ContainsKey(emotion)){
            emotionExecution.Remove(emotion);
        }
        faceBehave.SetEyeOffset(originalEyeOffSet);
    } 
    
    public string getCurrentEmotion(){
        return emotionExecution.Keys.Last();
    }
    public void setEmotion(string emotion, bool activate,float duration = 0f,string resetMod="reset"){
            
            // the code that you want to measure comes here
            if(dictEmotions.Count==0){
                dictEmotions = LoadEmotions("/emotionsInspector.dat");
            }
            if(dictEmotions.ContainsKey(emotion) && dictEmotions[emotion].shapes.Count > 0){
                if(!emotion.Equals("blink")){
                    Debug.Log("Configuring "+transform.name+" with "+emotion+" emotion.");

                }
                foreach(CustomBlendShape blendshape in dictEmotions[emotion].shapes){
                    StartCoroutine( ExecuteBlendShape(blendshape,emotion, activate,duration,dictEmotions[emotion].xEyeOffset,dictEmotions[emotion].yEyeOffset,resetMod));
                }
            }
            
           
            

    }
    public Dictionary<string, FaceEmotion> LoadEmotions(string path)
        {
            if(File.Exists(Application.persistentDataPath + path))
            {
                BinaryFormatter bf = new BinaryFormatter ();
                FileStream file = File.Open (Application.persistentDataPath + path, FileMode.Open);
                Dictionary<string, FaceEmotion> emotions = (Dictionary<string, FaceEmotion>)bf.Deserialize(file);
                file.Close ();
                return emotions;
            }else{
                Debug.Log("Emotion save file not found!!!");
            }
            return new Dictionary<string, FaceEmotion>();
        }

    public string[] getBlendShapeNames(){
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        
        if(skinnedMeshRenderer==null){
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        Mesh skinnedMesh = skinnedMeshRenderer.sharedMesh;   
        return getBlendShapeNames(skinnedMesh); 
    }

    public string[] getBlendShapeNames(Mesh mesh)
    {        

        string[] arr = new string [mesh.blendShapeCount];
        for (int i= 0; i < mesh.blendShapeCount; i++)
        {
            string s = mesh.GetBlendShapeName(i);
            //print("Blend Shape: " + i + " " + s); // Blend Shape: 4 FightingLlamaStance
            arr[i] = s;
        }
        return arr;
    }


}

[Serializable]
public class FaceEmotion
{
    public string name;
    public List<CustomBlendShape> shapes;
    //public List<
    public float xEyeOffset;
    public float yEyeOffset;
    public FaceEmotion(string name)
    {
        this.name = name;
        this.shapes = new List<CustomBlendShape>();        
        this.xEyeOffset = 0f;
        this.yEyeOffset = 0f;
        
    }
}

[Serializable]
public class CustomBlendShape
{
    public int shape;
    public float blend;
    public float range;
    //public List<

    public CustomBlendShape()
    {            
        shape = 0;
        blend = 2f;
        range = 0f;
    }

    public CustomBlendShape(int shape, float blend,float range)
    {            
        this.shape = shape;
        this.blend = blend;
        this.range = range;
        
    }
}
