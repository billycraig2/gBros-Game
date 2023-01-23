using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100;
    public float currentHealth;

    [Header("Movement")]
    [SerializeField] float movementSpeed = 2f;

    [Header("Damage")]
    [SerializeField] float damagePerHit = 20f;
    [SerializeField] float attackRange = 1f;
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
    Animator robotAnim;

    bool healthLowered;
    bool healthBoosted;
    bool isRunning;
    bool isHitting;
    bool isDead;
    bool dropSpawned;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        robotRB = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        robotRB = GetComponent<Rigidbody2D>();
        statTracker = GameObject.FindWithTag("StatTracker");
        roundManager = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        robotAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if(!isDead)
        {
            LookAtPlayer();
            ChasePlayer();
        }
        else
        {
            isRunning = false;
        }
        
        if(currentHealth <= 0)
        {
            Die();
        }

        if (roundManager.instantKillActive && !healthLowered)
        {
            healthBoosted = false;
            healthLowered = true;
            currentHealth = 1;
        }

        if(!roundManager.instantKillActive && !healthBoosted)
        {
            healthBoosted = true;
            healthLowered = false;
            currentHealth = 100;
        }

        AnimationConditions();
    }

    void AnimationConditions()
    {
        if(!isHitting)
        {
            robotAnim.SetBool("isHitting", false);
        }
        else
        {
            robotAnim.SetBool("isHitting", true);
        }

        if(!isRunning)
        {
            robotAnim.SetBool("isRunning", false);
        }
        else
        {
            robotAnim.SetBool("isRunning", true);
        }

        if(isDead)
        {
            robotAnim.SetBool("isDead", true);
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
        isRunning = true;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, movementSpeed * Time.deltaTime);

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            isHitting = true;
            if (Time.time >= lastAttackTime + attackCooldown && !isDead)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            isHitting = false;
        }
    }

    void Die()
    {
        robotRB.simulated = false;
        GetComponent<SpriteRenderer>().sortingOrder = 2;
        GetComponent<BoxCollider2D>().enabled = false;
        isDead = true;
        if(!dropSpawned)
        {
            dropSpawned = true;
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
        }
        StartCoroutine(DieRoutine());
    }

    void Attack()
    {
        player.GetComponent<Player>().TakeDamage(damagePerHit);
    }

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
