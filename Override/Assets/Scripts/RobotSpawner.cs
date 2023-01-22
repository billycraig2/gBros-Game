using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    [SerializeField] GameObject robotPrefab;
    RoundManager roundManager;
    [SerializeField] int minSpawnDelay = 3;
    [SerializeField] int maxSpawnDelay = 10;

    void Start()
    {
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        StartCoroutine(RobotSpawnRoutine());
    }

    IEnumerator RobotSpawnRoutine()
    {
        var spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        yield return new WaitForSeconds(spawnDelay);
        if(roundManager.robotsLeft > 0)
        {
            roundManager.robotsLeft -= 1;
            Instantiate(robotPrefab, this.transform.position, Quaternion.identity);
        }   
        StartCoroutine(RobotSpawnRoutine());
    }
}
