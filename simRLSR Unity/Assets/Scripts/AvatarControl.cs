using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
[RequireComponent(typeof(Animator))]
public class AvatarControl : MonoBehaviour {


    private Transform lookFocus;
    public Transform pivotFocus;
    private Animator animator;
    public float speedLook = 75.0f;
    private float minRotation = -100;
    private float maxRotation = 100;
    private bool blocked;
    private bool desativated;

    void Start () {
        blocked = false;
        desativated = true;
		animator = GetComponent<Animator>();
        lookFocus = pivotFocus.Find("LookFocus");
        Debug.Log("RHS>>> " + this.name + " " + this.GetType() + " is ready.");
    }    
	
	void Update () {
        if (!blocked && !desativated)
        {

            animator.SetFloat("VSpeed", (Input.GetAxis("Vertical") + 1) / 2);
            animator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));

            if (Input.GetButtonDown("Jump"))
            {
                animator.SetBool("Jumping", true);
                Invoke("StopJumping", 0.1f);
            }

            /*if (Input.GetButtonDown("Sit"))
            {
                animator.SetBool("Sitting", true);
            }*/


            //Controlling an animation with procedural coded motion		
            //If the Q key is being held down AND no action is currently being performed (we'll go into more about the actions later).
            if (Input.GetKey("a") && (animator.GetInteger("CurrentAction") == 0))
            {

                //Rotate the character procedurally based on Time.deltaTime.  This will give the illusion of moving
                //Even though the animations don't have root motion
                transform.Rotate(Vector3.down * Time.deltaTime * 100.0f);

                //Also, IF we're currently standing still (both vertically and horizontally)
                if ((Input.GetAxis("Vertical") == 0f))
                {
                    //change the animation to the 'inplace' animation
                    animator.SetBool("TurningLeft", true);
                }

            }
            else
            {
                //Else here means if the Q key is not being held down
                //Then we make sure that we are not playing the turning animation
                animator.SetBool("TurningLeft", false);
            }

