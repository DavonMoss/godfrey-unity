using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitStunState : EnemyAbstractState
{
    EnemyStateManager enemy;
    float animStart;
    float stunDuration;
    AnimatorStateInfo currentAnimState;

    public override void EnterState(EnemyStateManager enemy)
    {
        this.enemy = enemy;

        animStart = Time.time;

        enemy.SwitchAnimState(enemy.HITSTUN_ANIM);
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        currentAnimState = enemy.anim.GetCurrentAnimatorStateInfo(0);

        if (currentAnimState.IsName(enemy.HITSTUN_ANIM))
        {
            stunDuration = currentAnimState.length;

            if (Time.time - animStart >= stunDuration)
            {
                enemy.setHitstunned(false);
                enemy.SwitchState(enemy.IdleState);
            }
        }
    }
}
