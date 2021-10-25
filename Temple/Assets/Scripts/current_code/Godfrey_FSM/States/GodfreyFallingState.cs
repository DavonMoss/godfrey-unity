using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyFallingState : GodfreyAbstractState
{
    GodfreyStateManager godfrey;
    Vector3 velocity, moveDir, xz_movedir, y_movedir;
    float angle, targetAngle, turnSmoothVelocity;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        this.godfrey = godfrey;

        velocity = Vector3.zero;
        moveDir = Vector3.zero;
        xz_movedir = Vector3.zero;
        y_movedir = Vector3.zero;
        angle = 0;
        targetAngle = 0;
        turnSmoothVelocity = 0;

        godfrey.SwitchAnimState(godfrey.FALL_ANIM);
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        regenMeter();

        if (godfrey.controller.isGrounded)
        {
            godfrey.SwitchState(godfrey.IdleState);
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        velocity += new Vector3(horizontal, 0, vertical).normalized;

        if (velocity.magnitude >= 0.1f)
        {
            updateRotation();
            updateMovement();
        }

        applyGravity();
    }

    private void updateRotation()
    {
        targetAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg + godfrey.cam.eulerAngles.y;
        angle = Mathf.SmoothDampAngle(godfrey.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, godfrey.turnSmoothTime);

        godfrey.transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void updateMovement()
    {
        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        xz_movedir = moveDir.normalized * godfrey.speed * Time.deltaTime;

        godfrey.controller.Move(xz_movedir);
    }

    private void applyGravity()
    {
        y_movedir.y -= godfrey.baseGravity * Time.deltaTime;
        godfrey.controller.Move(y_movedir * godfrey.fallSpeedMult * Time.deltaTime);
    }

    private void regenMeter()
    {
        if (godfrey.getCurrentMeter() < godfrey.meter)
        {
            godfrey.changeMeter(godfrey.meterRegen);
        }
    }
}
