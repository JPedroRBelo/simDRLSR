using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class HumanBehavior : MonoBehaviour
{
    public bool updatePosition = false;
    public Transform dest_transform = null;
    public enum Roles {Librarian,Student,User};
    public Roles role = Roles.User;

    //Define o animator
    protected Animator animator;
    protected UnityEngine.AI.NavMeshAgent nav;
    private SimpleMovementOperations mO;


    public float distanceToReach = 0.2f;

    private Vector3 previousPosition;
    private int countUpdate;
    private Transform agentSpine;


    void Awake()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        mO = GetComponent<SimpleMovementOperations>();
        agentSpine = animator.GetBoneTransform(HumanBodyBones.Spine);
    }
    void Start()
    {

        nav.updateRotation = false;
        nav.updatePosition = updatePosition;
        previousPosition = new Vector3();
        countUpdate = 0;

    }

    // Update is called once per frame
    void Update()
    {
        nav.updatePosition = updatePosition;
        if (dest_transform != null) {
            nav.enabled = true;
            nav.isStopped = false;
            Vector3 destination = dest_transform.position;
            nav.SetDestination(destination);
            //AQUI, onde a magica acontece! Permite que o simulated position do nav.UpdatePosition seja sincronizado com o transform 
            //quando setado como false!!!!!!
            nav.nextPosition = transform.position;

            //Vector2 pos1 = new Vector2(transform.position.x, transform.position.z);
            //Vector2 pos2 = new Vector2(destination.x, destination.z);
            Vector3 pos1 = transform.position;
            Vector3 pos2 = dest_transform.position;

            float distance = (pos1 - pos2).magnitude;
            if (distance > nav.stoppingDistance)
            {
                Vector3 aux = nav.desiredVelocity;

                mO.Move(aux, false, false);
                
                if (!isV3Zero(previousPosition))
                {
                    Vector3 curMove = transform.position - previousPosition;
                    float velocity = curMove.magnitude / Time.deltaTime;
                    /*
                    if (velocity == 0 && countUpdate > 20)
                    {
                        pos1 = new Vector3(agentSpine.position.x, agentSpine.position.y, agentSpine.position.z);

                        float spineDistance = (pos1 - pos2).magnitude;
                        if (spineDistance < nav.stoppingDistance + distanceToReach)
                        {
                            Debug.Log("Success>>> " + this.name);
                        }
                        else
                        {
                            Debug.Log("Error>>> " + this.name + " Failed! Position is not reachable.");
                        }
                        previousPosition = Vector3.zero;
                        countUpdate = 0;
                    }
                    else
                    {
                        countUpdate++;
                        previousPosition = transform.position;
                    }
                    */
                }
                else
                {
                    previousPosition = transform.position;
                }

            }
            else
            {
                print("===================99999999999999=============");
                countUpdate = 0;
                mO.Move(Vector3.zero, false, false);
                previousPosition = Vector3.zero;
            }
        }
    }
    public bool isV3Zero(Vector3 a)
    {
        return Vector3.SqrMagnitude(Vector3.zero - a) < 0.0001;
    }
}
