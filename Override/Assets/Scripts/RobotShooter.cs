using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RobotShooter : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100;
    public float currentHealth;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float stoppingDistance = 5f;

    [Header("Damage")]
    [SerializeField] float damagePerHit = 20f;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float attackCooldown = 1f;
    float lastAttackTime;

    [Header("Points")]
    [SerializeField] int pointsPerDeath = 100;

    [Header("Power Ups")]
    [SerializeField] GameObject maxAmmo;
    [SerializeField] GameObject instantKill;
    [SerializeField] GameObject infiniteMana;
    [SerializeField] GameObject healthDrop;
    [SerializeField] int dropSpawnChancePercentage = 20;

    [Header("References")]
    GameObject player;
    GameObject statTracker;
    RoundManager roundManager;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    Animator robotAnim;

    bool healthLowered;
    bool healthBoosted;
    bool isRunning;
    bool isDead;
    bool dropSpawned;
    [SerializeField] float distanceToPlayer;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        currentHealth = maxHealth;
        statTracker = GameObject.FindWithTag("StatTracker");
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        robotAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if(distanceToPlayer > stoppingDistance && !isDead)
        {
            isRunning = true;
            robotAnim.SetBool("isRunning", true);
        }
        else
        {
            isRunning = false;
            robotAnim.SetBool("isRunning", false);
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        if (roundManager.instantKillActive && !healthLowered)
        {
            healthBoosted = false;
            healthLowered = true;
            currentHealth = 1;
        }

        if (!roundManager.instantKillActive && !healthBoosted)
        {
            healthBoosted = true;
            healthLowered = false;
            currentHealth = 100;
        }

        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (Time.time >= lastAttackTime + attackCooldown && !isDead)
        {
            Shoot();
            lastAttackTime = Time.time;
        }

        if (distanceToPlayer <= stoppingDistance)
        {
            GetComponent<AIPath>().maxSpeed = 0.1f;
        }
        else if (!isDead)
        {
            GetComponent<AIPath>().maxSpeed = movementSpeed;
        }
    }

    void Die()
    {
        GetComponent<AIPath>().enabled = false;
        GetComponent<SpriteRenderer>().sortingOrder = 2;
        GetComponent<BoxCollider2D>().enabled = false;
        isDead = true;
        robotAnim.SetBool("isDead", true);
        if (!dropSpawned)
        {
            dropSpawned = true;
            statTracker.GetComponent<StatTracker>().playerKills += 1;
            statTracker.GetComponent<StatTracker>().playerPoints += pointsPerDeath;
            var randomNumber = Random.Range(1, 100);
            if (randomNumber < dropSpawnChancePercentage)
            {
                var randomNumber2 = Random.Range(1, 5);
                if (randomNumber2 == 1)
                {
                    Instantiate(maxAmmo, this.transform.position, Quaternion.identity);
                }
                else if (randomNumber2 == 2)
                {
                    Instantiate(instantKill, this.transform.position, Quaternion.identity);
                }
                else if (randomNumber2 == 3)
                {
                    Instantiate(infiniteMana, this.transform.position, Quaternion.identity);
                }
                else if (randomNumber2 == 4)
                {
                    Instantiate(healthDrop, this.transform.position, Quaternion.identity);
                }
            }
        }
        StartCoroutine(DieRoutine());
    }

    void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        newBullet.GetComponent<PlayerBullet>().damage = damagePerHit;
        newBullet.GetComponent<PlayerBullet>().isPlayerBullet = false;
        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
    }

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
