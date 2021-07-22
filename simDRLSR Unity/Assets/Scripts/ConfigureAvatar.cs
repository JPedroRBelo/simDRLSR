using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Security.Cryptography;
using System;
using System.ComponentModel;
using System.Collections.Specialized;
using UnityEngine.AI;
using System.Diagnostics.Eventing.Reader;
//using System.Diagnostics;

//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(SimpleMovementOperations))]
//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(AudioSource))]
//[RequireComponent(typeof(AgentSpeech))]
//[RequireComponent(typeof(HearingProperties))]
//[RequireComponent(typeof(VisionProperties))]
//[RequireComponent(typeof(TouchManager))]
//[RequireComponent(typeof(TasteManager))]
//[RequireComponent(typeof(SmellManager))]
//[RequireComponent(typeof(HearingManager))]
//[RequireComponent(typeof(VisionManager))]
[RequireComponent(typeof(AvatarColliderManager))]
[RequireComponent(typeof(AgentHeadMovement))]
[RequireComponent(typeof(AgentBehave))]
[RequireComponent(typeof(AgentMovement))]
[RequireComponent(typeof(AgentInteraction))]
[RequireComponent(typeof(AvatarColliderManager))]
[RequireComponent(typeof(AvatarCommandsManager))]
[RequireComponent(typeof(AvatarBehaviors))]

public class ConfigureAvatar : MonoBehaviour
{
    //public string avatarConfigurationPath = "Avatar Configuration Prefabs";
    public float avatarScale = 2f;

    [Header("Configure Body Parts")]
    
    public bool configAnklePosition = true;
    public bool configThirdPersonCamera = false;
    public bool configCameraHead = false;
    public bool configCenterFocus = true;
    public bool configHandGrabLeft = true;
    public bool configHandGrabRight = true;
    public bool configObjectGrabLeft = true;
    public bool configObjectGrabRight = true;
    public bool configRefPosToGrabLeft = true;
    public bool configRefPosToGrabRight = true;
    public bool configSmellSense = false;
    public bool configTasteSense = false;
    public bool configNavMeshAgent = true;
    public bool configHeadCollider = true;

    [Header("Nav Mesh Settings")]

    public float baseOffset = 0f;
    public float speed = 2f;
    public float angularSpeed = 120f;
    public float acceleration = 8f;
    public float stoppingDistance = 1f;
    public bool autoBraking = true;
    public float radius = 0.2f;
    public float height = 1.8f;
    public int priority = 50;
    public bool autoTraverseOffMeshLink = true;
    public bool autoRepath = true;



    private GameObject prefabAnklePosition,gOAnklePosition;
    private GameObject prefabThirdPersonCamera, gOThirdPersonCamera;
    private GameObject prefabCameraHead, gOCameraHead;
    private GameObject prefabCenterFocus, gOCenterFocus;
    private GameObject prefabHandGrabLeft, gOHandGrabLeft;
    private GameObject prefabHandGrabRight, gOHandGrabRight;
    private GameObject prefabObjectGrabLeft, gOObjectGrabLeft;
    private GameObject prefabObjectGrabRight, gOObjectGrabRight;
    private GameObject prefabRefPosToGrabLeft, gORefPosToGrabLeft;
    private GameObject prefabRefPosToGrabRight, gORefPosToGrabRight;
    private GameObject prefabSmellSense, gOSmellSense;
    private GameObject prefabTasteSense, gOTasteSense;

    private Transform head;
    private Transform spine;
    private Transform leftShoulder;
    private Transform rightShoulder;
    private Transform leftHand;
    private Transform rightHand;

    
    private Animator animator;
    private NavMeshAgent navMeshAgent;


