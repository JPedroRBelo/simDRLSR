using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{
    /*Constantes de velocidade do Focus
     * 
     * MAXSPD = velocidade máxima, o Focus movimenta instantaneamente
     * NATURALSPD = velocidade que permite o Focus mover de forma natural até o ponto desejado
     * */
    private const float MAXSPD = 1f;
    private const float NATURALSPD = 0.05f;

    protected Animator animator;
    public enum States {S0,PosArm,Grab,Wait};
    public enum Events {None,ArmOk,GrabOk};

    
    //Flags
    public bool ikActive = true;
    private bool isRHReach = false;
    private bool isLHReach = false;

    //private int actualState;
    private int inputEvent;

    private bool isVisible = false;
    public Transform objct = null;
    private Transform leftHandObj = null;
    public Transform lookObj = null;
    private Transform RightHandPos = null;
    
    
    private Transform RightHandFocus = null;
    //private Transform RHObjectGrabCenter = null;
    private Transform leftHandPos = null;

    //Posição de referencia do RightHandFocus
    private Transform RHFocusRef = null;

    private bool itsGrab = false;
    private bool itsMove = false;
    private bool isMoved = false;

    private int[] actions;
    //private int iAction;
    private Transform posToMove;
    private Transform objToMove;

    //private Transform HandGrabPosition;

    //Velocidade de movimento do Focus
    private float spdFocusR;
    private float spdFocusL;

    //Armazena os Transforms de determinados ossos
    private Transform RightShoulder;
    private Transform Neck;
    private Transform RightHand;
    private Transform LeftHand;

    private Transform RightLowerArm;

    //Distancia para pegar
    public float RightHandReachDistance = 0.9f;
    public float LeftHandReachDistance = 0.9f;

    void Start()
    {
        //actualState = (int)States.S0;
        inputEvent = (int)Events.None;

        animator = GetComponent<Animator>();
        RightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        Neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        RightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        RightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        RightHandFocus = RightShoulder.Find("ObjectGrabCenter/HandFocus");
        RightHandPos = RightShoulder.Find("HandGrabPosition");
        //RHFocusRef = RightHand;

        focusObject(objct);
        leftHandObj = objct;
        leftHandPos = RightHandPos;
                
        actions = new int[] {1,2,3};
        //iAction = 0;
    }

    void focusObject(Transform obj)
    {
       /* Transform objGrab = obj.Find("GrabCenter/GrabHandle");
        
        if (objGrab != null)        
            RHFocusRef = objGrab;            
        else  */
            RHFocusRef = obj;
   
        lookObj = obj;
    }




    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            if (lookObj != null)
            {
                if (isVisible && ikActive)
                {
                    animator.SetFloat("HeadReach", 1, 0.5f, Time.deltaTime * 0.6f);
                    animator.SetLookAtWeight(animator.GetFloat("HeadReach"));
                    animator.SetLookAtPosition(lookObj.position);
                }                
            }
            else
            {
                animator.SetFloat("HeadReach", 0, 0.1f, Time.deltaTime * 0.3f);
                animator.SetLookAtWeight(animator.GetFloat("HeadReach"));

            }

            
                // Set the look target position, if one has been assigned              

                // Set the right hand target position and rotation, if one has been assigned
           if (RightHandFocus != null)
            {
                if (isRHReach && ikActive)
                {
                    animator.SetFloat("RightHandReach", 1, 0.1f, Time.deltaTime * 0.1f);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("RightHandReach"));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandFocus.position);
                    Vector3 dist = RightHandFocus.position - RightHandPos.position;
                    animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandFocus.rotation);


                }//if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetFloat("RightHandReach", 0, 0.1f, Time.deltaTime * 0.6f);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, animator.GetFloat("RightHandReach"));
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandFocus.position);
                    //itsGrab = true;
                }
            }

            

            if (isLHReach && ikActive)
            {
                // Set the look target position, if one has been assigned              

                // Set the right hand target position and rotation, if one has been assigned
                if (leftHandObj != null)
                {
                    
                    animator.SetFloat("LeftHandReach", 1, 0.1f, Time.deltaTime * 0.1f);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animator.GetFloat("LeftHandReach"));
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    Quaternion rotate = Quaternion.LookRotation(leftHandObj.position - leftHandPos.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, rotate);
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetFloat("LeftHandReach", 0, 0.1f, Time.deltaTime * 0.15f);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animator.GetFloat("LeftHandReach"));
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
            }
        }
    }
    
    void Update()
    {
        
        /*
        switch (actualState)
        {
            case (int)States.S0:
                switch (inputEvent)
                {
                    case (int)Events.None:

                        break;
                    default:
                        break;
                }
                break;

            case (int)States.PosArm:
                switch (inputEvent)
                {
                    case (int)Events.None:

                        break;
                    default:
                        break;
                }
                break;


            default:
                break;
        }*/


       /* if (RHFocusRef != null)
        {
            /*
            Quaternion targetRotation = Quaternion.LookRotation(RHFocusRef.parent.position - transform.position);
            Quaternion auxRotation = RHFocusRef.parent.rotation;
            Quaternion auxQuat = new Quaternion(auxRotation.x, targetRotation.y, RHFocusRef.parent.rotation.z, RHFocusRef.parent.rotation.w);
            RHFocusRef.parent.rotation = auxQuat;
            RightHandFocus.position = RHFocusRef.position;
            RightHandFocus.rotation = RHFocusRef.rotation;
            
             ./
            RightHandFocus.parent.position = RHFocusRef.position;

            Quaternion targetRotation = Quaternion.LookRotation(transform.position - RightHandFocus.parent.position );
            Quaternion auxRotation = RightHandFocus.parent.rotation;
            Quaternion auxQuat = new Quaternion(auxRotation.x, targetRotation.y, auxRotation.z, auxRotation.w);
            RightHandFocus.parent.rotation = auxQuat;           
           
        }//*/
        /*BLOCO QUE GERENCIA O IK
         * 
         *
        */
        

        if (RightHandFocus != null)
        {
            CheckIfIsReacheble();

            /*VERIFICA AÇÕES COM AS MÃOS
             *
            */

            //CheckRHandUpdates();
            if (animator.GetFloat("RightHandReach") > RightHandReachDistance && itsGrab)
            {
                //animator.GetBoneTransform(HumanBodyBones.RightHand).position = rightHandPos.position;
                //focus = rightHandPos;
                //animator.SetFloat("RightHandReach", 0.5f);
                RHFocusRef = null;
                moveObj(RightHandFocus, RightHandPos);
                lookObj.parent = RightHand;
                itsGrab = false;
                lookObj = null;
                
            }


            //CheckMovementUpdates();
            if (RHFocusRef != null)
            {
                checkSpeedFocus();
                RightHandFocus.parent.position = Vector3.Lerp(RightHandFocus.parent.position, RHFocusRef.position, spdFocusR);
                //objToMove.position = Vector3.Lerp(objToMove.position, posToMove.position, spdFocusR);
                
                if (RHFocusRef.tag == "Object")
                {
                   /* Quaternion targetRotation = Quaternion.LookRotation(transform.position - RightHandFocus.parent.position);
                    Quaternion auxRotation = RightHandFocus.parent.rotation;
                    Quaternion auxQuat = new Quaternion(auxRotation.x, targetRotation.y, auxRotation.z, auxRotation.w);
                    RightHandFocus.parent.rotation = auxQuat;*/

                    Vector3 targetPostition = new Vector3(transform.position.x, RightHandFocus.parent.position.y,transform.position.z);
                    RightHandFocus.parent.LookAt(targetPostition);
                }
                if (itsMove)
                {

                    if ((objToMove.position - RHFocusRef.position).magnitude > 0.01)
                    {
                        //objToMove.position = Vector3.Lerp(objToMove.position, posToMove.position, spdFocusR);
                        //print(posToMove + " " + RightHandPos.position);
                        isMoved = false;
                    }
                    else
                    {
                        isMoved = true;
                        itsMove = false;
                    }
                }
            }

     
        }
    }
    

    void CheckIfIsReacheble()
    {
        Vector3 directionToTarget = RightHandFocus.position - RightShoulder.position;
        float angleRHand = Vector3.Angle(transform.forward + transform.right, directionToTarget);
        float angleLHand = Vector3.Angle(transform.forward - transform.right, directionToTarget);
        float angleForward = Vector3.Angle(transform.forward, directionToTarget);
        float distance = directionToTarget.magnitude;
        //Verifica distancia e anglo da mão direita com o objeto
        if (Mathf.Abs(angleRHand) < 90 && (distance <= 0.55) || (RightHand.childCount >= 6))
        {
            isRHReach = true;
        }
        else
        {
            isRHReach = false;
        }
        //Verifica distancia da mão esquerda com o objeto

        if (Mathf.Abs(angleLHand) < 90 && distance < 1.15)
        {
            isLHReach = false;
        }
        else
        {
            isLHReach = false;
        }

        //Verifica a distância e anglo de visão com o objeto
        if (Mathf.Abs(angleForward) < 90 && distance < 10)
        {
            isVisible = true;
        }
        else
        {
            isVisible = false;
        }
    }
    void CheckRHandUpdates()
    {        
        
    }
    
    void CheckMovementUpdates()
    {
        
    }

    private void checkSpeedFocus()
    {
        if(animator.GetFloat("RightHandReach") < 0.1)
        {
            spdFocusR = MAXSPD;
        }else
        {
            spdFocusR = NATURALSPD;
        }
    }

    void CheckGrabUpdates(Transform hand)
    {
    }
    
    void moveObj(Transform obj, Transform position)
    {
        objToMove = obj;
        RHFocusRef = position;
        itsMove = true;       
    }

    public bool grabObjectRH(Transform obj)
    {
        ikActive = true;
        return true;         
    }
}

/*Relatório do dia 13/04
 * 
 * A função do objeto nao esta OK...
 * Ao invés de movimentar o objeto até a posição Grab desejada,
 * deve-se mudar o foco da mão para Grab, isto porque da maior 
 * naturalidade
 * 
 * 
 * Relatório 18/04
 * 
 * ****************IDEIA**************
 * 
 * Ao invés de criar childs GrabHandle para cada objeto, fazer isto em forma de código!!!!
 * Calcular automaticamente o valor Z da posição através do RAIO do objeto!!!!
 * 
 * 
 * Relatório 19/04
 * 
 * Nova ideia!!! Setar o GrabHandle no proprio objeto Focus e o pai de focus no centro do objeto.
 * 
 */