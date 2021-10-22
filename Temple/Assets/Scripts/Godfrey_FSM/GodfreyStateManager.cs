using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GodfreyStateManager : MonoBehaviour
{
    //
    // Tuning Variables.
    //
    // Not to be used for logic here, just for storing and easy access across states.
    //

    [Header("Player Components")]
    public CharacterController controller;
    public PlayerInput playerInput;
    public Animator anim;

    [Header("Other Components")]

    [Header("UI Objects")]
    public GameObject targetUI;
    public MeterScript meterBar;

    [Header("Movement Variables")]
    public float speed;
    public float jumpSpeed;
    public float fallSpeedMult;
    public float strafeMult;
    public float turnSmoothTime;
    public float blinkDist;
    public float baseGravity;

    [Header("Combat Variables")]
    public float lockOnDistance;
    public float attackDamage;
    public float critMultiplier;
    public float meter;
    public float meterBlinkCost;
    public float meterRegen;
    public float meterCritRegen;

    [Header("Player Particle FX")]
    public ParticleSystem slash;
    public ParticleSystem slash_crit;

    [Header("Camera Variables")]
    public Transform cam;
    public Cinemachine.CinemachineFreeLook freeLookCam;
    public Cinemachine.CinemachineVirtualCamera lockOnCam;

    //
    // Player State and Private Variables;
    //

    GodfreyAbstractState currentState;
    GodfreyAbstractState targetingState = new TargetingState();
    private float currentSpeed;
    private float currentMeter;
    private bool targeting = false;
    private bool crit = false;
    private bool freshKill = false;
    private bool canCritRegen = false;
    private bool attackActive = false;
    private Transform targetedEnemy = null;

    [Header("Player States")]
    public GodfreyIdleState IdleState = new GodfreyIdleState();
    public GodfreyFreelookMovingState FLMovingState = new GodfreyFreelookMovingState();
    public GodfreyStrafingState StrafingState = new GodfreyStrafingState();
    public GodfreyJumpingState JumpingState = new GodfreyJumpingState();
    public GodfreyFallingState FallingState = new GodfreyFallingState();
    public GodfreyAttackState_1 AttackState_1 = new GodfreyAttackState_1();
    public GodfreyAttackState_2 AttackState_2 = new GodfreyAttackState_2();

    //
    // Animator State Constants
    //

    string currentAnimState;

    [System.NonSerialized] public string IDLE_ANIM = "idle";
    [System.NonSerialized] public string RUN_ANIM = "run";
    [System.NonSerialized] public string STRAFE_ANIM = "strafe";
    [System.NonSerialized] public string JUMP_ANIM = "jump";
    [System.NonSerialized] public string FALL_ANIM = "falling";
    [System.NonSerialized] public string ATK1_ANIM = "attack1";
    [System.NonSerialized] public string ATK1_RCVRY_ANIM = "attack1_recovery";
    [System.NonSerialized] public string ATK2_ANIM = "attack2";
    [System.NonSerialized] public string ATK2_RCVRY_ANIM = "attack2_recovery";

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        currentSpeed = speed;
        currentMeter = meter;
        currentState = IdleState;

        currentState.EnterState(this);
        targetingState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        targetingState.UpdateState(this);
        currentState.UpdateState(this);

        updateMeterBar();
    }

    //
    // HELPER FUNCTIONS
    //

    public void SwitchState(GodfreyAbstractState state)
    {
        currentState = state;
        Debug.LogFormat("Switching to {0}", currentState);
        state.EnterState(this);
    }

    public void SwitchAnimState(string newState)
    {
        if (currentAnimState == newState) return;

        currentAnimState = newState;
        anim.Play(currentAnimState);
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

    public void updateMeterBar()
    {
        meterBar.setMeter(currentMeter/meter);
    }

    //
    // INPUT SYSTEM CALLBACKS
    //

    public void input_toggletarget(InputAction.CallbackContext value)
    {
        targetingState.ReceiveInput(value);
    }

    public void PassInput(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            currentState.ReceiveInput(value);
        }
    }

    //
    // GETTERS AND SETTERS
    //

    public bool isInputAxisActive()
    {
        return Mathf.Abs(Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical")) > 0;
    }

    public Transform getTargetedEnemy()
    {
        return targetedEnemy;
    }

    public bool isTargeting()
    {
        return targeting;
    }

    public void setTargeting(bool t, Transform te)
    {
        targeting = t;
        targetedEnemy = te;
    }

    public bool isCrit()
    {
        return crit;
    }

    public void setCrit(bool b)
    {
        crit = b;
    }

    public bool isFreshKill()
    {
        return freshKill;
    }

    public void setFreshKill(bool b)
    {
        freshKill = b;

        if (b)
        {
            Time.timeScale = 0.5f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public bool isCritRegen()
    {
        return canCritRegen;
    }

    public void setCritRegen(bool b)
    {
        canCritRegen = b;
    }

    public bool isAttackActive()
    {
        return attackActive;
    }

    public void setAttackActive(bool b)
    {
        attackActive = b;
    }

    public float getCurrentSpeed()
    {
        return currentSpeed;
    }

    public void setCurrentSpeed(float s)
    {
        currentSpeed = s;
    }

    public float getCurrentMeter()
    {
        return currentMeter;
    }

    public void setCurrentMeter(float m)
    {
        currentMeter = m;
        currentMeter = Mathf.Clamp(currentMeter, 0, 1);
    }

    public void changeMeter(float amt)
    {
        float adjustedAmt = amt;

        if (currentMeter + amt >= meter)
        {
            adjustedAmt = meter - currentMeter;
        }

        currentMeter += adjustedAmt;
    }
}