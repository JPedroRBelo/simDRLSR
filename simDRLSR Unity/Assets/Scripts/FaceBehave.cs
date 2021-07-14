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

public class FaceBehave : MonoBehaviour
{
	public bool randomBlink = true;

	public float value = 2.5f;
	public float blinkDuration = 0.15f;
	public float minBlinkTime = 0.15f;
	public float maxBlinkTime = 1f;

	public float emotionDuration = 0.5f;

	public Dictionary<string, bool> facialEmotions = new Dictionary<string,bool>(){
														{"smile", false},
														{"blink" , false},	
														{"angry" , false},
														{"happy" , false},
														{"sad" , false},
														{"surprise" , false},
														{"fear" , false} };
	/*
	public bool smile = false;
	public bool blink = false;
	public bool angry = false;
	public bool happy = false;
	public bool sad = false;
	public bool surprise = false;
	public bool fear = false;
	*/

	private Transform leftEye;
    private Transform rightEye;
    
    public Transform lookTarget;
    
    private Animator animator;
    private long startLookTime;
	private long startBlinkTime;
    public long maxLookTime = 300;
    public int xMaxLookAngle = 45;
    public int yMaxLookAngle = 45;
    public float speed = 5f;
	private float blinkTime;

    private float xangle;
    private float yangle;
    private float minAngleX;
    private float minAngleY;
    private float maxAngleX;
    private float maxAngleY;

	private ConfigBlendShapes blendShapes;


    private Vector3 originalLeftEyeAngle;
    private Vector3 originalRightEyeAngle;
    private Vector3 currentAngle;
    private Vector3 targetAngle;


	public Transform camera;
	
		//private bool flag = false;

	public void Awake()
    {
        animator = GetComponent<Animator>();

    }

	void Start()
	{
		// Get reference to the RandomEyes3D component
		//randomEyes = gameObject.GetComponent<RandomEyes3D>();
		blendShapes = gameObject.GetComponent<ConfigBlendShapes>();
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

		    leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
			rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);
			startLookTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			startBlinkTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			originalLeftEyeAngle = leftEye.rotation.eulerAngles;
			originalRightEyeAngle = rightEye.rotation.eulerAngles;
			minAngleX = leftEye.localEulerAngles.x;
			minAngleY = leftEye.localEulerAngles.y;
			maxAngleX = minAngleX;
			maxAngleY = minAngleY;
			currentAngle = transform.eulerAngles;
			targetAngle =   new Vector3(maxAngleX, maxAngleY, 0f);
			blinkTime = UnityEngine.Random.Range(minBlinkTime*1000, maxBlinkTime*1000);
		
	}
	
	void Update()
    {

		float timeSpeed = 1f;
		GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
		if(simManager != null){
			timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
		} 
		try{
			foreach(KeyValuePair<string, bool> emotion in facialEmotions){
				if(emotion.Value){
					float duration = emotionDuration;
					if(emotion.Key=="blink"){
						duration = blinkDuration;
					}
					blendShapes.setEmotion(emotion.Key,true,duration/timeSpeed);
					facialEmotions[emotion.Key] = false;
				}
			}
		}catch{}		
		
	}

	void LateUpdate(){

		currentAngle = new Vector3(
			Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime*speed),
			Mathf.LerpAngle(currentAngle.y, targetAngle.y, Time.deltaTime*speed),
			Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime*speed));

		leftEye.localEulerAngles = currentAngle;
		rightEye.localEulerAngles = currentAngle;

		float timeSpeed = 1f;
		GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
		if(simManager != null){
			timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
		}     
		long timeNow =  DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		if ((timeNow-startLookTime)>maxLookTime/timeSpeed){
				startLookTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				maxAngleX = UnityEngine.Random.Range(-xMaxLookAngle,xMaxLookAngle);
				maxAngleY = UnityEngine.Random.Range(-yMaxLookAngle,yMaxLookAngle);
				targetAngle =  new Vector3(maxAngleX, maxAngleY, 0f);
		}
		if(randomBlink){
			if ((timeNow-startBlinkTime)>blinkTime/timeSpeed){
					startBlinkTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					blinkTime = UnityEngine.Random.Range(minBlinkTime*1000, maxBlinkTime*1000);
					facialEmotions["blink"] = true;
			}
		}

        
        
    }


	float NextFloat(float min, float max){
		System.Random random = new System.Random();
		double val = (random.NextDouble() * (max - min) + min);
		return (float)val;
	}


}