using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailHeadBall : MonoBehaviour
{
    [SerializeField] float health = 500f;
    [SerializeField] TailBall[] tailBalls;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
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
        Destroy(GetComponent<Collider2D>());
        foreach(TailBall ball in tailBalls)
        {
            this.GetComponent<Animator>().SetBool("Die", true);
            ball.Die();
        }
        Destroy(gameObject,0.5f);
    }

    public float GetHealth()
    {
        return this.health;
    }
}
