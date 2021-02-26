using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Hand{

    //Mão esquerda ou direita?
    private Hands actHand; 
    //Armazena o BodyPart
    public Transform hand;
    //Auxilia no foco e da movimentação (IK do Unity) da mão
    public Transform objInHand;
    public Transform focus;
    public Vector3 focusDesiredPosition;
    //Referência para posicionar o objeto na mão
    public Transform refPosGrabObj;
    //Armazena a referencia de posição de descanso da mão
    private Transform restPosition;
    //Auxilia na estabilização da mão
    private Vector3 auxObjVector;
    //Velocidade do foco
    public float spdFocus;
    //Ligar o IK
    public bool ik;

    private Command command;

    private string animatorParam;

    private bool resetHand;

    public Vector3 originalFocusPosition;

    public Transform shoulder;



    public Hand(Hands actHand,Transform hand, Transform shoulder)
    {
        this.actHand = actHand;
        this.hand = hand;
        
        focus = shoulder.Find("ObjectGrabCenterLeft/HandFocus");
        
        restPosition = shoulder.Find("HandGrabPositionLeft");
        refPosGrabObj = hand.Find("RefPosToGrabLeft");
        if (actHand==Hands.Right)
        {
            focus = shoulder.Find("ObjectGrabCenterRight/HandFocus");
            restPosition = shoulder.Find("HandGrabPositionRight");
            refPosGrabObj = hand.Find("RefPosToGrabRight");
        }
        this.shoulder = shoulder;
        focusDesiredPosition = focus.position;
        spdFocus = 30f;
        ik = false;
        objInHand = null;
        command = null;
         //Flags indicando a necessidade de executar ResetCrouch e ResetSpine
        resetHand = false;
        switch (actHand)
        {
            case Hands.Left:
                animatorParam = "LeftHandReach";
                break;
            default:
                animatorParam = "RightHandReach";
                break;
        }
    }

    public bool hold(Transform obj)
    {
        if (isHandFree())
        {
            obj.parent = hand;
            Rigidbody auxRigidbody = obj.GetComponent<Rigidbody>();
            if(auxRigidbody!=null)
                auxRigidbody.isKinematic = true;
            obj.position = refPosGrabObj.position;//new Vector3(refPosGrabObj.position.x,obj.position.y, refPosGrabObj.position.z);
            TouchProperties tP = obj.GetComponent<TouchProperties>();
            if (tP != null)
            {
                tP.exertPressure(true);
            }
            objInHand = obj;
            
            return true;
        }
        return false;
    }

    public bool drop(Vector3 position)
    {
        if (!isHandFree())
        {
            objInHand.parent = null;
            objInHand.position = new Vector3(position.x, objInHand.position.y, position.z);
            Rigidbody auxRigidbody = objInHand.GetComponent<Rigidbody>();
            if (auxRigidbody != null)
                auxRigidbody.isKinematic = false;
            TouchProperties tP = objInHand.GetComponent<TouchProperties>();
            if (tP != null)
            {
                tP.exertPressure(false);
            }
            objInHand = null;          
            return true;
        }
        return false;
    }

    public bool setActivate(Transform switchObj,bool on)
    {
        switch (switchObj.tag)
        {
            case Constants.TAG_DOOR:
                DoorManager doorManager = switchObj.GetComponent<DoorManager>();
                if (doorManager != null)
                {
                    if (on)
                        doorManager.open();
                    else
                        doorManager.close();
                    return true;
                }
                break;
            case Constants.TAG_HAND:
                return true;
                break;
            case Constants.TAG_SIDEDOOR:
                SideDoor sideDoor = switchObj.GetComponent<SideDoor>();
                if (sideDoor != null)
                {
                    if (on)
                        sideDoor.open();
                    else
                        sideDoor.close();
                    return true;
                }
                break;
            case Constants.TAG_DRAWER:
                DrawerManager drawerManager = switchObj.GetComponent<DrawerManager>();
                if (drawerManager != null)
                {
                    if (on)
                        drawerManager.open();
                    else
                        drawerManager.close();
                    return true;
                }
                break;
            case Constants.TAG_SWITCH:
                SwitchManager switchManager = switchObj.GetComponent<SwitchManager>();
                if (switchManager != null)
                {
                    if (on)
                        switchManager.on();
                    else
                        switchManager.off();
                        return true;
                }
                break;
            case Constants.TAG_TAP:
                SinkManager sinkManager = switchObj.GetComponent<SinkManager>();
                if (sinkManager != null)
                {
                    if (on)
                        sinkManager.open();
                    else
                        sinkManager.close();
                        
                        return true;
                }
                break; ;
            default:
                return false;

        }
        return false;
    }

    public bool trigger(Transform switchObj)
    {
        switch (switchObj.tag)
        {
            case Constants.TAG_DOOR:
                DoorManager doorManager = switchObj.GetComponent<DoorManager>();
                if (doorManager != null)
                {
                    doorManager.trigger();
                    return true;
                }
                break;
            case Constants.TAG_SWITCH:
                SwitchManager switchManager = switchObj.GetComponent<SwitchManager>();
                if (switchManager != null)
                {
                    switchManager.trigger();
                    return true;
                }
                break; 
            case Constants.TAG_TAP:
                SinkManager sinkManager = switchObj.GetComponent<SinkManager>();
                if (sinkManager != null)
                {
                    sinkManager.trigger();
                    return true;
                }
                break; ;
            default:
                return false;

        }
      
        
        return false;
    }

    public bool isHandFree()
    {
        if (objInHand == null)
        {
            return true;
        }else
        {
            if(objInHand.parent != hand)
            {
                objInHand = null;
            }
        }
        return false;
    }



    public Transform getRestPosition()
    {
        return restPosition;
    }

    public string getAnimatorParam()
    {
        return animatorParam;
    }

    public void setCommand(Command command)
    {
        if((command.getAction()== Action.Take || command.getAction() == Action.Activate) && !isHandFree() )
        {
            command.fail();   
        }else
        {
            this.command = command;
        }
        
    }

    public Command getCommand()
    {
        return command;
    }

  


    public Hands getActHand()
    { 
        return actHand;
    }
}
