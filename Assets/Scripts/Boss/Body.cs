using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] float health = 1000f;
    Animator animator;
    float timer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (animator.GetBool("Hit") && timer>=0.5f)
        {
            animator.SetBool("Hit", false);
            timer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        timer = 0f;
        animator.SetBool("Hit", true);

        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetBool("NoMouth", true);
        Destroy(GetComponent<Collider2D>());
    }

    public float GetHealth()
    {
        return this.health;
    }
}
