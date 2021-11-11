using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDirection : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 forward;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        foreach (GameObject person in GameObject.FindGameObjectsWithTag("Person"))
        {
           Vector3 dirFromAtoB = (person.transform.position - transform.position).normalized;
           float dotProdAB = Vector3.Dot(dirFromAtoB, transform.forward);

           Vector3 dirFromBtoA = (transform.position - person.transform.position ).normalized;
           float dotProdBA = Vector3.Dot(dirFromBtoA,  person.transform.forward);
            GameObject ball1 = transform.Find("Ball").gameObject;
             GameObject ball2 = person.transform.Find("Ball").gameObject;
            if(dotProdAB>=0.75f){
                ball1.SetActive(true);
            }else{
                ball1.SetActive(false);
            }

            if(dotProdBA>=0.75f){
                ball2.SetActive(true);
            }else{
                ball2.SetActive(false);
            }
            
                
        }
    }
}
