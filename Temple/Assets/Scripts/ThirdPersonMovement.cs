using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    // Public Variables
    [Header("Player Movement")]
    public CharacterController controller;
    public Animator anim;
    public float speed, jumpSpeed, constantDownwardForce, leanDegrees, turnSmoothTime, blinkDist, baseGravity;

    [Header("Camera Movement")]
    public Transform cam;
    public GameObject ctg_script_obj;
    public Cinemachine.CinemachineFreeLook freeLookCam;
    public Cinemachine.CinemachineVirtualCamera lockOnCam;

    [Header("Player Combat")]
    public Transform attackPoint, groundPoundPoint;
    public LayerMask enemyLayers;
    public GameObject targetUI;
    public int playerLayer, enemyLayerInt;
    public float nextAttackWindow, lastClickTime, numAttacks, attackDamage, attackRange, groundPoundRange, lockOnDistance, groundPoundSpeed, critMultiplier;

    [Header("Player Particle FX")]
    public ParticleSystem slash_1, slash_crit;
    public ParticleSystem teleport_hor_outer, teleport_hor_inner, teleport_billboard;
    public ParticleSystem runningDust;

    // Private Variables
    private float turnSmoothVelocity, angle;
    private Vector3 velocity, moveDir;
    private Vector3 y_movedir = Vector3.zero;

    private bool moveEnabled = true,
                    chain = false,
                    targeting = false,
                    triggerLockOn = false,
                    inRecovery = false,
                    crit = false,
                    attackActive = false,
                    jumped = false;

    private float gravity;
    private CinemachineTargetGroup targetGroup;
    private EnemyManager enemyManager;
    private Transform targetedEnemy;
    private PlayerInput playerInput;
    private GameObject targetUIInstance;

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
        switchCamLockOnMethod(targeting);
        applyGravity();
        move();
        activateDust();
    }

    private void switchCamLockOnMethod(bool t)
    {
        if (t)
        {
            lockOnCam.Priority = 1;
            freeLookCam.Priority = 0;
        }
        else
        {
            freeLookCam.Priority = 1;
            lockOnCam.Priority = 0;
        }
    }

    // INPUT METHODS
    private void move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        velocity = new Vector3(horizontal, 0, vertical).normalized;

        if (velocity.magnitude >= 0.1f)
        {
            // ROTATION CODE
            float targetAngle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            if (targeting)
            {
                Vector3 enemyDir = targetedEnemy.position - transform.position;
                Vector3 lookDir = Vector3.RotateTowards(transform.forward, enemyDir, turnSmoothTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, angle, 0/**, leanDegrees * -velocity.x**/);
            }

            // MOVEMENT CODE
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 xz_movedir = moveDir.normalized * speed * Time.deltaTime;
            xz_movedir.y += constantDownwardForce * Time.deltaTime;

            if (moveEnabled)
            {
                controller.Move(xz_movedir);

                if (targeting)
                {
                    if (velocity.z <= 0 && velocity.x >= 0.1)
                    {
                        Debug.Log("Trying to strafe");
                        anim.SetBool("run", false);
                        anim.SetBool("strafe", true);
                        anim.SetFloat("horizontal_vel", velocity.x);
                        anim.SetFloat("vertical_vel", velocity.z);
                    }
                    else
                    {
                        Debug.Log("Should be running from strafe logic");
                        anim.SetBool("strafe", false);
                        anim.SetBool("run", true);
                    }
                }
                else
                {
                    Debug.Log("Skipping strafe logic");
                    anim.SetBool("strafe", false);
                    anim.SetBool("run", true);

                }
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
        {
            teleport_hor_inner.Clear();
            teleport_hor_outer.Clear();
            teleport_billboard.Clear();
            teleport_hor_inner.Play();
            teleport_hor_outer.Play();
            teleport_billboard.Play();

            controller.Move(moveDir * blinkDist);
        }
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
            jumped = true;
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
                if (anim.GetBool("grounded"))
                {
                    moveEnabled = false;
                    anim.SetBool("attack1", true);
                }
                else if (!anim.GetBool("grounded") && jumped)
                {
                    anim.SetBool("air_attack", true);
                    gravity = groundPoundSpeed;
                }
            }
            else if (numAttacks == 2 && chain)
            {
                moveEnabled = false;
                anim.SetBool("attack2", true);
                slash_1.Clear();
                chain = false;
            }

            numAttacks = Mathf.Clamp(numAttacks, 0, 2);
        }
    }

    // HELPER METHODS
    public void endJump()
    {
        jumped = false;
        disableMovement();
    }

    private void activateDust()
    {
        if (anim.GetBool("run") == true &&
            controller.isGrounded &&
            moveEnabled)
        {
            runningDust.Play();
        }
        else
        {
            runningDust.Stop();
            runningDust.Clear();
        }
    }

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
                targetGroup.AddMember(targetedEnemy, 1, 5);
                targeting = true;
                triggerLockOn = false;
                applyLockOnUI(targetedEnemy.transform);
                return;
            }
        }

        if (targetedEnemy == null)
        {
            targeting = false;
            applyLockOnUI(null);
        }

        if (targeting)
        {
            if (triggerLockOn)
            {
                targetGroup.RemoveMember(targetedEnemy);
                targeting = false;
                triggerLockOn = false;
                applyLockOnUI(null);
                return;
            }

            Vector3 distCoords = transform.position - targetedEnemy.position;
            enemyDistance = Mathf.Sqrt(Mathf.Pow(distCoords.x, 2) + Mathf.Pow(distCoords.y, 2));

            if (enemyDistance > lockOnDistance)
            {
                targetGroup.RemoveMember(targetedEnemy);
                applyLockOnUI(null);
                targeting = false;
            }
        }
    }

    private void applyLockOnUI(Transform parent)
    {
        if (parent != null)
        {
            targetUIInstance = GameObject.Instantiate(targetUI);
            targetUIInstance.transform.SetParent(parent, false);
        }
        else
        {
            GameObject.Destroy(targetUIInstance);
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
            y_movedir.y = Mathf.Clamp(y_movedir.y, 0, jumpSpeed);
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

    // for emitting the slash at a specific frame
    public void emitSlash1()
    {
        if (crit)
        {
            slash_crit.Emit(1);
        }
        else
        {
            slash_1.Emit(1);
        }
    }

    public void endAttack1()
    {
        anim.SetBool("attack1", false);
        attackWindowClose();

        // this is the crit check
        if (numAttacks >= 2)
        {
            moveEnabled = false;
            slash_1.Clear();
            crit = true;
            //anim.SetFloat("attack_speed", 3.0f);
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
        attackWindowClose();
        inRecovery = true;
    }

    public void attack2return()
    {
        anim.SetFloat("attack_speed", 1.0f);
        moveEnabled = true;
        inRecovery = false;
        crit = false;
        numAttacks = 0;
    }

    public void attackHit(Collider enemy)
    {
        if (enemy != null && attackActive)
        {
            float dmg = attackDamage;

            if (crit)
                dmg *= critMultiplier;

            enemy.GetComponent<EnemyScript>().takeDamage(dmg);
        }
    }

    public void attackWindowOpen()
    {
        attackActive = true;
        emitSlash1();
    }

    public void attackWindowClose()
    {
        attackActive = false;
    }

    private void activateGroundPound()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(groundPoundPoint.position, groundPoundRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyScript>().takeDamage(attackDamage);
        }

        inRecovery = true;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(attackPoint.position, attackRange);
        //Gizmos.DrawSphere(groundPoundPoint.position, groundPoundRange);
    }

    public void takeDamage(float dmg)
    {
        anim.SetTrigger("hit");
        moveEnabled = false;
        Debug.LogFormat("Took {0} Damage!", dmg);
    }

    public void hitStunReturn()
    {
        // resetting for clean idle
        moveEnabled = true;
        chain = false;
        triggerLockOn = false;
        inRecovery = false;
        crit = false;
        attackActive = false;
        jumped = false;

        // resetting anim fields
        anim.ResetTrigger("hit");
        anim.ResetTrigger("jump");
        anim.SetBool("attack1", false);
        anim.SetBool("attack2", false);
        anim.SetBool("air_attack", false);
    }
}
