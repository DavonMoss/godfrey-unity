using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherScript : MonoBehaviour
{
    public PlayerScript player_script;
    public GameObject playerSeat;

    private Animator curr_ani;
    private Rigidbody other_rb;
    private BoxCollider other_bc;

    [SerializeField] private float moveSpeed, followDist;
    private float mvmtDir, lookDir;

    // Start is called before the first frame update
    void Start()
    {
        setComponents();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(curr_ani.ToString());
        run();
    }

    public void setComponents()
    {
        curr_ani = player_script.other_ani;
        curr_ani.SetBool("canRun", true);
        lookDir = player_script.lookDir;
    }

    // Running
    // will follow the player controlled character on the X axis only for now
    private void run()
    {
        if (!curr_ani.GetBool("canRun"))
        {
            mvmtDir = 0;
        }
        else
        {
            Vector3 followPos = playerSeat.transform.position + new Vector3(followDist * -player_script.lookDir, 0, 0);
            Vector3 followDir = (followPos - transform.position).normalized;

            if (Mathf.Abs(transform.position.x - followPos.x) <= 0.05)
            {
                mvmtDir = 0;
                lookDir = player_script.lookDir;
            }
            else 
            {
                mvmtDir = followDir.x;
                lookDir = mvmtDir;
            }
        }

        transform.rotation = Quaternion.LookRotation(new Vector3(lookDir, 0, 0));
        transform.position += new Vector3(mvmtDir * moveSpeed * Time.deltaTime, 0, 0);

        // animation
        if (mvmtDir != 0f)
        {
            curr_ani.SetBool("Running", true);
        }
        else
        {
            curr_ani.SetBool("Running", false);
        }
    }
}
