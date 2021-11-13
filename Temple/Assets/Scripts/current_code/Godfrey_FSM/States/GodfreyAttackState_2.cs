using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyAttackState_2 : GodfreyAbstractState
{
    GodfreyStateManager godfrey;
    float animStart, atkDuration, recoveryDuration;
    AnimatorStateInfo currentAnimState;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
        if (value.action.name == "Blink")
        {
            godfrey.SwitchState(godfrey.SlideState);
        }
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        this.godfrey = godfrey;

        animStart = Time.time;

        godfrey.slash.Clear();
        godfrey.SwitchAnimState(godfrey.ATK2_ANIM);
        godfrey.setAttackActive(true);

        if (godfrey.isCrit())
        {
            godfrey.slash_crit.Emit(1);
        }
        else
        {
            godfrey.slash.Emit(1);
        }
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        currentAnimState = godfrey.anim.GetCurrentAnimatorStateInfo(0);

        if (currentAnimState.IsName(godfrey.ATK2_ANIM))
        {
            atkDuration = godfrey.anim.GetCurrentAnimatorStateInfo(0).length;

            if (Time.time - animStart >= atkDuration)
            {
                godfrey.setAttackActive(false);
                godfrey.SwitchAnimState(godfrey.ATK2_RCVRY_ANIM);
            }
        }
        else if (currentAnimState.IsName(godfrey.ATK2_RCVRY_ANIM))
        {
            recoveryDuration = godfrey.anim.GetCurrentAnimatorStateInfo(0).length;

            if (Time.time - animStart >= atkDuration + recoveryDuration)
            {
                godfrey.setAttackActive(false);
                godfrey.setCrit(false);
                godfrey.setFreshKill(false);
                godfrey.setCritRegen(false);
                godfrey.SwitchState(godfrey.IdleState);
            }
        }
    }
}
