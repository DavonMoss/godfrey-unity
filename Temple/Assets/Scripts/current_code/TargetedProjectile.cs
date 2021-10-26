using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedProjectile : MonoBehaviour
{
    GodfreyStateManager godfrey;
    public float growRate;
    Transform player;
    Vector3 moveDir;
    [System.NonSerialized] public bool lockedOn;
    public float maxDropDist;
    public float moveSpeed;
    public float missedLifeTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Godfrey").transform;
        godfrey = player.gameObject.GetComponent<GodfreyStateManager>();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        lockedOn = true;
        moveDir = (player.transform.position - transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!lockedOn)
        {
            StartCoroutine(killMe());
        }

        if (transform.localScale.x < 1)
        {
            transform.localScale += new Vector3(growRate, growRate, growRate) * Time.deltaTime;

            if (!godfrey.incomingProjectiles.Contains(this.gameObject))
            {
                godfrey.incomingProjectiles.Add(this.gameObject);
            }
        }
        else
        {
            if (lockedOn)
            {
                moveDir = (player.transform.position - transform.position);
            }

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    IEnumerator killMe()
    {
        yield return new WaitForSeconds(missedLifeTime);
        GameObject.Destroy(this.gameObject);
    }
}
