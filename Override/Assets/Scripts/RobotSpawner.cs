using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    [SerializeField] GameObject robotPrefabOne;
    [SerializeField] GameObject robotPrefabTwo;
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
            var RandomNo = Random.Range(1, 3);
            if(RandomNo == 1)
            {
                Instantiate(robotPrefabOne, this.transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(robotPrefabTwo, this.transform.position, Quaternion.identity);
            }
        }   
        StartCoroutine(RobotSpawnRoutine());
    }
}
