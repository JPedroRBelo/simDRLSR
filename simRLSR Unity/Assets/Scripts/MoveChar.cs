using UnityEngine;
using System.Collections;

public class MoveChar : MonoBehaviour {

    


    private Animator myAnimator;
    private Transform object1;
    private Transform handR;
    private string objectInHandR;
    private Transform focus;
    private bool isTurningRight;
    private bool isTurningLeft;
    // Use this for initialization

        
    void Start () {
		myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        

        myAnimator.SetFloat("VSpeed", (Input.GetAxis("Vertical") + 1) / 2);
        myAnimator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("Jump"))
        {
            myAnimator.SetBool("Jumping", true);
            Invoke("StopJumping", 0.1f);
        }


        if (Input.GetKey("q"))
        { 
            transform.Rotate(Vector3.down * Time.deltaTime * 100.0f);
            if ((Input.GetAxis("Vertical") == 0f) && (Input.GetAxis("Horizontal") == 0))
            {
                myAnimator.SetBool("TurningLeft", true);
            }

        }
        else
        {
            myAnimator.SetBool("TurningLeft", false);
        }

        if (Input.GetKey("e"))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * -100.0f);
            if ((Input.GetAxis("Vertical") == 0f) && (Input.GetAxis("Horizontal") == 0))
            {
                myAnimator.SetBool("TurningRight", true);
            }
        }
        else
        {
            myAnimator.SetBool("TurningRight", false);
        }

        if (Input.GetKeyDown("1"))
        {
            if (myAnimator.GetInteger("CurrentAction") == 0)
            {
                myAnimator.SetInteger("CurrentAction", 1);
            }
            else if (myAnimator.GetInteger("CurrentAction") == 1)
            {
                myAnimator.SetInteger("CurrentAction", 0);
            }
        }

        if (Input.GetKeyDown("2"))
        {
            if (myAnimator.GetInteger("CurrentAction") == 0)
            {
                myAnimator.SetInteger("CurrentAction", 2);
            }
            else if (myAnimator.GetInteger("CurrentAction") == 2)
            {
                myAnimator.SetInteger("CurrentAction", 0);
            }
        }

        if (Input.GetKeyDown("3"))
        {
            myAnimator.SetLayerWeight(1, 1f);
            myAnimator.SetInteger("CurrentAction", 3);
        }

        if (Input.GetKeyUp("3"))
        {
            myAnimator.SetInteger("CurrentAction", 0);
        }

        if (Input.GetKeyDown("4"))
        {
            myAnimator.SetLayerWeight(1, 1f);
            myAnimator.SetInteger("CurrentAction", 4);
        }

        if (Input.GetKeyUp("4"))
        {
            myAnimator.SetInteger("CurrentAction", 0);
        }

    }


    void StopJumping(){
		myAnimator.SetBool ("Jumping", false);
	}
	
}
