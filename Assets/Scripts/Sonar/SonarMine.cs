using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarMine : MonoBehaviour
{
    public GameObject pc;
    public GameObject enemy;

    public EnemyMine enemyScript;
    public float range = 20;
    public bool active = true;

    private void Start()
    {
        enemyScript = enemy.GetComponent<EnemyMine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        if (CheckPcInRange())
        {
            active = false;
            enemyScript.SonarTriggered(transform);
            Debug.Log("Triggered Sonar");
        }
    }

    bool CheckPcInRange()
    {
        float distanceToPc = Vector3.Distance(pc.transform.position, transform.position);
        if (distanceToPc <= range) return true;
        return false;
    }
}
