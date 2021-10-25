using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyAbstractState
{
    EnemyStateManager enemy;

    public override void EnterState(EnemyStateManager enemy)
    {
        this.enemy = enemy;

        enemy.SwitchAnimState(enemy.IDLE_ANIM);
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        // if enemy is hit
        if (enemy.isHitstunned())
        {
            enemy.SwitchState(enemy.HitStunState);
        }

        // if we're in aggro range walk towards player
        if (enemy.getDistFromPlayer() <= enemy.aggroDistance)
        {
            enemy.SwitchState(enemy.WalkingState);
        }

        // if we're already close enough to player, attack
        if (enemy.getDistFromPlayer() <= enemy.attackRange)
        {
            enemy.SwitchState(enemy.AttackState);
        }
    }
}
