using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraCullingMask : MonoBehaviour {
    
    private Camera cam;
    private int oldMask;
    
    void Start()
    {
        //offset = transform.position - player.position;
        //angleOffset = transform.rotation.y - player.rotation.y;
        cam = transform.GetComponent<Camera>();
        oldMask = cam.cullingMask;
        turnOffCullingMask(Constants.LAYER_SMELLPART);
    }

    /* void LateUpdate()
     {
         print(transform.rotation.y - player.rotation.y - angleOffset);
         offset = Quaternion.AngleAxis(transform.rotation.y - player.rotation.y - angleOffset, Vector3.up) * offset;
         transform.position = player.position + offset;
         transform.LookAt(player.position);

     }*/

    public void turnOffCullingMask(string name)
    {
        cam.cullingMask &= ~(1 << LayerMask.NameToLayer(name));
    }

    public void turnOnCullingMask(string name)
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer(name);
    }


    public void resetCullingMask()
    {
        cam.cullingMask = oldMask;
    }
}