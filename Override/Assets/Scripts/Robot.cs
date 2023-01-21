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

    [Header("References")]
    Rigidbody2D robotRB;
    GameObject player;
    GameObject statTracker;
    

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        robotRB = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        robotRB = GetComponent<Rigidbody2D>();
        statTracker = GameObject.FindWithTag("StatTracker");
    }

    void Update()
    {
        LookAtPlayer();
        ChasePlayer();

        if(currentHealth <= 0)
        {
            Die();
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Die()
    {
        statTracker.GetComponent<StatTracker>().playerKills += 1;
        Destroy(gameObject);
    }

    void Attack()
    {
        player.GetComponent<Player>().TakeDamage(damagePerHit);
    }
}
