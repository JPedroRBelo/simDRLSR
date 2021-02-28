using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*

[CustomEditor(typeof(ConfigureSaveImage))]
public class ImageSaver : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ConfigureSaveImage conf = (ConfigureSaveImage)target;
        //ImageSynthesis imageSynthesis = (ImageSynthesis)target;

        // Only display the "Save" button if playing
        if (EditorApplication.isPlaying && GUILayout.Button("Save Captures")) 
        {
            //Vector2 gameViewSize = Handles.GetMainGameViewSize();
            //imageSynthesis.Save(imageSynthesis.filename, width: (int)gameViewSize.x, height: (int)gameViewSize.y, imageSynthesis.filepath);
            List<ImageToSaveProperties> imgProp = new List<ImageToSaveProperties>();
            imgProp.Add(new ImageToSaveProperties("img_1","Captures",width: 320,height:240,ImageType.Grey));
            imgProp.Add(new ImageToSaveProperties("img_2","Captures",width: 320,height:240,ImageType.Depth));
            conf.CaptureImages(imgProp,0);
        }
    }
}
*/
