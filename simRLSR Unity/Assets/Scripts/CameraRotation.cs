using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    // Use this for initialization
    public float speed = 8f;
    public Transform cameraCenter;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraCenter.Rotate(new Vector3(0, 0,speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraCenter.Rotate(new Vector3(0, 0 ,- speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            cameraCenter.Rotate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraCenter.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
        }
    }

}