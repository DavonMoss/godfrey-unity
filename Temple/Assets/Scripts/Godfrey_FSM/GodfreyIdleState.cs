using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyIdleState : GodfreyAbstractState
{
    GodfreyStateManager godfrey;

    float targetAngle;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
        // Switch to Jump
        if (value.action.name == "Jump")
        {
            godfrey.SwitchState(godfrey.JumpingState);
        }

        // Switch to Melee
        if (value.action.name == "MeleeAttack")
        {
            godfrey.SwitchState(godfrey.AttackState_1);
        }

        // Idle blink logic
        if (value.action.name == "Blink")
        {
            if (godfrey.getCurrentMeter() >= godfrey.meterBlinkCost)
            {
                godfrey.changeMeter(-godfrey.meterBlinkCost);
                godfrey.controller.Move(calcBlink() * godfrey.blinkDist);
                updateRotation();
            }
        }
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        this.godfrey = godfrey;
        godfrey.SwitchAnimState(godfrey.IDLE_ANIM);
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        regenMeter();
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

    private Vector3 calcBlink()
    {
        targetAngle = godfrey.cam.eulerAngles.y;
        Vector3 lookDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.one;
        Vector3 blinkDir = Vector3.Cross(lookDir, Vector3.up).normalized;

        if (!godfrey.isTargeting())
        {
            blinkDir = -Vector3.forward;
        }

        return blinkDir;
    }

    private void updateRotation()
    {
        targetAngle = godfrey.cam.eulerAngles.y;

        Vector3 enemyDir = godfrey.getTargetedEnemy().position - godfrey.transform.position;
        Vector3 lookDir = Vector3.RotateTowards(godfrey.transform.forward, enemyDir, 360, 0.0f);
        godfrey.transform.rotation = Quaternion.LookRotation(lookDir);
    }

    private void regenMeter()
    {
        if (godfrey.getCurrentMeter() < godfrey.meter)
        {
            godfrey.changeMeter(godfrey.meterRegen/2.5f);
        }
    }
}
