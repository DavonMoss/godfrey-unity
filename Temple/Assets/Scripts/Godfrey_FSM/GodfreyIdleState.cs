using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyIdleState : GodfreyAbstractState
{
    public override void ReceiveInput(InputAction.CallbackContext value)
    {
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        godfrey.SwitchAnimState(godfrey.animStates[0]);
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        checkSwitches(godfrey);
    }

    public void checkSwitches(GodfreyStateManager godfrey)
    {
        // Switch to freelook motion.
        if (godfrey.isInputAxisActive())
        {
            if (godfrey.isTargeting())
            {
                godfrey.SwitchState(godfrey.StrafingState);
            }
            else
            {
                godfrey.SwitchState(godfrey.FLMovingState);
            }
        } 
    }
}
