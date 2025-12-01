using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper that applies a global speed multiplier to scroll components.
/// Useful for testing and quick tuning.
/// </summary>
public class GameSpeedManager : MonoBehaviour
{
    [Header("Debug / Tuning")]
    [Tooltip("Multiply camera/background speeds by this value at Start().")]
    [SerializeField] float globalMultiplier = 2.0f;

    void Start()
    {
        var cameras = FindObjectsOfType<CameraScroller>();
        foreach (var c in cameras)
        {
            c.SetSpeedMultiplier(globalMultiplier);
        }

        var backgrounds = FindObjectsOfType<BackgroundScroller>();
        foreach (var b in backgrounds)
        {
            b.SetSpeedMultiplier(globalMultiplier);
        }
    }

    public void SetGlobalMultiplier(float m)
    {
        globalMultiplier = m;
        var cameras = FindObjectsOfType<CameraScroller>();
        foreach (var c in cameras) c.SetSpeedMultiplier(globalMultiplier);
        var backgrounds = FindObjectsOfType<BackgroundScroller>();
        foreach (var b in backgrounds) b.SetSpeedMultiplier(globalMultiplier);
    }
}
