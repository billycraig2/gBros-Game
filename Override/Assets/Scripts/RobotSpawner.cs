using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    [SerializeField] GameObject robotPrefabOne;
    [SerializeField] GameObject robotPrefabTwo;
    RoundManager roundManager;
    public int minSpawnDelay = 3;
    public int maxSpawnDelay = 10;

    bool spawnDelayDecreaseOneDone;
    bool spawnDelayDecreaseTwoDone;

    void Start()
    {
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();

        if(transform.parent == null)
        {
            StartCoroutine(RobotSpawnRoutine());
        }
    }

    void Update()
    {
        if(roundManager.currentRound == 5 && !spawnDelayDecreaseOneDone)
        {
            spawnDelayDecreaseOneDone = true;
            minSpawnDelay = 2;
            maxSpawnDelay = 8;
        }

        if (roundManager.currentRound == 10 && !spawnDelayDecreaseTwoDone)
        {
            spawnDelayDecreaseTwoDone = true;
            minSpawnDelay = 1;
            maxSpawnDelay = 6;
        }
    }

    public IEnumerator RobotSpawnRoutine()
    {
        print("Spawn routine started");
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
