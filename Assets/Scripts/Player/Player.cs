using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //configuration parameters
    [Header("Player")]
    [SerializeField] float padding = 0.5f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float health = 300f;
    // store max health to show HUD
    float maxHealth;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileFiringPeriod = 0.1f;
    [SerializeField] AudioClip projectileSound;
    [Range(0f, 1f)] [SerializeField] float shotSoundVolume = 0.5f;

    [Header("Animation")]
    [SerializeField] GameObject turbo;
    [SerializeField] GameObject firing;
    [SerializeField] GameObject initialPath;

    Coroutine firingCoroutine;

    //cached references
    Animator playerAnimation;
    Animator turboAnimation;
    Animator firingAnimation;
    GameController gameController;
    AudioSource playerAudioSource;

    //configurations
    float xMin, yMin;
    float xMax, yMax;
    bool started = false;
    List<Transform> waypoints;
    int waypointIndex = 0;
    
    [Header("Systems")]
    [SerializeField] BatterySystem batterySystem;
    [SerializeField] RepairMinigame repairMinigame;

    enum PlayerMode { Starting, Flight, Repair }
    PlayerMode mode = PlayerMode.Starting;


    // Use this for initialization
    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        playerAudioSource = GetComponent<AudioSource>();

        playerAnimation = GetComponent<Animator>();
        turboAnimation = GetComponentInChildren<Animator>();
        firingAnimation = GetComponentInChildren<Animator>();

        turbo.SetActive(false);
        firing.SetActive(false);

        waypoints = new List<Transform>();
        GetWaypoints();
        transform.position = waypoints[waypointIndex].transform.position;

        // store max health and initialize HUD
        maxHealth = health;
        if (gameController != null)
        {
            gameController.UpdatePlayerHealth(health, maxHealth);
        }

        // subscribe to battery depleted event (optional)
        if (batterySystem != null)
        {
            batterySystem.onBatteryDepleted += OnBatteryDepleted;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == PlayerMode.Flight && started == true)
        {
            SetUpMoveBoundaries();
            Move();
            Fire();

            // drain battery while flying
            if (batterySystem != null)
            {
                batterySystem.DrainOverTime(Time.deltaTime);
            }
        }
        else
        {
            LevelStartTurbo();
        }
    }

    private void LevelStartTurbo()
    {
        turbo.SetActive(true);
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition)
            {
                waypointIndex++;
            }
        }
        else
        {
            started = true;
            mode = PlayerMode.Flight;
            turbo.SetActive(false);
        }
    }

    private void GetWaypoints()
    {
        foreach (Transform child in initialPath.transform)
        {
            waypoints.Add(child);
        }
    }

    private void Fire()
    {
        if (mode != PlayerMode.Flight) return; // cannot fire in other modes

        if (Input.GetButtonDown("Fire1"))
        {
            // check battery before starting continuous fire
            if (batterySystem != null && batterySystem.IsEmpty()) return;

            firingCoroutine = StartCoroutine(FireContinuously());
            firing.SetActive(true);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (firingCoroutine != null)
            {
                StopCoroutine(firingCoroutine);
            }
            firing.SetActive(false);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            Vector3 projectileInitialPosition = new Vector3(transform.position.x + 1f, transform.position.y - 0.1f, transform.position.z);
            GameObject laser = Instantiate(laserPrefab, projectileInitialPosition, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed, 0);
            ShotSFX();

            // consume battery per shot; if battery empty, stop firing
            if (batterySystem != null)
            {
                bool ok = batterySystem.ConsumeForShot();
                if (!ok)
                {
                    firing.SetActive(false);
                    yield break;
                }
            }

            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void ShotSFX()
    {
        AudioSource.PlayClipAtPoint(projectileSound, transform.position, shotSoundVolume);
    }

    private void Move()
    {
        var dX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var dY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        if (dY > 0)
        {
            playerAnimation.SetBool("Up", true);
            playerAnimation.SetBool("Down", false);
        }
        else if (dY < 0)
        {
            playerAnimation.SetBool("Up", false);
            playerAnimation.SetBool("Down", true);
        }
        else
        {
            playerAnimation.SetBool("Up", false);
            playerAnimation.SetBool("Down", false);
        }

        var newXPos = Mathf.Clamp(transform.position.x + dX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + dY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0.1f, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // handle repair zone trigger
        // Use safe string comparison instead of CompareTag("RepairZone") which throws if the tag doesn't exist
        try
        {
            if (other.gameObject.tag == "RepairZone")
        {
            TryStartRepair();
            return;
        }
        }
        catch (System.Exception)
        {
            // If tag system returns an unexpected error, warn once.
            Debug.LogWarning("RepairZone tag not found or inaccessible. Create a 'RepairZone' tag in the Tag Manager and assign it to repair-zone GameObjects to enable repairs.");
        }

        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    void TryStartRepair()
    {
        if (repairMinigame == null || batterySystem == null) return;
        if (repairMinigame.IsActive) return;

        // enter repair mode
        mode = PlayerMode.Repair;

        // stop firing if active
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firing.SetActive(false);
        }

        // start minigame and listen for result
        repairMinigame.onMinigameFinished += OnRepairFinished;
        repairMinigame.StartMinigame();
    }

    void OnRepairFinished(bool success)
    {
        repairMinigame.onMinigameFinished -= OnRepairFinished;
        if (success && batterySystem != null)
        {
            batterySystem.RepairSuccess();
        }

        // back to flight mode
        mode = PlayerMode.Flight;
    }

    void OnBatteryDepleted()
    {
        // simple reaction: stop firing and reduce speed to half until repaired
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firing.SetActive(false);
        }
        moveSpeed = Mathf.Max(0.1f, moveSpeed * 0.5f);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();

        if (damageDealer.tag != "Ground")
        {
            damageDealer.Hit();
        }

        // update health on HUD
        if (gameController != null)
        {
            gameController.UpdatePlayerHealth(Mathf.Max(0f, health), maxHealth);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firing.SetActive(false);
        }

        playerAnimation.SetBool("Die", true);
        Destroy(gameObject, 0.5f);
        // ensure HUD shows zero
        if (gameController != null)
        {
            gameController.UpdatePlayerHealth(0f, maxHealth);
        }
        gameController.GameOver();
    }

    // public getters used by GameController for HUD initialization
    public float GetHealth() => health;
    public float GetMaxHealth() => maxHealth;
}
