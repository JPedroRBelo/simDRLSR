using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
 
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
    public Dictionary<string, List<CustomBlendShape>> dictEmotions;
    public Dictionary<string,bool> emotionExecutation;

    //public string[] blendShapes;

    void Awake ()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();        
        if(skinnedMeshRenderer==null){
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        skinnedMesh = skinnedMeshRenderer.sharedMesh;
     

    }




    void Start ()
    {
        dictEmotions = new Dictionary<string, List<CustomBlendShape>>();
        blendShapeCount = skinnedMesh.blendShapeCount; 
        blendNames = getBlendShapeNames(skinnedMesh);
        index = 0;
        speed = 0;
    }

    public void setGUIBlendNames(){
        blendNames = getBlendShapeNames();
    }

    void Update ()
    {   /*
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

    IEnumerator ExecuteBlendShape(CustomBlendShape blendShape, bool activate ,float duration )
    {   
        float blendTime = 1/blendShape.blend;
        float elapsed = 0.0f;
        if(activate){            
            while (elapsed < blendTime )
            {
                speed = Mathf.Lerp( 0, blendShape.range, elapsed / blendTime );
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
        while (elapsed < blendTime )
        {
            speed = Mathf.Lerp( blendShape.range, 0, elapsed / blendTime );
            elapsed += Time.deltaTime;
            skinnedMeshRenderer.SetBlendShapeWeight(blendShape.shape, speed);
            yield return null;
        }
    } 
    

    public void setEmotion(string emotion, bool activate,float duration = 0f){
               // if(emotions[i].name.Equals(emotion)){

            foreach(CustomBlendShape blendshape in dictEmotions[emotion]){
                StartCoroutine( ExecuteBlendShape(blendshape, activate,duration));
            }
            

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
    public void printTest(string test){
        print(test);
    }

    public void setEmotions(List<FaceEmotion> emotions){
        //this.emotions = emotions;
        print(emotions);
    }


}

[Serializable]
public class FaceEmotion
{
    public string name;
    public List<CustomBlendShape> shapes;
    //public List<
    public FaceEmotion(string name)
    {
        this.name = name;
        this.shapes = new List<CustomBlendShape>();
        
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
        blend = 0f;
        range = 0f;
    }

    public CustomBlendShape(int shape, float blend,float range)
    {            
        this.shape = shape;
        this.blend = blend;
        this.range = range;
    }
}
