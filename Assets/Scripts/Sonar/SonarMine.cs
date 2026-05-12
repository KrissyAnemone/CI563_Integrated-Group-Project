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

    AudioSource audioSource;

    private void Start()
    {
        enemyScript = enemy.GetComponent<EnemyMine>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckPcInRange()) Trigger();
    }

    bool CheckPcInRange()
    {
        if (!pc) return false;
        float distanceToPc = Vector3.Distance(pc.transform.position, transform.position);
        if (distanceToPc <= range) return true;
        return false;
    }

    public void Trigger()
    {
        if (!active) return;
        active = false;

        audioSource.Play();
        enemyScript.SonarTriggered(transform);
        Debug.Log("Triggered Sonar");
    }
}
