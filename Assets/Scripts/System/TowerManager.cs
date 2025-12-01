using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Very small tower manager: controls advancing through tower floors and triggering repair minigames.
/// Scenes or level segments in Unity can be represented as GameObjects or prefabs assigned to the manager.
/// </summary>
public class TowerManager : MonoBehaviour
{
    [Header("Tower")]
    [SerializeField] List<GameObject> floors = new List<GameObject>();
    [SerializeField] int startFloorIndex = 0;

    [Header("References")]
    [SerializeField] RepairMinigame repairMinigame;
    [SerializeField] BatterySystem batterySystem;

    int currentFloor = 0;

    void Start()
    {
        currentFloor = Mathf.Clamp(startFloorIndex, 0, Mathf.Max(0, floors.Count - 1));
        for (int i = 0; i < floors.Count; i++)
        {
            floors[i].SetActive(i == currentFloor);
        }
    }

    public void AdvanceFloor()
    {
        if (currentFloor >= floors.Count - 1) return; // already top

        floors[currentFloor].SetActive(false);
        currentFloor++;
        floors[currentFloor].SetActive(true);
    }

    // Called when player reaches a repair station in a floor.
    public void StartRepairSequence()
    {
        if (repairMinigame == null || batterySystem == null) return;

        repairMinigame.onMinigameFinished += OnRepairFinished;
        repairMinigame.StartMinigame();
    }

    void OnRepairFinished(bool success)
    {
        repairMinigame.onMinigameFinished -= OnRepairFinished;
        if (success)
        {
            batterySystem.RepairSuccess();
        }
        // Afterwards, player continues flying, or we could start next floor
    }
}
