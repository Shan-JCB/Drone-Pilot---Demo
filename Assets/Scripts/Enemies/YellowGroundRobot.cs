using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowGroundRobot : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject laserPrefab;

    [Header("Status")]
    [SerializeField] float health = 100f;
    [SerializeField] int pointsIfDestroyed = 42;
    [SerializeField] float enemieSpeed = 10f;

    [Header("Projectile")]
    [SerializeField] float shotTimeCounter = 0;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 10f;

    //cached references
    Animator enemieAnimation;
    GameController gameController;
    Player player;
    Rigidbody2D enemie;

    //laser configurations
    GameObject laser;
    Vector3 playerPos;
    bool chegou = false;
    float projectileDeltaTime = 0f;
    float projectileVelocityX = 0f;
    float projectileVelocityY = 0f;

    //player configurations
    bool flipped = false;


    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        enemieAnimation = GetComponent<Animator>();
        gameController = FindObjectOfType<GameController>();
        enemie = GetComponent<Rigidbody2D>();
        shotTimeCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        //if the player die we need to refind the other one if the player retry
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        Shoot();
        Movement();
    }

    private void Movement()
    {
        if (player != null)
        {
            float movementThisFrame = Time.deltaTime * enemieSpeed;

            if (player.transform.position.x+5 < transform.position.x)
            {
                enemie.velocity = new Vector2(-movementThisFrame, 0);
                enemieAnimation.SetBool("Walk", true);
                enemie.GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (player.transform.position.x+5 > transform.position.x)
            {
                enemie.velocity = new Vector2(movementThisFrame, 0);
                enemieAnimation.SetBool("Walk", true);
                enemie.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                enemie.velocity = new Vector2(0, 0);
                enemieAnimation.SetBool("Walk", false);
                enemie.GetComponent<SpriteRenderer>().flipX = false;
            }

            //WalkAnimation();
        }
    }

    private void WalkAnimation()
    {
        if (enemie.velocity.x < 0)
        {
            enemieAnimation.SetBool("Walk", true);
        }
        else if (enemie.velocity.x > 0)
        {
            enemieAnimation.SetBool("Walk", true);
        }
        else
        {
            enemieAnimation.SetBool("Walk", false);
        }
    }

    private void Shoot()
    {
        float movementThisFrame = projectileSpeed * Time.deltaTime;

        if (laser == null)
        {
            CountDownAndShoot();
        }
        else
        {
            ProjectileDirection(movementThisFrame);
        }
    }

    private void ProjectileDirection(float movementThisFrame)
    {
        if (laser.transform.position != playerPos && chegou == false)
        {
            projectileDeltaTime += Time.deltaTime;
            laser.transform.position = Vector2.MoveTowards(laser.transform.position, playerPos, movementThisFrame);
            projectileVelocityX = (laser.transform.position.x - transform.position.x) / projectileDeltaTime;
            projectileVelocityY = (laser.transform.position.y - transform.position.y) / projectileDeltaTime;
        }
        else
        {
            chegou = true;
            laser.transform.position += new Vector3(projectileVelocityX * Time.deltaTime, projectileVelocityY * Time.deltaTime, 0);
        }
    }

    private void CountDownAndShoot()
    {
        projectileDeltaTime = 0f;
        chegou = false;
        shotTimeCounter -= Time.deltaTime;
        if (shotTimeCounter <= 0f && player != null)
        {
            Fire();
            shotTimeCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        playerPos = player.gameObject.transform.position;
        laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
            if (!damageDealer) { return; }
            ProcessHit(damageDealer);
        }
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        enemieAnimation.SetBool("Die", true);
        //Destroy(GetComponent<Collider2D>());
        gameController.AddToScore(pointsIfDestroyed);
        Destroy(gameObject, 0.5f);
    }
}
