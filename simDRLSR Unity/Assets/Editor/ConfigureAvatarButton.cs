using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(ConfigureAvatar))]
public class ConfigureAvatarButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ConfigureAvatar scriptConfigureAvatar = (ConfigureAvatar)target;
        if (GUILayout.Button("Configure Avatar"))
        {
            scriptConfigureAvatar.Configure();
        }
        if (GUILayout.Button("Remove Configurations"))
        {
            scriptConfigureAvatar.Remove();
        }
    }

}