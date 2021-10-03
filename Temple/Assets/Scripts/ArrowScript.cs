using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Vector3 dir;
    public float moveSpeed, startTime, lifeTime, arrowDamage, knockback;
    private Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        move();

        if (Time.time - startTime > lifeTime)
        {
            destroy();
        }
    }

    public void setMotion(Vector3 dir)
    {
        this.dir = dir;
    }

    public void move()
    {
        transform.position = transform.position + (dir * moveSpeed * Time.deltaTime);
    }

    public void destroy()
    {
        //Debug.Log("finna die!");
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool targets = false;

        if (this.gameObject.layer == LayerMask.NameToLayer("EvilArrows"))
        {
            targets = other.gameObject.layer == LayerMask.NameToLayer("Player");
        }
        else if (this.gameObject.layer == LayerMask.NameToLayer("Arrows"))
        {
            targets =
                other.gameObject.layer == LayerMask.NameToLayer("Enemies") ||
                other.gameObject.layer == LayerMask.NameToLayer("MariaVulnerable");

        }

        if (targets)
        {
            // hacky but ok for now
            EnemyScript es = other.GetComponent<EnemyScript>();

            if (es != null)
            {
                es.takeDamage(arrowDamage);
            }

            knockbackOther(other.GetComponent<Rigidbody>());

            col.enabled = false;
            destroy();
        }
    }

    private void knockbackOther(Rigidbody other)
    {
        if (other != null)
        { 
            other.velocity += dir * knockback;
        }
    }
}
