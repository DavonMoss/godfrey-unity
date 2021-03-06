using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] protected float maxHealth, detectionRange, moveSpeed, attackTriggerRange, attackRange, attackCD, rotateSpeed, offset, attackDamage;
    [SerializeField] protected Transform playerPos;
    protected bool detected = false, detecting = true, canAttack = true, canMove = true;
    protected float currentHealth, distFromPlayer;

    [SerializeField] protected CharacterController controller;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask playerLayers;

    public GameObject ctg_script_obj;
    private EnemyManager enemyManager;

    public HealthBarScript healthbar;

    GodfreyStateManager godfrey;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        godfrey = GameObject.FindGameObjectWithTag("Godfrey").GetComponent<GodfreyStateManager>();

        currentHealth = maxHealth;
        //healthbar.setStartHealth(maxHealth);

        playerPos = GameObject.FindGameObjectWithTag("Godfrey").transform;

        ctg_script_obj = GameObject.FindGameObjectWithTag("EnemyManager");
        enemyManager = ctg_script_obj.GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        kill();
        detectPlayer();
        applyGravity();

        if (detected)
        {
            if (canMove)
            {
                approach();
            }

            triggerAttack();
        }
    }

    public void takeDamage(float dmg)
    {
        anim.SetTrigger("hit");
        canMove = false;
        currentHealth -= dmg;
        healthbar.changeHealth(-dmg/maxHealth);
    }

    public void hitStunReturn()
    {
        canMove = true;
    }

    protected virtual void kill()
    {
        if (currentHealth <= 0)
        {
            godfrey.setFreshKill(true);
            GameObject.Destroy(gameObject);
        }
    }

    protected virtual void detectPlayer()
    {
        Vector3 distCoords = transform.position - playerPos.position;
        distFromPlayer = Mathf.Sqrt(Mathf.Pow(distCoords.x, 2) + Mathf.Pow(distCoords.y, 2));

        if (distFromPlayer <= detectionRange)
        {
            detected = true;
        }
        else
        {
            detected = false;
        }
    }

    private void applyGravity()
    { 
        controller.Move(new Vector3(0, -9.8f, 0) * Time.deltaTime);
    }

    protected virtual void approach()
    {
        Vector3 moveDir = transform.position - playerPos.position;
        Vector3 lookDir = Vector3.RotateTowards(transform.forward, moveDir, rotateSpeed, 0.0f);
        moveDir.y = 0;
        lookDir.y = 0;

        if (distFromPlayer > offset)
        {
            anim.SetBool("walk", true);
            transform.rotation = Quaternion.LookRotation(-lookDir);
            controller.Move(-moveDir.normalized * Time.deltaTime * moveSpeed);
        }
        else
        {
            anim.SetBool("walk", false);
        }
    }

    protected virtual void triggerAttack()
    {
        if (distFromPlayer <= attackTriggerRange && canAttack)
        {
            //attack
            anim.SetBool("attack", true);
            canAttack = false;
            canMove = false;
            StartCoroutine(attackCooldown(attackCD));
        }
    }

    protected virtual void activateAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);
        Collider[] allEnemies = hitEnemies.Concat(Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers)).ToArray();

        foreach (Collider player in allEnemies)
        {
            player.GetComponent<ThirdPersonMovement>().takeDamage(attackDamage);
        }
    }

    IEnumerator attackCooldown(float cd)
    {
        yield return new WaitForSecondsRealtime(cd);
        canAttack = true;
    }

    public void attackReturn()
    {
        canMove = true;
        anim.SetBool("attack", false);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRange);
    }

    public void OnBecameVisible()
    {
        enemyManager.enemiesInSight.Insert(0, gameObject);
    }

    public void OnBecameInvisible()
    {
        enemyManager.enemiesInSight.Remove(gameObject);
    }
}
