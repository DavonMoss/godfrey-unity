using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody player_rb;
    private BoxCollider player_bc;
    public Animator curr_ani, other_ani;

    // input and character variables
    private PlayerInput inputManager;
    private GameObject Maria, Protagonist, currentChar, otherChar;
    [SerializeField] GameObject AISeat;

    // camera variables
    [SerializeField] private float camXoffset, camYoffset, camZoffset, camSpeed;
    [SerializeField] private Camera cam;
    [SerializeField] GameObject followObj;

    // movement variables
    [SerializeField] private float moveSpeed, jumpHeight, fallSpeed, dashLength;
    private bool holdingJump = false;
    [SerializeField] LayerMask groundLayer;
    public float mvmtDir, lookDir, h_mvdir, v_mvdir;

    // protag combat variables
    [SerializeField] private int numAttacks;
    [SerializeField] private float lastAttack, maxComboDelay, groundPoundSpeed, groundPoundCD, groundPoundLastUsed, specialCD, specialLastUsed, attackForwardMvmt, dmgKnockback;

    // maria combat and aim variables
    [SerializeField] GameObject aimTarget;
    [SerializeField] GameObject handIKtarget;
    [SerializeField] GameObject handIKCollider;
    [SerializeField] GameObject elbowIKhint;
    [SerializeField] GameObject armRigObject;
    [SerializeField] GameObject headRigObject;
    [SerializeField] GameObject otherArmRigObject;
    private Rig armRig;
    private Rig headRig;
    private Rig otherArmRig;
    [SerializeField] LayerMask mouseAimMask, bodyRayMask;
    private bool aimOn = false;
    [SerializeField] GameObject arrowObject;
    [SerializeField] GameObject arrowSpawnPoint;

    // script variables
    [SerializeField] ProtagReturns pr_script;
    [SerializeField] OtherScript other_script;

    // Start is called before the first frame update
    void Awake()
    {
        player_rb = GetComponent<Rigidbody>();
        player_bc = GetComponent<BoxCollider>();

        Maria = GameObject.FindWithTag("Maria");
        Protagonist = GameObject.FindWithTag("Protagonist");
        currentChar = Protagonist;
        otherChar = Maria;

        currentChar.transform.position = transform.position;
        currentChar.transform.rotation = transform.rotation;
        currentChar.transform.parent = transform;

        otherChar.transform.position = AISeat.transform.position;
        otherChar.transform.rotation = transform.rotation;
        otherChar.transform.parent = AISeat.transform;

        inputManager = GetComponent<PlayerInput>();

        armRig = armRigObject.GetComponent<Rig>();
        headRig = headRigObject.GetComponent<Rig>();
        otherArmRig = otherArmRigObject.GetComponent<Rig>();
        armRig.weight = 0;
        headRig.weight = 0;
        otherArmRig.weight = 0;

        //hacky
        lookDir = -1f;
        setComponents();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        run();

        if (currentChar == Maria && aimOn)
        {
            aimTargetToMousePos();
            targetToBodyRay();
            placeElbowIKHint();
            flipCharToCursor(currentChar.transform);
        }
    }

    // for continuous physics calculations
    private void FixedUpdate()
    {
        groundCheck();
        checkFalling();
    }

    /**
     * =====================================================================================================================
     * SHARED METHODS: Used by both protagonist and maria characters 
     * =====================================================================================================================
     **/

    // set current components
    void setComponents()
    {
        curr_ani = currentChar.GetComponent<Animator>();
        other_ani = otherChar.GetComponent<Animator>();

        if (currentChar == Protagonist)
        {
            //inputManager.SwitchCurrentActionMap("protagControls");
        }
        else
        {
            //inputManager.SwitchCurrentActionMap("mariaControls");
            curr_ani.SetBool("canFire", true);
        }

        foreach (AnimatorControllerParameter parameter in other_ani.parameters)
        {
            other_ani.SetBool(parameter.name, false);
        }
        other_ani.SetBool("Grounded", true);
        curr_ani.SetBool("canRun", true);

        other_script.setComponents();
    }

    // ground check
    private void groundCheck()
    {
        RaycastHit hit;
        float distance = 0.125f;

        // extend ray if falling
        if (player_rb.velocity.y < 0)
            distance *= 4;

        Vector3 dir = new Vector3(0, -1);
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + distance/2, transform.position.z);
        Debug.DrawRay(origin, dir * distance, Color.green);

        if (Physics.Raycast(origin, dir, out hit, distance, groundLayer))
        {
            curr_ani.SetBool("Grounded", true);
        }
        else
        {
            curr_ani.SetBool("Grounded", false);
        }
    }

    // switch positions and character
    public void switchPos(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            // swap current character
            GameObject temp = currentChar;
            currentChar = otherChar;
            otherChar = temp;

            // swap player seats
            currentChar.transform.position = transform.position;
            currentChar.transform.rotation = transform.rotation;
            currentChar.transform.parent = transform;

            otherChar.transform.position = AISeat.transform.position;
            otherChar.transform.rotation = AISeat.transform.rotation;
            otherChar.transform.parent = AISeat.transform;

            setComponents();
        }
    }

    // Jumping
    public void jump(InputAction.CallbackContext value)
    {
        // jump code
        if (value.started)
        {
            holdingJump = true;
            if (curr_ani.GetBool("Grounded"))
            {
                //player_rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
                player_rb.velocity += Vector3.up * jumpHeight;
                curr_ani.SetTrigger("Jump");
            }
        }
        else if (value.canceled)
        {
            holdingJump = false;
        }
    }

    // Dash
    public void dash(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            player_rb.velocity = new Vector3(0, player_rb.velocity.y, player_rb.velocity.z);
            Vector3 dash = Vector3.right * dashLength * lookDir;
            Debug.Log(dash);
            player_rb.velocity += dash;
            //switchPos(value);
        }
    }

    void calcPlayerRotation()
    {

    }

    // Running
    // Handled by old input system so that characters can keep momentum between swaps, since their run
    // is identical
    private void run()
    {
        if (!curr_ani.GetBool("canRun"))
        {
            h_mvdir = 0;
            v_mvdir = 0;
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                h_mvdir = -0.75f;
                //lookDir = mvmtDir;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                h_mvdir = 0.75f;
                //lookDir = mvmtDir;
            }
            else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                h_mvdir = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                v_mvdir = -1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                v_mvdir = 0.35f;
            }
            else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                v_mvdir = 0;
            }
        }

        transform.position += new Vector3(v_mvdir * moveSpeed * Time.deltaTime, 0, h_mvdir * moveSpeed * Time.deltaTime);
        //player_rb.velocity = new Vector3(mvmtDir * moveSpeed, player_rb.velocity.y, 0);

        // animation
        if (mvmtDir != 0f)
        {
            if (!aimOn)
            {
                //transform.rotation = Quaternion.LookRotation(new Vector3(mvmtDir, 0, 0));
                //transform.rotation = Quaternion.Euler(new Vector3(0, cam.transform.rotation.y, 0));
                transform.LookAt(transform.position + Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up));
            }
            curr_ani.SetBool("Running", true);
        }
        else
        {
            curr_ani.SetBool("Running", false);
        }
    }

    // apex check
    private void checkFalling()
    {
        if (curr_ani.GetBool("Grounded") == false)
        {
            if (player_rb.velocity.y < 0)
            {
                player_rb.velocity += Vector3.up * Physics.gravity.y * (fallSpeed - 1) * Time.deltaTime;
            }
            else if (player_rb.velocity.y > 0 && !holdingJump)
            {
                player_rb.velocity += Vector3.up * Physics.gravity.y * (fallSpeed - 1) * Time.deltaTime;
            }
        }
    }

    // Collision between two non-trigger colliders
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            player_rb.velocity -= Vector3.left * dmgKnockback * -lookDir;
        }
    }

    /**
    * =====================================================================================================================
    * Protag-only METHODS: melee logic 
    * =====================================================================================================================
    **/

    // melee attack animation logic, handles combos
    public void melee(InputAction.CallbackContext value)
    {
        if (currentChar == Maria)
            switchPos(value);

        // timeout logic for button presses
        if (Time.time - lastAttack > maxComboDelay)
        {
            //numAttacks = 0;
            pr_script.numAttacks = 0;
        }

        if (value.started)
        {
            lastAttack = Time.time;
            //numAttacks++;
            pr_script.numAttacks++;

            if (pr_script.numAttacks == 1)
            {
                //Debug.Log("First attack");
                curr_ani.SetBool("attack1", true);
                //curr_ani.SetBool("canRun", false);

                //player_rb.AddForce(new Vector3(lookDir * attackForwardMvmt, 0, 0), ForceMode.Impulse);
            }
            pr_script.numAttacks = Mathf.Clamp(pr_script.numAttacks, 0, 2);
        }
    }

    /**
    * =====================================================================================================================
    * Maria-only METHODS: ranged logic 
    * =====================================================================================================================
    **/

    // aimTarget to mouse position code
    void aimTargetToMousePos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            aimTarget.transform.position = hit.point;
        }
    }

    void targetToBodyRay()
    {
        Vector3 dir = (aimTarget.transform.position - handIKCollider.transform.position).normalized;
        Ray ray = new Ray(aimTarget.transform.position, -dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, bodyRayMask))
        {
            handIKtarget.transform.position = hit.point;
        }

        Debug.DrawRay(aimTarget.transform.position, -dir, Color.green);
    }

    void placeElbowIKHint()
    {
        Vector3 dir = (aimTarget.transform.position - handIKCollider.transform.position).normalized;
        Vector3 newPos = handIKtarget.transform.position - dir;
        newPos.z += 0.5f;
        elbowIKhint.transform.position = newPos;
    }

    void flipCharToCursor(Transform t)
    {
        float dir = mvmtDir;

        if (aimTarget.transform.position.x > t.position.x)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }

        lookDir = dir;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir, 0, 0));
    }

    public void aimToggle(InputAction.CallbackContext value)
    {
        if (currentChar == Protagonist)
            switchPos(value);

        if (curr_ani.GetBool("canFire"))
        { 
            if (value.started || value.performed)
            {
                aimOn = true;
                armRig.weight = 1;
                headRig.weight = 0.75f;
                otherArmRig.weight = 1;
                curr_ani.SetLayerWeight(curr_ani.GetLayerIndex("upperBody"), 1);
                //curr_ani.SetBool("canRun", false);
                Time.timeScale = 0.25f;
            }
            else if (value.canceled)
            {
                if (aimOn)
                {
                    fireArrow();
                    curr_ani.SetBool("canFire", false);
                    Time.timeScale = 1.0f;
                    IEnumerator coroutine = mariaDelay(0.4f, curr_ani);
                    StartCoroutine(coroutine);
                }

                aimOn = false;
                armRig.weight = 0;
                headRig.weight = 0;
                otherArmRig.weight = 0;
                curr_ani.SetLayerWeight(curr_ani.GetLayerIndex("upperBody"), 0);

            }
        }
    }

    public void fireArrow()
    {
        GameObject arrow = Instantiate(arrowObject);
        Vector3 dir = (aimTarget.transform.position - handIKCollider.transform.position).normalized;

        arrow.transform.position = arrowSpawnPoint.transform.position;
        arrow.GetComponent<ArrowScript>().setMotion(dir);

        //player_rb.AddForce(new Vector3(-lookDir * attackForwardMvmt, 0, 0), ForceMode.Impulse);
        if (!curr_ani.GetBool("Grounded"))
            player_rb.velocity += -dir * attackForwardMvmt;
    }

    public IEnumerator mariaDelay(float time, Animator mariaAnim)
    {
        yield return new WaitForSeconds(time);
        //mariaAnim.SetBool("canRun", true);
        mariaAnim.SetBool("canFire", true);
    }
}
