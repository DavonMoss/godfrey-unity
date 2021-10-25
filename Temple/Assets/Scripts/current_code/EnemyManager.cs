using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemiesInSight;

    // Start is called before the first frame update
    void Start()
    {
        enemiesInSight = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
