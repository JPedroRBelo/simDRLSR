using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AgentBehave : MonoBehaviour
{

    private Command command;
    private long timeStart;
    private Animator animator;    
    public bool printLog = false;

    void Awake()
    {      
        command = null;
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    private void Log(string text){
        if(printLog)
        {
            Debug.Log(text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (command != null)
        {

       
            switch (command.getAction())
            {
                case Action.Animate:
                    string animation = command.getAnimation();
                    Vector3 initialPosition = transform.position;
                    Transform reference = command.getReference();
                    switch (command.getActionStateID())
                    {
                        case (int)Animate.Start:
                            Log("Command>>> " + this.name + " command " + command.getId() + " Started!");
                            timeStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            initialPosition = transform.position;
                            if (command.getAnimation() != "Wait")
                            {
                                animator.SetBool(animation, true);
                                if ((command.getAnimation() == "Sitting") || (command.getAnimation() == "Typing"))
                                {
                                    ChairManager chairSit = reference.GetComponent<ChairManager>();
                                    chairSit.sit(transform);
                                }
                            }
                            timeStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            command.next();
                            break;
                        case (int)Animate.Position:
                            long timeNow = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            animation = command.getAnimation();

                            float timeSpeed = 1f;
                            GameObject[] simManager = GameObject.FindGameObjectsWithTag("SimulatorManager");
                            if(simManager != null){
                                timeSpeed = simManager[0].GetComponent<TimeManagerKeyboard>().getTime();
                            }
                            if (timeNow - timeStart >= (command.getTime()/timeSpeed))
                            {
                                
                                if (command.getAnimation() != "Wait")
                                {                               
                                    animator.SetBool(animation, false);
                                    if ((command.getAnimation() == "Sitting")|| (command.getAnimation() == "Typing"))
                                    {
                                        ChairManager chairStand = reference.GetComponent<ChairManager>();
                                        chairStand.stand(transform);                                        
                                        AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
                                        if (chairStand.isFree())
                                        {
                                            Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
                                            command.success();
                                        }

                                    }else{
                                        command.success();
                                    }

                                    
                                }
                                else
                                {
                                    Log("Command>>> " + this.name + " command " + command.getId() + " Success!");
                                    command.success();
                                }
                                
                            }
                            
                            break;
                        case (int)Animate.End:
                            break;
                    }
                    break;
               
                default:
                    Log("Command>>> " + this.name + " command " + command.getId() + " Failed! ERROR!");
                    break;
            }
        }
    }



    public bool sendCommand(Command command)
    {
        this.command = command;
        Log("Command>>> " + this.name + " received command " + command.getStringCommand());
        return true;
    }

}
