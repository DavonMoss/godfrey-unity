using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeterScript : MonoBehaviour
{
    private float startMeter = 1;
    private float currentMeter;

    public Image fillbar;

    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        currentMeter = startMeter;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        fillbar.fillAmount = currentMeter;
        transform.LookAt(transform.position + cam.forward);
    }

    public void setStartMeter(float amt)
    {
        startMeter = amt;
    }

    public void changeMeter(float amt)
    {
        currentMeter += amt;
    }

    public void setMeter(float n)
    {
        currentMeter = n;
    }
}