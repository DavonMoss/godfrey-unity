using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkingState : EnemyAbstractState
{
    EnemyStateManager enemy;

    public override void EnterState(EnemyStateManager enemy)
    {
        this.enemy = enemy;

        enemy.SwitchAnimState(enemy.WALK_ANIM);
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        // if enemy is hit
        if (enemy.isHitstunned())
        {
            enemy.SwitchState(enemy.HitStunState);
        }

        // if we're out of aggro range, stop walking
        if (enemy.getDistFromPlayer() >= enemy.dropAggroDistance)
        {
            enemy.SwitchState(enemy.IdleState);
        }

        // if we're already close enough to player, attack
        if (enemy.getDistFromPlayer() <= enemy.attackRange)
        {
            enemy.SwitchState(enemy.AttackState);
        }

        move();
    }

    void move()
    {
        Vector3 moveDir = enemy.transform.position - enemy.getGodfrey().gameObject.transform.position;
        Vector3 lookDir = Vector3.RotateTowards(enemy.transform.forward, moveDir, enemy.turnSpeed, 0.0f);

        moveDir.y = 0;
        lookDir.y = 0;

        enemy.transform.rotation = Quaternion.LookRotation(-lookDir);
        enemy.controller.Move(-moveDir.normalized * Time.deltaTime * enemy.moveSpeed);
    }
}
