using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [SerializeField] int pointsPerHit = 20;
    public bool isPierceShot;
    public bool isPlayerBullet;

    void Start()
    {
        if(isPierceShot)
        {
            StartCoroutine(PierceShot());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(isPlayerBullet)
        {
            if (other.CompareTag("Robot"))
            {
                GameObject.FindWithTag("StatTracker").GetComponent<StatTracker>().playerPoints += pointsPerHit;
                GameObject robot = other.gameObject;
                robot.GetComponent<Robot>().currentHealth -= damage;
                if (!isPierceShot)
                {
                    Destroy(gameObject);
                }
            }
            else if (!other.CompareTag("Bullet") && !other.CompareTag("Spawner") && !other.CompareTag("Drop") && !isPierceShot)
            {
                Destroy(gameObject);
            }

            if (other.CompareTag("RobotShooter"))
            {
                GameObject.FindWithTag("StatTracker").GetComponent<StatTracker>().playerPoints += pointsPerHit;
                GameObject robot = other.gameObject;
                robot.GetComponent<RobotShooter>().currentHealth -= damage;
                if (!isPierceShot)
                {
                    Destroy(gameObject);
                }
            }
            else if (!other.CompareTag("Bullet") && !other.CompareTag("Spawner") && !other.CompareTag("Drop") && !isPierceShot)
            {
                Destroy(gameObject);
            }
        }
        else if(!isPlayerBullet)
        {
            if(other.CompareTag("Player"))
            {
                GameObject player = other.gameObject;
                player.GetComponent<Player>().TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (!other.CompareTag("Bullet") && !other.CompareTag("Spawner") && !other.CompareTag("Drop") && !isPierceShot)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator PierceShot()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