            //Same thing for E key, just rotating the other way!
            if (Input.GetKey("d") && (animator.GetInteger("CurrentAction") == 0))
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 100.0f);
                if ((Input.GetAxis("Vertical") == 0f))
                {
                    animator.SetBool("TurningRight", true);
                }

            }
            else
            {
                animator.SetBool("TurningRight", false);
            }


            //Something to keep in mind when controlling motion through code is that it can be difficult to 'match' the animation speed.
            //For example, our rotation speed is handled by the "Vector3 * Time.deltaTime * 100.0f" portion.  Changing that float
            //to another number will decrease or increase your rate of rotation and it may not match your animation anymore.
            

            //Toggleable Animations
            //Sometimes you may want to turn an animation on and off yourself.
            //Here we want the behaviour to be a little different.  We'll check if the input button is pressed and then check what the current state is.
            //If the CurrentAction is 0, we will change it to 2. If it is 2, we will change it to 0.  This creates a toggle effect when the button is pressed.
            //With this method we aren't looking for a button being held down OR being released, but just using the key down to listen for key presses.

            if (Input.GetKeyDown("1"))
            {
                if (animator.GetInteger("CurrentAction") == 0)
                {
                    animator.SetInteger("CurrentAction", 1);
                }
                else if (animator.GetInteger("CurrentAction") == 1)
                {
                    animator.SetInteger("CurrentAction", 0);
                }
            }


            //Combining methods.
            //You can combine all of these methods (and much more advanced logic) in many ways!  Let's go over an example.  We want our character to kneel down
            //Stay in a kneeling loop idle, then stand up when we tell them to.  For this we'll need to combine a triggered 'transition' animation with looping
            //animations.  You can see in the Animator Controller that Action3 (kneeling) is actually comprised of 3 animations.  "kneeling down", "kneeling idle"
            //and "kneeling stand".

            //The first is "kneeling down".  The intro requirement is the same as before, CurrentAction = 3.  We can control this using the same "SetInteger" method
            //we used in our previous examples.  However, the exit transition is based on exit time.  So as soon as that intro "kneeling down" animation plays
            //The animation will transition into a kneeling idle.  Now we want the character to remain there until we tell it to get up, so the exit transition
            //From kneeling idle is "currentaction = 0".  This means we'll need to set up a toggle just like in Example #4.  When we toggle from 3 to 0, we'll transition
            //Into the kneeling stand animation, which will get our character back to their feet.  Finally the exit transition for kneeling stand is exit time, so
            //As soon as they are done standing up they will go the next state ("idle/walk").

            if (Input.GetKeyDown("2"))
            {
                if (animator.GetInteger("CurrentAction") == 0)
                {
                    animator.SetInteger("CurrentAction", 2);
                }
                else if (animator.GetInteger("CurrentAction") == 2)
                {
                    animator.SetInteger("CurrentAction", 0);
                }
            }

            //Example #5:  Combining animations with Layers
            //Unity has a layering system inside of Mecanim that you can use to control animations on specific parts of your character.  For this example, we'll play a wave
            //animation but only on part of the body.  Our layer is set to override so it will completely replace the animation on the base layer.
            //First, we define the region we want to show by creating an avatar body mask. You can see our mask by going to the Mixamo > Demo folder and finding
            //the "UpperBodyMask" asset.  Click on it and the inspector will show the areas animation will play in green and where animations will not play in red.


            //We use the GetKeyDown instead of "GetKey".  GetKey means "while the button is held down".  GetKeyDown will only set once,
            //when the key is initially depressed.
            if (Input.GetKeyDown("3"))
            {
                //We want to turn the layer's weight up to 1.  You canconsider weight as influence and by default it is 0.
                animator.SetLayerWeight(1, 1f);
                animator.SetInteger("CurrentAction", 3);
            }

            //We also need to reset the CurrentAction integer so that it doesn't immediately transition back into the animation.
            if (Input.GetKeyUp("3"))
            {
                //Let's also turn the layer's weight back to 0 so it's not influencing the base layer at all.
                animator.SetInteger("CurrentAction", 0);
            }

            pivotFocus.Rotate(Vector3.up * Time.deltaTime * speedLook * Input.GetAxis("Horizontal2"));
            pivotFocus.Rotate(Vector3.right * Time.deltaTime * speedLook * -Input.GetAxis("Vertical2"));
            Vector3 currentRotation = pivotFocus.localRotation.eulerAngles;

            currentRotation.y = ClampAngle(currentRotation.y, minRotation, maxRotation);
            currentRotation.x = ClampAngle(currentRotation.x, minRotation, maxRotation);
            //  currentRotation.z = 0;
            pivotFocus.localRotation = Quaternion.Euler(currentRotation);

            //



            //If you are holding down the W button to walk and press the 3 button to wave you'll see that the body continues to walk and swing it's arm while just
            //the unmasked arm, head and torso are affected by the wave.
        }
        else
        {
            animator.SetFloat("VSpeed", 0.5f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //print(EventSystem.current.currentSelectedGameObject == dropActions.gameObject);
                GameObject aux = EventSystem.current.currentSelectedGameObject;
                if (aux != null)
                {
                    //print(aux.name);
                    if (aux.layer == LayerMask.NameToLayer("UI"))
                    {
                        blocked = true;
                    }
                    else
                    {
                        blocked = false;
                    }
                }
                else
                {

                    blocked = false;

                }
            }
        }
       
    }
	
	//This method is called after jumping is started to stop the jumping!
	void StopJumping(){
        animator.SetBool ("Jumping", false);
	}

    void OnAnimatorIK()
    {
        if (animator)
        {                
                if (lookFocus != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookFocus.position);
                }       

            }        
    }
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void activate()
    {
        desativated = false;
    }

    public void desactivate()
    {
        desativated = true;
    }
   
}




