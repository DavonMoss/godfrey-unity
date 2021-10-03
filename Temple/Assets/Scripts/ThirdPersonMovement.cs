using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator anim;
    public Transform cam;

    public float speed, jumpSpeed, constantDownwardForce, leanDegrees, turnSmoothTime;
    public float nextAttackWindow, lastClickTime, numAttacks, blinkDist, attackDamage, attackRange;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private float turnSmoothVelocity, angle;
    private Vector3 velocity, moveDir;
    private Vector3 y_movedir = Vector3.zero;
    private bool moveEnabled, chain;

    public float gravity;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        attack1();
        jump();
        blink();
        applyGravity();

        //if (moveEnabled)
        move();
    }

    private void move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        velocity = new Vector3(horizontal, 0, vertical).normalized;

        if (velocity.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, leanDegrees * -velocity.x);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 xz_movedir = moveDir.normalized * speed * Time.deltaTime;
            xz_movedir.y += constantDownwardForce * Time.deltaTime;

            if (moveEnabled)
            {
                controller.Move(xz_movedir);
                anim.SetBool("run", true);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
            anim.SetBool("run", false);
        }
    }

    private void blink()
    {
        if (Input.GetKeyDown(KeyCode.E))
            controller.Move(moveDir * blinkDist);
    }

    // gravity
    private void applyGravity()
    {
        y_movedir.y -= gravity * Time.deltaTime;

        if (!controller.isGrounded)
        {
            anim.SetBool("grounded", false);
        }
        else
        {
            anim.SetBool("grounded", true);
        }

        controller.Move(y_movedir * Time.deltaTime);
    }

    //jump
    private void jump()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            y_movedir.y = jumpSpeed;
            anim.SetTrigger("jump");
        }
    }

    public void disableMovement()
    {
        moveEnabled = false;
    }

    public void enableMovement()
    {
        moveEnabled = true;
    }

    //attack 1
    private void attack1()
    {
        if (Time.time - lastClickTime > nextAttackWindow)
        {
            numAttacks = 0;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            lastClickTime = Time.time;
            numAttacks++;

            if (numAttacks == 1)
            {
                if (anim.GetBool("grounded"))
                {
                    moveEnabled = false;
                    anim.SetBool("attack1", true);
                }
                else
                {
                    anim.SetBool("air_attack", true);
                }
                
            }

            numAttacks = Mathf.Clamp(numAttacks, 0, 2);
        }
    }

    public void midairAttackReturn()
    {
        anim.SetBool("air_attack", false);
    }

    public void endAttack1()
    {
        anim.SetBool("attack1", false);

        if (numAttacks >= 2)
        {
            moveEnabled = false;
            anim.SetBool("attack2", true);
        }
    }

    public void attack1return()
    {
        moveEnabled = true;
    }

    public void endAttack2()
    {
        anim.SetBool("attack2", false);
    }

    public void attack2return()
    {
        moveEnabled = true;
        numAttacks = 0;
    }

    private void activateAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        Collider[] allEnemies = hitEnemies.Concat(Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers)).ToArray();

        foreach (Collider enemy in allEnemies)
        {
            enemy.GetComponent<EnemyScript>().takeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }

    public void takeDamage(float dmg)
    {
        Debug.LogFormat("Took {0} Damage!", dmg);
    }
}
