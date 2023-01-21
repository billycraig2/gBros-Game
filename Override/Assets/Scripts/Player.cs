using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth;

    [Header("Movement")]
    [SerializeField] float movementSpeed;

    [Header("Dashing")]
    [SerializeField] float doubleTapTime = 0.5f;
    [SerializeField] float dashWaitTime = 2.0f;
    [SerializeField] float dashSpeed = 5f;
    [SerializeField] float dashLength = .5f;
    [SerializeField] float dashCost = 20f;
    float lastDashButtonTimeW;
    float lastDashButtonTimeA;
    float lastDashButtonTimeS;
    float lastDashButtonTimeD;
    float lastDashTime;

    [Header("Gun Mana")]
    [SerializeField] float currentGunMana = 100;
    [SerializeField] float maxGunMana = 100;
    [SerializeField] float manaRegenSpeed = 1;

    [Header("Gun")]
    [SerializeField] float bulletDamage = 20f;
    [SerializeField] float bulletSpeed = 1f;

    [Header("References")]
    Rigidbody2D playerRB;
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentGunMana = maxGunMana;
    }

    void FixedUpdate()
    {
        LookAtMouse();
        Move();    
    }

    void Update()
    {
        GunMana();
        Dash();
        Shooting();
    }

    void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));
    }

    void Move()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerRB.velocity = input.normalized * movementSpeed;
    }

    void GunMana()
    {
        if(currentGunMana < maxGunMana)
        {
            currentGunMana += manaRegenSpeed * Time.deltaTime;
        }
    }

    void Dash()
    {
        if(dashPossible && currentGunMana >= dashCost && Input.GetKeyDown(KeyCode.W))
        {
            if(Time.time - lastDashButtonTimeW < doubleTapTime)
            {
                DoDash();
            }

            lastDashButtonTimeW = Time.time;
        }

        if (dashPossible && currentGunMana >= dashCost && Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastDashButtonTimeA < doubleTapTime)
            {
                DoDash();
            }

            lastDashButtonTimeA = Time.time;
        }

        if (dashPossible && currentGunMana >= dashCost && Input.GetKeyDown(KeyCode.S))
        {
            if (Time.time - lastDashButtonTimeS < doubleTapTime)
            {
                DoDash();
            }

            lastDashButtonTimeS = Time.time;
        }

        if (dashPossible && currentGunMana >= dashCost && Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastDashButtonTimeD < doubleTapTime)
            {
                DoDash();
            }

            lastDashButtonTimeD = Time.time;
        }
    }

    bool dashPossible
    {
        get
        {
            return Time.time - lastDashTime > dashWaitTime;
        }
    }

    void DoDash()
    {
        currentGunMana -= dashCost;
        lastDashTime = Time.time;
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        movementSpeed += dashSpeed;
        yield return new WaitForSeconds(dashLength);
        movementSpeed -= dashSpeed;
    }

    void Shooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            newBullet.GetComponent<PlayerBullet>().damage = bulletDamage;
            Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(float DamagePerHit)
    {
        currentHealth -= DamagePerHit;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
