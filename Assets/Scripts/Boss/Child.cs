using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    [SerializeField] float health = 10000f;
    Animator animator;
    float animationTimer = 0f;
    float shotTimeCounter = 0;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] GameObject laserPrefab;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        shotTimeCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        Animations();
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        if(!animator.GetBool("Die") && animator.GetBool("isAlive"))
        {
            shotTimeCounter -= Time.deltaTime;
            if (shotTimeCounter <= 0f)
            {
                animationTimer = 0f;
                animator.SetBool("Attack", true);
                Fire();
                shotTimeCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
            }
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(-projectileSpeed, 0);
    }

    private void Animations()
    {
        animationTimer += Time.deltaTime;
        if (health <= 9000 && !animator.GetBool("Born"))
        {
            animationTimer = 0f;
            animator.SetBool("Born", true);
        }
        
        if (animationTimer >= 1f && animator.GetBool("Born") && !animator.GetBool("Die"))
        {
            animationTimer = 0f;
            animator.SetBool("isAlive", true);
        }

        if (animationTimer >= 0.5f && animator.GetBool("Die"))
        {
            animationTimer = 0f;
            transform.position = new Vector3(216.81f, transform.position.y, transform.position.z);
            animator.SetBool("Dead", true);
        }

        if (animator.GetBool("Attack") && animationTimer >= 0.5f)
        {
            animator.SetBool("Attack", false);
            animationTimer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!animator.GetBool("Die"))
        {
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
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
        animationTimer = 0f;
        animator.SetBool("Die", true);
        Destroy(GetComponent<Collider2D>());
    }

    public float GetHealth()
    {
        return this.health;
    }
}
