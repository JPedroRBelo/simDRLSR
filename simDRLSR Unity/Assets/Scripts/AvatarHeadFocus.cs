using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarHeadFocus : MonoBehaviour {

    protected Animator animator;
    public Transform robot;
    //Define alguma partes do corpo do agente
    private Transform agentHead;
    public bool ik = true;
    private Transform robotHead;


	// Use this for initialization
	void Start () {

        animator = GetComponent<Animator>();
        robotHead = robot.GetComponent<AgentHeadMovement>().getRobotHead();

    }
	
	// Update is called once per frame
	void Update () {

		
	}

    void OnAnimatorIK()
    {
        if (animator && robot!=null)
        {
           
                if (ik)
                {

                        animator.SetFloat("HeadReach", 1, 0.5f, Time.deltaTime * 0.6f);
                        animator.SetLookAtWeight(animator.GetFloat("HeadReach"));
                        animator.SetLookAtPosition(robotHead.position);
                }
                else
                {
                    animator.SetFloat("HeadReach", 0, 0.1f, Time.deltaTime * 0.3f);
                    animator.SetLookAtWeight(animator.GetFloat("HeadReach"));
                }
            }
        
    }
}
