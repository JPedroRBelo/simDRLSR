using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SmellProperties))]
class DecalMeshHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SmellProperties smellProperties = (SmellProperties)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Apply"))
        {
            smellProperties.applyValues();
        }
            
    }
}