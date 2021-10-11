using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public string enemyTag;
    public GameObject weaponOwner;
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
            weaponOwner.GetComponent<ThirdPersonMovement>().attackHit(other);

            contactFlash.transform.position = other.ClosestPoint(transform.position);
            hitSpray.Play();
        }
    }
}
