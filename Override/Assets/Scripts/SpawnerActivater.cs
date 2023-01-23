using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerActivater : MonoBehaviour
{
    GameObject childSpawner;

    void Start()
    {
        childSpawner = this.gameObject.transform.GetChild(0).gameObject;
        childSpawner.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            childSpawner.SetActive(true);
        }
    }

}
