using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [SerializeField] float CameraScrollSpeed = 0.5f; // base units per second — tweak in inspector
    [Header("Tuning")]
    [Tooltip("Global multiplier to quickly speed up/slow down the camera for testing or difficulty scaling.")]
    [SerializeField] float speedMultiplier = 1f;
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
        transform.position += velocity * Time.deltaTime * speedMultiplier;
    }

    // runtime API to change the scroll speed multiplier
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Max(0f, multiplier);
    }

    public void Reset()
    {
        stop = false;
        transform.position = new Vector3(-7, 0, -1);
    }
}
