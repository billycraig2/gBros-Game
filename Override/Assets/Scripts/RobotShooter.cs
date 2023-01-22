using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotShooter : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100;
    public float currentHealth;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 2f;
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
    [SerializeField] int dropSpawnChancePercentage = 20;

    [Header("References")]
    Rigidbody2D robotRB;
    GameObject player;
    GameObject statTracker;
    RoundManager roundManager;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    bool healthLowered;
    bool healthBoosted;
    [SerializeField] float distanceToPlayer;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        robotRB = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        robotRB = GetComponent<Rigidbody2D>();
        statTracker = GameObject.FindWithTag("StatTracker");
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
    }

    void Update()
    {
        LookAtPlayer();

        if(distanceToPlayer > stoppingDistance)
        {
            ChasePlayer();
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

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Shoot();
            lastAttackTime = Time.time;
        }
    }

    void LookAtPlayer()
    {
        var offset = 90f;
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
    }

    void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, movementSpeed * Time.deltaTime);
    }

    void Die()
    {
        statTracker.GetComponent<StatTracker>().playerKills += 1;
        statTracker.GetComponent<StatTracker>().playerPoints += pointsPerDeath;

        var randomNumber = Random.Range(1, 100);
        if (randomNumber < dropSpawnChancePercentage)
        {
            var randomNumber2 = Random.Range(1, 3);
            if (randomNumber2 == 1)
            {
                Instantiate(maxAmmo, this.transform.position, Quaternion.identity);
            }
            else if (randomNumber2 == 2)
            {
                Instantiate(instantKill, this.transform.position, Quaternion.identity);
            }
        }
        Destroy(gameObject);
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
}
