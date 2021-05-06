using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckTest : MonoBehaviour
{

    public Transform target;
    public float RotationSpeed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         Vector3 relativePos = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, 
                                          rotation, Time.deltaTime * RotationSpeed);
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);
    }
}
