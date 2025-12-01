using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01 : MonoBehaviour
{
    [SerializeField] CameraScroller cameraScroller;
    [SerializeField] Body mouth;
    [SerializeField] TailHeadBall tail;
    [SerializeField] Child child;
    [SerializeField] GameObject[] Explosions;
    float animationTimer = 0f;
    bool dead = false;

    private void Awake()
    {
        foreach (GameObject exp in Explosions)
        {
            exp.SetActive(false);
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        animationTimer += Time.deltaTime;
        if (mouth.GetHealth() <= 0 && (tail.GetHealth() <= 0 || tail == null) && child.GetHealth() <= 0 && !dead)
        {
            if(!dead)
            {
                animationTimer = 0f;
            }
            Die();
        }
        if (animationTimer >= 2f && dead)
        {
            foreach (GameObject exp in Explosions)
            {
                exp.SetActive(false);
            }
            cameraScroller.Stop(false);
            Destroy(gameObject);
        }
    }

    private void Die()
    {
        Destroy(GetComponent<SpriteRenderer>());
        dead = true;
        foreach (GameObject exp in Explosions)
        {
            exp.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemieSpawnTrigger")
        {
            cameraScroller.Stop(true);
        }
    }
}
