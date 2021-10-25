using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    // PUBLIC VARS

    [Header("Enemy Components")]
    public Animator anim;
    public HealthBarScript healthBar;
    public CharacterController controller;
    public EnemyManager enemyManager;

    [Header("Combat Variables")]
    public float maxHealth;
    public float aggroDistance;
    public float dropAggroDistance;
    public float attackRange;

    [Header("Movement Variables")]
    public float moveSpeed;
    public float turnSpeed;

    // STATES

    EnemyAbstractState currentState;

    public EnemyIdleState IdleState = new EnemyIdleState();
    public EnemyWalkingState WalkingState = new EnemyWalkingState();
    public EnemyAttackState AttackState = new EnemyAttackState();
    public EnemyHitStunState HitStunState = new EnemyHitStunState();

    // ANIM STATES

    string currentAnimState;

    [System.NonSerialized] public string IDLE_ANIM = "idle";
    [System.NonSerialized] public string WALK_ANIM = "walk";
    [System.NonSerialized] public string ATTACK_ANIM = "attack";
    [System.NonSerialized] public string HITSTUN_ANIM = "hitstun";

    // ENEMY STATE AND PRIVATE VARIABLES

    private float distFromPlayer;
    private float currentHealth;

    private bool hitstunned = false;

    private GameObject godfrey;

    // Start is called before the first frame update
    void Start()
    {
        godfrey = GameObject.FindGameObjectWithTag("Godfrey");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        currentHealth = maxHealth;
        currentState = IdleState;

        currentState.EnterState(this);    
    }

    // Update is called once per frame
    void Update()
    {
        watchForDeath();
        detectPlayer();

        currentState.UpdateState(this);
    }

    // STATE MACHINE HELPERS

    public void SwitchState(EnemyAbstractState state)
    {
        currentState = state;
        //Debug.LogFormat("Switching to {0}", currentState);
        currentState.EnterState(this);
    }

    public void SwitchAnimState(string state)
    {
        if (currentAnimState == state) return;

        currentAnimState = state;
        anim.Play(currentAnimState);
    }

    // CUSTOM HELPERS

    void detectPlayer()
    {
        Vector3 distCoords = transform.position - godfrey.transform.position;
        distFromPlayer = distCoords.magnitude;
    }

    void watchForDeath()
    {
        if (currentHealth <= 0)
        {
            godfrey.GetComponent<GodfreyStateManager>().setFreshKill(true);
            GameObject.Destroy(gameObject);
        }
    }

    public void takeDamage(float dmg)
    {
        currentHealth -= dmg;
        healthBar.changeHealth(-dmg/maxHealth);
        hitstunned = true;
    }

    // UNITY CALLBACKS

    public void OnBecameVisible()
    {
        enemyManager.enemiesInSight.Insert(0, gameObject);
    }

    public void OnBecameInvisible()
    {
        enemyManager.enemiesInSight.Remove(gameObject);   
    }

    // GETTERS AND SETTERS

    public void setDistFromPlayer(float d)
    {
        distFromPlayer = d;
    }

    public float getDistFromPlayer()
    {
        return distFromPlayer;
    }

    public void setHitstunned(bool b)
    {
        hitstunned = b;
    }

    public bool isHitstunned()
    {
        return hitstunned;
    }

    public GodfreyStateManager getGodfrey()
    {
        return godfrey.GetComponent<GodfreyStateManager>();
    }
}
