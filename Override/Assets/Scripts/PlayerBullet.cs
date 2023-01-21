using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [SerializeField] int pointsPerHit = 20;
    public bool isPierceShot;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            GameObject.FindWithTag("StatTracker").GetComponent<StatTracker>().playerPoints += pointsPerHit;
            GameObject robot = other.gameObject;
            robot.GetComponent<Robot>().currentHealth -= damage;
            if(!isPierceShot)
            {
                Destroy(gameObject);
            }   
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
