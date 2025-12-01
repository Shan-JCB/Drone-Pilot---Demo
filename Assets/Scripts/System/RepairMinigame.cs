using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Lightweight, testable skeleton for a repair minigame.
/// This class runs a simple "press X times within T seconds" game and returns success/failure through a callback.
/// Hook this to UI later (Canvas + prompts).
/// </summary>
public class RepairMinigame : MonoBehaviour
{
    [Header("Minigame")]
    [SerializeField] KeyCode inputKey = KeyCode.Space;
    [SerializeField] int requiredPresses = 3;
    [SerializeField] float timeLimitSeconds = 5f;

    public bool IsActive { get; private set; }

    public event Action<bool> onMinigameFinished; // bool = success

    Coroutine running;

    public void StartMinigame()
    {
        if (IsActive) return;
        running = StartCoroutine(RunMinigame());
    }

    public void CancelMinigame()
    {
        if (!IsActive) return;
        StopCoroutine(running);
        IsActive = false;
        onMinigameFinished?.Invoke(false);
    }

    IEnumerator RunMinigame()
    {
        IsActive = true;

        float endTime = Time.time + timeLimitSeconds;
        int presses = 0;

        while (Time.time < endTime && presses < requiredPresses)
        {
            if (Input.GetKeyDown(inputKey))
            {
                presses++;
            }
            yield return null;
        }

        bool success = presses >= requiredPresses;

        IsActive = false;
        onMinigameFinished?.Invoke(success);
    }
}
