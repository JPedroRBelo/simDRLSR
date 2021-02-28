using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPanelManager : MonoBehaviour {
    
    public Camera cam;
    private RectTransform panel;

    private float aspect;
	// Use this for initialization
	void Start () {
        panel = GetComponent<RectTransform>();
        aspect = cam.aspect;
        
	}
	
	// Update is called once per frame
	void Update () {
        aspect = cam.aspect;
        panel.sizeDelta = new Vector2(aspect * panel.sizeDelta.y, panel.sizeDelta.y);
    }
}