    public void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        InitComponents();
    }
    private void InitComponents()
    {
        prefabAnklePosition = (GameObject)Resources.Load("AvatarPrefabs/AnklePosition", typeof(GameObject));
        prefabThirdPersonCamera = (GameObject)Resources.Load("AvatarPrefabs/AgentThirdPersonCamera", typeof(GameObject));
        prefabCameraHead = (GameObject)Resources.Load("AvatarPrefabs/CameraHead", typeof(GameObject));
        prefabCenterFocus = (GameObject)Resources.Load("AvatarPrefabs/CenterFocus", typeof(GameObject));
        prefabHandGrabLeft = (GameObject)Resources.Load("AvatarPrefabs/HandGrabPositionLeft", typeof(GameObject));
        prefabHandGrabRight = (GameObject)Resources.Load("AvatarPrefabs/HandGrabPositionRight", typeof(GameObject));
        prefabObjectGrabLeft = (GameObject)Resources.Load("AvatarPrefabs/ObjectGrabCenterLeft", typeof(GameObject));
        prefabObjectGrabRight = (GameObject)Resources.Load("AvatarPrefabs/ObjectGrabCenterRight", typeof(GameObject));
        prefabRefPosToGrabLeft = (GameObject)Resources.Load("AvatarPrefabs/RefPosToGrabLeft", typeof(GameObject));
        prefabRefPosToGrabRight = (GameObject)Resources.Load("AvatarPrefabs/RefPosToGrabRight", typeof(GameObject));
        prefabSmellSense = (GameObject)Resources.Load("AvatarPrefabs/SmellSensor", typeof(GameObject));
        prefabTasteSense = (GameObject)Resources.Load("AvatarPrefabs/TasteSensor", typeof(GameObject));
        head = animator.GetBoneTransform(HumanBodyBones.Head);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

    }

    private void setNavMeshAgentParameters()
    {
        navMeshAgent.baseOffset = baseOffset;
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;
        navMeshAgent.acceleration = acceleration;
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.autoBraking = autoBraking;
        navMeshAgent.radius = radius;
        navMeshAgent.height = height;
        navMeshAgent.avoidancePriority = priority;
        navMeshAgent.autoTraverseOffMeshLink = autoTraverseOffMeshLink;
        navMeshAgent.autoRepath = autoRepath;
    }




    private GameObject SetTransformParent(Transform parent, GameObject child){
        GameObject gOChild = Instantiate(child);
        gOChild.name = child.name;
        gOChild.transform.parent = parent;
        gOChild.transform.localPosition = child.transform.position;
        return (gOChild);
    }

    public void Configure()
    {
        Awake();
        Remove();
        if (configAnklePosition)
        {
            gOAnklePosition =  SetTransformParent(transform, prefabAnklePosition);
        }
        if (configThirdPersonCamera)
        {
            gOThirdPersonCamera = SetTransformParent(transform, prefabThirdPersonCamera);
        }
        
        if (configCameraHead)
        {
            
            if (head)
            {
                gOCameraHead = SetTransformParent(head, prefabCameraHead);
            }
            else
            {
                Debug.Log("Cant Find HEAD");
            }
        }
        else
        {
            VisionManager vision = GetComponent<VisionManager>();
            if(vision !=null){
                vision.enabled = false;
            }
            
        }

        if (configCenterFocus)
        {
            
            if (spine)
            {
                gOCenterFocus = SetTransformParent(spine, prefabCenterFocus);
            }
            else
            {
                Debug.Log("Cant Find SPINE");
            }
        }
        if (configHandGrabLeft)
        {
            if (leftShoulder)
            {
                gOHandGrabLeft = SetTransformParent(leftShoulder, prefabHandGrabLeft);
            }
            else
            {
                Debug.Log("Cant Find LEFT SHOULDER");
            }
        }
        if (configHandGrabRight)
        {
            if (rightShoulder)
            {
                gOHandGrabRight = SetTransformParent(rightShoulder, prefabHandGrabRight);
            }
            else
            {
                Debug.Log("Cant Find RIGHT SHOULDER");
            }
        }
        if (configObjectGrabLeft)
        {
            if (leftShoulder)
            {
                gOObjectGrabLeft = SetTransformParent(leftShoulder, prefabObjectGrabLeft);
            }
            else
            {
                Debug.Log("Cant Find LEFT SHOULDER");
            }
        }
        if (configObjectGrabRight)
        {
            if (rightShoulder)
            {
                gOObjectGrabRight = SetTransformParent(rightShoulder, prefabObjectGrabRight);
            }
            else
            {
                Debug.Log("Cant Find RIGHT SHOULDER");
            }
        }
        if (configRefPosToGrabLeft)
        {
            if (leftHand)
            {
                gORefPosToGrabLeft = SetTransformParent(leftHand, prefabRefPosToGrabLeft);
            }
            else
            {
                Debug.Log("Cant Find LEFT HAND");
            }
        }
        if (configRefPosToGrabRight)
        {
            if (rightHand)
            {
                gORefPosToGrabRight = SetTransformParent(rightHand, prefabRefPosToGrabRight);
            }
            else
            {
                Debug.Log("Cant Find RIGHT HAND");
            }
        }
        if (configTasteSense)
        {

            if (head)
            {
                gOTasteSense = SetTransformParent(head, prefabTasteSense);
            }
            else
            {
                Debug.Log("Cant Find HEAD");
            }
        }
        if (configSmellSense)
        {

            if (head)
            {
                gOSmellSense = SetTransformParent(head, prefabSmellSense);
            }
            else
            {
                Debug.Log("Cant Find HEAD");
            }
        }
        if (configHeadCollider)
        {

            if (head)
            {
                SphereCollider m_Collider = head.gameObject.AddComponent<SphereCollider>();
                m_Collider.center = new Vector3(0f, 0.05f, 0.01f);
                m_Collider.radius = 0.09f;
            }
            else
            {
                Debug.Log("Cant Find HEAD");
            }
        }
        if (configNavMeshAgent)
        {
            setNavMeshAgentParameters();
        }
        transform.localScale = new Vector3(avatarScale, avatarScale, avatarScale);

    }

    public bool DestroyGameObject(Transform parent, GameObject child)
    {

        Transform tfchild = parent.Find(child.name);
        //Transform tAnklePosition = transform.Find("AnklePosition");
        if (tfchild != null)
        {
            bool flag = true;
            while (flag)
            {
                DestroyImmediate(tfchild.gameObject);
                tfchild = parent.Find(child.name);
                flag = (tfchild != null);
            }         

            return (true);
        }
        return (false);

    }

    public void Remove()
    {
        Awake();

        if (configAnklePosition)
        {
            DestroyGameObject(transform, prefabAnklePosition);
        }
        if (configThirdPersonCamera)
        {
            DestroyGameObject(transform, prefabThirdPersonCamera);
        }

        if (configCameraHead)
        {
            DestroyGameObject(head, prefabCameraHead);
        }

        if (configCenterFocus)
        {
            DestroyGameObject(spine, prefabCenterFocus);
        }

        if (configHandGrabLeft)
        {
            DestroyGameObject(leftShoulder, prefabHandGrabLeft);
        }
        if (configHandGrabRight)
        {
            DestroyGameObject(rightShoulder, prefabHandGrabRight);
        }
        if (configObjectGrabLeft)
        {
            DestroyGameObject(leftShoulder, prefabObjectGrabLeft);
        }
        if (configObjectGrabRight)
        {
            DestroyGameObject(rightShoulder, prefabObjectGrabRight);
        }
        if (configRefPosToGrabLeft)
        {
            DestroyGameObject(leftHand, prefabRefPosToGrabLeft);
        }
        if (configRefPosToGrabRight)
        {
            DestroyGameObject(rightHand, prefabRefPosToGrabRight);
        }
        if (configTasteSense)
        {
            DestroyGameObject(head, prefabTasteSense);
        }
        if (configSmellSense)
        {
            DestroyGameObject(head, prefabSmellSense);
        }
        transform.localScale = new Vector3(1, 1, 1);
    }
}