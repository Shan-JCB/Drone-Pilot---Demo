using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAudioController : MonoBehaviour
{
    [SerializeField] AudioClip mainSound;

    //cached references
    AudioSource cameraAudioSource;

    // Use this for initialization
    void Start()
    {
        cameraAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!cameraAudioSource.isPlaying)
        {
            cameraAudioSource.PlayOneShot(mainSound);
        }
    }
}
