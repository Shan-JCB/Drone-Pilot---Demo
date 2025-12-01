using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] float backgroundScrollSpeed = 0.5f;
    Material myMaterial;
    Vector2 offset;
    bool stop = false;

    // Use this for initialization
    void Start()
    {
        myMaterial = GetComponent<Renderer>().material;
        offset = new Vector2(backgroundScrollSpeed, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!stop)
        {
            Roll();
        }
    }

    public void Stop()
    {
        stop = true;
    }

    private void Roll()
    {
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
    }

    public void Reset()
    {
        stop = false;
    }
}
