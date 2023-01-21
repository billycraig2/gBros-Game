using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [HideInInspector] public float damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            GameObject robot = other.gameObject;
            robot.GetComponent<Robot>().currentHealth -= damage;
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
