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
    [SerializeField] bool dashUnlocked = false;
    [SerializeField] float doubleTapTime = 0.5f;
    [SerializeField] float dashWaitTime = 2.0f;
    [SerializeField] float dashSpeed = 5f;
    [SerializeField] float dashLength = .5f;
    [SerializeField] float dashCost = 20f;
    float lastDashButtonTime;
    float lastDashTime;

    [Header("Gun Mana")]
    [SerializeField] float currentGunMana = 100;
    [SerializeField] float maxGunMana = 100;
    [SerializeField] float manaRegenSpeed = 1;

    [Header("Bullet")]
    [SerializeField] float bulletDamage = 20f;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] int ammoStockpile = 24;
    [SerializeField] int ammoInMag;
    [SerializeField] int magSize = 12;
    float nextFire;

    [Header("Gun Evolution")]
    [SerializeField] int gunLevel;
    [SerializeField] int playerKills;

    [Header("Reloading")]
    bool isReloading;
    [SerializeField] float reloadTime;

    [Header("References")]
    Rigidbody2D playerRB;
    public GameObject bulletPrefab;
    public Transform firePoint;
    GameObject statTracker;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentGunMana = maxGunMana;
        statTracker = GameObject.FindWithTag("StatTracker");
        gunLevel = 1;

        ammoInMag = magSize;
        ammoStockpile -= magSize;
    }

    void FixedUpdate()
    {
        LookAtMouse();
        Move();
    }

    void Update()
    {
        GunMana();

        if(dashUnlocked)
        {
            Dash();
        }
     
        Shooting();
        GetCurrentKills();
        GunLeveling();
        Reloading();
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
        if(dashPossible && currentGunMana >= dashCost && Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.time - lastDashButtonTime < doubleTapTime)
            {
                DoDash();
            }

            lastDashButtonTime = Time.time;
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
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire && ammoInMag >= 1)
        {
            ammoInMag -= 1;
            nextFire = Time.time + fireRate;
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

    void GetCurrentKills()
    {
        playerKills = statTracker.GetComponent<StatTracker>().playerKills;
    }

    void GunLeveling()
    {
        switch (playerKills)
        {
            case 0:
                gunLevel = 1;
                break;
            case 1:
                gunLevel = 2;
                break;
            case 2:
                gunLevel = 3;
                break;
            case 3:
                gunLevel = 4;
                break;
            case 4:
                gunLevel = 5;
                break;
        }

        switch (gunLevel)
        {
            case 1:
                print("The gun is level one!");
                fireRate = 1f;
                bulletDamage = 20f;
                break;
            case 2:
                print("The gun is level two!");
                fireRate = .7f;
                bulletDamage = 30f;
                dashUnlocked = true;
                break;
            case 3:
                print("The gun is level three!");
                fireRate = .5f;
                bulletDamage = 40f;
                break;
            case 4:
                print("The gun is level four!");
                fireRate = .3f;
                bulletDamage = 50f;
                break;
            case 5:
                print("The gun is level five!");
                fireRate = .1f;
                bulletDamage = 60f;
                break;
        }
    }

    void Reloading()
    {
        if((Input.GetKeyDown("R") || ammoInMag == 0) && !isReloading && ammoStockpile > 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        ammoStockpile -= magSize;
        ammoInMag = magSize;
    }
}
