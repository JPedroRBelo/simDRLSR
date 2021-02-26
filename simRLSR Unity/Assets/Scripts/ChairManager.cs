using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Helper
{
    public static void LerpTransform(this Transform t1, Transform t2, float t)
    {
        t1.position = Vector3.Lerp(t1.position, t2.position, t);
        t1.rotation = Quaternion.Lerp(t1.rotation, t2.rotation, t);
    }
}
public class ChairManager : MonoBehaviour
{
    // Start is called before the first frame update

    private bool free;
    private bool sitting;
    private bool standing;
    private Transform occupant;
    private Transform initialPosition;
    private Transform sitPosition;



    void Start()
    {
        free = true;
        sitting = false;
        standing = false;
        sitPosition = transform.Find("ReferencePosition");
        if(sitPosition == null)
        {
            sitPosition = transform;
        }
        GameObject emptyGO = new GameObject();
        initialPosition = emptyGO.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (sitting)
        {
            Vector2 occupant2dposition = new Vector2(occupant.position.x, occupant.position.z);
            Vector2 sitPosition2dposition = new Vector2(sitPosition.position.x, sitPosition.position.z);
            if ((Quaternion.Dot(occupant.rotation, sitPosition.rotation) < 0.9)&& (Vector3.Distance(occupant2dposition, sitPosition2dposition) < 1f))
            {
                occupant.position = new Vector3(sitPosition.position.x, occupant.position.y, sitPosition.position.z);
                occupant.rotation = sitPosition.rotation;
                 sitting = false;
            }
            else
            {
                occupant.LerpTransform(sitPosition, 2*Time.deltaTime);                
                
            }
            
        }
        if (standing)
        {
            if ((Quaternion.Dot(occupant.rotation, initialPosition.rotation) < 1f) && (Vector3.Distance(occupant.position, initialPosition.position) <1f))
            {
                occupant.position = new Vector3(initialPosition.position.x, occupant.position.y, initialPosition.position.z);
                standing = false;
                free = true;
                occupant = null;
            }
            else
            {
                occupant.LerpTransform(initialPosition, 2f*Time.deltaTime);
            }
        }
    }

    public void sit(Transform occupant)
    {
        free = false;
        this.occupant = occupant;
        initialPosition.position = occupant.position;
        initialPosition.rotation = occupant.rotation;
        sitting = true;
    }

    public void stand(Transform occupant)
    {        
        this.occupant = occupant;        
        standing = true;
    }

    public bool isFree()
    {
        return free;
    }

    
}
