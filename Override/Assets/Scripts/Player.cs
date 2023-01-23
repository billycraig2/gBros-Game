using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 60;
    public float currentHealth;

    [Header("Perks")]
    public bool hasJug;
    public bool hasStamina;
    public bool hasSpeedCola;
    public bool hasManaBoost;
    public bool hasQuickRevive;

    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float boostedSpeed = 9f;

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
    [SerializeField] float shotgunSpread = 10f;
    float nextFire;

    [Header("PierceShot")]
    [SerializeField] Sprite bigBulletSprite;
    [SerializeField] float pierceCost;
    public bool hasPierceShot;
    [SerializeField] float pierceFireRate = 4.0f;
    float pierceNextFire;

    [Header("Grenade")]
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] float throwForce = 20f;
    [SerializeField] float grenadeCost = 50f;
    [SerializeField] float grenadeWaitTime = 2.0f;
    [SerializeField] bool grenadeUnlocked;
    float grenadeNextFire;
    float lastGrenadeTime;

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
    [SerializeField] GameObject dashEffect;

    [Header("Audio")]
    [SerializeField] AudioSource singleFireSound;
    [SerializeField] AudioSource deathSound;
    [SerializeField] AudioSource painTwoSound;
    [SerializeField] AudioSource painOneSound;
    [SerializeField] AudioSource reloadSound;
    [SerializeField] AudioSource walkSound;

    [Header("Animations")]
    Animator playerAnim;
    public RuntimeAnimatorController markTwoController;
    public RuntimeAnimatorController markThreeController;
    public RuntimeAnimatorController markFourController;
    public RuntimeAnimatorController markFiveController;

    [Header("UI")]
    [SerializeField] GameObject heartOne;
    [SerializeField] GameObject heartTwo;
    [SerializeField] GameObject heartThree;
    [SerializeField] GameObject heartFour;
    [SerializeField] GameObject heartFive;
    [SerializeField] GameObject emptyHeartOne;
    [SerializeField] GameObject emptyHeartTwo;


    [Header("Misc")]
    public bool controlsEnabled;
    bool hasDied = false;
    bool walkSoundStarted;

    bool jugEffectApplied;
    bool staminaBoostApplied;
    bool speedColaBoostApplied;
    bool manaBoostApplied;
    bool quickReviveApplied;
    bool extraLifeUsed;
    public bool maxManaActive;

    void Start()
    {
        reloadTime = 1.2f;
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
        playerRB.angularVelocity = 0f;
        if (controlsEnabled)
        {
            Shooting();
            if (dashUnlocked)
            {
                Dash();              
            }
            Reloading();
            Grenade();
        }

        HealthUIUpdate();
        GetCurrentKills();
        GunLeveling();    
        AmmoCounter();
        CheckMovement();

        if(!hasJug)
        {
            emptyHeartOne.SetActive(false);
            emptyHeartTwo.SetActive(false);
        }
        
        if(hasJug && !jugEffectApplied)
        {
            jugEffectApplied = true;
            maxHealth = 100;
            currentHealth = maxHealth;
            emptyHeartOne.SetActive(true);
            emptyHeartTwo.SetActive(true);
        }

        if(hasStamina && !staminaBoostApplied)
        {
            staminaBoostApplied = true;
            movementSpeed = boostedSpeed;
        }

        if(hasSpeedCola && !speedColaBoostApplied)
        {
            speedColaBoostApplied = true;
            reloadTime = 0.6f;
            reloadSound.pitch = 2f;
        }

        if (hasManaBoost && !manaBoostApplied)
        {
            manaBoostApplied = true;
            maxGunMana = 200f;
            currentGunMana = maxGunMana;
            manaRegenSpeed = 2f;
        }
    }

    void HealthUIUpdate()
    {
        if (currentHealth <= 0)
        {
            heartOne.SetActive(false);
            heartTwo.SetActive(false);
            heartThree.SetActive(false);
            heartFour.SetActive(false);
            heartFive.SetActive(false);
        }

        if (currentHealth <= 20 && currentHealth > 0)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(false);
            heartThree.SetActive(false);
            heartFour.SetActive(false);
            heartFive.SetActive(false);
        }

        if(currentHealth <= 40 && currentHealth > 20)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(true);
            heartThree.SetActive(false);
            heartFour.SetActive(false);
            heartFive.SetActive(false);
        }

        if (currentHealth <= 60 && currentHealth > 40)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(true);
            heartThree.SetActive(true);
            heartFour.SetActive(false);
            heartFive.SetActive(false);
        }

        if (currentHealth <= 80 && currentHealth > 60)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(true);
            heartThree.SetActive(true);
            heartFour.SetActive(true);
            heartFive.SetActive(false);
        }

        if (currentHealth <= 100 && currentHealth > 80)
        {
            heartOne.SetActive(true);
            heartTwo.SetActive(true);
            heartThree.SetActive(true);
            heartFour.SetActive(true);
            heartFive.SetActive(true);
        }
    }

    void Grenade()
    {
        if(Input.GetKeyDown(KeyCode.G) && grenadePossible && currentGunMana >= grenadeCost && grenadeUnlocked)
        {
            print("Throw grenade");
            currentGunMana -= grenadeCost;
            GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * throwForce, ForceMode2D.Impulse);
            lastGrenadeTime = Time.time;
            StartCoroutine(grenade.GetComponent<Grenade>().Explode());
        }
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
            if(!walkSoundStarted)
            {
                walkSoundStarted = true;
                walkSound.Play();
            }         
            playerAnim.SetBool("isRunning", true);
        }
        else
        {
            walkSoundStarted = false;
            walkSound.Stop();
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
        if(currentGunMana < maxGunMana && !maxManaActive)
        {
            currentGunMana += manaRegenSpeed * Time.deltaTime;
        }

        if(maxManaActive)
        {
            currentGunMana = maxGunMana;
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

    bool grenadePossible
    {
        get
        {
            return Time.time - lastGrenadeTime > grenadeWaitTime;
        }
    }

    void DoDash()
    {
        Vector2 direction = playerRB.velocity.normalized;
        GameObject dashObj = Instantiate(dashEffect, firePoint.position, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        dashObj.transform.rotation = rotation;
        currentGunMana -= dashCost;
        lastDashTime = Time.time;
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        movementSpeed += dashSpeed;
        yield return new WaitForSeconds(dashLength);
        movementSpeed -= dashSpeed;
        playerRB.angularVelocity = 0f;
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
        if(gunLevel == 3)
        {
            ShootShotgunShoot();
        }
        else
        {
            ammoInMag -= 1;
            singleFireSound.Play();
            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            newBullet.GetComponent<PlayerBullet>().damage = bulletDamage;
            newBullet.GetComponent<PlayerBullet>().isPierceShot = false;
            newBullet.GetComponent<PlayerBullet>().isPlayerBullet = true;
            Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletSpeed, ForceMode2D.Impulse);
        }       
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

    void ShootShotgunShoot()
    {
        if(ammoInMag < 3)
        {
            StartCoroutine(ReloadRoutine());
        }
        else
        {
            ammoInMag -= 3;
            singleFireSound.Play();
            for (int i = -1; i <= 1; i++)
            {
                GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                newBullet.GetComponent<PlayerBullet>().damage = bulletDamage;
                newBullet.GetComponent<PlayerBullet>().isPierceShot = false;
                newBullet.GetComponent<PlayerBullet>().isPlayerBullet = true;
                Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
                rb.velocity = Quaternion.Euler(0, 0, shotgunSpread * i) * firePoint.up * bulletSpeed;
            }
        }      
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
        if (hasQuickRevive && !extraLifeUsed)
        {
            extraLifeUsed = true;
            currentHealth = maxHealth;
            transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            playerRB.simulated = false;
            GetComponent<SpriteRenderer>().sortingOrder = 2;
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
                bulletDamage = 60f;
                break;
            case 2:
                //Rifle
                ChangeMarkTwoAnims();
                fireRate = .7f;
                isFullAuto = false;
                bulletDamage = 40f;
                dashUnlocked = true;
                break;
            case 3:
                //Shotgun
                ChangeMarkThreeAnims();
                hasPierceShot = true;
                fireRate = 2f;
                isFullAuto = false;
                bulletDamage = 100f;
                break;
            case 4:
                //Smg
                ChangeMarkFourAnims();
                fireRate = .2f;
                isFullAuto = true;
                bulletDamage = 20f;
                break;
            case 5:
                //Assualt Rifle
                ChangeMarkFiveAnims();
                grenadeUnlocked = true;
                fireRate = .4f;
                isFullAuto = true;
                bulletDamage = 40f;
                break;
        }
    }

    void Reloading()
    {
        if((Input.GetKeyDown(KeyCode.R) || ammoInMag == 0) && !isReloading && ammoStockpile > 0 && ammoInMag != magSize)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        reloadSound.Play();
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

    public void ActivateMaxMana()
    {
        StartCoroutine(MaxMana());
    }

    IEnumerator MaxMana()
    {
        maxManaActive = true;
        yield return new WaitForSeconds(30f);
        maxManaActive = false;
    }
}
