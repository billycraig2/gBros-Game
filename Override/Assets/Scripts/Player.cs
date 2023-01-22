using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField] bool isFullAuto;
    float nextFire;

    [Header("PierceShot")]
    [SerializeField] Sprite bigBulletSprite;
    [SerializeField] float pierceCost;
    public bool hasPierceShot;
    [SerializeField] float pierceFireRate = 4.0f;
    float pierceNextFire;

    [Header("Gun Evolution")]
    [SerializeField] int gunLevel;
    [SerializeField] int playerKills;

    [Header("Reloading")]
    bool isReloading;
    [SerializeField] float reloadTime = 3f;

    [Header("References")]
    Rigidbody2D playerRB;
    public GameObject bulletPrefab;
    public Transform firePoint;
    GameObject statTracker;
    TextMeshProUGUI ammoCounterText;
    TextMeshProUGUI healthText;

    [Header("Audio")]
    [SerializeField] AudioSource singleFireSound;
    [SerializeField] AudioSource deathSound;
    [SerializeField] AudioSource painTwoSound;
    [SerializeField] AudioSource painOneSound;

    [Header("Animations")]
    Animator playerAnim;
    public RuntimeAnimatorController markTwoController;
    public RuntimeAnimatorController markThreeController;
    public RuntimeAnimatorController markFourController;
    public RuntimeAnimatorController markFiveController;

    [Header("Misc")]
    public bool controlsEnabled;
    bool hasDied = false;

    void Start()
    {
        healthText = GameObject.FindWithTag("HealthText").GetComponent<TextMeshProUGUI>();
        playerRB = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentGunMana = maxGunMana;
        statTracker = GameObject.FindWithTag("StatTracker");
        playerAnim = GetComponent<Animator>();
        gunLevel = 1;

        ammoInMag = magSize;
        ammoStockpile -= magSize;

        ammoCounterText = GameObject.FindWithTag("AmmoText").GetComponent<TextMeshProUGUI>();
        controlsEnabled = true;
    }

    void FixedUpdate()
    {
        if(controlsEnabled)
        {
            LookAtMouse();
            Move();
        }  
    }

    void Update()
    {
        GunMana();
       
        if(controlsEnabled)
        {
            Shooting();
            if (dashUnlocked)
            {
                Dash();
                Reloading();
            }
        }
        
        GetCurrentKills();
        GunLeveling();    
        AmmoCounter();
        CheckMovement();
        HealthCounter();
    }

    void HealthCounter()
    {
        healthText.text = "Health: " + currentHealth;
    }

    void ChangeMarkTwoAnims()
    {
        playerAnim.runtimeAnimatorController = markTwoController;
    }

    void ChangeMarkThreeAnims()
    {
        playerAnim.runtimeAnimatorController = markThreeController;
    }

    void ChangeMarkFourAnims()
    {
        playerAnim.runtimeAnimatorController = markFourController;
    }

    void ChangeMarkFiveAnims()
    {
        playerAnim.runtimeAnimatorController = markFiveController;
    }

    void CheckMovement()
    {
        if (playerRB.velocity != Vector2.zero)
        {
            playerAnim.SetBool("isRunning", true);
        }
        else
        {
            playerAnim.SetBool("isRunning", false);
        }
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
        if (isFullAuto)
        {
            if (Input.GetButton("Fire1") && Time.time > nextFire && ammoInMag >= 1)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire && ammoInMag >= 1)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }
        }

        if (currentGunMana >= pierceCost && Input.GetKeyDown(KeyCode.Q) && hasPierceShot && Time.time > pierceNextFire)
        {
            pierceNextFire = Time.time + pierceFireRate;
            ShootPierceShot(); 
        }
    }

    void Shoot()
    {
        singleFireSound.Play();
        ammoInMag -= 1;
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        newBullet.GetComponent<PlayerBullet>().damage = bulletDamage;
        newBullet.GetComponent<PlayerBullet>().isPierceShot = false;
        newBullet.GetComponent<PlayerBullet>().isPlayerBullet = true;
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
    }

    void ShootPierceShot()
    {
        currentGunMana -= 20;
        singleFireSound.Play();
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        newBullet.GetComponent<PlayerBullet>().damage = 1000;
        newBullet.GetComponent<PlayerBullet>().isPierceShot = true;
        newBullet.GetComponent<SpriteRenderer>().sprite = bigBulletSprite;
        newBullet.GetComponent<PlayerBullet>().isPlayerBullet = true;
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
    }

    public void TakeDamage(float DamagePerHit)
    {
        if(!hasDied)
        {
            currentHealth -= DamagePerHit;
            var RandomNo = Random.Range(1, 3);

            if (RandomNo == 1)
            {
                painOneSound.Play();
            }
            else
            {
                painTwoSound.Play();
            }

            if (currentHealth <= 0 && !hasDied)
            {
                Die();
            }
        }       
    }

    void Die()
    {
        hasDied = true;
        playerAnim.SetBool("isDead", true);
        controlsEnabled = false;
        playerRB.isKinematic = true;
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        deathSound.Play();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Robot");
        foreach (GameObject enemy in enemies)
        GameObject.Destroy(enemy);

        GameObject[] spawners = GameObject.FindGameObjectsWithTag("RobotSpawner");
        foreach (GameObject spawner in spawners)
        GameObject.Destroy(spawner);
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
                //Pistol
                hasPierceShot = false;
                fireRate = 1f;
                isFullAuto = false;
                bulletDamage = 30f;
                reloadTime = 2f;
                break;
            case 2:
                //Rifle
                ChangeMarkTwoAnims();
                fireRate = .7f;
                isFullAuto = false;
                bulletDamage = 40f;
                dashUnlocked = true;
                reloadTime = 2f;
                break;
            case 3:
                //Shotgun
                ChangeMarkThreeAnims();
                hasPierceShot = true;
                fireRate = 2f;
                isFullAuto = false;
                bulletDamage = 100f;
                reloadTime = 2f;
                break;
            case 4:
                //Smg
                ChangeMarkFourAnims();
                fireRate = .2f;
                isFullAuto = true;
                bulletDamage = 10f;
                reloadTime = 2f;
                break;
            case 5:
                //Assualt Rifle
                fireRate = .4f;
                isFullAuto = true;
                bulletDamage = 20f;
                reloadTime = 2f;
                break;
        }
    }

    void Reloading()
    {
        if((Input.GetKeyDown(KeyCode.R) || ammoInMag == 0) && !isReloading && ammoStockpile > 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        int ammoToTake;
        if(ammoStockpile >= magSize)
        {
            ammoToTake = magSize - ammoInMag;
            ammoStockpile -= ammoToTake;
            ammoInMag = magSize;
        }
        else
        {
            ammoToTake = ammoStockpile;
            ammoInMag = ammoToTake;
            ammoStockpile -= ammoToTake;
        }

        isReloading = false;
    }

    void AmmoCounter()
    {
        ammoCounterText.text = ammoInMag + "/" + ammoStockpile;
    }

    public void MaxAmmo()
    {
        ammoStockpile += 50;
    }
}
