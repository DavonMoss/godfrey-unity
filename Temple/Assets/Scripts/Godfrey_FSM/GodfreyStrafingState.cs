using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyStrafingState : GodfreyAbstractState
{
    Vector3 velocity, moveDir, xz_movedir;
    float angle, targetAngle, turnSmoothVelocity;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        velocity = Vector3.zero;
        moveDir = Vector3.zero;
        xz_movedir = Vector3.zero;
        angle = 0;
        targetAngle = 0;
        turnSmoothVelocity = 0;

        godfrey.SwitchAnimState(godfrey.animStates[2]);
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        velocity = new Vector3(horizontal, 0, vertical).normalized;

        if (velocity.magnitude >= 0.1f)
        {
            // if we're not targeting, don't stay here
            if (!godfrey.isTargeting())
            {
                godfrey.SwitchState(godfrey.FLMovingState);
            }

            godfrey.anim.SetFloat("horizontal_vel", velocity.x);
            godfrey.anim.SetFloat("vertical_vel", velocity.z);
            updateRotation(godfrey);
            updateMovement(godfrey);
        }
        else
        {
            // if we're not moving don't stay here
            godfrey.transform.rotation = Quaternion.Euler(0, angle, 0);
            godfrey.SwitchState(godfrey.IdleState);
        }
    }

    private void updateRotation(GodfreyStateManager godfrey)
    {
        targetAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg + godfrey.cam.eulerAngles.y;
        angle = Mathf.SmoothDampAngle(godfrey.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, godfrey.turnSmoothTime);

        Vector3 enemyDir = godfrey.getTargetedEnemy().position - godfrey.transform.position;
        Vector3 lookDir = Vector3.RotateTowards(godfrey.transform.forward, enemyDir, godfrey.turnSmoothTime, 0.0f);
        godfrey.transform.rotation = Quaternion.LookRotation(lookDir);
    }

    private void updateMovement(GodfreyStateManager godfrey)
    {
        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        xz_movedir = moveDir.normalized * godfrey.speed * Time.deltaTime;
        xz_movedir.y += godfrey.constantDownwardForce * Time.deltaTime;

        godfrey.controller.Move(xz_movedir);
    }
}