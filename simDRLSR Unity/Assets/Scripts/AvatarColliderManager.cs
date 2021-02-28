using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AvatarColliderManager : MonoBehaviour {

    private NavMeshAgent nMA;
    private  Transform anklePosition;
    public float distOfGround = 0.08f;
    

    private float initNMABaseOffset;
    private float initNMARadius;
    private float initNMAHeight;

    private float initDiffValue;


    private Transform agentRightFoot;
    private Transform agentLeftFoot;
    private Animator animator;


    // Use this for initialization
    void Start () {
        anklePosition = transform.Find(Constants.POS_ANKLE);
        animator = GetComponent<Animator>();
        nMA = GetComponent<NavMeshAgent>();
        initNMABaseOffset = nMA.baseOffset;
        initNMARadius = nMA.radius;
        initNMAHeight = nMA.height;
        initDiffValue = anklePosition.localPosition.y- initNMABaseOffset;
        agentRightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        agentLeftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);        
    }
	
	// Update is called once per frame
	void Update () {

        AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorInfo.tagHash == Animator.StringToHash("FootTransition"))
        {
            float additionalValue =0.2f;
            nMA.baseOffset = transform.position.y - agentLeftFoot.position.y + additionalValue  ;
            nMA.height = initNMAHeight - anklePosition.localPosition.y;
        }
        else
        {

            float auxBaseOffset = initNMABaseOffset - anklePosition.localPosition.y;
            //nMA.baseOffset = initDiffValue;
            nMA.baseOffset = auxBaseOffset + distOfGround;
            nMA.height = initNMAHeight - anklePosition.localPosition.y;
        }
      
    }
}
