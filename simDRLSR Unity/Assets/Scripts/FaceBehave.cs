using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Collections;
using CrazyMinnow.SALSA; // Import SALSA classes from the CrazyMinnow namespace

public class FaceBehave : MonoBehaviour
{
	public RandomEyes3D randomEyes; // Public reference to RandomEyes3D
	public float value = 2.5f;
	public float blinkTime = 0.15f;
	public bool smile = false;
	public bool blink = false;
	//private bool flag = false;

	private 

	void Start()
	{
		// Get reference to the RandomEyes3D component
		randomEyes = gameObject.GetComponent<RandomEyes3D>();

		/*
		randomEyes.SetBlinkSpeed(20f); // The speed of a blink		
		randomEyes.SetOpenMax(10f); // The maximum amount the eyes can open (0=max)	
		randomEyes.SetCloseMax(100f); // The maximum amount the eyes can close (100=max)
		randomEyes.SetCustomShapeRandom(true); // Enable random custom shapes
		randomEyes.SetCustomShape("smile"); // Set “smile” as the [Current Custom Shape]
		randomEyes.SetCustomShape("smile", 2.5f); // Same as above but for 2.5 seconds
		randomEyes.SetCustomShapeOverride("smile", true); // Override smile
		randomEyes.SetCustomShapeOverride("smile", 2.5f); // Same as above but for 2.5 sec
		randomEyes.SetCustomShapeBlendSpeed("smile", 3f); // Smile blendSpeed to 3
		randomEyes.SetCustomShapeRangeOfMotion("smile", 80f); // Smile rangeOfMotion to 80
		*/
		
	}
	
	void Update()
    {

		float timeSpeed = 1f;
		GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
		if(simManager != null){
			timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
		} 

		
		if (smile)
		{
			randomEyes.SetGroup("smile", true); // Activate/deactivate multi-BlendShape smile
			randomEyes.SetGroup("smile", value/timeSpeed); // Activate multi-BlendShape smile for 1.5 seconds
			smile = false;
		}

		if (blink)
		{
			randomEyes.SetGroup("blink", true); // Activate/deactivate multi-BlendShape smile
			randomEyes.SetGroup("blink", value/timeSpeed); // Activate multi-BlendShape smile for 1.5 seconds
			blink = false;
		}
		
		
	}

	float NextFloat(float min, float max){
		System.Random random = new System.Random();
		double val = (random.NextDouble() * (max - min) + min);
		return (float)val;
	}


}