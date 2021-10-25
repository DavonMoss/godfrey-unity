using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyAbstractState
{
    EnemyStateManager enemy;
    float animStart;
    float attackDuration;
    AnimatorStateInfo currentAnimState;

    public override void EnterState(EnemyStateManager enemy)
    {
        this.enemy = enemy;

        animStart = Time.time;

        enemy.SwitchAnimState(enemy.ATTACK_ANIM);
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        enemy.setHitstunned(false);
        currentAnimState = enemy.anim.GetCurrentAnimatorStateInfo(0);

        if (currentAnimState.IsName(enemy.ATTACK_ANIM))
        {
            attackDuration = currentAnimState.length;

            if (Time.time - animStart >= attackDuration)
            {
                enemy.SwitchState(enemy.IdleState);
            }
        }
    }
}
