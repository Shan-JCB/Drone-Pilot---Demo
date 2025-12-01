using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [SerializeField] float CameraScrollSpeed = 0.5f;
    Vector3 velocity;
    bool stop = false;

    // Use this for initialization
    void Start()
    {
        InitialState();
        velocity = new Vector3(CameraScrollSpeed, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!stop)
        {
            Roll();
        }
    }

    private void InitialState()
    {
        transform.position = new Vector3(-7,0,-1);
    }

    public void Stop(bool toggle)
    {
        stop = toggle;
    }

    private void Roll()
    {
        transform.position += velocity * Time.deltaTime;
    }

    public void Reset()
    {
        stop = false;
        transform.position = new Vector3(-7, 0, -1);
    }
}
