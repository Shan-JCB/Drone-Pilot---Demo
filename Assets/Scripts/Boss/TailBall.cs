using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBall : MonoBehaviour
{
    //[SerializeField] float moveSpeed = 5f;
    [SerializeField] float frequency = 1f;
    [SerializeField] float magnitude = 4f;

    Vector3 pos;

    // Use this for initialization
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        //senoildal movement
        //pos += transform.up * moveSpeed * Time.deltaTime;
        //transform.position = pos + transform.right * Mathf.Sin(Time.time * frequency) * magnitude;

        float x = pos.x + magnitude * Mathf.Sin(frequency * Time.time);
        float y = pos.y + magnitude * Mathf.Sin(frequency * Time.time);

        transform.position = new Vector3(x, y, pos.z);


        //polar equation movement
        //r = a + b * cos(k*t)
        //a=2,b=2,k=2 == infinity sign
        //float r = 2 + 2 * Mathf.Cos(2 * Time.time);


        //r = a + b * sin(k*t)
        //a=2,b=2,k=2 == diagonal infinity sign 
    }

    public void Die()
    {
        Destroy(GetComponent<Collider2D>());
        this.GetComponent<Animator>().SetBool("Die", true);
        Destroy(gameObject, 0.5f);
    }
}
