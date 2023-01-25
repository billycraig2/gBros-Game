using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] float currentGunMana = 50;
    [SerializeField] float maxGunMana = 50;
    [SerializeField] float manaRegenSpeed = 1;
    [SerializeField] float manaPercentage = 1;

    [Header("Bullet")]
    [SerializeField] float bulletDamage = 20f;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] float fireRate = 1f;
    public int ammoStockpile = 24;
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

    [Header("RapidFire")]
    [SerializeField] bool rapidFireActive;
    [SerializeField] bool rapidFireUnlocked;
    [SerializeField] bool rapidFireCooledDown;
    [SerializeField] float rapidFireCost;

    [Header("Gun Evolution")]
    [SerializeField] int gunLevel;
    [SerializeField] int playerKills;
    [SerializeField] float upgradeHoldTime = 3f;
    [SerializeField] GameObject upgradeExplosion;
    public int killsToNextUpgrade;
    float holdTime = 0f;
    bool keyIsHeld;

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
    [SerializeField] TextMeshProUGUI promptText;

    [Header("Audio")]
    [SerializeField] AudioSource singleFireSound;
    [SerializeField] AudioSource deathSound;
    [SerializeField] AudioSource painTwoSound;
    [SerializeField] AudioSource painOneSound;
    [SerializeField] AudioSource reloadSound;
    [SerializeField] AudioSource walkSound;
    [SerializeField] AudioSource upgradeSound;
    [SerializeField] AudioSource explosionSound;

    [Header("Animations")]
    public RuntimeAnimatorController markTwoController;
    public RuntimeAnimatorController markThreeController;
    public RuntimeAnimatorController markFourController;
    public RuntimeAnimatorController markFiveController;
    Animator playerAnim;

    [Header("UI")]
    [SerializeField] GameObject heartOne;
    [SerializeField] GameObject heartTwo;
    [SerializeField] GameObject heartThree;
    [SerializeField] GameObject heartFour;
    [SerializeField] GameObject heartFive;
    [SerializeField] GameObject emptyHeartOne;
    [SerializeField] GameObject emptyHeartTwo;
    [SerializeField] GameObject pistolIcon;
    [SerializeField] GameObject rifleIcon;
    [SerializeField] GameObject shotgunIcon;
    [SerializeField] GameObject smgIcon;
    [SerializeField] GameObject arIcon;
    [SerializeField] GameObject emptyLevelBar;
    [SerializeField] GameObject firstLevelBar;
    [SerializeField] GameObject secondLevelBar;
    [SerializeField] GameObject thirdLevelBar;
    [SerializeField] GameObject fourthLevelBar;
    [SerializeField] GameObject readyToUpgradeText;
    [SerializeField] Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    [SerializeField] GameObject emptyManaBar;
    [SerializeField] GameObject firstManaBar;
    [SerializeField] GameObject secondManaBar;
    [SerializeField] GameObject thirdManaBar;
    [SerializeField] GameObject fourthManaBar;

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
    bool upgradeSoundStarted;
    bool readyToUpgrade;
    public bool maxManaActive;

    public float levelPercentage;
    float level2killsneeded = 10;
    float level3killsneeded = 20;
    float level4killsneeded = 30;
    float level5killsneeded = 40;

    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] TextMeshProUGUI roundTextEnd;

    bool isGameOver;
    bool gunLevelPromptDone;
    bool dashPromptDone;
    bool rapidFirePromptDone;
    bool piercePromptDone;
    bool grenadePromptDone;


    void Start()
    {
        mainCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        rapidFireCooledDown = true;
        killsToNextUpgrade = 10;
        readyToUpgrade = false;
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
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
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
        GunUIUpdate();
        LevelPercentageCalculation();
        RapidFire();
        ManaBar();

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

        if(isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Final Scene", LoadSceneMode.Single);
        }

        if (isGameOver && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        if(gunLevel == 1 && readyToUpgrade && !gunLevelPromptDone)
        {
            StartCoroutine(LevelGun());
        }

        if(gunLevel == 2 && !dashPromptDone)
        {
            StartCoroutine(DashPromptRoutine());
        }

        if (gunLevel == 3 && !piercePromptDone)
        {
            StartCoroutine(PiercePromptRoutine());
        }

        if (gunLevel == 4 && !rapidFirePromptDone)
        {
            StartCoroutine(RapidFirePromptRoutine());
        }

        if (gunLevel == 5 && !grenadePromptDone)
        {
            StartCoroutine(GrenadePromptRoutine());
        }
    }

    void ManaBar()
    {
        manaPercentage = currentGunMana / maxGunMana;

        if (manaPercentage > .80f && manaPercentage <= 1f)
        {
            emptyManaBar.SetActive(false);
            firstManaBar.SetActive(false);
            secondManaBar.SetActive(false);
            thirdManaBar.SetActive(false);
            fourthManaBar.SetActive(true);
        }

        if (manaPercentage > .60f && manaPercentage <= .80f)
        {
            emptyManaBar.SetActive(false);
            firstManaBar.SetActive(false);
            secondManaBar.SetActive(false);
            thirdManaBar.SetActive(true);
            fourthManaBar.SetActive(false);
        }

        if (manaPercentage > .40f && manaPercentage <= .60f)
        {
            emptyManaBar.SetActive(false);
            firstManaBar.SetActive(false);
            secondManaBar.SetActive(true);
            thirdManaBar.SetActive(false);
            fourthManaBar.SetActive(false);
        }

        if (manaPercentage > .20f && manaPercentage <= .40f)
        {
            emptyManaBar.SetActive(false);
            firstManaBar.SetActive(true);
            secondManaBar.SetActive(false);
            thirdManaBar.SetActive(false);
            fourthManaBar.SetActive(false);
        }

        if (manaPercentage <= .2f)
        {
            emptyManaBar.SetActive(true);
            firstManaBar.SetActive(false);
            secondManaBar.SetActive(false);
            thirdManaBar.SetActive(false);
            fourthManaBar.SetActive(false);
        }
    }

    void RapidFire()
    {
        if(Input.GetKeyDown(KeyCode.E) && rapidFireUnlocked && currentGunMana >= rapidFireCost && rapidFireCooledDown)
        {
            StartCoroutine(RapidFireRoutine());
        }
    }

    IEnumerator RapidFireRoutine()
    {
        rapidFireCooledDown = false;
        currentGunMana -= rapidFireCost;
        rapidFireActive = true;
        yield return new WaitForSeconds(10f);
        rapidFireActive = false;
        yield return new WaitForSeconds(20f);
        rapidFireCooledDown = true;
    }

    void LevelPercentageCalculation()
    {
        if (gunLevel == 1)
        {
            levelPercentage = killsToNextUpgrade / level2killsneeded;
        }

        if (gunLevel == 2)
        {
            levelPercentage = killsToNextUpgrade / level3killsneeded;
        }

        if (gunLevel == 3)
        {
            levelPercentage = killsToNextUpgrade / level4killsneeded;
        }

        if (gunLevel == 4)
        {
            levelPercentage = killsToNextUpgrade / level5killsneeded;
        }

        if (gunLevel == 5)
        {
            levelPercentage = 0f;
        }

        if (levelPercentage > .75f && levelPercentage <= 1f)
        {
            emptyLevelBar.SetActive(true);
            firstLevelBar.SetActive(false);
            secondLevelBar.SetActive(false);
            thirdLevelBar.SetActive(false);
            fourthLevelBar.SetActive(false);
            readyToUpgradeText.SetActive(false);
        }

        if (levelPercentage > .50f && levelPercentage <= .75f)
        {
            emptyLevelBar.SetActive(false);
            firstLevelBar.SetActive(true);
            secondLevelBar.SetActive(false);
            thirdLevelBar.SetActive(false);
            fourthLevelBar.SetActive(false);
            readyToUpgradeText.SetActive(false);
        }

        if (levelPercentage > .25f && levelPercentage <= .50f)
        {
            emptyLevelBar.SetActive(false);
            firstLevelBar.SetActive(false);
            secondLevelBar.SetActive(true);
            thirdLevelBar.SetActive(false);
            fourthLevelBar.SetActive(false);
            readyToUpgradeText.SetActive(false);
        }

        if (levelPercentage > .0f && levelPercentage <= .25f)
        {
            emptyLevelBar.SetActive(false);
            firstLevelBar.SetActive(false);
            secondLevelBar.SetActive(false);
            thirdLevelBar.SetActive(true);
            fourthLevelBar.SetActive(false);
            readyToUpgradeText.SetActive(false);
        }

        if (levelPercentage <= 0f)
        {
            emptyLevelBar.SetActive(false);
            firstLevelBar.SetActive(false);
            secondLevelBar.SetActive(false);
            thirdLevelBar.SetActive(false);
            fourthLevelBar.SetActive(true);
            readyToUpgradeText.SetActive(true);
        }
    }

    void GunUIUpdate()
    {
        if(gunLevel == 1)
        {
            pistolIcon.SetActive(true);
            rifleIcon.SetActive(false);
            shotgunIcon.SetActive(false);
            smgIcon.SetActive(false);
            arIcon.SetActive(false);
        }

        if (gunLevel == 2)
        {
            pistolIcon.SetActive(false);
            rifleIcon.SetActive(true);
            shotgunIcon.SetActive(false);
            smgIcon.SetActive(false);
            arIcon.SetActive(false);
        }

        if (gunLevel == 3)
        {
            pistolIcon.SetActive(false);
            rifleIcon.SetActive(false);
            shotgunIcon.SetActive(true);
            smgIcon.SetActive(false);
            arIcon.SetActive(false);
        }

        if (gunLevel == 4)
        {
            pistolIcon.SetActive(false);
            rifleIcon.SetActive(false);
            shotgunIcon.SetActive(false);
            smgIcon.SetActive(true);
            arIcon.SetActive(false);
        }

        if (gunLevel == 5)
        {
            pistolIcon.SetActive(false);
            rifleIcon.SetActive(false);
            shotgunIcon.SetActive(false);
            smgIcon.SetActive(false);
            arIcon.SetActive(true);
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
            GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>().RecordRound();
            mainCanvas.SetActive(false);
            gameOverCanvas.SetActive(true);
            roundTextEnd.text = "You survived " + GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>().currentRound.ToString() + " rounds!";
            isGameOver = true;
        }       
    }

    void GetCurrentKills()
    {
        playerKills = statTracker.GetComponent<StatTracker>().playerKills;
    }

    void GunLeveling()
    {
        if(killsToNextUpgrade <= 0)
        {
            if(gunLevel != 2)
            {
                readyToUpgrade = true;
            }

            if (gunLevel != 3)
            {
                readyToUpgrade = true;
            }

            if (gunLevel != 4)
            {
                readyToUpgrade = true;
            }

            if (gunLevel != 6)
            {
                readyToUpgrade = true;
            }
        }

        switch (gunLevel)
        {
            case 1:
                //Pistol
                hasPierceShot = false;
                if(rapidFireActive)
                {
                    fireRate = .1f;
                }
                else
                {
                    fireRate = 1f;
                }            
                isFullAuto = false;
                bulletDamage = 50f;
                break;
            case 2:
                //Rifle
                ChangeMarkTwoAnims();
                if (rapidFireActive)
                {
                    fireRate = .1f;
                }
                else
                {
                    fireRate = .7f;
                }              
                isFullAuto = false;
                bulletDamage = 100f;
                dashUnlocked = true;
                break;
            case 3:
                //Shotgun
                ChangeMarkThreeAnims();
                hasPierceShot = true;
                if (rapidFireActive)
                {
                    fireRate = .1f;
                }
                else
                {
                    fireRate = 2f;
                }        
                isFullAuto = false;
                bulletDamage = 100f;
                break;
            case 4:
                //Smg
                rapidFireUnlocked = true;
                ChangeMarkFourAnims();
                if (rapidFireActive)
                {
                    fireRate = .1f;
                }
                else
                {
                    fireRate = .2f;
                }             
                isFullAuto = true;
                bulletDamage = 40f;
                break;
            case 5:
                //Assualt Rifle
                ChangeMarkFiveAnims();
                grenadeUnlocked = true;
                if (rapidFireActive)
                {
                    fireRate = .1f;
                }
                else
                {
                    fireRate = .4f;
                }
                isFullAuto = true;
                bulletDamage = 60f;
                break;
        }

        if(readyToUpgrade && Input.GetKey(KeyCode.LeftShift))
        {
            if(!upgradeSoundStarted)
            {
                upgradeSoundStarted = true;
                upgradeSound.Play();
            }
            
            if(!keyIsHeld)
            {
                keyIsHeld = true;
                holdTime = Time.time;
            }
            else
            {
                if (Time.time - holdTime >= upgradeHoldTime && readyToUpgrade)
                {
                    readyToUpgrade = false;
                    killsToNextUpgrade = ((gunLevel * 10) + 10);

                    Instantiate(upgradeExplosion, transform.position, Quaternion.identity);
                    explosionSound.Play();                    
                    gunLevel += 1;
                }
            }
        }
        else
        {
            upgradeSoundStarted = false;
            upgradeSound.Stop();
            keyIsHeld = false;
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

    IEnumerator LevelGun()
    {
        gunLevelPromptDone = true;
        promptText.text = "Hold shift to evolve!";
        yield return new WaitForSeconds(4f);
        promptText.text = "";
    }

    IEnumerator DashPromptRoutine()
    {
        dashPromptDone = true;
        promptText.text = "Dash Unlocked!";
        yield return new WaitForSeconds(4f);
        promptText.text = "";
    }

    IEnumerator PiercePromptRoutine()
    {
        piercePromptDone = true;
        promptText.text = "Pierce Shot Unlocked!";
        yield return new WaitForSeconds(4f);
        promptText.text = "";
    }

    IEnumerator RapidFirePromptRoutine()
    {
        rapidFirePromptDone = true;
        promptText.text = "Rapid Fire Unlocked!";
        yield return new WaitForSeconds(4f);
        promptText.text = "";
    }

    IEnumerator GrenadePromptRoutine()
    {
        grenadePromptDone = true;
        promptText.text = "Grenades Unlocked!";
        yield return new WaitForSeconds(4f);
        promptText.text = "";
    }
}