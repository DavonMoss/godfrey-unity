using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyAttackState_1 : GodfreyAbstractState
{
    GodfreyStateManager godfrey;
    float animStart, atkDuration, recoveryDuration;
    AnimatorStateInfo currentAnimState;
    bool attackQueued;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
        if (value.action.name == "MeleeAttack")
        {
            if (currentAnimState.IsName(godfrey.ATK1_RCVRY_ANIM))
            {
                godfrey.setCrit(false);
                godfrey.setAttackActive(false);
                godfrey.SwitchState(godfrey.AttackState_2);
            }
            else if (currentAnimState.IsName(godfrey.ATK1_ANIM))
            {
                attackQueued = true;
                godfrey.setCrit(true);
            }
        } 
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        this.godfrey = godfrey;

        animStart = Time.time;
        attackQueued = false;

        godfrey.slash.Clear();
        godfrey.SwitchAnimState(godfrey.ATK1_ANIM);
        godfrey.setAttackActive(true);
        godfrey.slash.Emit(1);

    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        currentAnimState = godfrey.anim.GetCurrentAnimatorStateInfo(0);

        if (currentAnimState.IsName(godfrey.ATK1_ANIM))
        {
            atkDuration = godfrey.anim.GetCurrentAnimatorStateInfo(0).length;

            if (Time.time - animStart >= atkDuration)
            {
                godfrey.setAttackActive(false);
                godfrey.SwitchAnimState(godfrey.ATK1_RCVRY_ANIM);
            }
        }
        else if (currentAnimState.IsName(godfrey.ATK1_RCVRY_ANIM))
        {
            triggerNextAttack();
            recoveryDuration = godfrey.anim.GetCurrentAnimatorStateInfo(0).length;

            if (Time.time - animStart >= atkDuration + recoveryDuration)
            {
                godfrey.setAttackActive(false);
                godfrey.SwitchState(godfrey.IdleState);
            }
        }
    }

    private void triggerNextAttack()
    {
        if (attackQueued)
        {
            godfrey.setAttackActive(false);
            godfrey.SwitchState(godfrey.AttackState_2);
        }
    }
}
