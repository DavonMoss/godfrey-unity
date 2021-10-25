using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public GodfreyStateManager godfrey;

    public string enemyTag;
    public GameObject contactFlash;
    public ParticleSystem hitSpray;

    // Start is called before the first frame update
    void Start()
    {
        hitSpray = contactFlash.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(enemyTag))
        {
            //Debug.LogFormat("I have hit {0}!", other.ToString());
            godfrey.attackHit(other);

            if (godfrey.isCritRegen())
            {
                godfrey.changeMeter(godfrey.meterCritRegen);
                godfrey.setCritRegen(false);
            }

            contactFlash.transform.position = other.ClosestPoint(transform.position);
            hitSpray.Play();
        }
    }
}
