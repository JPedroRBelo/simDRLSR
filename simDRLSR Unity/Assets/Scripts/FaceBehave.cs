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



	public enum EkmanEmotions
	{
		Neutral,
        Anger,
		Contempt,
        Disgust,
        Enjoyment,
        Fear, 
        Sadness,
        Surprise

    }

	public enum EkmanGroupEmotions
	{
		Neutral,
		Positive,
		Negative
	}


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
														{"anger" , false},
														{"enjoyment" , false},
														{"sadness" , false},
														{"surprise" , false},
														{"fear" , false} };

	public Dictionary<string,EkmanEmotions> facialEmotionsToEkman = new Dictionary<string, EkmanEmotions>(){
																{"neutral",EkmanEmotions.Neutral},
																{"anger",EkmanEmotions.Anger},
																{"contempt",EkmanEmotions.Contempt},
																{"disgust",EkmanEmotions.Disgust},
																{"enjoyment",EkmanEmotions.Enjoyment},
																{"fear",EkmanEmotions.Fear},
																{"sadness",EkmanEmotions.Sadness},
																{"surprise",EkmanEmotions.Surprise}
	};

	public Dictionary<EkmanEmotions,string> EkmanToFacialEmotions = new Dictionary< EkmanEmotions, string>(){
																{EkmanEmotions.Neutral,"neutral"},
																{EkmanEmotions.Anger,"anger"},
																{EkmanEmotions.Contempt,"contempt"},
																{EkmanEmotions.Disgust,"disgust"},
																{EkmanEmotions.Enjoyment,"enjoyment"},
																{EkmanEmotions.Fear,"fear"},
																{EkmanEmotions.Sadness,"sadness"},
																{EkmanEmotions.Surprise,"surprise"}
	};
	

	public Dictionary<EkmanGroupEmotions,List<EkmanEmotions>> GroupedEmotions = new Dictionary<EkmanGroupEmotions,List<EkmanEmotions>>(){
																{EkmanGroupEmotions.Neutral,new List<EkmanEmotions>{EkmanEmotions.Neutral}},
																{EkmanGroupEmotions.Positive,new List<EkmanEmotions>{EkmanEmotions.Enjoyment}},
																{EkmanGroupEmotions.Negative,new List<EkmanEmotions>{EkmanEmotions.Sadness}}
	};



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
	

	public bool sadEye = false;
	public float xDefaultEyeOffset = 0;
	public float yDefaultEyeOffset = 0;

	private float yEyeOffset;
	private float xEyeOffset;

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

	void Awake()
	{
		animator = GetComponent<Animator>();
		blendShapes = gameObject.GetComponent<ConfigBlendShapes>();
	}

	void Start()
	{	
		yEyeOffset = 0;
		xEyeOffset = 0;
		// Get reference to the RandomEyes3D component
		//randomEyes = gameObject.GetComponent<RandomEyes3D>();
		
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
		StartCoroutine(LateStart(1));
     }
 
     IEnumerator LateStart(float waitTime)
     {
         yield return new WaitForSeconds(waitTime);
         
     }
	
	void Update()
    {

		float timeSpeed = 1f;
		GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
		if(simManager != null){
			timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
		} 
		/*
		if(executeAngry){
			facialEmotions["anger"] = true;
			executeAngry = false;
		}*/
		try{
			foreach(KeyValuePair<string, bool> emotion in facialEmotions){
				if(emotion.Value){
					float duration = emotionDuration;
					if(emotion.Key=="blink"){
						duration = blinkDuration;
						blendShapes.setEmotion(emotion.Key,true,duration/timeSpeed,"without_eyegaze");
						facialEmotions[emotion.Key] = false;
					}
					/*
					else if(emotion.Key=="anger"){
						duration = 10;
					}
					*/
					
				}
			}
		}catch{}	
			
	}
	

	void LateUpdate(){

		currentAngle = new Vector3(
			Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime*speed* Time.timeScale),
			Mathf.LerpAngle(currentAngle.y, targetAngle.y, Time.deltaTime*speed* Time.timeScale),
			Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime*speed* Time.timeScale));

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
				maxAngleX = UnityEngine.Random.Range((-xMaxLookAngle+xEyeOffset),(xMaxLookAngle+xEyeOffset));
				maxAngleY = UnityEngine.Random.Range((-yMaxLookAngle+yEyeOffset),(yMaxLookAngle+yEyeOffset));
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

	public void SetEyeOffset(float xEyeOffset,float yEyeOffset){
		this.xEyeOffset = xEyeOffset;
		this.yEyeOffset = yEyeOffset;
	}


	public void SetEyeOffset(Vector2 eyeOffSet){
		this.xEyeOffset = eyeOffSet.x;
		this.yEyeOffset = eyeOffSet.y;
	}

	public Vector2 GetEyeOffset(){
		return new Vector2(this.xEyeOffset,this.yEyeOffset);
	}


	public void ResetEyeOffset(){
		
		this.xEyeOffset = xDefaultEyeOffset;
		this.yEyeOffset = yDefaultEyeOffset;
	}

	float NextFloat(float min, float max){
		System.Random random = new System.Random();
		double val = (random.NextDouble() * (max - min) + min);
		return (float)val;
	}

	public Vector2 getDefaultEyeOffset()
	{
		return new Vector2(this.xDefaultEyeOffset,this.yDefaultEyeOffset);
	}

	public EkmanEmotions getCurrentEmotion(){
		return facialEmotionsToEkman[blendShapes.getCurrentEmotion()];
	}
	public string getNameCurrentEmotion(){
		return blendShapes.getCurrentEmotion();
	}

	public EkmanGroupEmotions getCurrentGroupEmotion(){
		EkmanEmotions emotion = facialEmotionsToEkman[blendShapes.getCurrentEmotion()];
		foreach(KeyValuePair<EkmanGroupEmotions, List<EkmanEmotions>> ege in GroupedEmotions)
		{
			if(ege.Value.Contains(emotion)){
				return ege.Key;
			}
		}
		Debug.Log("Error! Unable to search current emotion group!");
		return EkmanGroupEmotions.Neutral;
	}
	public void setConstantEmotion(EkmanEmotions ekmanEmotion){
		
		blendShapes.setEmotion(EkmanToFacialEmotions[ekmanEmotion],true,Mathf.Infinity);
		facialEmotions[EkmanToFacialEmotions[ekmanEmotion]] =true;
	}

	public void setConstantEmotion(string ekmanEmotion){
		
		setConstantEmotion(facialEmotionsToEkman[ekmanEmotion]);

	}
}