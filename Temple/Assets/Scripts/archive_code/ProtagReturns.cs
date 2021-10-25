using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProtagReturns : MonoBehaviour
{
    public int numAttacks;
    Animator ani;

    // attack registration variables
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange, attackDamage, knockback, hitDelay;
    [SerializeField] private LayerMask enemyLayers, andreasVulnerableLayers;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerScript ps;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // helper return functions for combo logic
    public void return1()
    {
        //Debug.Log("Entered return1()");
        if (numAttacks >= 2)
        {
            //Debug.Log("Second attack");
            ani.SetBool("attack1", false);
            ani.SetBool("attack2", true);
        }
        else
        {
            //Debug.Log("Reset [return1()]");
            ani.SetBool("attack1", false);
            ani.SetBool("canRun", true);
            numAttacks = 0;
        }
    }
    public void return2()
    {
        //Debug.Log("Reset [return3()]");
        ani.SetBool("attack1", false);
        ani.SetBool("attack2", false);
        ani.SetBool("canRun", true);
        numAttacks = 0;
    }

    // check attacks
    private void activateAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        Collider[] allEnemies = hitEnemies.Concat(Physics.OverlapSphere(attackPoint.position, attackRange, andreasVulnerableLayers)).ToArray();

        foreach (Collider enemy in allEnemies)
        {
            enemy.GetComponent<EnemyScript>().takeDamage(attackDamage);
            knockbackOther(enemy.GetComponent<Rigidbody>());
        }

        if (allEnemies.Length > 0)
        {
            //changeTime(0.05f);
            //Debug.Log("slow down!");
            knockbackSelf();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void changeTime(float newTime)
    {
        Time.timeScale = newTime;
        StartCoroutine(restoreTime(hitDelay));
    }

    IEnumerator restoreTime(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        changeTime(1);
    }

    private void knockbackSelf()
    {
        rb.velocity -= Vector3.left * knockback * -ps.lookDir;
    }

    private void knockbackOther(Rigidbody other)
    {
        other.velocity -= Vector3.left * knockback * ps.lookDir;
    }
}
