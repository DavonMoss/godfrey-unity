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

    [Header("Movement Variables")]
    public float speed;
    public float jumpSpeed;
    public float constantDownwardForce;
    public float turnSmoothTime;
    public float blinkDist;
    public float baseGravity;

    [Header("Combat Variables")]
    public float lockOnDistance;

    [Header("Camera Variables")]
    public Transform cam;
    public Cinemachine.CinemachineFreeLook freeLookCam;
    public Cinemachine.CinemachineVirtualCamera lockOnCam;

    //
    // Player State and Private Variables;
    //

    GodfreyAbstractState currentState;
    GodfreyAbstractState targetingState = new TargetingState();
    private bool targeting;
    private Transform targetedEnemy = null;

    [Header("Player States")]
    public GodfreyIdleState IdleState = new GodfreyIdleState();
    public GodfreyFreelookMovingState FLMovingState = new GodfreyFreelookMovingState();
    public GodfreyStrafingState StrafingState = new GodfreyStrafingState();

    //
    // Animator State Constants
    //

    string currentAnimState;

    public string[] animStates = new string[]
    {
        "idle",     // 0
        "run",      // 1
        "strafe"    // 2
    };

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentState = IdleState;
        currentState.EnterState(this);
        targetingState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        targetingState.UpdateState(this);
        currentState.UpdateState(this);
    }

    //
    // HELPER FUNCTIONS
    //

    public void SwitchState(GodfreyAbstractState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void SwitchAnimState(string newState)
    {
        if (currentAnimState == newState) return;

        currentAnimState = newState;
        anim.Play(currentAnimState);
    }
    
    public bool isInputAxisActive()
    {
        return Mathf.Abs(Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical")) > 0;
    }

    public bool isTargeting()
    {
        return targeting;
    }

    public Transform getTargetedEnemy()
    {
        return targetedEnemy;
    }

    public void setTargeting(bool t, Transform te)
    {
        targeting = t;
        targetedEnemy = te;
    }

    //
    //  INPUT LISTENERS FOR EACH ACTION
    //

    public void input_targetToggle(InputAction.CallbackContext value)
    {
        targetingState.ReceiveInput(value);
    }
}
