using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    private float startHealth = 1;
    private float currentHealth;

    public Image fillbar;

    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startHealth;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        fillbar.fillAmount = currentHealth;
        transform.LookAt(transform.position + cam.forward);
    }

    public void setStartHealth(float amt)
    {
        startHealth = amt;
    }

    public void changeHealth(float amt)
    {
        currentHealth += amt;
    }
}
