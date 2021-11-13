using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreySlideState : GodfreyAbstractState
{
    GodfreyStateManager godfrey;
    Vector3 velocity, moveDir, xz_movedir;
    float angle, targetAngle, turnSmoothVelocity, deceleration;

    public override void ReceiveInput(InputAction.CallbackContext value)
    {
    }

    public override void EnterState(GodfreyStateManager godfrey)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
        {
            velocity = -Vector3.forward;
        }
        else
        {
            Debug.LogFormat("H: {0}, V: {1}", horizontal, vertical);
            velocity += new Vector3(horizontal, 0, vertical).normalized;
        }

        moveDir = Vector3.zero;
        xz_movedir = Vector3.zero;
        angle = 0;
        targetAngle = 0;
        turnSmoothVelocity = 0;
        deceleration = 0;

        this.godfrey = godfrey;
    }

    public override void UpdateState(GodfreyStateManager godfrey)
    {
        if (inRange(velocity.x, -0.1f, 0.1f) && inRange(velocity.z, -0.1f, 0.1f))
        {
            godfrey.SwitchState(godfrey.IdleState);
        }
        else
        {
            updateRotation();
            updateMovement();

            if (velocity.x > 0)
            {
                velocity -= new Vector3(deceleration, 0, 0);
            }
            if (velocity.x < 0)
            {
                velocity += new Vector3(deceleration, 0, 0);
            }
            if (velocity.z > 0)
            {
                velocity -= new Vector3(0, 0, deceleration);
            }
            if (velocity.z < 0)
            {
                velocity += new Vector3(0, 0, deceleration);
            }

            //velocity -= new Vector3(deceleration, 0, deceleration);
            deceleration += godfrey.friction;
        }
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
        xz_movedir = moveDir.normalized * godfrey.slideSpeed * Time.deltaTime;

        Debug.Log(xz_movedir);

        godfrey.controller.Move(xz_movedir);
    }

    private bool inRange(float n, float min, float max)
    {
        return (n >= min && n <= max);
    }
}
