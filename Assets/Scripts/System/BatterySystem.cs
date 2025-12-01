using System;
using UnityEngine;

/// <summary>
/// Simple battery system for the RX-1 drone.
/// Provides APIs to drain/recharge battery and exposes events for UI.
/// </summary>
public class BatterySystem : MonoBehaviour
{
    [Header("Battery")]
    [SerializeField] float maxBattery = 100f;
    [SerializeField] float drainPerSecondWhileFlying = 2f;
    [SerializeField] float drainPerShot = 1f;
    [SerializeField] float rechargePerRepairSuccess = 25f;

    [SerializeField] float criticalThreshold = 10f; // below this, warn the player

    public event Action<float> onBatteryChanged; // current normalized [0..1]
    public event Action onBatteryDepleted;

    float currentBattery;

    void Awake()
    {
        currentBattery = maxBattery;
    }

    public float GetCurrentBattery() => currentBattery;

    public float GetMaxBattery() => maxBattery;

    public float GetBatteryNormalized() => Mathf.Clamp01(currentBattery / maxBattery);

    public bool IsCriticallyLow() => currentBattery <= criticalThreshold;

    public bool IsEmpty() => currentBattery <= 0f;

    // Drain per second (call from Player.Update when flying)
    public void DrainOverTime(float dt)
    {
        if (dt <= 0f) return;
        if (IsEmpty()) return;

        currentBattery -= drainPerSecondWhileFlying * dt;
        currentBattery = Mathf.Max(currentBattery, 0f);

        NotifyChanged();

        if (IsEmpty()) onBatteryDepleted?.Invoke();
    }

    // Drain when firing; returns true if the shot is allowed
    public bool ConsumeForShot()
    {
        if (IsEmpty()) return false;

        currentBattery -= drainPerShot;
        currentBattery = Mathf.Max(currentBattery, 0f);

        NotifyChanged();

        if (IsEmpty()) onBatteryDepleted?.Invoke();
        return true;
    }

    // Recharge battery by amount
    public void Recharge(float amount)
    {
        if (amount <= 0f) return;
        currentBattery += amount;
        currentBattery = Mathf.Min(currentBattery, maxBattery);
        NotifyChanged();
    }

    // Shortcut for minigame success
    public void RepairSuccess()
    {
        Recharge(rechargePerRepairSuccess);
    }

    void NotifyChanged()
    {
        onBatteryChanged?.Invoke(GetBatteryNormalized());
    }
}
