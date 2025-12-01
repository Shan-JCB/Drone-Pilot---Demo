//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject laserPrefab;

    [Header("Status")]
    [SerializeField] float health = 100;
    [SerializeField] int pointsIfDestroyed = 42;

    [Header("Projectiles")]
    [SerializeField] float shotTimeCounter = 0;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 10f;

    Animator enemieAnimation;

    //cached references
    GameController gameController;

    // Use this for initialization
    void Start()
    {
        enemieAnimation = GetComponent<Animator>();
        gameController = FindObjectOfType<GameController>();
        shotTimeCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotTimeCounter -= Time.deltaTime;
        if (shotTimeCounter <= 0f)
        {
            Fire();
            shotTimeCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileSpeed, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
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
        Destroy(GetComponent<Collider2D>());
        gameController.AddToScore(pointsIfDestroyed);
        Destroy(gameObject, 0.5f);
    }
}
