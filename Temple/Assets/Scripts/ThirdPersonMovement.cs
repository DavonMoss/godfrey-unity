using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator anim;
    public Transform cam;

    public float speed, jumpSpeed, constantDownwardForce, leanDegrees, turnSmoothTime;
    public float nextAttackWindow, lastClickTime, numAttacks, blinkDist, attackDamage, attackRange, groundPoundRange, lockOnDistance, groundPoundSpeed, baseGravity;
    public Transform attackPoint, groundPoundPoint;
    public LayerMask enemyLayers;
    public int playerLayer, enemyLayerInt;

    private float turnSmoothVelocity, angle;
    private Vector3 velocity, moveDir;
    private Vector3 y_movedir = Vector3.zero;
    private bool moveEnabled = true, chain = false, targeting = false, triggerLockOn = false, inRecovery = false;

    private float gravity;

    public GameObject ctg_script_obj;
    private CinemachineTargetGroup targetGroup;
    private EnemyManager enemyManager;
    private Transform targetedEnemy;

    private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        targetGroup = ctg_script_obj.GetComponent<CinemachineTargetGroup>();
        enemyManager = ctg_script_obj.GetComponent<EnemyManager>();
        playerInput = GetComponent<PlayerInput>();
        gravity = baseGravity;
    }

    // Update is called once per frame
    void Update()
    {
        lockOn();
        applyGravity();
        move();
    }

    // INPUT METHODS
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

    public void blink(InputAction.CallbackContext value)
    {
        if (value.started)
            controller.Move(moveDir * blinkDist);
    }

    public void lockOnInput(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            triggerLockOn = true;
        }
        else if (value.canceled)
        {
            triggerLockOn = false;
        }
    }

    public void jump(InputAction.CallbackContext value)
    {
        if (value.started &&
            controller.isGrounded &&
            !inRecovery &&
            !anim.GetBool("attack1") &&
            !anim.GetBool("attack2") &&
            !anim.GetBool("air_attack"))
        {
            anim.SetTrigger("jump");
            y_movedir.y = jumpSpeed;
        }
    }

    public void attack1(InputAction.CallbackContext value)
    {
        if (Time.time - lastClickTime > nextAttackWindow)
        {
            numAttacks = 0;
        }

        if (value.started)
        {
            lastClickTime = Time.time;
            numAttacks++;

            if (numAttacks == 1)
            {
                moveEnabled = false;

                if (anim.GetBool("grounded"))
                {
                    anim.SetBool("attack1", true);
                }
                else
                {
                    anim.SetBool("air_attack", true);
                    gravity = groundPoundSpeed;
                }

            }
            else if (numAttacks == 2 && chain)
            {
                moveEnabled = false;
                anim.SetBool("attack2", true);
                chain = false;
            }

            numAttacks = Mathf.Clamp(numAttacks, 0, 2);
        }
    }

    // HELPER METHODS
    private void lockOn()
    {
        float enemyDistance = 0;
        List<float> enemyDistances = new List<float>();

        if (!targeting && enemyManager.enemiesInSight.Count > 0)
        {
            foreach (GameObject enemy in enemyManager.enemiesInSight)
            {
                Vector3 distCoords = transform.position - enemy.transform.position;
                enemyDistances.Add(Mathf.Sqrt(Mathf.Pow(distCoords.x, 2) + Mathf.Pow(distCoords.y, 2)));
            }

            if (triggerLockOn)
            {
                int minDistIdx = enemyDistances.IndexOf(enemyDistances.Min());

                targetedEnemy = enemyManager.enemiesInSight[minDistIdx].transform;
                targetGroup.AddMember(targetedEnemy, 1, 1);
                targeting = true;
                triggerLockOn = false;
                return;
            }
        }

        if (targetedEnemy == null)
        {
            targeting = false;
        }

        if (targeting)
        {
            if (triggerLockOn)
            {
                targetGroup.RemoveMember(targetedEnemy);
                targeting = false;
                triggerLockOn = false;
                return;
            }

            Vector3 distCoords = transform.position - targetedEnemy.position;
            enemyDistance = Mathf.Sqrt(Mathf.Pow(distCoords.x, 2) + Mathf.Pow(distCoords.y, 2));

            if (enemyDistance > lockOnDistance)
            {
                targetGroup.RemoveMember(targetedEnemy);
                targeting = false;
            }
        }
    }

    private void applyGravity()
    {
        y_movedir.y -= gravity * Time.deltaTime;

        if (!controller.isGrounded)
        {
            anim.SetBool("grounded", false);
            Physics.IgnoreLayerCollision(playerLayer, enemyLayerInt, true);
        }
        else
        {
            anim.SetBool("grounded", true);
            Physics.IgnoreLayerCollision(playerLayer, enemyLayerInt, false);
        }

        controller.Move(y_movedir * Time.deltaTime);
    }

    public void disableMovement()
    {
        moveEnabled = false;
    }

    public void enableMovement()
    {
        moveEnabled = true;
    }
    
    public void midairAttackReturn()
    {
        anim.SetBool("air_attack", false);
        gravity = baseGravity;
        moveEnabled = true;
        inRecovery = false;
        numAttacks = 0;
    }

    public void endAttack1()
    {
        anim.SetBool("attack1", false);

        if (numAttacks >= 2)
        {
            moveEnabled = false;
            anim.SetBool("attack2", true);
        }
        else
        {
            inRecovery = true;
            chain = true;
        }
    }

    public void attack1return()
    {
        moveEnabled = true;
        inRecovery = false;
        chain = false;
    }

    public void endAttack2()
    {
        anim.SetBool("attack2", false);
        inRecovery = true;
    }

    public void attack2return()
    {
        moveEnabled = true;
        inRecovery = false;
        numAttacks = 0;
    }

    private void activateAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        //Collider[] allEnemies = hitEnemies.Concat(Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers)).ToArray();

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyScript>().takeDamage(attackDamage);
        }
    }

    private void activateGroundPound()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(groundPoundPoint.position, groundPoundRange, enemyLayers);
        //Collider[] allEnemies = hitEnemies.Concat(Physics.OverlapSphere(groundPoundPoint.position, groundPoundRange, enemyLayers)).ToArray();

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyScript>().takeDamage(attackDamage);
        }

        inRecovery = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRange);
        Gizmos.DrawSphere(groundPoundPoint.position, groundPoundRange);
    }

    public void takeDamage(float dmg)
    {
        anim.SetTrigger("hit");
        moveEnabled = false;
        Debug.LogFormat("Took {0} Damage!", dmg);
    }

    public void hitStunReturn()
    {
        moveEnabled = true;
    }
}
