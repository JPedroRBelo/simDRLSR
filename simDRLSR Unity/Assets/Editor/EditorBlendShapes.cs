using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
     
    [CustomEditor(typeof(ConfigBlendShapes))]
    public class EditorBlendShapes:Editor {
     
        ConfigBlendShapes blendShapes;
        public List<int> selected = new List<int>();
        public string emotionName = "emotion";
        private Dictionary<string, List<CustomBlendShape>> dictEmotions;

       
     
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.DrawDefaultInspector();
            string[] blendNames = blendShapes.getBlendShapeNames();
 
           //string[] blendNames = new  string[]{"Option1", "Option2", "Option3"};
                
            if(GUILayout.Button("Load Emotion Files")){
                dictEmotions = LoadEmotions("/emotionsInfo.dat");
            }

            if(GUILayout.Button("Save Emotion Files")){
                SaveEmotions(dictEmotions,"/emotionsInfo.dat");                
            }

            if(GUILayout.Button("Reset Configuration")){
                
                dictEmotions = new Dictionary<string, List<CustomBlendShape>>();              
            }
            try{
                foreach(KeyValuePair<string, List<CustomBlendShape>> emotion in dictEmotions){           
                //for (int j = 0; j < emotions.Count;j++){
                    GUILayout.BeginVertical("GroupBox");
                    GUILayout.BeginHorizontal("HelpBox");
                    if(GUILayout.Button("\u272A".ToString(),Styles.buttonGreen, GUILayout.Width(25), GUILayout.Height(25))){
                        float duration = blendShapes.gameObject.GetComponent<FaceBehave>().emotionDuration;
                        blendShapes.setEmotion(emotion.Key,true,duration);
                    }
                    EditorGUILayout.Space(20);
                    GUILayout.Label("Emotion : "+emotion.Key);
                    
                    if(GUILayout.Button("X",Styles.buttonRed, GUILayout.Width(25), GUILayout.Height(25))){
                        dictEmotions.Remove(emotion.Key);
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("box");               
                    
                    GUILayout.Label("Shape");
                    EditorGUILayout.Space();
                    GUILayout.Label("Blend");
                    GUILayout.Label("Value");
                    GUILayout.Label("Delete",Styles.fontStyle);
                    GUILayout.EndHorizontal();
                    float sizeMultiplier = 0f;
                    //[Range(0f, 100f)]
                    float range = 0f;
                    
                    for (int i = 0; i < emotion.Value.Count;i++){                    
                        GUILayout.BeginHorizontal("box");                    
                        //EditorGUILayout.Space(20);
                        emotion.Value[i].shape = EditorGUILayout.Popup(emotion.Value[i].shape, blendNames);
                        EditorGUILayout.Space(30);
                        emotion.Value[i].blend  = EditorGUILayout.FloatField("", emotion.Value[i].blend, GUILayout.Width(40));
                        EditorGUILayout.Space();
                        emotion.Value[i].range = EditorGUILayout.Slider(emotion.Value[i].range, 0f, 100f);
                        //GUILayout.Label("");
                        EditorGUILayout.Space(20);
                        if(GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20))){
                            emotion.Value.RemoveAt(i);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal("box");
                    
                    if(GUILayout.Button("Add Blend Shape")){//, GUILayout.Width(20), GUILayout.Height(20))){
                        emotion.Value.Add(new CustomBlendShape());
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
            catch{

            }

            GUILayout.BeginHorizontal("box");
            if(GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20))){
                dictEmotions.Add(emotionName,new List<CustomBlendShape>());
            }
            emotionName = EditorGUILayout.TextField("Add Emotion: ", emotionName);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
           
            /*for (int i = 0; i < test.blendShapes.Count; i++)
            {
                SerializedProperty arraysp = listsp.GetArrayElementAtIndex(i).FindPropertyRelative("theFloatArray");
                EditorGUI.BeginChangeCheck();          
     
                EditorGUILayout.PropertyField(arraysp, true);
             
                if (EditorGUI.EndChangeCheck())
                {              
                    serializedObject.ApplyModifiedProperties();
                }
            } */   

            blendShapes.dictEmotions = dictEmotions;
            SaveEmotions(dictEmotions,"/emotionsInspector.dat");
        }
    
        void OnEnable()
        {
            blendShapes = target as ConfigBlendShapes; 
            //dictEmotions = new Dictionary<string, List<CustomBlendShape>>();  
            dictEmotions = LoadEmotions("/emotionsInspector.dat");   
        }


        static class Styles {
 
            internal static GUIStyle fontStyle;
            internal static GUIStyle buttonRed;
            internal static GUIStyle buttonGreen;
            internal static GUIStyle buttonBlue;   
    
            static Styles() {   
               
                buttonRed = new GUIStyle(GUI.skin.button);
                buttonGreen = new GUIStyle(GUI.skin.button);
                buttonRed.normal.textColor = Color.red;
                buttonBlue = new GUIStyle(GUI.skin.button);
                buttonBlue.normal.textColor = Color.blue;
                buttonGreen.normal.textColor = Color.green;
                fontStyle = new GUIStyle( GUI.skin.label);
                fontStyle.alignment = TextAnchor.MiddleRight;
            }
        }


         public void SaveEmotions (Dictionary<string, List<CustomBlendShape>> emotion,string path) 
        {
            BinaryFormatter bf = new BinaryFormatter ();
            FileStream file = File.Create (Application.persistentDataPath + path);
            //List<FaceEmotion> emotions = new FaceEmotion();
            
            bf.Serialize (file, emotion);
            file.Close ();
        }
        public Dictionary<string, List<CustomBlendShape>> LoadEmotions(string path)
        {
            if(File.Exists(Application.persistentDataPath + path))
            {
                BinaryFormatter bf = new BinaryFormatter ();
                FileStream file = File.Open (Application.persistentDataPath + path, FileMode.Open);
                Dictionary<string, List<CustomBlendShape>> emotions = (Dictionary<string, List<CustomBlendShape>>)bf.Deserialize(file);
                file.Close ();
                return emotions;
            }else{
                Debug.Log("Emotion save file not found!!!");
            }
            return new Dictionary<string, List<CustomBlendShape>>();
        }

    }   


        
  
