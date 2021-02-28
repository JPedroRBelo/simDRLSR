using System;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(MovementOperations))]
public class AICharacterControl : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public MovementOperations mO { get; private set; } // the character we are controlling
    public Transform target;                                    // target to aim for


    private void Start()
    {
        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        mO = GetComponent<MovementOperations>();

        agent.updateRotation = false;
        agent.updatePosition = true;
    }


    private void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            Vector3 aux = agent.desiredVelocity;
            mO.Move(aux, false, false);
        }
        else
        {
            mO.Move(Vector3.zero, false, false);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    
}
 

