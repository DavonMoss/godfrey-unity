using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyIdleState : GodfreyAbstractState
{
    GodfreyStateManager godfrey;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
        // Switch to Jump
        if (value.action.name == "Jump")
        {
            godfrey.SwitchState(godfrey.JumpingState);
        }

        if (value.action.name == "MeleeAttack")
        {
            godfrey.SwitchState(godfrey.AttackState_1);
        }
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        this.godfrey = godfrey;
        godfrey.SwitchAnimState(godfrey.IDLE_ANIM);
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        checkSwitches();
    }

    public void checkSwitches()
    {
        // Switch to freelook or strafing motion.
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
