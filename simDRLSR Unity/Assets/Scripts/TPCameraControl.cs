using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TPCameraControl : MonoBehaviour {

    public float turnSpeed = 4.0f;
    public Transform player;

    
    private Vector3 startPosition;
    private Quaternion startPivotRotation;

    public float sensitivity = 10f;

    private Vector3 offset;
    private Vector3 originalOffset;
    private Transform referencePosition = null;

    void Start()
    {
        
        originalOffset = offset;
        referencePosition = player.Find(Constants.REF_POSITION);
        if (referencePosition == null)
        {
            referencePosition = player;
        }
        offset = new Vector3(referencePosition.position.x, referencePosition.position.y + 8.0f, referencePosition.position.z + 7.0f);
        startPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (GetComponent<Camera>().enabled)
        {


            //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
           
                if (Input.GetMouseButton(0) && EventSystem.current.currentSelectedGameObject == null)
            {

                offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
                offset = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.forward) * offset;
                // offset = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offset;
                transform.position = referencePosition.position + offset;
                transform.LookAt(referencePosition.position);
            }
            else
            {
                offset = transform.position - referencePosition.position;
            }

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    offset = originalOffset;
                    transform.localPosition = startPosition;
                    transform.LookAt(referencePosition.position);
                }
                transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel"));
            }
        }
    }

}
